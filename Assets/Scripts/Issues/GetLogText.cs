using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using System;

public class GetLogText : MonoBehaviour
{
    public Transform logContent;
    public GameObject textPrefab;
    private string readFromFilePath;
    private int lastLineCount = 0;
    private DateTime lastReadTime;

    void Start()
    {
        // get desktop path based on the operating system
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // path to log file on desktop
        readFromFilePath = Path.Combine(desktopPath, "Recall_Log", "ResolveLog.txt");


        // check if file exists and read its contents
        if (File.Exists(readFromFilePath))
        {
            lastReadTime = File.GetLastWriteTime(readFromFilePath);
            ReadFileAndUpdate();
        }

        // start routine to check for updates to log file
        StartCoroutine(CheckForUpdates());
    }

    void ReadFileAndUpdate()
    {
        // check if file exists
        if (File.Exists(readFromFilePath))
        {
            try
            {
                List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();

                // only read and display new lines since the last read
                for (int i = lastLineCount; i < fileLines.Count; i++)
                {
                    GameObject textObject = Instantiate(textPrefab, logContent);
                    textObject.GetComponent<TMP_Text>().text = fileLines[i];
                }

                // update count of last read line
                lastLineCount = fileLines.Count;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read or process the log file. Error: " + e.Message);
            }
        }
    }

    IEnumerator CheckForUpdates()
    {
        // this routine checks for new content in the log file every 5 seconds
        while (true)
        {
            yield return new WaitForSeconds(5);

            // if the file's last modification time is more recent than last read, it indicates new content
            if (File.Exists(readFromFilePath) && File.GetLastWriteTime(readFromFilePath) > lastReadTime)
            {
                ReadFileAndUpdate();
                lastReadTime = File.GetLastWriteTime(readFromFilePath);  // update last read time to the most recent read
            }
        }
    }
}
