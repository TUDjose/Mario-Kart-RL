using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Policies;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public int BestLapTime = 100_000;

    private KartAgent[] agents;
    private TrackCheckpoints[] trackCheckpoints;

    private void Start()
    {
        agents = (KartAgent[])FindObjectsOfType(typeof(KartAgent));
        trackCheckpoints = (TrackCheckpoints[])FindObjectsOfType(typeof(TrackCheckpoints));
    }

    public void HeuristicsTesting()
    {
        foreach (KartAgent agent in agents)
        {
            agent.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.HeuristicOnly;
        }

        foreach (TrackCheckpoints TC in trackCheckpoints)
        {
            List<Transform> lst = new();
            foreach (Transform T in TC.checkpoints)
            {
                lst.Add(T);
            }

            foreach (Transform T in lst.Skip(1))
            {
                T.gameObject.SetActive(false);
            }
        }
    }
}
