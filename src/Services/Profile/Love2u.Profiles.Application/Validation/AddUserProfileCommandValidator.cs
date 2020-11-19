using FluentValidation;
using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Requests.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love2u.Profiles.Application.Validation
{
    internal class AddUserProfileCommandValidator : AbstractValidator<AddUserProfileCommand>
    {
        public AddUserProfileCommandValidator()
        {
            RuleFor(command => command.UserId)
                .NotEmpty()
                .WithMessage("User ID is required when saving user profile.");

            RuleFor(command => command.EyeColor)
                .Must(eyeColor => Enumeration
                .FromName<EyeColor>(eyeColor) != null)
                .When(command => !string.IsNullOrWhiteSpace(command.EyeColor))
                .WithMessage("Specified eye color is not a valid value.");
        }
    }
}
