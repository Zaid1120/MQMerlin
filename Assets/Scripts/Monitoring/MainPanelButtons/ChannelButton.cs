using UnityEngine;
using TMPro;

public class ChannelButton : MonoBehaviour
{
    public GameObject inspectPanel;
    private bool isPanelActive = false;
    public static ChannelButton CurrentActiveButton; // static variable tracking currently displayed channel

    public void ToggleInspectorPanel(ReceiveData.Channel channelData)
    {
        if (channelData == null)
        {
            Debug.LogError("Received null channelData in ToggleInspectorPanel.");
            return;
        }

        if (inspectPanel == null)
        {
            Debug.LogError("Inspect panel is not assigned in the inspector.");
            return;
        }

        if (isPanelActive && this == CurrentActiveButton)
        {
            inspectPanel.SetActive(false);
            isPanelActive = false;
            CurrentActiveButton = null;
        }
        else
        {
            UpdateDetails(channelData);
            inspectPanel.SetActive(true);
            isPanelActive = true;
            CurrentActiveButton = this;
        }
    }

    private void UpdateDetails(ReceiveData.Channel channel)
    {
        if (channel == null)
        {
            Debug.LogError("Received null channelData in UpdateDetails.");
            return;
        }

        if (inspectPanel == null)
        {
            Debug.LogError("Inspect panel is not assigned in the inspector.");
            return;
        }

        SetTextInChild(inspectPanel, "name_cont", channel.channel_name);
        SetTextInChild(inspectPanel, "type_cont", channel.channel_type);
        SetTextInChild(inspectPanel, "description_cont", channel.description);
        SetTextInChild(inspectPanel, "messagelength_cont", channel.max_message_length.ToString());
        SetTextInChild(inspectPanel, "heartbeatinterval_cont", channel.heartbeat_interval.ToString());
        SetTextInChild(inspectPanel, "transporttype_cont", channel.transport_type);
    }

    private void SetTextInChild(GameObject parent, string childName, string text)
    {
        Transform childTransform = parent.transform.Find(childName);
        if (childTransform != null)
        {
            TextMeshProUGUI tmpComponent = childTransform.GetComponent<TextMeshProUGUI>();
            if (tmpComponent != null)
            {
                tmpComponent.text = text ?? "N/A"; // set to "N/A" if text is null.
            }
            else
            {
                Debug.LogWarning($"Child '{childName}' does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find child named '{childName}' in '{parent.name}'.");
        }
    }
}
