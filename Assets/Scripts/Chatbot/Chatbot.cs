using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlaskServer;
using TMPro;
using Newtonsoft.Json;
using ReceiveData;
using System;

public class Chatbot : MonoBehaviour
{
    public TMP_InputField userInput;
    public TMP_Text waitingText;
    public TMP_Text errorText;
    public GameObject userMessagePrefab;
    public GameObject chatbotMessagePrefab;
    public GameObject chatbotDisclaimer;
    string uri = "https://127.0.0.1:5000/chatbotquery";

    public Transform MessagesParent;

    private Dictionary<string, string> messageData = new Dictionary<string, string>
    {
        { "question", "" },
        { "indicator", "" }
    };

    // clear messages every time the GameObject becomes active
    private void OnEnable()
    {
        ClearMessages();
        Instantiate(chatbotDisclaimer, MessagesParent);
        userInput.text = "";
        waitingText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }

    public async void SendMessage()
    {
        try
        {
            // check if userInput is empty or just whitespace. If it is, return without doing anything
            if (string.IsNullOrWhiteSpace(userInput.text))
            {
                return;
            }

            messageData["question"] = userInput.text;
            messageData["indicator"] = "userMessage";

            string postResponse = await Utilities.Instance.PostRequest(uri, messageData);
            ResponseMessage chatbotPostResponse = JsonConvert.DeserializeObject<ResponseMessage>(postResponse);

            if (chatbotPostResponse.message == "Query stored successfully.")
            {
                Debug.Log("Success: User message sent to chatbot");

                waitingText.gameObject.SetActive(true);

                StartCoroutine(CycleWaitingText());

                GameObject userInstance = Instantiate(userMessagePrefab, MessagesParent);

                TMP_Text userMessageContent = userInstance.transform.Find("UserText").GetComponent<TMP_Text>();
                userMessageContent.text = userInput.text;
                userInput.text = "";

                string response = await Utilities.Instance.GetRequest(uri);
                ResponseMessage chatbotResponse = JsonConvert.DeserializeObject<ResponseMessage>(response);
                Debug.Log("Success: Chatbot message retrieved.");


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
            // handle any exceptions that might occur
            Debug.LogWarning($"SendMessage Error: {ex.Message}");
            waitingText.gameObject.SetActive(false);
            errorText.text = $"An error has occured: {ex.Message} ";
            errorText.gameObject.SetActive(true);
        }
    }

    // coroutine to update waitingText at intervals.
    private IEnumerator CycleWaitingText()
    {
        waitingText.text = "Contacting MQMerlin...";
        yield return new WaitForSeconds(3); // wait for 3 seconds
        waitingText.text = "Message received...";
        yield return new WaitForSeconds(3); // wait for 3 seconds
        waitingText.text = "Getting response...";
    }

    // check if waiting or error messages are currently being displayed.
    public bool IsWaitingOrErrorActive()
    {
        return waitingText.gameObject.activeInHierarchy || errorText.gameObject.activeInHierarchy;
    }

    // clear all messages from the chat window.
    private void ClearMessages()
    {
        foreach (Transform child in MessagesParent)
        {
            Destroy(child.gameObject);
        }
    }
}
