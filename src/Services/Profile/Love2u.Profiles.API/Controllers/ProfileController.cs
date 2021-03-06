﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Love2u.Profiles.Domain.Requests.Commands;
using Love2u.Profiles.Domain.Requests.Queries;
using Love2u.Profiles.Domain.Requests.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Love2u.ProfileAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            Log.Debug($"Initiated profiles controller for route {Url}");
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<ActionResult> Get() => 
            Result(await _mediator.Send(new FindUserProfileQuery(RetrieveUserId())));

        [HttpPost("")]
        public async Task<ActionResult> Post(AddUserProfileCommand command, CancellationToken cancellationToken)
        {
            command.UserId = RetrieveUserId();

            return Result(await _mediator.Send(command, cancellationToken));
        }

        [HttpDelete("")]
        public async Task<ActionResult> Delete() => 
            Result(await _mediator.Send(new DeleteUserProfileCommand(RetrieveUserId())));

        [HttpPut("")]
        public async Task<ActionResult> Put(UpdateUserProfileCommand command, CancellationToken cancellationToken) =>
            Result(await _mediator.Send(command, cancellationToken));

        private ActionResult Result<T>(BaseResult<T> result)
        {
            if (!result.HasError || result.HasDomainEvents) 
            {
                SendDomainEvents(result.DomainEvents);
            }

            return result switch
            {
                null => throw new ArgumentNullException("Argument 'result' cannot be null."),
                var r when r.Errors.Any() => BadRequest(r.Errors.Select(e => e.Message)),
                BaseResult<bool> r when !r.Result => StatusCode((int)HttpStatusCode.InternalServerError),
                BaseResult<bool> r when r.Result => Ok("Request handled succesfully."),
                BaseResult<T> r when r.Result != null => Ok(new ResourceResult<T>(r.Result, r.Etag)),
                QueryResult<T> r when r.Result == null => NotFound(),
                _ => NoContent()
            };
        }

        private void SendDomainEvents(IReadOnlyCollection<INotification> domainEvents) 
        {
            foreach (var domainEvent in domainEvents) 
            {
                _mediator.Publish(domainEvent);
            }
        }

        private Guid RetrieveUserId() 
        {
            var userIdClaim = User.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            return Guid.Parse(userIdClaim.Value);
        }

        private class ResourceResult<T>
        {
            public T Resource { get; }
            public string Etag { get; }

            public ResourceResult(T resource, string etag)
            {
                Resource = resource;
                Etag = etag;
            }
        }
    }
}
