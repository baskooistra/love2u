using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Commands
{
    public class DeleteUserProfileCommandResult : CommandResult<bool, DeleteUserProfileCommand>
    {
        public DeleteUserProfileCommandResult(DeleteUserProfileCommand command) : base(command)
        {
            Result = false;
        }

        public DeleteUserProfileCommandResult(DeleteUserProfileCommand command, DataStoreResult<bool> result) : base(command, result)
        {

        }

        public DeleteUserProfileCommandResult(DeleteUserProfileCommand command, DomainError error) : base(command, error)
        {
        }

        public DeleteUserProfileCommandResult(DeleteUserProfileCommand command, IEnumerable<DomainError> errors) : base(command, errors)
        {
        }
    }
}
