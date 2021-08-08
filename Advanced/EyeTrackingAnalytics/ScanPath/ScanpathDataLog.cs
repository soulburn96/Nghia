using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class ScanpathDataLog : MonoBehaviour
{
    StreamWriter writer = null;

    public void Log(string sequence)
    {
        DateTime now = DateTime.Now;

        string fileName = string.Format("ScanpathDataLog-{0}-{1:00}-{2:00}-{3:00}-{4:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute);
        string logPath = Application.dataPath + "/Logs/";
        Directory.CreateDirectory(logPath);

        string path = logPath + fileName + ".csv";
        writer = new StreamWriter(path);

        writer.WriteLine(sequence);
        writer.Flush();
        writer.Close();
        Debug.Log("Scanpath Logged");
    }
}
