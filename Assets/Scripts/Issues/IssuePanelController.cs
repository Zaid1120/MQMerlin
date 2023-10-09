using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlaskServer;
using TMPro;
using Newtonsoft.Json;
using ReceiveData;
using System;

public class IssuePanelController : MonoBehaviour
{
    public TMP_Text errorText;
    public TMP_Text waitingText;
    public GameObject userMessagePrefab;
    public GameObject chatbotMessagePrefab;
    public GameObject chatbotDisclaimer;
    public Transform MessagesParent;
    public Issue AssociatedIssue;
    public UIManager uiManager;
    string uri = "https://127.0.0.1:5000/chatbotquery";

    private void Awake()
    {
        // initialising the waiting and error text display as hidden
        waitingText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // clear messages every time the GameObject becomes active
        ClearMessages();
        // spawn disclaimer
        Instantiate(chatbotDisclaimer, MessagesParent);
    }

    public async void SendSystemMessage(Issue issue)
    {
        Dictionary<string, object> technicalDetailsDict = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(issue.technicalDetails.appName))
        {
            technicalDetailsDict["appName"] = issue.technicalDetails.appName;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.connName))
        {
            technicalDetailsDict["connName"] = issue.technicalDetails.connName;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.channelName))
        {
            technicalDetailsDict["channelName"] = issue.technicalDetails.channelName;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.CSPUserId))
        {
            technicalDetailsDict["CSPUserId"] = issue.technicalDetails.CSPUserId;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.userId))
        {
            technicalDetailsDict["userId"] = issue.technicalDetails.userId;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.isActiveIssue))
        {
            technicalDetailsDict["isActiveIssue"] = issue.technicalDetails.isActiveIssue;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.timeSinceReset))
        {
            technicalDetailsDict["timeSinceReset"] = issue.technicalDetails.timeSinceReset;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.highQDepth))
        {
            technicalDetailsDict["highQDepth"] = issue.technicalDetails.highQDepth;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.enQCount))
        {
            technicalDetailsDict["enQCount"] = issue.technicalDetails.enQCount;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.deQCount))
        {
            technicalDetailsDict["deQCount"] = issue.technicalDetails.deQCount;
        }

        if (!string.IsNullOrEmpty(issue.technicalDetails.QName))
        {
            technicalDetailsDict["QName"] = issue.technicalDetails.QName;
        }

        if (issue.technicalDetails.archivedRequestRates != null && issue.technicalDetails.archivedRequestRates.Count > 0) 
        {
            technicalDetailsDict["archivedRequestRates"] = issue.technicalDetails.archivedRequestRates;
        }

        if (issue.technicalDetails.archivedTimestamps != null && issue.technicalDetails.archivedTimestamps.Count > 0)
        {
            technicalDetailsDict["archivedTimestamps"] = issue.technicalDetails.archivedTimestamps;
        }

        if (issue.technicalDetails.archivedConnRates != null && issue.technicalDetails.archivedConnRates.Count > 0) 
        {
            technicalDetailsDict["archivedConnRates"] = issue.technicalDetails.archivedConnRates;
        }

        if (issue.technicalDetails.archivedconns != null && issue.technicalDetails.archivedconns.Count > 0) 
        {
            technicalDetailsDict["archivedconns"] = issue.technicalDetails.archivedconns;
        }

        if (issue.technicalDetails.archivedlogTimes != null && issue.technicalDetails.archivedlogTimes.Count > 0) 
        {
            technicalDetailsDict["archivedlogTimes"] = issue.technicalDetails.archivedlogTimes;
        }

        if (issue.technicalDetails.archiveduserRatio != null && issue.technicalDetails.archiveduserRatio.Count > 0) 
        {
            technicalDetailsDict["archiveduserRatio"] = issue.technicalDetails.archiveduserRatio;
        }

        if (issue.technicalDetails.archivedRequestCount != null && issue.technicalDetails.archivedRequestCount.Count > 0) 
        {
            technicalDetailsDict["archivedRequestCount"] = issue.technicalDetails.archivedRequestCount;
        }

        Dictionary<string, object> issueData = new Dictionary<string, object>
        {
            { "issueCode", issue.issueCode },
            { "startTimeStamp", issue.startTimeStamp },
            { "generalDesc", issue.generalDesc },
            { "mqobjectType", issue.mqobjectType },
            { "mqobjectName", issue.mqobjectName },
            { "objectDetails", issue.objectDetails },
            { "technicalDetails", technicalDetailsDict }
        };

        try
        {
            // Making a POST request to the chatbot with the issue data.
            string postResponse = await Utilities.Instance.PostChatbotRequest(uri, issueData);

            ResponseMessage chatbotPostResponse = JsonConvert.DeserializeObject<ResponseMessage>(postResponse);

            uiManager.ShowChatBotPanel();

            if (chatbotPostResponse.message == "Query stored successfully.")
            {
                Debug.Log("Success: Issue details sent to chatbot.");

                waitingText.gameObject.SetActive(true);
                StartCoroutine(CycleWaitingText());

                GameObject userInstance = Instantiate(userMessagePrefab, MessagesParent);

                TMP_Text userMessageContent = userInstance.transform.Find("UserText").GetComponent<TMP_Text>();
                userMessageContent.text = $" Tell me about the {issue.issueCode} notification for {issue.mqobjectName}.";

                string response = await Utilities.Instance.GetRequest(uri);
                ResponseMessage chatbotResponse = JsonConvert.DeserializeObject<ResponseMessage>(response);
                Debug.Log("Success: Chatbot response about issue retrieved.");


                GameObject chatbotInstance = Instantiate(chatbotMessagePrefab, MessagesParent);
                TMP_Text chatbotMessageContent = chatbotInstance.transform.Find("ChatbotText").GetComponent<TMP_Text>();
                chatbotMessageContent.text = chatbotResponse.message;

                errorText.gameObject.SetActive(false);
                waitingText.gameObject.SetActive(false);
            }

            else if (chatbotPostResponse.message == "Please wait, one question at a time...")
            {
                waitingText.gameObject.SetActive(false);
                errorText.gameObject.SetActive(true);
            }

        }
        catch (Exception ex)
        {
            StartCoroutine(ErrorOccured(ex));
            Debug.LogError($"Error while sending system message: {ex.Message}");
        }
    }

    // coroutine to show error for limited time
    private IEnumerator ErrorOccured(Exception ex)
    {
        errorText.text = $"An error has occured: {ex.Message}";
        errorText.gameObject.SetActive(true);
        waitingText.gameObject.SetActive(false);
        yield return new WaitForSeconds(10); // wait for seconds
        errorText.text = "";
        errorText.gameObject.SetActive(false);
    }

    private IEnumerator CycleWaitingText()
    {
        waitingText.text = "Contacting MQMerlin...";
        yield return new WaitForSeconds(3); // wait for 3 seconds
        waitingText.text = "Message received...";
        yield return new WaitForSeconds(3); // wait for 3 seconds
        waitingText.text = "Getting response...";
    }

    public bool IsWaitingOrErrorActive()
    {
        return waitingText.gameObject.activeInHierarchy || errorText.gameObject.activeInHierarchy;
    }

    private void ClearMessages()
    {
        foreach (Transform child in MessagesParent)
        {
            Destroy(child.gameObject);
        }
    }
}