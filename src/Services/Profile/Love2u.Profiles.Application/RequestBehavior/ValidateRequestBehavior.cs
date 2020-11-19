using FluentValidation;
using Love2u.Profiles.Domain.Models;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Love2u.Profiles.Application.RequestBehavior
{
    internal class ValidateRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IReadOnlyCollection<IValidator> Validators;

        public ValidateRequestBehavior(IReadOnlyCollection<IValidator<TRequest>> validators) => Validators = validators;

        Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResult = Validators.SelectMany(validator => validator.Validate(context).Errors).Where(error => error != null);

            if (validationResult.Any()) 
            {
                var type = typeof(TResponse);
                var constructor = type.GetConstructor(new[] { typeof(IEnumerable<DomainError>) });

                if (constructor == null)
                    throw new MissingMethodException($"Invalid attempt to construct error command result. Type {type.Name} does not contain a constructor accepting an IEnumerable of DomainErrors.");

                var errors = validationResult.Select(v => new DomainError(ErrorType.ValidationError, v.ErrorMessage));

                return Task.FromResult((TResponse)constructor.Invoke(new[] { errors }));
            }

            return next();
        }
    }
}
