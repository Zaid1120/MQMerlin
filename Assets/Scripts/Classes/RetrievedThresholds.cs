using System.Collections.Generic;

[System.Serializable]
public class RootThresholds
{
    public RetrievedThresholds retrievedThresholds;
}

[System.Serializable]
public class RetrievedThresholds
{
    public Apps apps;
    public QueueManager queue_manager;
    public Queues queues;

    [System.Serializable]
    public class Apps
    {
        public int connThreshold;
        public float connOpRatioThreshold;
        public int minimumConns;
    }

    [System.Serializable]
    public class QueueManager
    {
        public int errorThreshold;
        public int maxMQConns;
        public int maxMQOps;
    }

    [System.Serializable]
    public class Queues
    {
        public int errorThreshold;
        public Dictionary<string, QueueThresholds> queueThresholds;

        [System.Serializable]
        public class QueueThresholds
        {
            public int depth;
            public int activity;
        }
    }
}