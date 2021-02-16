using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using notification.Services;
using notification.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace notification.APIController
{
    [Route("api/notificationn")]
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
        [Route("login")]
        public IActionResult Login( [FromBody] DataViewModel dmm)
        {
            DataViewModel dm = new DataViewModel();

            try
            {
                dm.checkresult = _notification.Authenticate(dmm.username, dmm.password);
                if(dm.checkresult==false)
                { 
                    /*using (var channel = conn.CreateModel())
                    {
                        channel.QueueDeclare(queue: "notification",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = "User: " + dmm.username + " with this Password: " + dmm.password + "is Failed";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "notification",
                                             basicProperties: null,
                                                 body: body);
                    }*/
                    dm.messages = "Your Data has not been submitted successfully.";
                }
                else
                {
                    using (var channel = conn.CreateModel())
                    {
                        channel.QueueDeclare(queue: "notification",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = "User: " + dmm.username + " with this Password: " + dmm.password + "is success";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "notification",
                                             basicProperties: null,
                                                 body: body);
                    }
                    
                }
                dm.messages = "Your Data has been submitted.You will be notified of the response in 1 minute";
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
