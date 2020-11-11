using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Commands
{
    public class DeleteUserProfileCommand : IRequest<DeleteUserProfileCommandResult>
    {
        public Guid UserId { get; }

        public DeleteUserProfileCommand(Guid id)
        {
            UserId = id;
        }
    }
}
