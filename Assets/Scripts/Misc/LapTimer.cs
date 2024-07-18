using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LapTimer : MonoBehaviour
{
    private PlayerManager pm;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private CheckpointSingle chkpt;
    private float elapsedTime;
    
    private static string directory = "/SaveData/";
    private static string fileName = "SaveGame.sav";


    private void Awake()
    {
        pm = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
        pm.OnStartTrack += StartLap;
        pm.OnEndTrack += StoreLeaderboardData;
    }

    private void Update()   
    {
        if (pm.kart.mode == GameMode.Player && pm.isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplayTimer();
        }
    }

    private void UpdateDisplayTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000F) % 1000F);
        timer.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void StartLap(object sender, EventArgs e)
    {
        elapsedTime = 0f;
    }

    private void StoreLeaderboardData(object sender, EventArgs e)
    {
        string dir = Application.persistentDataPath + directory;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        int idx = int.Parse(SceneManager.GetActiveScene().name);
        switch (idx)
        {
            case 1 :
                pm.Data.Time1 = timer.text;
                break;
            case 2:
                pm.Data.Time2 = timer.text;
                break;
            case 3 :
                pm.Data.Time3 = timer.text;
                break;
        }

        string json = JsonConvert.SerializeObject(pm.Data);
        File.AppendAllText(dir+fileName, json);
    }
}