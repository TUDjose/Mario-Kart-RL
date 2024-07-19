using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LapTimer : MonoBehaviour
{
    public PlayerManager pm;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private CheckpointSingle chkpt;
    private float elapsedTime;


    private void Awake()
    {
        pm = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
        pm.OnStartTrack += StartLap;
        pm.OnEndTrack += StoreLeaderboardData;
        
        if(SceneManager.GetActiveScene().name == "1") Time.timeScale = 0;
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
        string dir = Application.persistentDataPath + "/times.txt";
        string[] times = File.ReadAllLines(dir);
        times[int.Parse(SceneManager.GetActiveScene().name) - 1] = timer.text;
        File.WriteAllLines(dir, times);
    }
}