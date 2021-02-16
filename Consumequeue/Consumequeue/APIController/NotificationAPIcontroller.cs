using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Consumequeue.Services;
using Consumequeue.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumequeue.APIController
{
    [Route("api/notification")]
    [ApiController]

    public class NotificationAPIcontroller : Controller

    {
        readonly IConfiguration _configuration;
        private readonly IConnection conn;
        private readonly INotification _notification;
        public NotificationAPIcontroller( INotification notification, IConfiguration configuration, IConnection _conn)
        {
            _notification = notification;
            _configuration = configuration;
            conn = _conn;
        }

        [HttpPost]
        [Route("consume")]
        public IActionResult Login()
        {
            DataViewModel dm = new DataViewModel();

            try
            {
               
                    using (var rabbitMqChannel = conn.CreateModel())
                    {
                        rabbitMqChannel.QueueDeclare(queue: "notification",
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                        rabbitMqChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        int messageCount = Convert.ToInt16(rabbitMqChannel.MessageCount("notification"));
                        Console.WriteLine(" Listening to the queue. This channels has {0} messages on the queue", messageCount);

                        var consumer = new EventingBasicConsumer(rabbitMqChannel);
                        consumer.Received += (model, ea) =>
                        {
                            var body2 = ea.Body.ToArray(); 
                            var message = Encoding.UTF8.GetString(body2);
                            dm.messages= Encoding.UTF8.GetString(body2);
                            Console.WriteLine("error message : " + message);
                            rabbitMqChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            Thread.Sleep(1000);
                        };
                        rabbitMqChannel.BasicConsume(queue: "notification",
                                             autoAck: false,
                                             consumer: consumer);

                        Thread.Sleep(1000 * messageCount);
                        Console.WriteLine(" Connection closed, no more messages.");
                       
                    }
               
                dm.statusCode = System.Net.HttpStatusCode.OK;
            }
            catch(Exception e)
            {
                dm.statusCode = System.Net.HttpStatusCode.BadRequest;
            }
            Console.WriteLine(dm);

            return new ObjectResult(dm);
        }
       




    }
}
