using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Commands
{
    public class UpdateUserProfileCommandResult : CommandResult<UserProfile, UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandResult(UpdateUserProfileCommand command) : base(command)
        {
        }

        public UpdateUserProfileCommandResult(UpdateUserProfileCommand command, DataStoreResult<UserProfile> result) : base(command, result)
        {
        }

        public UpdateUserProfileCommandResult(UpdateUserProfileCommand command, DomainError error) : base(command, error)
        {
        }

        public UpdateUserProfileCommandResult(UpdateUserProfileCommand command, IEnumerable<DomainError> errors) : base(command, errors)
        {
        }
    }
}
