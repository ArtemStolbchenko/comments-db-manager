using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comments_database_manager.Communication;
using Comments_database_manager.Models;
using CommentsService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Comments_database_manager.Communication
{
    public class RabbitMQManager : IRabbitMQManager
    {
        private IConnection _connection;
        private IModel _channel;
        private const string HOSTNAME = "host.docker.internal",
                             QUEUE = "comments";
        private const int PORT = 5772;

        public event Reply ReadRequested;
        public event NoReply CreateRequested, UpdateRequested, DeleteRequested;
        public RabbitMQManager()
        {
            _connection = Connect();

            StartListening();
        }
        private async Task StartListening()
        {
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: QUEUE,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: QUEUE,
                                 autoAck: false,
                                 consumer: consumer);
            Console.WriteLine("[x] listening to RPC requests");

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("RabbitMQ: A message has arrived!");

                ProcessMessage(_channel, ea);
            };
        }
        private void ProcessMessage(IModel Channel, BasicDeliverEventArgs ea)
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            CommentDTO commentDTO = JsonConvert.DeserializeObject<CommentDTO>(json);
            var comment = new Comment(commentDTO);

            switch (commentDTO.Action)
            {
                case CommentAction.Create:
                    CreateRequested?.Invoke(comment);
                    Channel.BasicAck(ea.DeliveryTag, false);
                    break;
                case CommentAction.Read:
                    ReadRequested?.Invoke(_channel, ea);
                    break;
                case CommentAction.Update:
                    UpdateRequested?.Invoke(comment);
                    Channel.BasicAck(ea.DeliveryTag, false);
                    break;
                case CommentAction.Delete:
                    DeleteRequested?.Invoke(comment);
                    Channel.BasicAck(ea.DeliveryTag, false);
                    break;
            }
        }
        private IConnection Connect(int Try = 0)
        {
            try
            {
                System.Console.WriteLine($"Trying to connect to {HOSTNAME}:{PORT}. Try #{Try+1}");
                var factory = new ConnectionFactory { HostName = HOSTNAME, Port = PORT };

                return factory.CreateConnection();
            }
            catch (BrokerUnreachableException exception)
            {
                Console.WriteLine("Connection failed");
                if (Try < 5)
                {
                    Thread.Sleep(1000);
                    return Connect(Try + 1);
                } else
                {
                    throw exception; 
                }
            }
        }
        public async Task Respond(IModel Channel, BasicDeliverEventArgs ea, object body)
        {
            try
            {
                Console.WriteLine($"Sending response: \n{JsonConvert.SerializeObject(body)}");

                var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));

                Channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ea.BasicProperties.ReplyTo,
                                     basicProperties: ea.BasicProperties,
                                     body: message);
                Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        }
        private void DisposeConnection(object? sender, EventArgs? e)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
