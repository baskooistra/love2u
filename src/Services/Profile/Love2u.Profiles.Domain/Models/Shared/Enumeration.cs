using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Love2u.Profiles.Domain.Models.Shared
{
    public abstract class Enumeration : IComparable
    {
        public int Id { get; }
        public string Name { get; }

        private protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration otherValue))
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static IEnumerable<T> GetAll<T>() where T : Enumeration 
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(field => field.GetValue(null)).Cast<T>();
        }

        public static T FromName<T>(string name) where T : Enumeration
        {
            var option = GetAll<T>().FirstOrDefault(option => option.Name == name);

            if (option == null)
                throw new ArgumentException($"Invalid name '{name}' for enumeration type {typeof(T).Name}");

            return option;
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var option = GetAll<T>().FirstOrDefault(option => option.Id == value);

            if (option == null)
                throw new ArgumentException($"Invalid value '{value}' for enumeration type {typeof(T).Name}");

            return option;
        }
    }
}
