using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

public class Leaderboard : MonoBehaviour
{
    private static string directory = "/SaveData/";
    private static string fileName = "SaveGame.sav";

    [SerializeField] private ScrollView leaders;
    [SerializeField] private GameObject single;
    private List<SaveData> data;

    private void Start()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;
        string[] jsonStrings = File.ReadAllLines(fullPath);
        foreach (string js in jsonStrings)
        {
            data.Add(JsonConvert.DeserializeObject<SaveData>(js));
        }
    }
}

[Serializable]
public struct SaveData
{
    public string PlayerName;
    public string Time1;
    public string Time2;
    public string Time3;

    public SaveData(string _pn, string t1, string t2, string t3)
    {
        PlayerName = _pn;
        Time1 = t1;
        Time2 = t2;
        Time3 = t3;
    }
}