using Love2u.Profiles.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Shared
{
    public abstract class BaseResult<T>
    {
        public T Result { get; set; }

        public string Etag { get; set; }

        private readonly List<DomainError> _errors = new List<DomainError>();

        public IEnumerable<DomainError> Errors => _errors;

        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents.Remove(eventItem);
        }

        public bool HasDomainEvents => _domainEvents.Any();

        public bool HasError => _errors.Any();

        protected BaseResult()
        {
        }

        protected BaseResult(DataStoreResult<T> result)
        {
            Result = result.Resource;
            Etag = result.Etag;
        }

        protected BaseResult(DomainError error)
        {
            AddError(error);
        }

        protected BaseResult(IEnumerable<DomainError> errors)
        {
            AddErrors(errors);
        }

        public void AddError(DomainError error)
        {
            if (error == null) throw new ArgumentException("Argument 'error' cannot be null.");
            _errors.Add(error);
        }

        public void AddErrors(IEnumerable<DomainError> errors)
        {
            if (errors == null || !errors.Any()) throw new ArgumentException("Argument 'errors' cannot be null or empty.");
            _errors.AddRange(errors);
        }
    }
}
