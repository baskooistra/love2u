using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Events
{
    public abstract class UserProfileDomainEvent : INotification
    {
        public UserProfileDomainEvent(UserProfile resource, string etag)
        {
            ResourceId = resource.Id;
            UserId = resource.UserId;
            Etag = etag;
            Rersource = resource;
        }

        public Guid ResourceId { get; protected set; }
        public Guid UserId { get; protected set; }

        public string Etag { get; protected set; }

        public UserProfile Rersource { get; protected set; }

        public ResourceAction Action { get; protected set; }
    }

    public class UserProfileCreatedDomainEvent : UserProfileDomainEvent
    {
        private UserProfileCreatedDomainEvent(UserProfile resource, string etag)
            : base(resource, etag)
        {
            Action = ResourceAction.Create;
        }

        public static UserProfileDomainEvent Create(UserProfile resource, string etag) 
        {
            return new UserProfileCreatedDomainEvent(resource, etag);
        }
    }

    public class UserProfileUpdatedDomainEvent : UserProfileDomainEvent
    {
        private UserProfileUpdatedDomainEvent(UserProfile resource, string etag)
            : base(resource, etag) 
        {
            Action = ResourceAction.Update;
        }

        public static UserProfileDomainEvent Create(UserProfile resource, string etag)
        {
            return new UserProfileUpdatedDomainEvent(resource, etag);
        }
    }

    public class UserProfileDeletedDomainEvent : UserProfileDomainEvent
    {
        private UserProfileDeletedDomainEvent(Guid userId, string etag)
            : base(null, etag) 
        {
            UserId = userId;
            Action = ResourceAction.Delete;
        }

        public static UserProfileDomainEvent Create(Guid userId, string etag)
        {
            return new UserProfileDeletedDomainEvent(userId, etag);
        }
    }

    public enum ResourceAction 
    {
        Create,
        Update,
        Delete
    }
}
