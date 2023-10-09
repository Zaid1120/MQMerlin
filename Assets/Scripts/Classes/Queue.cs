using System;

namespace ReceiveData
{
    [Serializable]
    public class Queue
    {
        public string queue_name;
        public int max_number_of_messages;
        public int max_message_length;
        public bool inhibit_put;
        public bool inhibit_get;
        public string description;
        public string time_created;
        public string type_name;
        public string time_altered;
        public int current_depth;
        public float threshold;
        public string target_queue_name;
        public string target_qmgr_name;
        public string transmission_queue_name;
    }
}
