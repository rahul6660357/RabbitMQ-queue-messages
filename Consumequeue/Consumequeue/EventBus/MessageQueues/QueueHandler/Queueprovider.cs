using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QueueModel;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.MessageQueues
{
    public class Queueprovider : IQueueProvider
    {
        readonly IConfiguration _configuration;
        private readonly IConnection conn;
        public Queueprovider(IConfiguration configuration, IConnection _conn)
        {

            _configuration = configuration;
            conn = _conn;
        }

        public async Task<bool> RegisterApplicationLog(ApplicationLogs log)
        {
            string logmessage = JsonConvert.SerializeObject(log);
            string exchange = _configuration.GetSection("MessageQuque").GetSection("Applicationlogs").GetSection("exchangene").Value;
            string routingkey = _configuration.GetSection("MessageQuque").GetSection("Applicationlogs").GetSection("routingkey").Value;
            return await Task.Run(() => SendMessagetoExchange(exchange, routingkey, logmessage));
        }

        private bool SendMessagetoExchange(string exchange, string routingkey, string message)
        {

            bool result = true;
            try
            {
                using (var channel = conn.CreateModel())
                {
                    var data = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange, routingkey, null, data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Send update to notification service 
                result = false;
            }

            return result;

        }
    }
}




