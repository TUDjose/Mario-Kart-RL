using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
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
            AddReward(.1f);
            if (e.final)
            {
                AddReward(1f);

                TrainingManager tm = GameObject.FindWithTag("Manager").GetComponent<TrainingManager>();
                
                if (e.numSteps <= tm.BestLapTime)
                {
                    tm.BestLapTime = e.numSteps;
                    AddReward(5f);
                }
                
                EndEpisode();
            }
        }
    }
    private void PassWrongCheckpoint(object sender, TrackCheckpoints.OnKartThroughCheckpointArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
    
    private void KartHitBarrier(object sender, TrackBarrier.OnHitBarrierArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(-.5f);

            if (inference) Destroy(gameObject);
            else EndEpisode();
        }
    }

    private void KartHitObstacle(object sender, TrackObstacles.OnHitObstacleArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
    
    public override void OnEpisodeBegin()
    {
        transform.position = spawnPoint.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-4f, 4f));
        transform.forward = spawnPoint.forward;
        kartController.StopKart();
        trackCheckpoints.ResetCheckpoints(transform);
        trackObstacles.MoveObstacle();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = trackCheckpoints.GetNextCheckpoint(transform).transform.position - transform.position;
        Vector3 obs = new Vector3(diff.x, transform.position.y, diff.z) / 10f;
        sensor.AddObservation(obs);
        
        sensor.AddObservation(kartController.realSpeed);

        AddReward((kartController.realSpeed - 30f)/4000);      // reward high speeds and punish low ones
        AddReward(-0.002f);     // existence punishment to speed up kart
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
    
        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = -1f;
                //AddReward(-0.1f);
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
        action[0] = (int) Input.GetAxisRaw("Vertical") + 1;
        action[1] = (int)Input.GetAxisRaw("Horizontal") + 1;
    }

    // public void ClickChangeBehaviourToInference()
    // {
    //     inference = !inference;
    //     BehaviorType behaviour = inference ? BehaviorType.InferenceOnly : BehaviorType.Default;
    //     
    //     foreach (Transform kart in trackCheckpoints.kartTransformList)
    //     {
    //         kart.GetComponent<KartAgent>().inference = inference;
    //         BehaviorParameters behaviorParameters = kart.GetComponent<BehaviorParameters>();
    //         behaviorParameters.BehaviorType = behaviour;
    //     }
    // }
    
}
