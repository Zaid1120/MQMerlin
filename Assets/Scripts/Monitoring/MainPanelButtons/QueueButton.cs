using UnityEngine;
using TMPro;
using FlaskServer;

public class QueueButton : MonoBehaviour
{

    public GameObject inspectPanel;
    public static QueueButton CurrentActiveButton; // a static variable tracking currently displayed channel

    public void ToggleInspectorPanel(ReceiveData.Queue queueData)
    {
        if (queueData == null)
        {
            Debug.LogError("Provided queueData is null!");
            return;
        }

        if (inspectPanel == null)
        {
            Debug.LogError("InspectPanel is not assigned!");
            return;
        }

        if (CurrentActiveButton != null && CurrentActiveButton.inspectPanel != null)
        {
            CurrentActiveButton.inspectPanel.SetActive(false);
        }

        if (this == CurrentActiveButton)
        {
            inspectPanel.SetActive(false);
            CurrentActiveButton = null;
        }
        else
        {
            UpdateDetails(queueData);
            inspectPanel.SetActive(true);
            CurrentActiveButton = this;
        }
    }

    private void UpdateDetails(ReceiveData.Queue queue)
    {
        if (queue == null)
        {
            Debug.LogError("Provided queue is null in UpdateDetails.");
            return;
        }

        if (inspectPanel == null)
        {
            Debug.LogError("Inspect panel is not assigned in UpdateDetails.");
            return;
        }

        SetTextInChild(inspectPanel, "name_cont", queue.queue_name?.ToString());
        SetTextInChild(inspectPanel, "type_cont", queue.type_name?.ToString());
        SetTextInChild(inspectPanel, "inhibitput_cont", queue.inhibit_put.ToString());
        SetTextInChild(inspectPanel, "timemodified_cont", Utilities.Instance.FormatDateTimetoYMDHHmmss(queue.time_altered?.ToString()));
        SetTextInChild(inspectPanel, "description_cont", queue.description?.ToString());

        if (queue.type_name == "Local")
        {
            inspectPanel.transform.Find("currentdepth_cont").GetComponent<TextMeshProUGUI>().text = queue.current_depth.ToString();
            inspectPanel.transform.Find("maxdepth_cont").GetComponent<TextMeshProUGUI>().text = queue.max_number_of_messages.ToString();
            inspectPanel.transform.Find("messagelength_cont").GetComponent<TextMeshProUGUI>().text = queue.max_message_length.ToString();
            inspectPanel.transform.Find("inhibitget_cont").GetComponent<TextMeshProUGUI>().text = queue.inhibit_get.ToString();
            inspectPanel.transform.Find("timecreated_cont").GetComponent<TextMeshProUGUI>().text = Utilities.Instance.FormatDateTimetoYMDHHmmss(queue.time_created.ToString());
            inspectPanel.transform.Find("currentdepthpercent_cont").GetComponent<TextMeshProUGUI>().text = $"{queue.threshold}%";
        }
        else if (queue.type_name == "Transmission")
        {
            inspectPanel.transform.Find("currentdepth_cont").GetComponent<TextMeshProUGUI>().text = queue.current_depth.ToString();
            inspectPanel.transform.Find("maxdepth_cont").GetComponent<TextMeshProUGUI>().text = queue.max_number_of_messages.ToString();
            inspectPanel.transform.Find("inhibitget_cont").GetComponent<TextMeshProUGUI>().text = queue.inhibit_get.ToString();
            inspectPanel.transform.Find("currentdepthpercent_cont").GetComponent<TextMeshProUGUI>().text = $"{queue.threshold}%";
            inspectPanel.transform.Find("timecreated_cont").GetComponent<TextMeshProUGUI>().text = Utilities.Instance.FormatDateTimetoYMDHHmmss(queue.time_created.ToString());
        }
        else if (queue.type_name == "Alias")
        {
            inspectPanel.transform.Find("baseqname_cont").GetComponent<TextMeshProUGUI>().text = queue.target_queue_name.ToString();
        }
        else if (queue.type_name == "Remote")
        {
            inspectPanel.transform.Find("transqueuename_cont").GetComponent<TextMeshProUGUI>().text = queue.transmission_queue_name.ToString();
            inspectPanel.transform.Find("remoteqmgrname_cont").GetComponent<TextMeshProUGUI>().text = queue.target_qmgr_name.ToString();
            inspectPanel.transform.Find("remotequeuename_cont").GetComponent<TextMeshProUGUI>().text = queue.target_queue_name.ToString();
        }
    }

    private void SetTextInChild(GameObject parent, string childName, string text)
    {
        Transform childTransform = parent.transform.Find(childName);
        if (childTransform != null)
        {
            TextMeshProUGUI tmpComponent = childTransform.GetComponent<TextMeshProUGUI>();
            if (tmpComponent != null)
            {
                tmpComponent.text = text ?? "N/A";
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
