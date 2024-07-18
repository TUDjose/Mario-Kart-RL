using System;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    [SerializeField] private bool ignore;
    private PlayerManager pm;

    private void Awake()
    {
        pm = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out KartAgent kart))
        {
            trackCheckpoints.KartThroughCheckpoint(this, other.transform);

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
    }

    public void SetTrackCheckpoints(TrackCheckpoints _trackCheckpoints)
    {
        trackCheckpoints = _trackCheckpoints;
    }
}
