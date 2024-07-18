﻿using UnityEngine;

[RequireComponent(typeof(TrackCheckpoints))]
[RequireComponent(typeof(TrackBarrier))]
[RequireComponent(typeof(TrackObstacles))]
public class MapData : MonoBehaviour
{
    public TrackCheckpoints Checkpoints;
    public TrackBarrier Barriers;
    public TrackObstacles Obstacles;
    public Transform Spawn;
    public Transform Track;

    public void SpawnObjects(string obj)
    {
        switch (obj)
        {
            case "Obstacles":
                Obstacles.SpawnObstacles();
                break;
            case "Offroad":
                foreach (Transform offroad in Track)
                {
                    if (offroad.gameObject.CompareTag("Grass"))
                    {
                        offroad.GetComponent<MeshCollider>().enabled = true;
                    }
                }
                break;
            case "Boost":
                foreach (Transform speed in Track)
                {
                    if (speed.gameObject.CompareTag("Speed"))
                    {
                        speed.GetComponent<MeshCollider>().enabled = true;
                    }
                }
                break;
        }
    }
}