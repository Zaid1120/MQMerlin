using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QueueConfigSearchManager : MonoBehaviour
{
    public TMP_InputField SearchBar;
    public List<GameObject> AllQueueButtons = new List<GameObject>();

    private string lastSearchText; // Store the last search text

    private void Start()
    {
        if (SearchBar != null)
        {
            SearchBar.onValueChanged.AddListener(delegate { Search(); });
        }
        else
        {
            Debug.LogError("SearchBar is not assigned in QueueConfigSearchManager.");
        }
    }

    public void AddQueueConfig(GameObject newConfig)
    {
        if (newConfig != null && !AllQueueButtons.Contains(newConfig))
        {
            AllQueueButtons.Add(newConfig);
            ApplySearchFilter(newConfig); // apply the last search filter when a new button is added
        }
        else if (newConfig == null)
        {
            Debug.LogWarning("Attempted to add a null config to AllQueueButtons.");
        }
    }

    public void RemoveConfigButton(GameObject buttonToRemove)
    {
        if (buttonToRemove != null)
        {
            AllQueueButtons.Remove(buttonToRemove);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a null button from AllQueueButtons.");
        }
    }

    public void Search()
    {
        if (SearchBar != null)
        {
            lastSearchText = SearchBar.text.Trim(); // store the search text and remove any white spaces
            var tempConfigButtons = new List<GameObject>(AllQueueButtons);

            foreach (GameObject ele in tempConfigButtons)
            {
                ApplySearchFilter(ele);
            }
        }
        else
        {
            Debug.LogError("SearchBar is not assigned. Search cannot proceed.");
        }
    }

    private void ApplySearchFilter(GameObject configObject)
    {
        if (configObject == null)
        {
            Debug.LogWarning("Attempted to apply search filter on a null config object.");
            return;
        }

        TextMeshProUGUI objectNameTextComponent = configObject.transform.Find("queuename")?.GetComponent<TextMeshProUGUI>();

        if (objectNameTextComponent != null)
        {
            string objectNameText = objectNameTextComponent.text;
            bool textMatches = string.IsNullOrEmpty(lastSearchText) || objectNameText.StartsWith(lastSearchText, System.StringComparison.OrdinalIgnoreCase);

            configObject.SetActive(textMatches);
        }
        else
        {
            Debug.LogWarning($"Config object '{configObject.name}' does not have a TextMeshProUGUI component named 'queuename'.");
        }
    }
}
