using System.Collections.Generic;

namespace ReceiveData
{

    [System.Serializable]
    public class Root
    {
        public List<Queue> All_Queues ;
        public List<Channel> All_Channels ;
        public List<Application> All_Applications ;
        public List<QueueManager> All_Queue_Managers ;
    }
}
