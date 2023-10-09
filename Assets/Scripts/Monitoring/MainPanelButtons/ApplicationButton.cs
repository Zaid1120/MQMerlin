using UnityEngine;
using TMPro;

public class ApplicationButton : MonoBehaviour
{
    public GameObject inspectPanel;
    private bool isPanelActive = false;
    public static ApplicationButton CurrentActiveButton; // a static variable tracking currently displayed channel

    public void ToggleInspectorPanel(ReceiveData.Application applicationData)
    {
        if (applicationData == null)
        {
            Debug.LogError("Received null applicationData in ToggleInspectorPanel.");
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
            UpdateDetails(applicationData);
            inspectPanel.SetActive(true);
            isPanelActive = true;
            CurrentActiveButton = this;
        }
    }

    private void UpdateDetails(ReceiveData.Application application)
    {
        if (inspectPanel == null)
        {
            Debug.LogError("Inspect panel is not assigned in the inspector.");
            return;
        }

        SetTextInChild(inspectPanel, "name_cont", application.conn?.ToString());
        SetTextInChild(inspectPanel, "type_cont", application.type?.ToString());
        SetTextInChild(inspectPanel, "description_cont", application.appldesc?.ToString());
        SetTextInChild(inspectPanel, "tag_cont", application.appltag?.ToString());
    }

    private void SetTextInChild(GameObject parent, string childName, string text)
    {
        Transform childTransform = parent.transform.Find(childName);
        if (childTransform != null)
        {
            TextMeshProUGUI tmpComponent = childTransform.GetComponent<TextMeshProUGUI>();
            if (tmpComponent != null)
            {
                tmpComponent.text = text;
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
