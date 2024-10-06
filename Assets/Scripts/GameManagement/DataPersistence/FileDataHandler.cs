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
            fileStream = new FileStream(fullPath, FileMode.OpenOrCreate);
            fileWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8, 64, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to open file: " + fullPath + "\n" + e);
            fileStream.Close();
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
        catch (Exception)
        {
            throw;
        }
    }

    //Interact with file methods
    public void WriteObjectToJson(object data)
    {
        try
        {
            string dataToStore = JsonUtility.ToJson(data, true);
            fileWriter.WriteLine(dataToStore);
            Debug.Log(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to write to file: " + fullPath + "\n" + e);
        }
    }

    public object ReadObjectFromJson(string jsonTag, Type returnType)
    {
        try
        {
            fileReader.DiscardBufferedData();
            fileReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string data = GetSingleJsonObjectString(jsonTag);
            Debug.Log(data);
            var method = typeof(JsonUtility).GetMethod("FromJson", new Type[] { typeof(string) });
            var genericMethod = method.MakeGenericMethod(returnType);
            return genericMethod.Invoke(null, new object[] { data });
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to read file: " + fullPath + "\n" + e);
            return null;
        }
    }

    private string GetSingleJsonObjectString(string tag)
    {
        string array = "";
        string json = fileReader.ReadToEnd();
        int startIndex = json.IndexOf(tag);
        if (startIndex != -1)
        {
            startIndex = json.Substring(0, startIndex).LastIndexOf("{");
            //Find index of corresponding closing brace
            int endIndex = -1;
            Stack<char> braces = new Stack<char>();
            for(int i = startIndex; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    braces.Push(char.ToUpper(json[i]));
                } else if (json[i] == '}')
                {
                    braces.Pop();
                    if(braces.Count == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            if(endIndex != -1)
            {
                array = json.Substring(startIndex, (endIndex - startIndex) + 1);
            }
        }
        return array;
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
