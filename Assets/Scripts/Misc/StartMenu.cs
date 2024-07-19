using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Transform kart, figure;
    private List<string> tracks;
    [SerializeField] private GameObject start, diy, rl, leader;

    private void Start()
    {
        diy.SetActive(false);
        rl.SetActive(false);
        
        tracks = new List<string>
        {
            "2",
            "3",
            "1",
            "SimpleTrack",
            "HardTrack",
            "Training"
        };
    }

    private void Update()
    {
        if(kart != null && figure != null);
        {   
            kart.rotation *= Quaternion.Euler(0f, 10*Time.deltaTime, 0f);
            if (kart.rotation.y > 360) kart.rotation = Quaternion.Euler(kart.rotation.x, 0f, 0f);
            figure.rotation *= Quaternion.Euler(0f, 15*Time.deltaTime, 0f);
            if (figure.rotation.y > 360) figure.rotation = Quaternion.Euler(figure.rotation.x, 0f, 0f);
        }
    }

    public void SelectTrack(int i)
    {
        if (i > tracks.Count - 1) return;
        SceneManager.LoadScene(tracks[i]);
    }

    public void ClickDIY()
    {
        start.SetActive(false);
        diy.SetActive(true);
    }

    public void ClickRL()
    {
        start.SetActive(false);
        rl.SetActive(true);
    }

    public void Return()
    {
        diy.SetActive(false);
        rl.SetActive(false);
        leader.SetActive(false);
        start.SetActive(true);
    }

    public void ClickLeaderboard()
    {
        start.SetActive(false);
        leader.SetActive(true);
    }

    public void ClickExit()
    {
        Application.Quit();
    }
    
}
