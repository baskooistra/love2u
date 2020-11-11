using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Queries
{
    public class FindUserProfileQuery : IRequest<FindUserProfileQueryResult>
    {
        public Guid UserId { get; }

        public FindUserProfileQuery(Guid id)
        {
            UserId = id;
        }
    }
}
