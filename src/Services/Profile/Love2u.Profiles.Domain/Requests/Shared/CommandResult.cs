using Love2u.Profiles.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Shared
{
    public abstract class CommandResult<T, U> : BaseResult<T>
    {
        public U Command { get; set; }

        protected CommandResult(U command)
            : base()
        {
            Command = command;
        }

        protected CommandResult(U command, DataStoreResult<T> result)
            : base(result)
        {
            Command = command;
        }

        protected CommandResult(U command, DomainError error)
            : base(error)
        {
            Command = command;
        }

        protected CommandResult(U command, IEnumerable<DomainError> errors)
            : base(errors)
        {
            Command = command;
        }
    }
}
