using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Love2u.IdentityProvider.Data.Annotations
{
    internal class MinimumAgeAttribute : ValidationAttribute
    {
        private int MinimumAge;

        public MinimumAgeAttribute(int minimumAge)
            : base()
        {
            MinimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date)) 
            {
                bool validAge = (date.AddYears(MinimumAge) < DateTime.Now);
                if (!validAge) return new ValidationResult(base.FormatErrorMessage(MinimumAge.ToString()));
                else return ValidationResult.Success;
            }

            return new ValidationResult("Specified date was not in a correct format");
        }

    }
}
