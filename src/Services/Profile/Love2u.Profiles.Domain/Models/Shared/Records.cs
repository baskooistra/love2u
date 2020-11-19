using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love2u.Profiles.Domain.Models.Shared
{
    public record DomainError(ErrorType Type, string Message);

    public record DataStoreResult<T>(T Item, ResultType Result, string Etag);

    public enum ResultType
    {
        Ok,
        Created,
        Updated,
        Notfound,
        Failed
    }
}
