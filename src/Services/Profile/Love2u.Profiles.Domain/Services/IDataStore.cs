using Love2u.Profiles.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Love2u.Profiles.Domain.Services
{
    public interface IDataStore<T>
    {
        Task<DataStoreResult<T>> GetItem(Guid id, CancellationToken cancellationToken);

        Task<DataStoreResult<T>> AddItem(T item, CancellationToken cancellationToken);

        Task<DataStoreResult<T>> UpdateItem(T item, CancellationToken cancellationToken);

        Task<DataStoreResult<bool>> DeleteItem(Guid id, CancellationToken cancellationToken);
    }
}
