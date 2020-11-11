using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Models.Shared
{
    public class ErrorType : Enumeration
    {
        public ErrorType(int id, string name)
            : base(id, name)
        { }

        public static readonly ErrorType ValidationError = new ErrorType(0, "ValidationError");
    }
}
