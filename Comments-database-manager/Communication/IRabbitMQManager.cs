using CommentsService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments_database_manager.Communication
{
    public delegate void Reply(IModel Channel, BasicDeliverEventArgs props);
    public delegate void NoReply(Comment comment);
    public interface IRabbitMQManager
    {

        public event Reply ReadRequested;

        public event NoReply CreateRequested, UpdateRequested, DeleteRequested;
        public Task Respond(IModel Channel, BasicDeliverEventArgs ea, object body);
    }
}
