using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;


public class Config
{
    private static Config instance;
    public static Config Instance
    {
        get {
            if (instance == null)
            {
               
                instance = new Config();
                instance.ReadConfig();
            } 
            return instance;
        }

    }
    public  string ServerIP;
    private CMUTrackerPreset<int> cmTrackPreset;
    public CMUTrackerPreset<int> CMTrackPreset
    {
        get
        {
            if (cmTrackPreset != null)
            {
                return cmTrackPreset;
            }
            else
            {
                return null;
            }
        }
    }
    private Config()
    {
        
    }
    private  void ReadConfig()
    {
        string Application_dataPath = Application.dataPath;
        Application_dataPath = Application_dataPath.Replace("/", "\\");

        string jsonString = LoadFile(Application_dataPath, "Config.json");

        cmTrackPreset = JsonUtility.FromJson<CMUTrackerPreset<int>>(jsonString);
        ServerIP = "MCServer@" + cmTrackPreset.ServerIP;
        cmTrackPreset.ServerIP = ServerIP;

    }
    private string LoadFile(string path, string fileName)
    {

        if (!File.Exists(path + "\\"+ fileName))
        {
            File.Create(path + "\\" + fileName);

        }

        return ReadJonsFile(path + "\\" + fileName);
    }
    public static string ReadJonsFile(string JsonFlieUrl)
    {
        StreamReader streamReader = new StreamReader(JsonFlieUrl, Encoding.UTF8);
        string JsonString = streamReader.ReadToEnd();
        streamReader.Close();
        return JsonString;
    }

  
}

public class CMUTrackerPreset<T>
{
    public string ServerIP;
    public List<T> Bodies;
    public List<T> IMUBodies;
    public List<T> Humans;

}