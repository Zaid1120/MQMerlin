using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IssueSearchManager : MonoBehaviour
{
    public TMP_InputField SearchBar;
    public List<GameObject> AllIssueButtons = new List<GameObject>();

    private string lastSearchText; // Store the last search text

    private void Start()
    {
        if (SearchBar != null)
        {
            SearchBar.onValueChanged.AddListener(delegate { Search(); });
        }
        else
        {
            Debug.LogError("SearchBar is not assigned in IssueSearchManager.");
        }
    }

    public void AddIssueButton(GameObject newButton)
    {
        if (newButton != null && !AllIssueButtons.Contains(newButton))
        {
            AllIssueButtons.Add(newButton);
            ApplySearchFilter(newButton); // apply the last search filter when a new button is added
        }
        else if (newButton == null)
        {
            Debug.LogWarning("Attempted to add a null button to AllIssueButtons.");
        }
    }

    public void RemoveIssueButton(GameObject buttonToRemove)
    {
        if (buttonToRemove != null)
        {
            AllIssueButtons.Remove(buttonToRemove);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a null button from AllIssueButtons.");
        }
    }

    public void Search()
    {
        if (SearchBar != null)
        {
            lastSearchText = SearchBar.text.Trim(); // store the search text and remove any white spaces
            var tempIssueButtons = new List<GameObject>(AllIssueButtons);

            foreach (GameObject ele in tempIssueButtons)
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

        TextMeshProUGUI objectNameTextComponent = button.transform.Find("ObjectName")?.GetComponent<TextMeshProUGUI>();

        if (objectNameTextComponent != null)
        {
            string objectNameText = objectNameTextComponent.text;
            bool textMatches = string.IsNullOrEmpty(lastSearchText) || objectNameText.StartsWith(lastSearchText, System.StringComparison.OrdinalIgnoreCase);

            button.SetActive(textMatches);
        }
        else
        {
            Debug.LogWarning($"Button '{button.name}' does not have a TextMeshProUGUI component named 'ObjectName'.");
        }
    }
}
