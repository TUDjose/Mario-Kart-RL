using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public TextMeshProUGUI maxCP, time;
    public TrackCheckpoints tcp;
    private int MAX;
    private int TIME = 100_000;

    private void Awake()
    {
        tcp.OnCorrectCheckpoint += GetFastestTime;
    }

    private void GetFastestTime(object sender, TrackCheckpoints.OnKartThroughCheckpointArgs e)
    {
        if (e.final)
        {
            TIME = e.numSteps < TIME ? e.numSteps : TIME;
        }
    }
    private void Update()
    {
        MAX = tcp.nextCheckpointIndexList.Max() > MAX ? tcp.nextCheckpointIndexList.Max() : MAX;
        maxCP.text = "Max Checkpoint: " + MAX;
        time.text = "Fastest Lap Time: " + TIME;
    }
}
