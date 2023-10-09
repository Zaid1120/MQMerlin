using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FlaskServer;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections;

public class Issues : MonoBehaviour
{
    public GameObject CritialIssuePrefab;
    public GameObject WarningIssuePrefab;
    public GameObject PanelTooMany2035sQMGR;
    public GameObject PanelTooMany2035sQueue;
    public GameObject PanelTooMuchActivtyQueue;
    public GameObject PanelTooMuchActivtyQMGR;
    public GameObject PanelTooMany2085s;
    public GameObject PanelMisconfigApp;
    public GameObject PanelThresholdExceeded;
    public GameObject PanelQueueServiceHigh;
    public GameObject PanelQueueFull;

    public Transform panelParent;
    public Transform issueParent;

    public TMP_Text IssueCount;
    public TMP_Text errorText;

    public IssuePanelController issuePanelController; //this is for sending issue to chatbot
    public Resolve resolveManager; // for resolving issue
    private int lastChildCount;

    string uri = "https://127.0.0.1:5000/issues";

    public static List<(string, string)> objectStatusList = new List<(string, string)>();

    public Dictionary<(string, string), Issue> issuesDictionary = new Dictionary<(string, string), Issue>();
    public static Dictionary<string, Dictionary<string, int>> colour_indicator = new Dictionary<string, Dictionary<string, int>>();
    public Dictionary<GameObject, GameObject> buttonToPanelMap = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, Issue> panelToIssueMap = new Dictionary<GameObject, Issue>(); // new

    public IssueSearchManager issueSearchManager;

    private void Awake()
    {
        Utilities.Instance.ClearParent(panelParent);
        Utilities.Instance.ClearParent(issueParent);
        issuesDictionary.Clear();
        objectStatusList.Clear();
        buttonToPanelMap.Clear();
        panelToIssueMap.Clear();
        colour_indicator.Clear();
    }

    void OnEnable()
    {
        InvokeRepeating(nameof(GetIssuesFromServer), 0f, 30f);
        issuesDictionary.Clear();
        objectStatusList.Clear();
        buttonToPanelMap.Clear();
        panelToIssueMap.Clear();
        colour_indicator.Clear();

        // initialise the lastChildCount variable
        int numberOfChildren = issueParent.childCount;
        UpdateText(lastChildCount);
        ClearSearchManager();
    }

    void OnDisable()
    {
        CancelInvoke(nameof(GetIssuesFromServer));
    }

    private void GetIssuesFromServer()
    {
        _ = FetchAndProcessIssues();
    }

    private async Task FetchAndProcessIssues()
    {
        string getResponse = await Utilities.Instance.GetRequest(uri);
        IssueCollection issueCollection = JsonConvert.DeserializeObject<IssueCollection>(getResponse);

        foreach (var issue in issueCollection.Issues)
        {
            var key = (issue.mqobjectName, issue.issueCode);

            if (issuesDictionary.ContainsKey(key))
            {
                UpdatePrefab(issue);
            }
            else
            {
                CreatePrefab(issue);
                issuesDictionary[key] = issue;
            }
        }
    }

    private void CreatePrefab(Issue issue)
    {
        try
        {
            GameObject issueObject = null;
            GameObject detailsPanel = null;

            if (issue.issueCode == "Too_Much_Activity") //Too_Much_Activity - critical
            {
                issueObject = Instantiate(CritialIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Too Much Activity");

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["critical"] += 1;

                if (issue.mqobjectType == "<QMGR>")
                {
                    detailsPanel = Instantiate(PanelTooMuchActivtyQMGR, panelParent);
                    SetTextMeshProText(detailsPanel, "Content/details_list/connectionrate/value", issue.technicalDetails.archivedConnRates[issue.technicalDetails.archivedConnRates.Count - 1]);
                }

                else //if queue
                {
                    detailsPanel = Instantiate(PanelTooMuchActivtyQueue, panelParent);
                }

                //common stuff between panels
                SetTextMeshProText(detailsPanel, "Content/details_list/requestrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]);
                detailsPanel.SetActive(false);
            }

            else if (issue.issueCode == "Too_Many_2035s") //Too_many_errors - critical
            {

                issueObject = Instantiate(CritialIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Too Many Errors"); //name the button

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["critical"] += 1;

                if (issue.mqobjectType == "<QMGR>")
                {
                    detailsPanel = Instantiate(PanelTooMany2035sQMGR, panelParent);

                    SetTextMeshProText(detailsPanel, "Content/details_list/channelname/value", issue.technicalDetails.channelName);
                    SetTextMeshProText(detailsPanel, "Content/details_list/connectionname/value", issue.technicalDetails.connName);
                    SetTextMeshProText(detailsPanel, "Content/details_list/cspuserid/value", issue.technicalDetails.CSPUserId);
                }
                else
                {
                    detailsPanel = Instantiate(PanelTooMany2035sQueue, panelParent);
                }

                //// name common stuff on the panel
                SetTextMeshProText(detailsPanel, "Content/details_list/userId/value", issue.technicalDetails.userId);
                SetTextMeshProText(detailsPanel, "Content/details_list/requestsrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/appname/value", issue.technicalDetails.appName);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", Utilities.Instance.FormatDateTimetoHHmmss(issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]));
                detailsPanel.SetActive(false);
            }

            else if (issue.issueCode == "Too_Many_2085s") //Too_Much_Activity - critical
            {
                issueObject = Instantiate(CritialIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Too Many Errors"); //name the button

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["critical"] += 1;

                detailsPanel = Instantiate(PanelTooMany2085s, panelParent);

                SetTextMeshProText(detailsPanel, "Content/details_list/requestsrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", Utilities.Instance.FormatDateTimetoHHmmss(issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]));
                SetTextMeshProText(detailsPanel, "Content/details_list/appname/value", issue.technicalDetails.appName);
                SetTextMeshProText(detailsPanel, "Content/details_list/channelname/value", issue.technicalDetails.channelName);
                SetTextMeshProText(detailsPanel, "Content/details_list/connectionname/value", issue.technicalDetails.connName);
                SetTextMeshProText(detailsPanel, "Content/details_list/queuename/value", issue.technicalDetails.QName);

                detailsPanel.SetActive(false);
            }

            else if (issue.issueCode == "Misconfigured_Connection_Pattern") //Misconfigured_Connection_Pattern - warning
            {
                issueObject = Instantiate(WarningIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Misconfigured Application");

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["warning"] += 1;

                detailsPanel = Instantiate(PanelMisconfigApp, panelParent);
                SetTextMeshProText(detailsPanel, "Content/issue/issue_text", "Misconfigured Application");

                SetTextMeshProText(detailsPanel, "Content/details_list/numberofconnections/value", issue.technicalDetails.archivedconns[issue.technicalDetails.archivedconns.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/logtimerange/value", issue.technicalDetails.archivedlogTimes[issue.technicalDetails.archivedlogTimes.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/userratio/value", issue.technicalDetails.archiveduserRatio[issue.technicalDetails.archiveduserRatio.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requestcount/value", issue.technicalDetails.archivedRequestCount[issue.technicalDetails.archivedRequestCount.Count - 1]);

                detailsPanel.SetActive(false);
            }

            else if (issue.issueCode == "Threshold_Exceeded") // warning
            {
                if (WarningIssuePrefab == null)
                {
                    Debug.LogError("WarningIssuePrefab is null!");
                }

                if (issueParent == null)
                {
                    Debug.LogError("issueParent is null!");
                }

                issueObject = Instantiate(WarningIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Threshold Exceeded");

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["warning"] += 1;

                detailsPanel = Instantiate(PanelThresholdExceeded, panelParent);
                detailsPanel.SetActive(false);

                SetTextMeshProText(detailsPanel, "Content/details_list/maxthreshold/value", issue.technicalDetails.maxThreshold);
            }

            else if (issue.issueCode == "Queue_Full") //critical
            {
                issueObject = Instantiate(CritialIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "Queue Capacity Full");

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["critical"] += 1;

                detailsPanel = Instantiate(PanelQueueFull, panelParent);
                detailsPanel.SetActive(false);

                SetTextMeshProText(detailsPanel, "Content/details_list/maxthreshold/value", issue.technicalDetails.maxThreshold);
            }

            else if (issue.issueCode == "Queue_Service_High") //warning
            {
                issueObject = Instantiate(WarningIssuePrefab, issueParent);
                issueSearchManager.AddIssueButton(issueObject);

                SetTextMeshProText(issueObject, "IssueName", "High Queue Service");

                // Update colour_indicator
                if (!colour_indicator.ContainsKey(issue.mqobjectName))
                {
                    // initialise new entry with warning and critical keys
                    colour_indicator[issue.mqobjectName] = new Dictionary<string, int> { { "warning", 0 }, { "critical", 0 } };
                }

                // Increment the 'critical' count by 1
                colour_indicator[issue.mqobjectName]["warning"] += 1;

                detailsPanel = Instantiate(PanelQueueServiceHigh, panelParent);

                SetTextMeshProText(detailsPanel, "Content/details_list/timeSinceReset/value", $"{issue.technicalDetails.timeSinceReset} seconds");
                SetTextMeshProText(detailsPanel, "Content/details_list/highQDepth/value", issue.technicalDetails.highQDepth);
                SetTextMeshProText(detailsPanel, "Content/details_list/enQCount/value", issue.technicalDetails.enQCount);
                SetTextMeshProText(detailsPanel, "Content/details_list/deQCount/value", issue.technicalDetails.deQCount);

                detailsPanel.SetActive(false);
            }

            // assign details to buttons
            SetTextMeshProText(issueObject, "ObjectName", issue.mqobjectName);
            SetTextMeshProText(issueObject, "DatetimeText", Utilities.Instance.FormatDateTimetoHHmm(issue.startTimeStamp));

            // assign details to panels
            SetTextMeshProText(detailsPanel, "Content/issue/issue_text", issue.issueCode);
            SetTextMeshProText(detailsPanel, "Content/objecttype/objecttype_text", issue.mqobjectType);
            SetTextMeshProText(detailsPanel, "Content/object/object_text", issue.mqobjectName);
            SetTextMeshProText(detailsPanel, "Content/description/description_text", issue.generalDesc);

            SetTextMeshProText(detailsPanel, "Content/starttime/starttime_text", Utilities.Instance.FormatDateTimetoYMDHHmmss(issue.startTimeStamp));
            SetTextMeshProText(detailsPanel, "Content/currenttime/currenttime_text", Utilities.Instance.FormatDateTimetoYMDHHmmss(issue.startTimeStamp));

            issue.PrefabInstance = issueObject;

            buttonToPanelMap[issueObject] = detailsPanel;
            panelToIssueMap[detailsPanel] = issue;  // Save the association - new

            Button buttonComponent = issueObject.GetComponent<Button>();
            if (buttonComponent)
            {
                var currentIssueObject = issueObject; // create a local copy
                buttonComponent.onClick.AddListener(() =>
                {
                    DeactivateAllPanels();
                    if (buttonToPanelMap.TryGetValue(currentIssueObject, out GameObject panel))
                    {
                        panel.SetActive(true);
                    }
                });
            }

            // Search for the SendChatbot button in the detailsPanel
            Button sendChatbotButton = detailsPanel.transform.Find("SendChatbot").GetComponent<Button>();
            if (sendChatbotButton)
            {
                sendChatbotButton.onClick.AddListener(() =>
                {
                    if (panelToIssueMap.TryGetValue(detailsPanel, out Issue associatedIssue))
                    {
                        issuePanelController.SendSystemMessage(associatedIssue); //uncomment this

                    }
                });
            }

            // Search for the resolve button in the detailsPanel
            Button resolveButton = detailsPanel.transform.Find("Resolve").GetComponent<Button>();
            if (resolveButton)
            {
                resolveButton.onClick.AddListener(() =>
                {
                    if (panelToIssueMap.TryGetValue(detailsPanel, out Issue associatedIssue))
                    {
                        resolveManager.showPanel(associatedIssue);
                    }
                });
            }
            Debug.Log("Success: Issue and details panel instantiated.");

        }
        catch (Exception ex)
        {
            StartCoroutine(ErrorOccured(ex));
            Debug.LogError($" An issue has occured while getting issues: {ex.Message}");
        }
    }

    private void UpdatePrefab(Issue issue)
    {
        try
        {
            var key = (issue.mqobjectName, issue.issueCode);

            // we know the prefab exists because this method was called only if the key exists in our dictionary
            Issue existingIssue = issuesDictionary[key];
            if (existingIssue == null)
            {
                Debug.LogError("existingIssue is null for key: " + key);
                return;
            }

            GameObject issueObject = existingIssue.PrefabInstance;

            if (issueObject == null)
            {
                Debug.LogError($"issueObject is null before SetTextMeshProText for {issue.issueCode}.");
            }

            GameObject detailsPanel = buttonToPanelMap[issueObject];

            SetTextMeshProText(detailsPanel, "Content/currenttime/currenttime_text", Utilities.Instance.FormatDateTimetoYMDHHmmss(issue.startTimeStamp));
            SetTextMeshProText(detailsPanel, "Content/description/description_text", issue.generalDesc);


            if (issue.issueCode == "Too_Many_2035s")
            {
                if (issue.mqobjectType == "<QMGR>")
                {
                    SetTextMeshProText(detailsPanel, "Content/details_list/channelname/value", issue.technicalDetails.channelName);
                    SetTextMeshProText(detailsPanel, "Content/details_list/connectionname/value", issue.technicalDetails.connName);
                    SetTextMeshProText(detailsPanel, "Content/details_list/cspuserid/value", issue.technicalDetails.CSPUserId);
                }

                // name common stuff on the panel
                SetTextMeshProText(detailsPanel, "Content/details_list/userId/value", issue.technicalDetails.userId);
                SetTextMeshProText(detailsPanel, "Content/details_list/requestsrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/appname/value", issue.technicalDetails.appName);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", Utilities.Instance.FormatDateTimetoHHmmss(issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]));
            }


            else if (issue.issueCode == "Too_Much_Activity") //Too_Much_Activity
            {
                if (issue.mqobjectType == "<QMGR>")
                {
                    SetTextMeshProText(detailsPanel, "Content/details_list/connectionrate/value", issue.technicalDetails.archivedConnRates[issue.technicalDetails.archivedConnRates.Count - 1]);
                }

                //common stuff between panels
                SetTextMeshProText(detailsPanel, "Content/details_list/requestrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]);
            }


            else if (issue.issueCode == "Too_Many_2085s") //Too_Much_Activity
            {
                SetTextMeshProText(detailsPanel, "Content/details_list/requestsrate/value", issue.technicalDetails.archivedRequestRates[issue.technicalDetails.archivedRequestRates.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requesttimestamp/value", Utilities.Instance.FormatDateTimetoHHmmss(issue.technicalDetails.archivedTimestamps[issue.technicalDetails.archivedTimestamps.Count - 1]));
                SetTextMeshProText(detailsPanel, "Content/details_list/appname/value", issue.technicalDetails.appName);
                SetTextMeshProText(detailsPanel, "Content/details_list/channelname/value", issue.technicalDetails.channelName);
                SetTextMeshProText(detailsPanel, "Content/details_list/connectionname/value", issue.technicalDetails.connName);
                SetTextMeshProText(detailsPanel, "Content/details_list/queuename/value", issue.technicalDetails.QName);
            }

            else if (issue.issueCode == "Misconfigured_Connection_Pattern") //Misconfigured_Connection_Pattern
            {
                SetTextMeshProText(detailsPanel, "Content/details_list/logtimerange/value", issue.technicalDetails.archivedconns[issue.technicalDetails.archivedconns.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/numberofconnections/value", issue.technicalDetails.archivedlogTimes[issue.technicalDetails.archivedlogTimes.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/userratio/value", issue.technicalDetails.archiveduserRatio[issue.technicalDetails.archiveduserRatio.Count - 1]);
                SetTextMeshProText(detailsPanel, "Content/details_list/requestcount/value", issue.technicalDetails.archivedRequestCount[issue.technicalDetails.archivedRequestCount.Count - 1]);
            }

            else if (issue.issueCode == "Threshold_Exceeded")
            {
                SetTextMeshProText(detailsPanel, "Content/details_list/maxthreshold/value", issue.technicalDetails.maxThreshold);
            }

            else if (issue.issueCode == "Queue_Full")
            {
                SetTextMeshProText(detailsPanel, "Content/details_list/maxthreshold/value", issue.technicalDetails.maxThreshold);
            }

            else if (issue.issueCode == "Queue_Service_High")
            {
                SetTextMeshProText(detailsPanel, "Content/details_list/timeSinceReset/value", $"{issue.technicalDetails.timeSinceReset} seconds");
                SetTextMeshProText(detailsPanel, "Content/details_list/highQDepth/value", issue.technicalDetails.highQDepth);
                SetTextMeshProText(detailsPanel, "Content/details_list/enQCount/value", issue.technicalDetails.enQCount);
                SetTextMeshProText(detailsPanel, "Content/details_list/deQCount/value", issue.technicalDetails.deQCount);
            }
            Debug.Log("Success: Issue and details panel updated.");

        }
        catch (Exception ex)
        {
            StartCoroutine(ErrorOccured(ex));
            Debug.LogError($" An issue has occured while getting issues: {ex.Message}");
        }
    }


    // coroutine to show error for limited time
    private IEnumerator ErrorOccured(Exception ex)
    {
        errorText.text = "An error occurred: " + ex.Message;
        yield return new WaitForSeconds(10); // wait for seconds
        errorText.text = "";
    }


    private void SetTextMeshProText(GameObject parent, string childName, string text)
    {
        // early exit if the parent GameObject is null
        if (parent == null)
        {
            Debug.LogWarning("SetTextMeshProText: parent GameObject is null.");
            return;
        }

        Transform textTransform = parent.transform.Find(childName);
        if (textTransform != null)
        {
            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            else
            {
                Debug.LogWarning("SetTextMeshProText: TextMeshProUGUI component not found on " + childName);
            }
        }
        else
        {
            Debug.LogWarning("SetTextMeshProText: Could not find child with name " + childName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the child count has changed
        if (lastChildCount != issueParent.childCount)
        {
            // Update the lastChildCount variable
            lastChildCount = issueParent.childCount;

            // Update the text object
            UpdateText(lastChildCount);
        }
    }

    void UpdateText(int childCount)
    {
        // Set the text to display the current number of children
        IssueCount.text = $"Number of Issues: {childCount}";
    }

    private void ClearSearchManager()
    {
        foreach (Transform child in issueParent)
        {
            Image img = child.GetComponent<Image>();
            if (img != null) Destroy(img);
            issueSearchManager.RemoveIssueButton(child.gameObject);
            Destroy(child.gameObject);
        }

        foreach (Transform child in panelParent)
        {
            Image img = child.GetComponent<Image>();
            if (img != null) Destroy(img);
            issueSearchManager.RemoveIssueButton(child.gameObject);
            Destroy(child.gameObject);
        }
        Debug.Log("Success: ClearSearchManager");
    }


    // Deactivates all panels stored in the buttonToPanelMap.
    public void DeactivateAllPanels()
    {
        foreach (var panel in buttonToPanelMap.Values)
        {
            // Check if the panel is null before attempting to set it inactive.
            if (panel != null)
            {
                panel.SetActive(false);
            }
            else
            {
                Debug.LogError("A panel in buttonToPanelMap is null.");
            }
        }
    }
}
