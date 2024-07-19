using System;
using System.IO;
using TMPro;
using UnityEngine;


public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI[] lapTimes;

    private void OnEnable()
    {
        string path = Application.persistentDataPath + "/times.txt";
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("0");
            }
        }
        
        string[] times = File.ReadAllLines(path);
        for (int i = 0; i < 3; i++)
        {
            lapTimes[i].text = times[i];
        }
    }
}