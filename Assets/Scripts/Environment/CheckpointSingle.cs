using System;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    public bool ignore;
    private PlayerManager pm;

    private void Awake()
    {
        pm = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        KartAgent kart = other.gameObject.GetComponent<KartAgent>();
        
        if (kart.mode == GameMode.Training)
        {
            trackCheckpoints.KartThroughCheckpoint(this, other.transform);
        }

        if (kart.mode == GameMode.Player && !ignore)
        {
            trackCheckpoints.KartThroughCheckpoint(this, other.transform);
        }
        
        SetTiming(kart);
    }

    private void SetTiming(KartAgent kart)
    {
        if (pm != null && kart.mode == GameMode.Player && !ignore)
        {
            if (!pm.isTiming)
            {
                pm.InvokeStart();
                pm.isTiming = true;
            }
            else
            {
                pm.InvokeEnd();
                pm.isTiming = false;
            }
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints _trackCheckpoints)
    {
        trackCheckpoints = _trackCheckpoints;
    }
}
