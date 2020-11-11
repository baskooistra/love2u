using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace Love2u.Profiles.Domain.Models
{
    public abstract class Entity
    {
        private readonly int _hashCode;

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; }

        public Entity()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Updated = (DateTime?)null;
            _hashCode = HashCode.Combine(Id, Created, Updated);
        }

        public Entity(Guid entityId, DateTime created)
        {
            Id = entityId;
            Created = created;
            Updated = DateTime.Now;
            _hashCode = HashCode.Combine(Id, Created, Updated);
        }

        private List<INotification> _domainEvents;

        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents.Remove(eventItem);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            var item = (Entity)obj;

            return GetHashCode() == item.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (left == null)
                return right == null;

            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public override int GetHashCode() 
        {
            return _hashCode;
        }
    }
}
