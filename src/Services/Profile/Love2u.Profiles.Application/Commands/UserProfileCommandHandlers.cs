using Love2u.Profiles.Domain.Events;
using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Commands;
using Love2u.Profiles.Domain.Requests.Queries;
using Love2u.Profiles.Domain.Services;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Love2u.Profiles.Application.Commands
{
    internal class AddUserProfileCommandHandler : 
        IRequestHandler<AddUserProfileCommand, AddUserProfileCommandResult>,
        IRequestHandler<FindUserProfileQuery, FindUserProfileQueryResult>,
        IRequestHandler<DeleteUserProfileCommand, DeleteUserProfileCommandResult>,
        IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileCommandResult>
    {
        private readonly IDataStore<UserProfile> _store;

        public AddUserProfileCommandHandler(IDataStore<UserProfile> store)
        {
            _store = store;
        }

        async Task<FindUserProfileQueryResult> IRequestHandler<FindUserProfileQuery, FindUserProfileQueryResult>.Handle(FindUserProfileQuery request, CancellationToken cancellationToken)
        {
            Log.Information("Fetching user profile...", request);
            var result = await _store.GetItem(request.UserId, cancellationToken);
            
            if (result.Result == ResultType.Notfound)
            {
                Log.Information($"Could not find user profile user with ID {request.UserId}.", result);
                return new FindUserProfileQueryResult(request.UserId);
            }
            else
            {
                Log.Information("Succesfully retrieved user profile.", result);
                return new FindUserProfileQueryResult(request.UserId, result);
            }
        }

        async Task<AddUserProfileCommandResult> IRequestHandler<AddUserProfileCommand, AddUserProfileCommandResult>.Handle(AddUserProfileCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Saving user profile...", request);
            var result = await _store.AddItem(CreateUserProfile(request), cancellationToken);

            Log.Information("Succesfully saved user profile.", result);

            var commandResult = new AddUserProfileCommandResult(request, result);
            commandResult.AddDomainEvent(UserProfileCreatedDomainEvent.Create(result.Resource, result.Etag));
            return commandResult;
        }

        async Task<DeleteUserProfileCommandResult> IRequestHandler<DeleteUserProfileCommand, DeleteUserProfileCommandResult>.Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Deleting user profile...", request);
            var result = await _store.DeleteItem(request.UserId, cancellationToken);

            if (result.Result == ResultType.Notfound)
            {
                Log.Information($"Could not find user profile user with ID {request.UserId}.", result);
                return new DeleteUserProfileCommandResult(request);
            }
            else
            {
                Log.Information("Succesfully deleted user profile.", result);
                
                var commandResult = new DeleteUserProfileCommandResult(request, result);
                commandResult.AddDomainEvent(UserProfileDeletedDomainEvent.Create(request.UserId, result.Etag));
                return commandResult;
            }
        }

        async Task<UpdateUserProfileCommandResult> IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileCommandResult>.Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Updating user profile...", request);
            var result = await _store.UpdateItem(CreateUserProfile(request), cancellationToken);

            if (result.Result == ResultType.Notfound)
            {
                Log.Information($"Could not find user profile user with ID {request.UserId}.", result);
                return new UpdateUserProfileCommandResult(request);
            }
            else
            {
                Log.Information("Succesfully updated user profile.", result);

                var commandResult = new UpdateUserProfileCommandResult(request, result);
                commandResult.AddDomainEvent(UserProfileUpdatedDomainEvent.Create(result.Resource, result.Etag));
                return commandResult;
            }
        }

        private UserProfile CreateUserProfile(AddUserProfileCommand command) 
        {
            return new UserProfile()
            {
                UserId = command.UserId,
                Description = command.Description,
                EyeColor = Enumeration.FromName<EyeColor>(command.EyeColor)
            };
        }

        private UserProfile CreateUserProfile(UpdateUserProfileCommand command)
        {
            return new UserProfile(command.Id, command.Created)
            {
                UserId = command.UserId,
                Description = command.Description,
                EyeColor = Enumeration.FromName<EyeColor>(command.EyeColor)
            };
        }
    }
}
