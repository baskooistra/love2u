using Newtonsoft.Json;
using System;

namespace Love2u.Profiles.Domain.Models.Aggregates.UserProfile
{
    public class UserProfile : Entity, IAggregateRoot
    {
        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "eyeColor")]
        public EyeColor EyeColor { get; set; }

        public UserProfile()
            : base()
        {

        }

        public UserProfile(Guid id, DateTime created)
            : base(id, created)
        {

        }
    }
}
