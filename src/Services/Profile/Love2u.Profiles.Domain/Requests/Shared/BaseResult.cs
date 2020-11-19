using Love2u.Profiles.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Shared
{
    public abstract class BaseResult<T>
    {
        public T Result { get; set; }

        public string Etag { get; set; }

        private readonly List<DomainError> _errors = new List<DomainError>();

        public IEnumerable<DomainError> Errors => _errors;

        public bool HasError => _errors.Any();

        protected BaseResult()
        {
        }

        protected BaseResult(DataStoreResult<T> result)
        {
            Result = result.Item;
            Etag = result.Etag;
        }

        protected BaseResult(DomainError error)
        {
            AddError(error);
        }

        protected BaseResult(IEnumerable<DomainError> errors)
        {
            AddErrors(errors);
        }

        public void AddError(DomainError error)
        {
            if (error == null) throw new ArgumentException("Argument 'error' cannot be null.");
            _errors.Add(error);
        }

        public void AddErrors(IEnumerable<DomainError> errors)
        {
            if (errors == null || !errors.Any()) throw new ArgumentException("Argument 'errors' cannot be null or empty.");
            _errors.AddRange(errors);
        }
    }
}
