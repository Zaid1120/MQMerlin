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
    private DateTime lastReadTime;  // Store last read time

    void Start()
    {
        // Get the desktop path based on the operating system
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Path to the log file on the desktop
        readFromFilePath = Path.Combine(desktopPath, "Recall_Log", "ResolveLog.txt");


        // Check if the file exists and read its contents
        if (File.Exists(readFromFilePath))
        {
            lastReadTime = File.GetLastWriteTime(readFromFilePath);
            ReadFileAndUpdate();
        }

        // Start the routine to periodically check for updates to the log file
        StartCoroutine(CheckForUpdates());


    }

    void ReadFileAndUpdate()
    {
        // Check if the file exists
        if (File.Exists(readFromFilePath))
        {
            try
            {
                List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();

                // Only read and display new lines since the last read
                for (int i = lastLineCount; i < fileLines.Count; i++)
                {
                    GameObject textObject = Instantiate(textPrefab, logContent);
                    textObject.GetComponent<TMP_Text>().text = fileLines[i];
                }

                // Update the count of the last read line
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
        // This routine checks for new content in the log file every 5 seconds
        while (true)
        {
            yield return new WaitForSeconds(5);

            // If the file's last modification time is more recent than our last read, it indicates new content
            if (File.Exists(readFromFilePath) && File.GetLastWriteTime(readFromFilePath) > lastReadTime)
            {
                ReadFileAndUpdate();
                lastReadTime = File.GetLastWriteTime(readFromFilePath);  // Update the last read time to the most recent read
            }
        }
    }
}
