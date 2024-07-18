using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class KartAgent : Agent
{
    [SerializeField] private MapData mapData;
    private TrackCheckpoints trackCheckpoints;
    private TrackBarrier trackBarrier;
    private TrackObstacles trackObstacles;
    private Transform spawnPoint;
    
    private KartController kartController;

    public bool inference;

    public int LAP_TIME = 100_000;
    public int numCheckpoints = 0;

    public Lesson lesson;
    public GameMode mode;
    
    protected override void Awake()
    {
        inference = false;
        kartController = GetComponent<KartController>();

        trackCheckpoints = mapData.Checkpoints;
        trackBarrier = mapData.Barriers;
        trackObstacles = mapData.Obstacles;
        spawnPoint = mapData.Spawn;
        
        trackCheckpoints.kartTransformList.Add(transform);
    }
    
    private void Start()
    {
        trackCheckpoints.OnCorrectCheckpoint += PassCorrectCheckpoint;
        trackCheckpoints.OnWongCheckpoint += PassWrongCheckpoint;
        trackBarrier.OnHitBarrier += KartHitBarrier;
        trackObstacles.OnHitObstacle += KartHitObstacle;
    }

    private void OnDestroy()
    {
        trackCheckpoints.OnCorrectCheckpoint -= PassCorrectCheckpoint;
        trackCheckpoints.OnWongCheckpoint -= PassWrongCheckpoint;
        trackBarrier.OnHitBarrier -= KartHitBarrier;
        trackObstacles.OnHitObstacle -= KartHitObstacle;
    }

    private void PassCorrectCheckpoint(object sender, TrackCheckpoints.OnKartThroughCheckpointArgs e)
    {
        if (e.kartT == transform)
        {
            numCheckpoints++;
            AddReward(lesson.checkpointReward);
            if (e.final)
            {
                AddReward(lesson.finishReward);

                TrainingManager tm = GameObject.FindWithTag("Manager").GetComponent<TrainingManager>();
                
                if (e.numSteps <= tm.BestLapTime)
                {
                    tm.BestLapTime = e.numSteps;
                    AddReward(lesson.bestTimeReward);
                }
                
                CompleteEpisode(true);
            }
        }
    }
    private void PassWrongCheckpoint(object sender, TrackCheckpoints.OnKartThroughCheckpointArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.wrongCheckpointReward);
            CompleteEpisode();
        }
    }
    
    private void KartHitBarrier(object sender, TrackBarrier.OnHitBarrierArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.hitBarrierReward);

            if (inference) Destroy(gameObject);
            else CompleteEpisode();
        }
    }

    private void KartHitObstacle(object sender, TrackObstacles.OnHitObstacleArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.hitObstacleReward);
            CompleteEpisode();
        }
    }
    
    public override void OnEpisodeBegin()
    {
        kartController.StopKart();
        transform.position = spawnPoint.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-4f, 4f));
        transform.forward = spawnPoint.forward;
        numCheckpoints = 0;
        trackCheckpoints.ResetCheckpoints(transform);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = trackCheckpoints.GetNextCheckpoint(transform).transform.position - transform.position;
        Vector3 obs = new Vector3(diff.x, transform.position.y, diff.z) / 10f;
        sensor.AddObservation(obs);
        
        sensor.AddObservation(kartController.realSpeed);

        if(lesson.speedReward) AddReward((kartController.realSpeed - 15)/4000);      
        AddReward(lesson.existenceReward);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
    
        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = -1f;
                AddReward(lesson.reverseReward);
                break;
            case 1: forwardAmount = 0f; 
                break;
            case 2: forwardAmount = 1f;
                break;
        }
    
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = -1f;
                break;
            case 1: turnAmount = 0f;
                break;
            case 2: turnAmount = 1f;
                break;
        }
        
        kartController.SetInputs(forwardAmount, turnAmount);
    }
    
    public override void Heuristic(in ActionBuffers actionOut)
    {
        var action = actionOut.DiscreteActions;
        
        if (Input.GetKey(KeyCode.Space))
        {
            action[0] = 1;
        }
        else
        {
            action[0] = (int)Input.GetAxisRaw("Vertical") + 1;
        }
        
        action[1] = (int)Input.GetAxisRaw("Horizontal") + 1;
    }

    private void CompleteEpisode(bool completed = false)
    {
        // create and store analytics data
        TrainingManager tm = GameObject.FindWithTag("Manager").GetComponent<TrainingManager>();
        AnalyticsData data = new AnalyticsData()
        {
            AgentID = name,
            FinishedLap = completed,
            EpisodeLength = StepCount,
            Reward = GetCumulativeReward(),
            CurrLesson = tm.Curriculum.IndexOf(lesson),
            Step = tm.envs * Academy.Instance.TotalStepCount
        };
        tm.StoreAnalytics(data);
        
        // end episode to reset agent
        EndEpisode();
    }
}

public enum GameMode
{
    Training,
    Player,
    Compete
}
