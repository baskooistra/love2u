using Love2u.Profiles.Domain.Models;
using Love2u.Profiles.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Shared
{
    public abstract class QueryResult<T> : BaseResult<T>
    {
        public Guid Id { get; set; }

        protected QueryResult(Guid id)
            : base()
        {
            Id = id;
        }

        protected QueryResult(Guid id, DataStoreResult<T> result)
            : base(result)
        {
            Id = id;
        }

        protected QueryResult(Guid id, DomainError error)
            : base(error)
        {
            Id = id;
        }

        protected QueryResult(Guid id, IEnumerable<DomainError> errors)
            : base(errors)
        {
            Id = id;
        }
    }
}
