using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Requests.Commands
{
    public class UpdateUserProfileCommand : IRequest<UpdateUserProfileCommandResult>
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public string EyeColor { get; set; }
    }
}
