using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Commands
{
    public class AddUserProfileCommandResult : CommandResult<UserProfile, AddUserProfileCommand>
    {
        public AddUserProfileCommandResult(AddUserProfileCommand command) : base(command)
        {
        }

        public AddUserProfileCommandResult(AddUserProfileCommand command, DataStoreResult<UserProfile> result) : base(command, result)
        {
        }

        public AddUserProfileCommandResult(AddUserProfileCommand command, DomainError error) : base(command, error)
        {
        }

        public AddUserProfileCommandResult(AddUserProfileCommand command, IEnumerable<DomainError> errors) : base(command, errors)
        {
        }
    }
}
