using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Lesson : ScriptableObject
{
    [Header("Rewards")]
    public float checkpointReward;
    public float finishReward;
    public float bestTimeReward;
    public float wrongCheckpointReward;
    public float hitBarrierReward;
    public float hitObstacleReward;
    public float existenceReward;
    public float reverseReward;
    public bool speedReward;

    [Header("Spawning")] 
    public List<string> spawner;
}