using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Playables;
using UnityEngine.Profiling;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private string fullPath = "";

    private FileStream fileStream;
    private StreamWriter fileWriter;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public void OpenFile()
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            fileStream = new FileStream(fullPath, FileMode.Create);
            fileWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8, 64, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to open file: " + fullPath + "\n" + e);
        }
    }

    public void WriteObjectToJson(object data)
    {
        try
        {
            string dataToStore = JsonUtility.ToJson(data, true);
            fileWriter.WriteLine(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to write to file: " + fullPath + "\n" + e);
        }

    }

    public void CloseFile()
    {
        try
        {
            fileWriter.Close();
            fileStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to close file: " + fullPath + "\n" + e);
        }
    }
}
