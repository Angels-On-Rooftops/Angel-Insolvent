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
    private StreamReader fileReader;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    //Open file methods
    public void OpenFileSave()
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

    public void OpenFileLoad()
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            fileStream = new FileStream(fullPath, FileMode.Open);
            fileReader = new StreamReader(fileStream, System.Text.Encoding.UTF8, false, 64, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to open file: " + fullPath + "\n" + e);
        }
    }

    //Interact with file methods
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

    public object ReadObjectFromJson(Type type)
    {
        try
        {
            string data = fileReader.ReadToEnd();
            var method = typeof(JsonUtility).GetMethod("FromJson", new Type[] { typeof(string) });
            var genericMethod = method.MakeGenericMethod(type);
            return genericMethod.Invoke(null, new object[] { data });
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to read file: " + fullPath + "\n" + e);
            return null;
        }
    }

    //Close file methods
    public void CloseFileSave()
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

    public void CloseFileLoad()
    {
        try
        {
            fileReader.Close();
            fileStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to close file: " + fullPath + "\n" + e);
        }
    }
}
