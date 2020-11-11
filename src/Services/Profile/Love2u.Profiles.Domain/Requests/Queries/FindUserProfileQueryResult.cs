using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Queries
{
    public class FindUserProfileQueryResult : QueryResult<UserProfile>
    {
        public FindUserProfileQueryResult(Guid id) : base(id)
        {
        }

        public FindUserProfileQueryResult(Guid id, DataStoreResult<UserProfile> result) : base(id, result)
        {
        }

        public FindUserProfileQueryResult(Guid id, DomainError error) : base(id, error)
        {
        }

        public FindUserProfileQueryResult(Guid id, IEnumerable<DomainError> errors) : base(id, errors)
        {
        }
    }
}
