using EventBus.MessageQueues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Consumequeue.BusinessLogics;
using QueueModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Consumequeue.APIController;

namespace QueueConsumer
{
    public class ConsumeNotificationQueue : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly IQueueProvider _queueProvider;
        //private readonly NotificationAPIcontroller notificationAPIcontroller;
        public IConfiguration Configuration { get; }

        //private readonly AppLogService _appLogService;


        public ConsumeNotificationQueue(ILoggerFactory loggerFactory, IConnection _conn, IConfiguration configuration, IQueueProvider queueProvider)
        {
            _logger = loggerFactory.CreateLogger<ConsumeNotificationQueue>();
            _connection = _conn;
            Configuration = configuration;
            _queueProvider = queueProvider;
            //notificationAPIcontroller.Login();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            _channel = _connection.CreateModel();
            _channel.BasicQos(0, 1, false);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            string exchangeName = Configuration.GetSection("MessageQuque").GetSection("NotificationQueue").GetSection("exchangene").Value;
            string queue = Configuration.GetSection("MessageQuque").GetSection("NotificationQueue").GetSection("queue").Value;
            string routingKey = Configuration.GetSection("MessageQuque").GetSection("NotificationQueue").GetSection("routingkey").Value;

            _channel.QueueDeclare(queue, false, false, false, null);
            _channel.QueueBind(queue, exchangeName, routingKey);
            stoppingToken.ThrowIfCancellationRequested();
            // _channel.BasicGet(queue, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());


                // handle the received message  
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue, false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            try
            {
                EmailNotificationModel emailObj = new EmailNotificationModel();
                emailObj = JsonConvert.DeserializeObject<EmailNotificationModel>(content);


                NotificationClass.SendEmail(emailObj);

                // Send Update to Log Service.
                ApplicationLogs applogs = new ApplicationLogs();
                applogs.LogMessage = emailObj.Emailfrom + "\t" + emailObj.Emailto + "\t" + emailObj.CC + "\t" + emailObj.Content;
                _queueProvider.RegisterApplicationLog(applogs);
            }
            catch
            { }
        }
        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
