using Newtonsoft.Json;

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Issue
{
    public string issueCode;
    public string startTimeStamp;
    public string generalDesc;
    public TechnicalDetails technicalDetails;
    public string mqobjectType; // Renamed from object_type based on JSON
    public string mqobjectName; // Renamed from object_name based on JSON
    public string objectDetails;

    [System.NonSerialized] // don't wanna serialize this field when converting to/from JSON
    public GameObject PrefabInstance;
}

[System.Serializable]
public class TechnicalDetails
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string appName;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string connName;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string channelName;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string CSPUserId;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string userId;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string isActiveIssue;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string timeSinceReset;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string highQDepth;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string enQCount;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string deQCount;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string QName;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string maxThreshold; // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedRequestRates;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedTimestamps; // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedConnRates;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedconns; // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedlogTimes;  // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archiveduserRatio; // Added based on JSON

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> archivedRequestCount; // Added based on JSON
}

[System.Serializable]
public class IssueCollection
{
    public List<Issue> Issues;
}
