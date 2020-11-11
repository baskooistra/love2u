using Love2u.Profiles.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Models.Aggregates.UserProfile
{
    public class EyeColor : Enumeration
    {
        public EyeColor(int id, string name)
            : base(id, name)
        { }

        public static readonly EyeColor Blue = new EyeColor(0, "blue");

        public static readonly EyeColor Brown = new EyeColor(1, "brown");

        public static readonly EyeColor Green = new EyeColor(1, "green");
    }
}
