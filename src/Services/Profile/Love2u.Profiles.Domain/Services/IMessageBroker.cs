using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.Domain.Services
{
    public interface IMessageBroker
    {
        void SendNotification(INotification notification);
        INotification ReceiveNotifcation();
    }
}
