using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QueueSearchManager : MonoBehaviour, MonitoringSearchManager
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
            Debug.LogError("SearchBar is not assigned in QueueSearchManager.");
        }
    }

    public void AddQueueButton(GameObject newButton)
    {
        if (newButton != null && !AllQueueButtons.Contains(newButton))
        {
            AllQueueButtons.Add(newButton);
            ApplySearchFilter(newButton); // apply the last search filter when a new button is added
        }
        else if (newButton == null)
        {
            Debug.LogWarning("Attempted to add a null button to AllQueueButtons.");
        }
    }

    public void RemoveQueueButton(GameObject buttonToRemove)
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

    // leting interface know that these next two methods are its implementation
    public void AddButton(GameObject newButton)
    {
        AddQueueButton(newButton);
    }

    public void RemoveButton(GameObject buttonToRemove)
    {
        RemoveQueueButton(buttonToRemove);
    }

    public void Search()
    {
        if (SearchBar != null)
        {
            lastSearchText = SearchBar.text.Trim(); // store the search text and remove any white spaces
            var tempQueueButtons = new List<GameObject>(AllQueueButtons);

            foreach (GameObject ele in tempQueueButtons)
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
            TextMeshProUGUI tmpComponent = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (tmpComponent != null)
            {
                string buttonText = tmpComponent.text;
                // check if search text is empty or matches the button text and set the button to active or inactive accordingly
                bool shouldDisplay = string.IsNullOrEmpty(lastSearchText) || buttonText.StartsWith(lastSearchText, System.StringComparison.OrdinalIgnoreCase);
                button.SetActive(shouldDisplay);
            }
            else
            {
                Debug.LogWarning($"Button '{button.name}' does not have a TextMeshProUGUI component in its first child.");
            }
        }
    }
}




//using UnityEngine;
//using TMPro;
//using System.Collections.Generic;

//public class QueueSearchManager : MonoBehaviour
//{
//    public TMP_InputField SearchBar;
//    public List<GameObject> AllQueueButtons = new List<GameObject>();

//    private string lastSearchText; // Store the last search text

//    private void Start()
    //{
    //    if (SearchBar != null)
    //    {
    //        SearchBar.onValueChanged.AddListener(delegate { Search(); });
    //    }
    //    else
    //    {
    //        Debug.LogError("SearchBar is not assigned in QueueSearchManager.");
    //    }
    //}

    //public void AddQueueButton(GameObject newButton)
    //{
    //    if (newButton != null && !AllQueueButtons.Contains(newButton))
    //    {
    //        AllQueueButtons.Add(newButton);
    //        ApplySearchFilter(newButton); // apply the last search filter when a new button is added
    //    }
    //    else if (newButton == null)
    //    {
    //        Debug.LogWarning("Attempted to add a null button to AllQueueButtons.");
    //    }
    //}

    //public void RemoveQueueButton(GameObject buttonToRemove)
    //{
    //    if (buttonToRemove != null)
    //    {
    //        AllQueueButtons.Remove(buttonToRemove);
    //    }
    //    else
//        {
//            Debug.LogWarning("Attempted to remove a null button from AllQueueButtons.");
//        }
//    }

//    public void Search()
//    {
//        if (SearchBar != null)
//        {
//            lastSearchText = SearchBar.text.Trim(); // store the search text and remove any white spaces
//            var tempQueueButtons = new List<GameObject>(AllQueueButtons);

//            foreach (GameObject ele in tempQueueButtons)
//            {
//                ApplySearchFilter(ele);
//            }
//        }
//        else
//        {
//            Debug.LogError("SearchBar is not assigned. Search cannot proceed.");
//        }
//    }

//    private void ApplySearchFilter(GameObject button)
//    {
//        if (button == null)
//        {
//            Debug.LogWarning("Attempted to apply search filter on a null button.");
//            return;
//        }

//        if (button.transform.childCount > 0)
//        {
//            TextMeshProUGUI tmpComponent = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

//            if (tmpComponent != null)
//            {
//                string buttonText = tmpComponent.text;
//                // check if search text is empty or matches the button text and set the button to active or inactive accordingly
//                bool shouldDisplay = string.IsNullOrEmpty(lastSearchText) || buttonText.StartsWith(lastSearchText, System.StringComparison.OrdinalIgnoreCase);
//                button.SetActive(shouldDisplay);
//            }
//            else
//            {
//                Debug.LogWarning($"Button '{button.name}' does not have a TextMeshProUGUI component in its first child.");
//            }
//        }
//    }
//}
