using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Love2u.Profiles.Domain.Events;
using Love2u.Profiles.Domain.Services;
using MediatR;
using RabbitMQ.Client;
using Serilog;

namespace Love2u.Profiles.Application.Commands
{
    public class UserProfileDomainEventHandler : INotificationHandler<UserProfileDomainEvent>
    {
        private readonly IMessageBroker _messageBroker;

        public UserProfileDomainEventHandler(IMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public Task Handle(UserProfileDomainEvent notification, CancellationToken cancellationToken)
        {
            Log.Information<INotification>("Queueing domain event", notification);

            try
            {
                _messageBroker.SendNotification(notification);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error("Something went wrong while sending domain event.", ex);
                return Task.FromException(ex);
            }
        }
    }
}
