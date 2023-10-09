using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FlaskServer;
using Newtonsoft.Json;
using System.Collections;
using ReceiveData;
using System;

public class Login : MonoBehaviour
{
    // input fields for login
    public TMP_InputField AddressInput;
    public TMP_InputField AdminPortInput; 
    public TMP_InputField AppPortInput; 
    public TMP_InputField QueueManagerNameInput;
    public TMP_InputField ChannelNameInput;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;

    // display fields for queue manager name, response messages, and login text
    public TMP_Text QueueManagerName;
    public TMP_Text responseText;
    public TMP_Text LoginText;

    // gameObject references related to different functionalities
    public GameObject instantiation; // for instantiating queues
    public GameObject getDetails; // for making GET requests
    public GameObject config; // related to configuration settings
    public GameObject chatbot; // related to chatbot code gameobject
    public GameObject issues; // related to issue code gameobject
    public GameObject topPanel; // for app top panel 
    public TMP_Text logoutButton; //button for logging out
    public Chatbot chatbotScript;  // reference to the chatbot script
    public GameObject issueLogsPanel; // related to issues log panel
    public GameObject loginPanel;  // reference to login panel
    public GameObject resolvePanel; // gameobject related to resolve panel
    public GameObject mainPanel; // related to main panel
    public GameObject logoutPanel; // related to logout panel

    // URIs for login and logout server endpoints
    string loginURI = "https://127.0.0.1:5000/clientconfig";
    string logoutURI = "https://127.0.0.1:5000/logout";

    private bool toggled = false;
    private bool isSubmitting = false; 

    public UIManager uiManager;

    private void Awake()
    {
        // initialise UI state on awake
        loginPanel.SetActive(true);
        resolvePanel.SetActive(false);
        instantiation.SetActive(false);
        getDetails.SetActive(false);
        config.SetActive(false);
        chatbot.SetActive(false);
        issueLogsPanel.SetActive(false);
        issues.SetActive(false);
        topPanel.SetActive(false);
    }

    // method to handle login form submission
    public async void SubmitForm()
    {
        // prevent re-submission while a submission is in progress
        if (isSubmitting)
        {
            return;
        }

        isSubmitting = true;

        try
        {
            // collect form data
            var form = new Dictionary<string, string>();

            form["address"] = AddressInput.text;
            form["admin_port"] = AdminPortInput.text;
            form["app_port"] = AppPortInput.text;
            form["admin_channel"] = string.IsNullOrEmpty(ChannelNameInput.text) ? "DEV.ADMIN.SVRCONN" : ChannelNameInput.text;
            form["qmgr"] = QueueManagerNameInput.text;
            form["username"] = UsernameInput.text;
            form["password"] = PasswordInput.text;

            // update UI to indicate connection is in progress
            responseText.text = "";
            LoginText.text = "Connecting to Queue Manager...";

            // send a POST request and handle the response
            string jsonResult = await Utilities.Instance.PostRequest(loginURI, form);
            ResponseMessage response = JsonConvert.DeserializeObject<ResponseMessage>(jsonResult);

            // handling response
            if (response.message == "Login successful.")
            {

                Debug.Log("Success: queue manager connected.");
                // activate scripts
                ToggleScripts();
                // assign queue manager name
                QueueManagerName.text = QueueManagerNameInput.text;
                // open panels
                uiManager.OpenLoginPanels();
                // turn off login panel
                StartCoroutine(deactivateLoginPanel());
            }
            else
            {
                responseText.text = response.message;
                PasswordInput.text = "";
                LoginText.text = "";
            }
        }
        catch (Exception ex)
        {
            LoginText.text = "";
            StartCoroutine(ErrorOccured(ex));
            Debug.LogWarning("Error in SubmitForm: " + ex.ToString());
        }
        finally
        {
            // reset submission flag
            isSubmitting = false;
        }
    }

    // method to handle logout
    public async void Logout()
    {
        try
        {
            AddressInput.text = "";
            AdminPortInput.text = "";
            AppPortInput.text = "";
            QueueManagerNameInput.text = "";
            ChannelNameInput.text = "";
            UsernameInput.text = "";
            PasswordInput.text = "";

            loginPanel.SetActive(true);
            uiManager.ClosePanels();
            ToggleScripts();

            string logoutResponse = await Utilities.Instance.PostRequestEmpty(logoutURI);
            Debug.Log("Success: queue manager disconnected.");
        }
        catch (Exception ex)
        {
            loginPanel.SetActive(true);
            StartCoroutine(ErrorOccured(ex));
            Debug.LogWarning("Error in Logout: " + ex.ToString());
        }
    }

    // method to handle application quit
    public void OnApplicationQuit() { UnityEngine.Application.Quit(); }


    // coroutine to show error for limited time
    private IEnumerator ErrorOccured(Exception ex)
    {
        responseText.text = "An error occurred: " + ex.Message;
        yield return new WaitForSeconds(10); // wait for seconds
        responseText.text = "";
    }

    // coroutine to delay UI changes after login
    private IEnumerator deactivateLoginPanel()
    {
        // reset responseText for the next time the panel is shown (log out)
        responseText.text = "";
        LoginText.text = "";

        yield return new WaitForSeconds(2); // wait for seconds

        loginPanel.SetActive(false);
    }

    // method to toggle the activity state of certain scripts and components
    private void ToggleScripts()
    {
        Debug.Log("Success: scripts toggled.");
        // toggle value every time the method is called
        toggled = !toggled;

        getDetails.SetActive(toggled);
        instantiation.SetActive(toggled);
        chatbot.SetActive(toggled);
        config.SetActive(toggled);
        issues.SetActive(toggled);
    }
}