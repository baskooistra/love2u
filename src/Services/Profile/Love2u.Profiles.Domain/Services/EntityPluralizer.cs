using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Services
{
    public class EntityPluralizer
    {
        private static readonly IDictionary<string, string> plurals = new Dictionary<string, string>
        {
            { "UserProfile", "UserProfiles" }
        };

        public static string GetPlural(string singular) 
        {
            string plural;

            if (plurals.TryGetValue(singular, out plural))
            {
                Log.Debug($"Found plural name '{plural}' for singular '{singular}'.");
                return plural;
            }
            else 
            {
                Log.Error($"Found plural name for singular '{singular}'.");
                throw new ArgumentException($"Invalid singular name '{singular}'. No plural known. Please add it to EntityPluralizer dictionary.");
            }   
        }
    }
}
