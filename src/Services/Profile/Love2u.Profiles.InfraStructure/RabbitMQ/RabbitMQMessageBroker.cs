using Love2u.Profiles.Domain.Services;
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Love2u.Profiles.InfraStructure.RabbitMQ
{
    internal class RabbitMQMessageBroker : IMessageBroker
    {
        private readonly IModel _channel;

        private const string EXCHANGE_NAME = "love2u.userprofiles";
        private const string ROUTING_KEY = "love2u.userprofiles";

        public RabbitMQMessageBroker(IModel channel)
        {
            _channel = channel;
            _channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, true, false);
        }

        INotification IMessageBroker.ReceiveNotifcation()
        {
            throw new NotImplementedException();
        }

        void IMessageBroker.SendNotification(INotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: EXCHANGE_NAME,
                routingKey: ROUTING_KEY,
                mandatory: false,
                basicProperties: properties,
                body: bytes);
        }
    }
}
