using System.Collections.Generic;
using UnityEngine;
using ReceiveData;
using UnityEngine.UI;
using TMPro;
using System;

public class Instantiation : MonoBehaviour
{
    // reference to the GetDetails script
    private GetDetails _callCode;

    // reference to the inspector panels
    public GameObject queueLocalInspectPanel;
    public GameObject queueAliasInspectPanel;
    public GameObject queueRemoteInspectPanel;
    public GameObject queueTransInspectPanel;

    public GameObject channelInspectPanel;
    public GameObject applicationInspectPanel;

    // reference to the search managers
    public QueueSearchManager queueSearchManager;
    public ChannelSearchManager channelSearchManager;
    public ApplicationSearchManager applicationSearchManager;

    // reference to prefabs and parents for instantiating buttons
    public GameObject QueuePrefab;

    //buttons to instantiate
    public GameObject QueueLocalRunPrefab;
    public GameObject QueueLocalWarnPrefab;
    public GameObject QueueLocalCritPrefab;

    public GameObject QueueAliasRunPrefab;
    public GameObject QueueAliasWarnPrefab;
    public GameObject QueueAliasCritPrefab;

    public GameObject QueueTransRunPrefab;
    public GameObject QueueTransWarnPrefab;
    public GameObject QueueTransCritPrefab;

    public GameObject QueueRemoteRunPrefab;
    public GameObject QueueRemoteWarnPrefab;
    public GameObject QueueRemoteCritPrefab;

    public GameObject ChannelSDRPrefab;
    public GameObject ChannelRCVRPrefab;
    public GameObject ChannelSVRPrefab;
    public GameObject ChannelSVRCONNPrefab;
    public GameObject ChannelCLNTCONNPrefab;
    public GameObject ChannelCLUSSDRPrefab;
    public GameObject ChannelCLUSRCVRPrefab;
    public GameObject ChannelRQSTRPrefab;

    public GameObject ApplicationPrefab;

    //parent objects under which to instantiate the buttons
    public Transform QueueParent;
    public Transform ChannelParent;
    public Transform ApplicationParent;

    // store instantiated buttons    
    private Dictionary<string, GameObject> instantiatedQueueButtons = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> instantiatedChannelButtons = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> instantiatedApplicationButtons = new Dictionary<string, GameObject>();

    public TMP_Text errorText;

    private void Awake()
    { // Initial cleanup
        // Find GameObject with GetDetails component and get a reference to it
        GameObject callCodeObject = GameObject.Find("GetDetails"); // Finds the GameObject
        _callCode = callCodeObject.GetComponent<GetDetails>(); // Finds the code attached to the GameObject
    }

    private void OnEnable()
    {
        CleanUp();

        // Subscribe to the OnDataFetched event
        _callCode.OnDataFetched += AddButton;
    }

    private void Start()
    {
        // set inspector panels to inactive at the start
        queueLocalInspectPanel.SetActive(false);
        queueAliasInspectPanel.SetActive(false);
        queueRemoteInspectPanel.SetActive(false);
        queueTransInspectPanel.SetActive(false);
        channelInspectPanel.SetActive(false);
        applicationInspectPanel.SetActive(false);
    }

    // Unsubscribe when the object is deactivated to prevent memory leaks
    private void OnDisable()
    {
        if (_callCode != null)
        { // only try to unsubscribe if _callCode is not null
            _callCode.OnDataFetched -= AddButton;
        }
    }

    private void AddButton()
    {
        try
        {
            // Instantiating queues
            if (_callCode.QueuesResult?.All_Queues != null)
            {
                ClearOldButtons(instantiatedQueueButtons, queueSearchManager);
                foreach (var queue in _callCode.QueuesResult.All_Queues)
                {
                    switch (queue.type_name)
                    {
                        case "Local":
                            InstantiateQueueButton(queue, QueueLocalCritPrefab, QueueLocalWarnPrefab, QueueLocalRunPrefab, QueueParent, queueLocalInspectPanel);
                            break;
                        case "Alias":
                            InstantiateQueueButton(queue, QueueAliasCritPrefab, QueueAliasWarnPrefab, QueueAliasRunPrefab, QueueParent, queueAliasInspectPanel);
                            break;
                        case "Transmission":
                            InstantiateQueueButton(queue, QueueTransCritPrefab, QueueTransWarnPrefab, QueueTransRunPrefab, QueueParent, queueTransInspectPanel);
                            break;
                        default: // assuming "Remote"
                            InstantiateQueueButton(queue, QueueRemoteCritPrefab, QueueRemoteWarnPrefab, QueueRemoteRunPrefab, QueueParent, queueRemoteInspectPanel);
                            break;
                    }
                }

                Debug.Log("Success: Queues successfully instantiated for monitoring");
            }

            // Instantiating channels
            if (_callCode.ChannelsResult?.All_Channels != null)
            {
                ClearOldButtons(instantiatedChannelButtons, channelSearchManager);
                foreach (var channel in _callCode.ChannelsResult.All_Channels)
                {
                    switch (channel.channel_type)
                    {
                        case "SDR":
                            InstantiateChannelButton(channel, ChannelSDRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "RCVR":
                            InstantiateChannelButton(channel, ChannelRCVRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "SVRCONN":
                            InstantiateChannelButton(channel, ChannelSVRCONNPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "CLUSRCVR":
                            InstantiateChannelButton(channel, ChannelCLUSRCVRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "CLUSSDR":
                            InstantiateChannelButton(channel, ChannelCLUSSDRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "RQSTR":
                            InstantiateChannelButton(channel, ChannelRQSTRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "CLNTCONN":
                            InstantiateChannelButton(channel, ChannelCLNTCONNPrefab, ChannelParent, channelInspectPanel);
                            break;
                        case "SVR":
                            InstantiateChannelButton(channel, ChannelSVRPrefab, ChannelParent, channelInspectPanel);
                            break;
                        default:
                            Debug.LogWarning($"Unknown channel type '{channel.channel_type}' encountered.");
                            break;
                    }
                }
                Debug.Log("Success: Channels successfully instantiated for monitoring");
            }

            // Instantiating applications

            if (_callCode.ApplicationsResult?.All_Applications != null)
            {
                ClearOldButtons(instantiatedApplicationButtons, applicationSearchManager);
                foreach (var application in _callCode.ApplicationsResult.All_Applications)
                {
                    InstantiateApplicationButton(application, ApplicationPrefab, ApplicationParent, applicationInspectPanel);
                }
                Debug.Log("Success: Applications successfully instantiated for monitoring");
            }
        }
        catch (Exception ex)
        {
            errorText.text = "An error occurred: " + ex.Message;
            errorText.gameObject.SetActive(true);
            Debug.LogError("Error in SubmitForm: " + ex.ToString());
        }
    } // end of AddButton()

    private GameObject DetermineQueuePrefab(Queue queue, string typeName, GameObject runPrefab, GameObject warnPrefab, GameObject critPrefab)
    {
        try
        {
            if (Issues.colour_indicator.ContainsKey(queue.queue_name))
            {
                if (Issues.colour_indicator[queue.queue_name]["critical"] > 0)
                {
                    return critPrefab;
                }
                else if (Issues.colour_indicator[queue.queue_name]["warning"] > 0)
                {
                    return warnPrefab;
                }
            }
            return runPrefab;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error determining prefab for {typeName}: {ex.Message}");
            return runPrefab; // default to the running prefab
        }
    }

    // method to instantiate queue button and assign its associated panel detals
    private void InstantiateQueueButton(Queue queue, GameObject critPrefab, GameObject warnPrefab, GameObject runPrefab, Transform parentTransform, GameObject inspectPanel)
    {
        try
        {
            GameObject queuePrefab = DetermineQueuePrefab(queue, queue.type_name, runPrefab, warnPrefab, critPrefab);
            var queue_button = Instantiate(queuePrefab, parentTransform);
            queue_button.GetComponent<QueueButton>().inspectPanel = inspectPanel;
            queue_button.SetActive(true);
            queue_button.GetComponentInChildren<TextMeshProUGUI>().text = queue.queue_name;
            Button btn = queue_button.GetComponent<Button>();
            btn.onClick.AddListener(() => queue_button.GetComponent<QueueButton>().ToggleInspectorPanel(queue));
            instantiatedQueueButtons[queue.queue_name] = queue_button;
            queueSearchManager?.AddQueueButton(queue_button);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error instantiating queue button for {queue.queue_name}: {ex.Message}");
        }
    }

    // method to instantiate channel button and assign its associated panel detals
    private void InstantiateChannelButton(Channel channel, GameObject prefab, Transform parentTransform, GameObject inspectPanel)
    {
        try
        {
            var channel_button = Instantiate(prefab, parentTransform);
            channel_button.GetComponent<ChannelButton>().inspectPanel = inspectPanel;
            channel_button.SetActive(true);
            channel_button.GetComponentInChildren<TextMeshProUGUI>().text = channel.channel_name;
            Button btn = channel_button.GetComponent<Button>();
            btn.onClick.AddListener(() => channel_button.GetComponent<ChannelButton>().ToggleInspectorPanel(channel));
            instantiatedChannelButtons[channel.channel_name] = channel_button;
            channelSearchManager?.AddButton(channel_button);  // Assuming ChannelSearchManager has implemented ISearchManager
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error instantiating channel button for {channel.channel_name}: {ex.Message}");
        }
    }

    // method to instantiate app button and assign its associated panel detals
    private void InstantiateApplicationButton(ReceiveData.Application application, GameObject prefab, Transform parentTransform, GameObject inspectPanel)
    {
        try
        {
            var application_button = Instantiate(prefab, parentTransform);
            application_button.GetComponent<ApplicationButton>().inspectPanel = inspectPanel;
            application_button.SetActive(true);
            application_button.GetComponentInChildren<TextMeshProUGUI>().text = application.conn;  // conn is the name of the app.
            Button btn = application_button.GetComponent<Button>();
            btn.onClick.AddListener(() => application_button.GetComponent<ApplicationButton>().ToggleInspectorPanel(application));
            instantiatedApplicationButtons[application.conn] = application_button;
            applicationSearchManager?.AddButton(application_button);  // Assuming ApplicationSearchManager has implemented ISearchManager
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error instantiating application button for {application.conn}: {ex.Message}");
        }
    }

    // miscellaneous code
    private void ClearOldButtons(Dictionary<string, GameObject> buttonDict, MonitoringSearchManager searchManager = null)
    {
        try
        {
            foreach (var oldButton in buttonDict.Values)
            {
                searchManager?.RemoveButton(oldButton);
                Destroy(oldButton);
            }
            buttonDict.Clear();
            Debug.Log("Success: Old Buttons Cleared");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error clearing old buttons: {ex.Message}");
        }
    }

    private void ClearParent(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CleanupQueueButtons()
    {
        foreach (var oldButton in instantiatedQueueButtons.Values)
        {
            if (queueSearchManager != null)
            {
                queueSearchManager.RemoveQueueButton(oldButton);
            }
            Destroy(oldButton);
        }
        instantiatedQueueButtons.Clear();
    }

    private void CleanupChannelButtons()
    {
        foreach (var oldButton in instantiatedChannelButtons.Values)
        {
            if (channelSearchManager != null)
            {
                channelSearchManager.RemoveChannelButton(oldButton);
            }
            Destroy(oldButton);
        }
        instantiatedChannelButtons.Clear();
    }

    private void CleanUp()
    {
        CleanupQueueButtons();
        CleanupChannelButtons();
        CleanupApplicationButtons();
        ClearParent(QueueParent);
        ClearParent(ChannelParent);
        ClearParent(ApplicationParent);
    }

    private void CleanupApplicationButtons()
    {
        foreach (var oldButton in instantiatedApplicationButtons.Values)
        {
            if (applicationSearchManager != null)
            {
                applicationSearchManager.RemoveApplicationButton(oldButton);
            }
            Destroy(oldButton);
        }
        instantiatedApplicationButtons.Clear();
    }
}