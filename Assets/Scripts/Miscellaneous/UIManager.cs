using UnityEngine;

public class UIManager : MonoBehaviour
{
    // References to panels
    public GameObject configPanel;
    public GameObject monitoringPanel;
    public GameObject chatbotPanel;
    public GameObject issuesPanel;
    public GameObject issueLogsPanel;
    public GameObject resolvePanel;
    public GameObject queueConfigPanel;
    public GameObject qmgrConfigPanel;
    public GameObject appConfigPanel;
    public GameObject queuemonitoringPanel;
    public GameObject channelmonitoringPanel;
    public GameObject appmonitoringPanel;
    public GameObject logoutPanel;
    public GameObject loginPanel;
    public GameObject topPanel;

    private void TogglePanel(GameObject panel, bool state)
    {
        if (panel != null)
        {
            panel.SetActive(state);
        }
        else
        {
            Debug.LogError("Panel not assigned in the UIManager!");
        }
    }

    // This method toggles the configPanel on and other panels off
    public void ShowConfigPanel()
    {
        TogglePanel(configPanel, true);
        TogglePanel(monitoringPanel, false);
        TogglePanel(chatbotPanel, false);
        TogglePanel(issuesPanel, false);
    }

    // toggle monitoringPanel on 
    public void ShowmonitoringPanel()
    {
        TogglePanel(configPanel, false);
        TogglePanel(monitoringPanel, true);
        TogglePanel(chatbotPanel, false);
        TogglePanel(issuesPanel, false);
    }

    // toggle the ChatbotPanel on
    public void ShowChatBotPanel()
    {
        TogglePanel(configPanel,false);
        TogglePanel(monitoringPanel, false);
        TogglePanel(chatbotPanel, true);
        TogglePanel(issuesPanel, false);
    }

    // and so on...

    public void ShowIssuesPanel()
    {
        TogglePanel(configPanel, false);
        TogglePanel(monitoringPanel, false);
        TogglePanel(chatbotPanel, false);
        TogglePanel(issuesPanel, true);
    }

    public void ToggleLogsPanel()
    {
        if (issueLogsPanel != null)
        {
            TogglePanel(issueLogsPanel, !issueLogsPanel.activeSelf);
        }
        else
        {
            Debug.LogError("IssueLogsPanel not assigned in the UIManager!");
        }
    }

    //unused code for separating configs for different MQ objects into different panels

    //public void ShowQueuesConfig()
    //{
    //    TogglePanel(queueConfigPanel, true);
    //    TogglePanel(qmgrConfigPanel, false);
    //    TogglePanel(appConfigPanel, false);
    //}

    //public void ShowQMGRConfig()
    //{
    //    TogglePanel(queueConfigPanel, false);
    //    TogglePanel(qmgrConfigPanel, true);
    //    TogglePanel(appConfigPanel, false);
    //}

    //public void ShowAppConfig()
    //{
    //    TogglePanel(queueConfigPanel, false);
    //    TogglePanel(qmgrConfigPanel, false);
    //    TogglePanel(appConfigPanel, true);
    //}

    public void ShowQueuemonitoringPanel()
    {
        TogglePanel(queuemonitoringPanel, true);
        TogglePanel(channelmonitoringPanel, false);
        TogglePanel(appmonitoringPanel, false);
    }

    public void ShowChannelmonitoringPanel()
    {
        TogglePanel(queuemonitoringPanel, false);
        TogglePanel(channelmonitoringPanel, true);
        TogglePanel(appmonitoringPanel, false);
    }

    public void ShowAppmonitoringPanel()
    {
        TogglePanel(queuemonitoringPanel, false);
        TogglePanel(channelmonitoringPanel, false);
        TogglePanel(appmonitoringPanel, true);
    }

    public void ToggleLogoutPanel()
    {
        if (logoutPanel != null)
        {
            TogglePanel(logoutPanel, !logoutPanel.activeSelf);
        }
        else
        {
            Debug.LogError("logoutPanel not assigned in the UIManager!");
        }
    }

    public void ClosePanels()
    {
        TogglePanel(loginPanel, true);
        TogglePanel(monitoringPanel, false);
        TogglePanel(logoutPanel, false);
        TogglePanel(issuesPanel, false);
        TogglePanel(issueLogsPanel, false);
        TogglePanel(resolvePanel, false);
        TogglePanel(configPanel, false);
        TogglePanel(topPanel, false);
    }

    public void OpenLoginPanels()
    {
        // this next two methods are a dirty fix so that the chatbot panel sets up fine
        ShowChatBotPanel();
        ClosePanels();

        TogglePanel(monitoringPanel, true);
        TogglePanel(queuemonitoringPanel, true);
        TogglePanel(topPanel, true);
    }
}
