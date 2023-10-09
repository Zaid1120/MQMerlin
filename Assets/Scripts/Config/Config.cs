using UnityEngine;
using UnityEngine.UI; // Necessary for InputField reference
using TMPro;
using System.Collections.Generic;
using ReceiveData;
using FlaskServer;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

public class Config : MonoBehaviour
{
    public GameObject configUnitPrefab;
    public Transform parentTransform;
    private GetDetails _callCode; // reference to the GetDetails script
    public Button submitButton;
    private Dictionary<string, GameObject> queueToConfigMap = new Dictionary<string, GameObject>();
    private List<GameObject> instantiatedQueueConfigs = new List<GameObject>();
    public TMP_Text UpdateText;
    public TMP_Text ErrorText;

    public GameObject queueConfig;
    public GameObject AppConfig;
    public GameObject QmgrConfig;

    public QueueConfigSearchManager queueConfigSearchManager;

    string thresholdURI = "https://127.0.0.1:5000/queuethresholdmanager";

    public RootThresholds configurations;

    private void OnEnable()
    {
        ClearParent(parentTransform);
        queueToConfigMap.Clear();
        instantiatedQueueConfigs.Clear();
        ClearSearchManager();

        //find GameObject with GetDetails component and get a reference to it
        GameObject callCodeObject = GameObject.Find("GetDetails");

        if (callCodeObject)
        {
            _callCode = callCodeObject.GetComponent<GetDetails>();

            // add a listener for when data gets fetched
            _callCode.OnDataFetched += BeginGetRequest; // Subscribe to the OnDataFetched event
        }
    }

    private async void BeginGetRequest()
    {
        configurations = await GetConfigs();
        HandleConfigs(configurations);
    }

    private void OnDisable()
    {
        if (_callCode)
        {
            // Remove the listener for when data gets fetched
            _callCode.OnDataFetched -= BeginGetRequest; // Unsubscribe from the OnDataFetched event
        }
    }

    private void HandleConfigs(RootThresholds configurations)
    {
        foreach (var queue in configurations.retrievedThresholds.queues.queueThresholds.OrderBy(q => q.Key)) //sorting in an alphabetical order
        {
            if (!queueToConfigMap.ContainsKey(queue.Key))
            {
                //instantiate the queue instance
                GameObject queueInstance = Instantiate(configUnitPrefab, parentTransform);

                //add the instance to the search manager
                queueConfigSearchManager.AddQueueConfig(queueInstance);

                //Now setting instantiated instance name
                TMP_Text queueNameText = queueInstance.transform.Find("queuename").GetComponent<TMP_Text>();
                queueNameText.text = queue.Key;

                //Now setting the default thresholds
                TMP_Text depthThresholdPlaceholder = queueInstance.transform.Find("DepthThresholdInput/Text Area/Placeholder").GetComponent<TMP_Text>();
                depthThresholdPlaceholder.text = $"{queue.Value.depth}%";

                TMP_Text activityThresholdPlaceholder = queueInstance.transform.Find("ActivityThresholdInput/Text Area/Placeholder").GetComponent<TMP_Text>();
                activityThresholdPlaceholder.text = queue.Value.activity.ToString();

                queueToConfigMap[queue.Key] = queueInstance;
                instantiatedQueueConfigs.Add(queueInstance);
            }

            else //update it
            {
                GameObject queueInstance = queueToConfigMap[queue.Key];

                //updating the thresholds

                TMP_Text depthThresholdPlaceholder = queueInstance.transform.Find("DepthThresholdInput/Text Area/Placeholder").GetComponent<TMP_Text>();
                depthThresholdPlaceholder.text = $"{queue.Value.depth}%";

                TMP_Text activityThresholdPlaceholder = queueInstance.transform.Find("ActivityThresholdInput/Text Area/Placeholder").GetComponent<TMP_Text>();
                activityThresholdPlaceholder.text = queue.Value.activity.ToString();

            }
        }

        //updating the errors per minute
        TMP_Text errorsPerMinPlaceholder = queueConfig.transform.Find("input/Text Area/Placeholder").GetComponent<TMP_Text>();
        errorsPerMinPlaceholder.text = configurations.retrievedThresholds.queues.errorThreshold.ToString();

        //set the configs for applications
        TMP_Text connThresholdPlaceholder = AppConfig.transform.Find("connsthreshold/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        connThresholdPlaceholder.text = configurations.retrievedThresholds.apps.connThreshold.ToString();

        TMP_Text ratioPlaceholder = AppConfig.transform.Find("connsopratio/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        ratioPlaceholder.text = configurations.retrievedThresholds.apps.connOpRatioThreshold.ToString();

        TMP_Text minConnsPlaceholder = AppConfig.transform.Find("minconns/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        minConnsPlaceholder.text = configurations.retrievedThresholds.apps.minimumConns.ToString();

        //set configs for qmgr
        TMP_Text errorThreshold = QmgrConfig.transform.Find("errorthreshold/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        errorThreshold.text = configurations.retrievedThresholds.queue_manager.errorThreshold.ToString();

        TMP_Text maxConnsPlaceholder = QmgrConfig.transform.Find("maxconns/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        maxConnsPlaceholder.text = configurations.retrievedThresholds.queue_manager.maxMQConns.ToString();

        TMP_Text maxMQOpsPlaceholder = QmgrConfig.transform.Find("operations/input/Text Area/Placeholder").GetComponent<TMP_Text>();
        maxMQOpsPlaceholder.text = configurations.retrievedThresholds.queue_manager.maxMQOps.ToString();
    }

    public async void Submit()
    {
        try
        {
            //Queue Panel

            foreach (GameObject configUnit in instantiatedQueueConfigs)
            {
                TMP_Text queueNameText = configUnit.transform.Find("queuename").GetComponent<TMP_Text>();
                TMP_InputField depthThresholdInput = configUnit.transform.Find("DepthThresholdInput").GetComponent<TMP_InputField>();
                TMP_InputField actvityThresholdInput = configUnit.transform.Find("ActivityThresholdInput").GetComponent<TMP_InputField>();

                string queueName = queueNameText.text;
                string depthThresholdValue = depthThresholdInput.text;

                string activityThresholdValue = actvityThresholdInput.text;

                int depthValue;

                if (int.TryParse(depthThresholdValue, out depthValue))
                {
                    configurations.retrievedThresholds.queues.queueThresholds[queueName].depth = depthValue;
                }

                int activityValue;

                if (int.TryParse(activityThresholdValue, out activityValue))
                {
                    configurations.retrievedThresholds.queues.queueThresholds[queueName].activity = activityValue;

                }

                TMP_Text depthThresholdPlaceholder = depthThresholdInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
                TMP_Text activityThresholdPlaceholder = actvityThresholdInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

                //make both placeholder and text go blank to give feedback to user that action is happening
                depthThresholdInput.text = "";
                actvityThresholdInput.text = "";
                depthThresholdPlaceholder.text = "";
                activityThresholdPlaceholder.text = "";
            }

            //for errors per minute
            TMP_InputField queueErrorThresholdInput = queueConfig.transform.Find("input").GetComponent<TMP_InputField>();
            string queueErrorThresholdText = queueErrorThresholdInput.text;

            int queueErrorThreshold;

            if (int.TryParse(queueErrorThresholdText, out queueErrorThreshold))
            {
                configurations.retrievedThresholds.queues.errorThreshold = queueErrorThreshold;
            }

            TMP_Text queueErrorThresholdPlaceholder = queueErrorThresholdInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

            queueErrorThresholdInput.text = "";
            queueErrorThresholdPlaceholder.text = "";

            //App Panel

            TMP_InputField appConnThresholdInput = AppConfig.transform.Find("connsthreshold/input").GetComponent<TMP_InputField>();
            TMP_InputField appMinConnsInput = AppConfig.transform.Find("minconns/input").GetComponent<TMP_InputField>();
            TMP_InputField appRatioInput = AppConfig.transform.Find("connsopratio/input").GetComponent<TMP_InputField>();

            string appConnThresholdText = appConnThresholdInput.text;
            string appMinConnsText = appMinConnsInput.text;
            string appRatioText = appRatioInput.text;

            int appConnThreshold;

            if (int.TryParse(appConnThresholdText, out appConnThreshold))
            {
                configurations.retrievedThresholds.apps.connThreshold = appConnThreshold;
            }

            int appMinConns;

            if (int.TryParse(appMinConnsText, out appMinConns))
            {
                configurations.retrievedThresholds.apps.minimumConns = appMinConns;

            }

            float appRatio;

            if (float.TryParse(appRatioText, out appRatio))
            {
                configurations.retrievedThresholds.apps.connOpRatioThreshold = appRatio;
            }

            TMP_Text appConnThresholdPlaceholder = appConnThresholdInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
            TMP_Text appMinConnsPlaceholder = appMinConnsInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
            TMP_Text appRatioPlaceholder = appRatioInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

            appConnThresholdInput.text = "";
            appMinConnsInput.text = "";
            appRatioInput.text = "";

            appConnThresholdPlaceholder.text = "";
            appMinConnsPlaceholder.text = "";
            appRatioPlaceholder.text = "";

            //QMGR Panel

            TMP_InputField qmgrErrorInput = QmgrConfig.transform.Find("errorthreshold/input").GetComponent<TMP_InputField>();
            TMP_InputField qmgrMaxConnsInput = QmgrConfig.transform.Find("maxconns/input").GetComponent<TMP_InputField>();
            TMP_InputField qmgrMaxMQOpsInput = QmgrConfig.transform.Find("operations/input").GetComponent<TMP_InputField>();

            string qmgrErrorText = qmgrErrorInput.text;
            string qmgrMaxConnsText = qmgrMaxConnsInput.text;
            string qmgrMaxMQOpsText = qmgrMaxMQOpsInput.text;

            int qmgrError;

            if (int.TryParse(qmgrErrorText, out qmgrError))
            {
                configurations.retrievedThresholds.queue_manager.errorThreshold = qmgrError;
            }

            int qmgrMaxConns;

            if (int.TryParse(qmgrMaxConnsText, out qmgrMaxConns))
            {
                configurations.retrievedThresholds.queue_manager.maxMQConns = qmgrMaxConns;
            }

            int qmgrMaxMQOps;

            if (int.TryParse(qmgrMaxMQOpsText, out qmgrMaxMQOps))
            {
                configurations.retrievedThresholds.queue_manager.maxMQOps = qmgrMaxMQOps;
            }

            qmgrErrorInput.text = "";
            qmgrMaxConnsInput.text = "";
            qmgrMaxMQOpsInput.text = "";

            TMP_Text qmgrErrorPlaceholder = qmgrErrorInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
            TMP_Text qmgrMaxConnsPlaceholder = qmgrMaxConnsInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
            TMP_Text qmgrMaxMQOpsPlaceholder = qmgrMaxMQOpsInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

            qmgrErrorPlaceholder.text = "";
            qmgrMaxConnsPlaceholder.text = "";
            qmgrMaxMQOpsPlaceholder.text = "";

            string jsonString = JsonConvert.SerializeObject(configurations, Formatting.Indented);

            ResponseMessage responseMessage = await postRequestConfig();
            if (responseMessage.message == "Configuration updated successfully.")
            {
                configurations = await GetConfigs();
                HandleConfigs(configurations);
                ErrorText.text = ""; // empty it out before the update message is shown
                StartCoroutine(UpdateSuccess());
            }
            else
            {
                UpdateText.text = ""; // empty it out before the error message is shown
                StartCoroutine(UpdateFailed(responseMessage));
            }
        }
        catch
        {
            Debug.LogWarning("An error occurred while submitting configurations.");
        }
    }

    public async Task<RootThresholds> GetConfigs()
    {
        try
        {
            // Get request to obtain the configurations from flask
            string thresholdJson = await Utilities.Instance.GetRequest(thresholdURI);

            // Deserialize configuration for use
            configurations = JsonConvert.DeserializeObject<RootThresholds>(thresholdJson);
            Debug.Log("Success: configurations retrieved.");
            return configurations;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching or parsing configuration: {ex.Message}");
            return null;
        }
    }

    public async Task<ResponseMessage> postRequestConfig()
    {
        try
        {
            string response = await Utilities.Instance.PostRequestConfig(thresholdURI, configurations);
            ResponseMessage postResponse = JsonConvert.DeserializeObject<ResponseMessage>(response);
            Debug.Log("Success: configurations posted.");
            return postResponse;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error posting or parsing configuration: {ex.Message}");
            return null;
        }
    }

    private IEnumerator UpdateSuccess()
    {
        UpdateText.text = "Update Successful!";
        yield return new WaitForSeconds(5); // wait for seconds
        UpdateText.text = "";
    }

    private IEnumerator UpdateFailed(ResponseMessage responseMessage)
    {
        ErrorText.text = responseMessage.message;
        yield return new WaitForSeconds(7); // wait for seconds
        ErrorText.text = "";
    }

    private void ClearParent(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Success: ClearParent");
    }

    private void ClearSearchManager()
    {
        foreach (Transform child in parentTransform)
        {
            queueConfigSearchManager.RemoveConfigButton(child.gameObject);
        }
        Debug.Log("Success: ClearSearchManager");
    }

}

