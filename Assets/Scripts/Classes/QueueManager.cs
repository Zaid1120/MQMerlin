using System.Collections.Generic;

namespace ReceiveData
{    
    [System.Serializable]
    public class QueueManager
    {
        public string qmgr_name;
        public string state;
        public List<Queue> queues;
        public List<Channel> channels;
        public List<Application> applications;
        public string installation_name;
        public string permit_standby;
        public bool is_default_qmgr;
        public string publish_subscribe_state;
        public int? connection_count;
        public string channel_initiator_state;
        public string ldap_connection_state;
        public string started;
    }
}
