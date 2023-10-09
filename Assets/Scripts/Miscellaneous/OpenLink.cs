using UnityEngine;

public class OpenLink : MonoBehaviour
{
    // URL to be opened when GoToURL is called
    public string url = "https://github.com/Zaid1120/MQMerlin";

    // method to open the specified URL in a web browser
    public void GoToURL()
    {
        // check if URL is not null or empty
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL not set or is empty in OpenLink script.");
            return;
        }

        // attempt to open the URL in web browser
        try
        {
            Application.OpenURL(url);
            Debug.Log("Success: Help link accessed.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open the URL. Error: " + e.Message);
        }
    }
}
