namespace ReceiveData
{

    [System.Serializable]
    public class Channel
    {
        public string channel_name;
        public string channel_type;
        public string description;
        public int max_message_length;
        public int heartbeat_interval;
        public string transport_type;
    }
}

