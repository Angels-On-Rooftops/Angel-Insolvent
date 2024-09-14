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
        //open json file
        fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            fileStream = new FileStream(fullPath, FileMode.Create);
            fileWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8, 64, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public void WriteObjectToJson(object data)
    {
        //write data object to file (string, int, transform, etc.)
        string dataToStore = JsonUtility.ToJson(data, true);

        // write the serialized data to the file
        fileWriter.WriteLine(dataToStore);
    }

    public void CloseFile()
    {
        //close json file
        fileWriter.Close();
        fileStream.Close();
    }
}
