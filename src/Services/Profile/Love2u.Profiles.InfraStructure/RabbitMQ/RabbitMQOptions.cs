using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.InfraStructure.RabbitMQ
{
    internal class RabbitMQOptions
    {
        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
