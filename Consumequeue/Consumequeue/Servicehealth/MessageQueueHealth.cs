using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consumequeue.Servicehealth
{
    public class MessageQueueHealth : IHealthCheck
    {

        private readonly IConnection _conn;
        public MessageQueueHealth(IConnection conn)
        {

            _conn = conn;
        }


        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_conn.IsOpen)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Message Quque is live"));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Message Quque not live."));
        }
    }
}
