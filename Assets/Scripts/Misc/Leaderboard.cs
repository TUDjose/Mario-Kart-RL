using System;
using System.IO;
using TMPro;
using UnityEngine;


public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI[] lapTimes;

    private void Start()
    {
        string[] times = File.ReadAllLines(Application.persistentDataPath + "/times.txt");
        for (int i = 0; i < 3; i++)
        {
            lapTimes[i].text = times[i];
        }
    }
}