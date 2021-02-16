using QueueModel;
using System.Threading.Tasks;

namespace EventBus.MessageQueues
{
    public interface IQueueProvider
    {
        public Task<bool> RegisterApplicationLog(ApplicationLogs log);
    }
}
