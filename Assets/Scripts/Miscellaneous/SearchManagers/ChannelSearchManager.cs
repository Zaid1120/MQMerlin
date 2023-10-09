using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ChannelSearchManager : MonoBehaviour, MonitoringSearchManager
{
    public TMP_InputField SearchBar;
    public List<GameObject> AllChannelButtons = new List<GameObject>();

    private string lastSearchText; // Store last search text

    private void Start()
    {
        // Ensure SearchBar is assigned
        if (SearchBar != null)
        {
            SearchBar.onValueChanged.AddListener(delegate { Search(); });
        }
        else
        {
            Debug.LogError("SearchBar is not assigned in ChannelSearchManager.");
        }
    }

    public void AddChannelButton(GameObject newButton)
    {
        if (newButton != null && !AllChannelButtons.Contains(newButton))
        {
            AllChannelButtons.Add(newButton);
            ApplySearchFilter(newButton); // Apply last search filter when a new button is added
        }
        else if (newButton == null)
        {
            Debug.LogWarning("Attempted to add a null button to AllChannelButtons.");
        }
    }

    public void RemoveChannelButton(GameObject buttonToRemove)
    {
        if (buttonToRemove != null)
        {
            AllChannelButtons.Remove(buttonToRemove);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a null button from AllChannelButtons.");
        }
    }

    // leting interface know that these next two methods are its implementation
    public void AddButton(GameObject newButton)
    {
        AddChannelButton(newButton);
    }

    public void RemoveButton(GameObject buttonToRemove)
    {
        RemoveChannelButton(buttonToRemove);
    }

    public void Search()
    {
        if (SearchBar != null)
        {
            lastSearchText = SearchBar.text.Trim(); // Store search text and remove any white spaces
            var tempChannelButtons = new List<GameObject>(AllChannelButtons);

            foreach (GameObject ele in tempChannelButtons)
            {
                ApplySearchFilter(ele);
            }
        }
        else
        {
            Debug.LogError("SearchBar is not assigned. Search cannot proceed.");
        }
    }

    private void ApplySearchFilter(GameObject button)
    {
        if (button == null)
        {
            Debug.LogWarning("Attempted to apply search filter on a null button.");
            return;
        }

        if (button.transform.childCount > 0)
        {
            TextMeshProUGUI textComponent = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                string buttonText = textComponent.text;
                // Check if search text is empty or matches the button text and set the button to active or inactive accordingly
                bool shouldDisplay = string.IsNullOrEmpty(lastSearchText) || buttonText.StartsWith(lastSearchText, System.StringComparison.OrdinalIgnoreCase);
                button.SetActive(shouldDisplay);
            }
            else
            {
                Debug.LogWarning("Button does not have a TextMeshProUGUI component on its first child.");
            }
        }
    }
}
