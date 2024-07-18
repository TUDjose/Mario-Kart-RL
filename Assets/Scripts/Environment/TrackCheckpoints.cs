using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackCheckpoints : MonoBehaviour
{
    private bool hide;
    public Transform checkpoints;
    
    private List<CheckpointSingle> checkpointsList;
    public List<int> nextCheckpointIndexList;
    public List<Transform> kartTransformList;

    public event EventHandler<OnKartThroughCheckpointArgs> OnCorrectCheckpoint;
    public event EventHandler<OnKartThroughCheckpointArgs> OnWongCheckpoint;

    public class OnKartThroughCheckpointArgs : EventArgs
    {
        public Transform kartT;
        public bool final;
        public int numSteps;
    }
    
    private void Awake()
    {
        nextCheckpointIndexList = new List<int>();

        foreach (Transform kart in kartTransformList)
        {
            nextCheckpointIndexList.Add(0);
        }
        
        checkpointsList = new List<CheckpointSingle>();
        
        foreach (Transform cp in checkpoints)
        {
            CheckpointSingle cpSingle = cp.GetComponent<CheckpointSingle>();
            cpSingle.SetTrackCheckpoints(this);
            checkpointsList.Add(cpSingle);
        }
    }

    public void KartThroughCheckpoint(CheckpointSingle cpSingle, Transform kartTransform)
    {
        int idx = kartTransformList.IndexOf(kartTransform);
        int nextCheckpointIndex = nextCheckpointIndexList[idx];
        KartAgent agent = kartTransform.GetComponent<KartAgent>();
        
        if (nextCheckpointIndex == 0 && agent.numCheckpoints == checkpointsList.Count)
        {
            OnCorrectCheckpoint?.Invoke(this, new OnKartThroughCheckpointArgs
            {
                kartT = kartTransform,
                final = true,
                numSteps = agent.StepCount
            });
        }
        else if (checkpointsList.IndexOf(cpSingle) == nextCheckpointIndex)
        {
            nextCheckpointIndexList[idx] = (nextCheckpointIndexList[idx] + 1) % checkpointsList.Count;
            OnCorrectCheckpoint?.Invoke(this, new OnKartThroughCheckpointArgs{kartT = kartTransform});
        }
        else
        {
            OnWongCheckpoint?.Invoke(this, new OnKartThroughCheckpointArgs{kartT = kartTransform});
            nextCheckpointIndexList[idx] = 0;
        }
    }

    public void ResetCheckpoints(Transform kart)
    {
        int idx = kartTransformList.IndexOf(kart);
        nextCheckpointIndexList[idx] = 0;
    }

    public GameObject GetNextCheckpoint(Transform kart)
    {
        int idx = kartTransformList.IndexOf(kart);
        int currCheckpoint = nextCheckpointIndexList[idx];
        return checkpointsList[currCheckpoint].gameObject;
    }

    public void ClickHide()
    {
        foreach (Transform cp in checkpoints)
        {
            cp.GetComponent<MeshRenderer>().enabled = hide;
        }
    }
}
