using System;
using UnityEngine;
using FlaskServer;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReceiveData
{
    public class GetDetails : MonoBehaviour
    {
        private bool isRunning = false;
        public string urlPort = "https://127.0.0.1:5000";

        // data storage for results of server requests
        public Root QueuesResult;
        public Root ChannelsResult;
        public Root ApplicationsResult;
        public Root QueueManagersResult;
        public Dictionary<string, float> QueueThresholds;

        // event to notify when data is fetched
        public event Action OnDataFetched;

        // flag to determine if requests should be made
        public bool canMakeRequests = true;

        // start making requests when component is enabled
        private void Start()
        {
            BeginMakingRequests();
        }

        // stop making requests when component is disabled
        private void OnDisable()
        {
            StopMakingRequests();
        }

        // resume making requests when component is re-enabled
        private void OnEnable()
        {
            BeginMakingRequests();
        }

        // schedule periodic execution of MakeRequests()
        private void BeginMakingRequests()
        {
            InvokeRepeating(nameof(MakeRequests), 0f, 30f);
        }

        // cancel periodic execution of MakeRequests()
        private void StopMakingRequests()
        {
            CancelInvoke(nameof(MakeRequests));
        }

        // fetch data from server and parse it
        private async void MakeRequests()
        {
            // check flag if we should and can make requests
            if (!canMakeRequests || isRunning)
            {
                return;
            }

            if (isRunning) // locking mechanism
            {
                return;
            }

            isRunning = true;

            try
            {
                // fetch and parse queues data
                string queuesJson = await Utilities.Instance.GetRequest(urlPort + "/getallqueues");
                if (!string.IsNullOrEmpty(queuesJson))
                {
                    QueuesResult = JsonConvert.DeserializeObject<Root>(queuesJson);
                }

                // fetch and parse channels data
                string channelsJson = await Utilities.Instance.GetRequest(urlPort + "/getallchannels");
                if (!string.IsNullOrEmpty(channelsJson))
                {
                    ChannelsResult = JsonConvert.DeserializeObject<Root>(channelsJson);
                }

                // fetch and parse applications data
                string applicationJson = await Utilities.Instance.GetRequest(urlPort + "/getallapplications");
                if (!string.IsNullOrEmpty(applicationJson))
                {
                    ApplicationsResult = JsonConvert.DeserializeObject<Root>(applicationJson);
                }

                // feature to be utilised in production version to manage multiple queue managers

                //// fetch and parse queue managers data
                //string queueManagersJson = await Utilities.Instance.GetRequest(urlPort + "/getallqueuemanagers");
                //if (!string.IsNullOrEmpty(queueManagersJson))
                //{
                //    QueueManagersResult = JsonConvert.DeserializeObject<Root>(queueManagersJson);
                //}

                // notify listeners that data has been fetched
                OnDataFetched?.Invoke();
            }
            catch (Exception e)
            {
                // log any errors during fetching or parsing
                Debug.LogError("Error fetching or parsing data: " + e.Message);
            }
            finally
            {
                isRunning = false;
            }
        }
    }
}
