using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
//using System.Net.Security;
//using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace FlaskServer
{
    // provides utility functions to interact with a flask server
    public class Utilities : MonoBehaviour
    {
        // singleton instance for easy access from other classes
        public static Utilities Instance;

        // handles the mechanics of sending HTTP requests and receiving HTTP responses
        private HttpClient client;

        // provides a mechanism to customise the process of handling HTTP requests
        private HttpClientHandler handler;

        private void Awake()
        {
            Instance = this;

            // ignore SSL certificate errors - not recommended for production
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => { return true; };
            client = new HttpClient(handler);
        }

        // makes an asynchronous GET request to the specified URI
        public async Task<string> GetRequest(string uri)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Success: GetRequest");
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    Debug.LogWarning("Error: " + (response == null ? "No response" : response.StatusCode.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("GetRequest error: " + ex.ToString());
                return null;
            }
        }

        // makes an asynchronous POST request to the specified URI with provided form data
        public async Task<string> PostRequest(string uri, Dictionary<string, string> form)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(form);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Success: PostRequest");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    Debug.LogWarning("Error: " + (response == null ? "No response" : response.StatusCode.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("PostRequest error: " + ex.ToString());
                return null;
            }
        }

        // POST request method with configuration data
        public async Task<string> PostRequestConfig(string uri, RootThresholds configData)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(configData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Success: PostRequestConfig");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    Debug.LogWarning("Error: " + (response == null ? "No response" : response.StatusCode.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("PostRequestConfig error: " + ex.ToString());
                return null;
            }
        }

        // POST request specifically for chatbot
        public async Task<string> PostChatbotRequest(string uri, Dictionary<string, object> issueData)
        {
            try
            {

                // Serialise the issue data
                string issueDataJson = JsonConvert.SerializeObject(issueData);

                Dictionary<string, string> messageData = new Dictionary<string, string>
                {
                    { "question", issueDataJson },
                    { "indicator", "systemMessage" }
                };

                // Serialise the messageData dictionary
                string jsonData = JsonConvert.SerializeObject(messageData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Success: PostChatbotRequest");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    Debug.LogWarning("Error: " + (response == null ? "No response" : response.StatusCode.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("PostChatbotRequest error: " + ex.ToString());
                return null;
            }
        }

        // empty POST request
        public async Task<string> PostRequestEmpty(string uri)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, null);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Success: PostRequestEmpty");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    Debug.LogWarning("Error: " + (response == null ? "No response" : response.StatusCode.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("PostRequestEmpty error: " + ex.ToString());
                return null;
            }
        }

        // helper methods for formatting date and time
        public string FormatDateTimetoHHmm(string dateTimeStr)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeStr, out dateTime))
            {
                return dateTime.ToString("HH:mm");
            }
            return string.Empty;
        }

        public string FormatDateTimetoHHmmss(string dateTimeStr)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeStr, out dateTime))
            {
                return dateTime.ToString("HH:mm:ss");
            }
            return string.Empty;
        }

        public string FormatDateTimetoYMDHHmmss(string dateTimeStr)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeStr, out dateTime))
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }


        // destroys all children of given transform
        public void ClearParent(Transform parentTransform)
        {
            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
                Debug.Log("Success: ClearParent");
            }
        }
    }
}
