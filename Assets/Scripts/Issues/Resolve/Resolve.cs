using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System;
using FlaskServer;
using System.Collections.Generic;
using System.Collections;

public class Resolve : MonoBehaviour
{
    public GameObject resolvePanel;
    public GameObject panelContent;
    public GameObject DesktopMessage;
    public Toggle yesToggle;
    public Toggle noToggle;
    public TMP_InputField inputField;
    public Slider slider;
    public Issues issueManager;
    public Transform instantiatedIssueButtons;
    public Transform instantiatedIssuePanels;
    private TMP_Text issueTextComponent;
    private TMP_Text objectNameTextComponent;
    private TMP_Text ratingTextComponent;
    public TMP_Text responseText;
    public TMP_Text QueueManagerName;

    public IssueSearchManager issueSearchManager;

    string uri = "https://127.0.0.1:5000/resolve";

    //flags to make sure user interacts with the panel
    private bool hasInteractedWithYesToggle = false;
    private bool hasInteractedWithNoToggle = false;
    private bool hasInteractedWithSlider = false;

    private void Awake()
    {
        issueTextComponent = panelContent.transform.Find("Issue_text").GetComponent<TMP_Text>();
        objectNameTextComponent = panelContent.transform.Find("ObjectName_text").GetComponent<TMP_Text>();
        ratingTextComponent = panelContent.transform.Find("Slider_value").GetComponent<TMP_Text>();
    }

    public async void CreateOrUpdateLog()
    {
        try
        {
            //unique conditions below

            // No interactions with both toggles and slider
            if (!hasInteractedWithSlider && !(hasInteractedWithYesToggle || hasInteractedWithNoToggle))
            {
                responseText.text = "Please answer all questions before proceeding.";
                responseText.gameObject.SetActive(true);
                return;
            }

            // Interacted with toggles but not with slider
            if ((hasInteractedWithYesToggle || hasInteractedWithNoToggle) && !hasInteractedWithSlider)
            {
                responseText.text = "Please give a rating before proceeding.";
                responseText.gameObject.SetActive(true);
                return;
            }

            // Interacted with slider but not with toggles
            if (hasInteractedWithSlider && !(yesToggle.isOn || noToggle.isOn))
            {
                responseText.text = "Please select a toggle option before proceeding.";
                responseText.gameObject.SetActive(true);
                return;
            }

            string filename = "ResolveLog.txt";  // Replace this with the desired name for your file

            // Determine the desktop path based on OS
            string desktopPath = "";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix ||
                     Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                desktopPath = Environment.GetEnvironmentVariable("HOME") + "/Desktop";
            }
            else
            {
                StartCoroutine(PlatformNotFound());
                Debug.LogWarning("Unknown operating system. Using current directory.");
                desktopPath = Directory.GetCurrentDirectory();
            }

            string folderPath = Path.Combine(desktopPath, "Recall_Log");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Success: Recall_Log folder created.");
            }

            string path = Path.Combine(folderPath, filename);

            if (!File.Exists(path)) //create file if doesn't exist
            {
                File.WriteAllText(path, "");
                Debug.Log("Success: Log file created.");
            }

            string issueText = issueTextComponent.text;
            string objectName = objectNameTextComponent.text;
            string rating = ratingTextComponent.text;
            string feedback = inputField.text;
            string chatbotUse = yesToggle.isOn ? "Yes" : "No";
            string qmgr = QueueManagerName.text;


            //posting to flask
            // Prepare the data in the required Dictionary format
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "mqobjectName", objectName },
                { "issueCode", issueText }
            };


            // Call the existing PostRequest method 
            string response = await Utilities.Instance.PostRequest(uri, data);

            if (string.IsNullOrEmpty(response))
            {
                responseText.text = "Failed to post data. Server may not be active";
                responseText.gameObject.SetActive(true);
                Debug.LogWarning("Failed to post data. Server may not be active");
                return;
            }

            //log file content
            DateTime currentDateTime = DateTime.Now;
            string content = $"DateTime resolved: {currentDateTime}, " +
                $"IssueCode: {issueText}, " +
                $"ObjectName: {objectName}, " +
                $"Queue Manager: {qmgr}, " +
                $"UseChatBot? {chatbotUse}, EXPRating: {rating}, " +
                $"Feedback: {feedback} \n";

            File.AppendAllText(path, content);

            Debug.Log("Success: Log file updated.");

            resolvePanel.SetActive(false);

            string convertedIssueText = ConvertToReadableIssue(issueText);
            var key = (objectName, convertedIssueText);

            CheckAndDestroy(instantiatedIssueButtons, "ObjectName", "IssueName", key);
            CheckAndDestroy(instantiatedIssuePanels, "Content/object/object_text", "Content/issue/issue_text", key);

            //removing button from issue search manager
            RemoveMatchingIssueButton(issueSearchManager, instantiatedIssueButtons, objectName, convertedIssueText);

            var keyIssueDict = (objectName, issueText);

            if (issueManager.issuesDictionary.ContainsKey(keyIssueDict))
            {
                // The tuple exists in the dictionary
                issueManager.issuesDictionary.Remove(keyIssueDict);
            }
            else
            {
                Debug.LogWarning("The tuple does not exist in the dictionary");
            }


            if (Issues.colour_indicator.ContainsKey(objectName))
            {
                if (issueText == "Too_Much_Activity" || issueText == "Too_Many_2035s" || issueText == "Too_Many_2085s" || issueText == "Queue_Full")
                {
                    // Decrement 'critical' by 1
                    Issues.colour_indicator[objectName]["critical"] -= 1;
                }
                else if (issueText == "Misconfigured_Connection_Pattern" || issueText == "Queue_Service_High" || issueText == "Threshold_Exceeded")
                {
                    // Decrement 'warning' by 1
                    Issues.colour_indicator[objectName]["warning"] -= 1;
                }
            }

            Debug.Log("Success: Issue resolved and deleted.");

        }
        catch (Exception ex)
        {
            StartCoroutine(ErrorOccured(ex));
            Debug.LogError("An error has occured during resolution");
        }
    }

    // coroutine to show error for limited time
    private IEnumerator ErrorOccured(Exception ex)
    {
        responseText.text = "An error occurred: " + ex.Message;
        responseText.gameObject.SetActive(true);
        yield return new WaitForSeconds(10); // wait for seconds
        responseText.text = "";
        responseText.gameObject.SetActive(false);
    }

    private void RemoveMatchingIssueButton(IssueSearchManager issueSearchManager, Transform instantiatedIssueButtons, string objectName, string convertedIssueText)
    {
        if (issueSearchManager)
        {
            foreach (Transform child in instantiatedIssueButtons)
            {
                TMP_Text objectNameText = child.Find("ObjectName").GetComponent<TMP_Text>();
                TMP_Text issueCodeText = child.Find("IssueName").GetComponent<TMP_Text>();

                if (objectNameText && issueCodeText && objectNameText.text == objectName && issueCodeText.text == convertedIssueText)
                {
                    issueSearchManager.RemoveIssueButton(child.gameObject);
                }
            }
        }
    }

    private void CheckAndDestroy(Transform parent, string objectPath, string issuePath, (string, string) key)
    {
        for (int i = parent.childCount - 1; i >= 0; i--) // Looping in reverse to safely remove items.
        {
            GameObject item = parent.GetChild(i).gameObject;

            TMP_Text objectNameText = item.transform.Find(objectPath)?.GetComponent<TMP_Text>();
            TMP_Text issueCodeText = item.transform.Find(issuePath)?.GetComponent<TMP_Text>();

            if (objectNameText == null || issueCodeText == null)
            {
                Debug.LogError("Either objectNameText or issueCodeText is null. Skipping this iteration.");
                continue;
            }

            var currentKey = (objectNameText.text, issueCodeText.text);

            if (currentKey == key)
            {
                if (parent == instantiatedIssueButtons)
                {
                    if (issueManager.buttonToPanelMap.ContainsKey(item))
                    {
                        GameObject panelToDestroy = issueManager.buttonToPanelMap[item];
                        Destroy(panelToDestroy);
                        issueManager.buttonToPanelMap.Remove(item);
                    }
                    else
                    {
                        Debug.LogError($"No panel associated with {item} found in buttonToPanelMap.");
                    }
                }
                Destroy(item);

            }
        }
    }
   
    private string ConvertToReadableIssue(string issueText)
    {
        switch (issueText)
        {
            case "Too_Many_2035s":
                return "Too Many Errors";
            case "Too_Many_2085s":
                return "Too Many Errors";
            case "Misconfigured_Connection_Pattern":
                return "Misconfigured Application";
            case "Queue_Service_High":
                return "High Queue Service";
            case "Too_Much_Activity":
                return "Too Much Activity";
            case "Threshold_Exceeded":
                return "Threshold Exceeded";
            case "Queue_Full":
                return "Queue Capacity Full";
            default:
                return issueText; // Return the original string if no match is found
        }
    }

    

    public void OnYesToggleChanged(bool isOn)
    {
        if (isOn)
        {
            hasInteractedWithYesToggle = true;
        }
    }

    public void OnNoToggleChanged(bool isOn)
    {
        if (isOn)
        {
            hasInteractedWithNoToggle = true;
        }
    }

    public void OnSliderValueChanged()
    {
        hasInteractedWithSlider = true;
    }

    public void showPanel(Issue issue)
    {
        ResetFields();
        issueTextComponent.text = issue.issueCode;
        objectNameTextComponent.text = issue.mqobjectName;
        resolvePanel.SetActive(true);
        responseText.gameObject.SetActive(false); // deactivates responseText when panel is shown

    }

    public void closePanel()
    {
        resolvePanel.SetActive(false);

        // reset fields
        ResetFields();

        responseText.gameObject.SetActive(false); // deactivates responseText when panel is closed

    }

    private void ResetFields()
    {
        // reset text components
        issueTextComponent.text = "";
        objectNameTextComponent.text = "";
        ratingTextComponent.text = "1";

        // reset input field
        inputField.text = "";
        slider.value = slider.minValue;

        // reset toggles
        yesToggle.isOn = false;
        noToggle.isOn = false;

        // reset flags
        hasInteractedWithYesToggle = false;
        hasInteractedWithNoToggle = false;
        hasInteractedWithSlider = false;
    }

    // notify user that the app does not know what OS they're using
    private IEnumerator PlatformNotFound()
    {
        DesktopMessage.SetActive(true);
        yield return new WaitForSeconds(4); // wait for seconds
        DesktopMessage.SetActive(false);
    }
}

