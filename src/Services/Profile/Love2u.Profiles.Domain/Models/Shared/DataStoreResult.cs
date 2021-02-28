using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Models.Shared
{
    public class DataStoreResult<T>
    {
        public T Resource { get; }

        public ResultType Result { get; }

        public string Etag { get; }

        public DataStoreResult(T item, ResultType result, string etag)
        {
            Resource = item;
            Etag = etag;
            Result = result;
        }
    }

    public enum ResultType 
    {
        Ok,
        Created,
        Updated,
        Notfound,
        Failed
    }
}
