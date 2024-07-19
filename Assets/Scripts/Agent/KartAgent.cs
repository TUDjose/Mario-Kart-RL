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
            AddReward(lesson.checkpointReward);     // give reward for passing correct checkpoint
            if (e.final)
            {
                AddReward(lesson.finishReward);     // give reward for finishing lap

                TrainingManager tm = GameObject.FindWithTag("Manager").GetComponent<TrainingManager>();
                
                if (e.numSteps <= tm.BestLapTime)
                {
                    tm.BestLapTime = e.numSteps;
                    AddReward(lesson.bestTimeReward);   // give large reward for improving lap time
                }
                
                CompleteEpisode(true);
            }
        }
    }
    private void PassWrongCheckpoint(object sender, TrackCheckpoints.OnKartThroughCheckpointArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.wrongCheckpointReward);    // give negative reward if wrong checkpoint is passed (i.e. kart reverses)
            CompleteEpisode();
        }
    }
    
    private void KartHitBarrier(object sender, TrackBarrier.OnHitBarrierArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.hitBarrierReward);     // give negative reward if kart collides with track barriers

            if (inference) Destroy(gameObject);
            else CompleteEpisode();
        }
    }

    private void KartHitObstacle(object sender, TrackObstacles.OnHitObstacleArgs e)
    {
        if (e.kartT == transform)
        {
            AddReward(lesson.hitObstacleReward);    // give negative reward if kart collides with obstacles
            CompleteEpisode();
        }
    }
    
    public override void OnEpisodeBegin()   // reset environment and kart position after episode ends
    {
        kartController.StopKart();
        transform.position = spawnPoint.position +
                             new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-4f, 4f)); // some randomness in initial kart position
        transform.forward = spawnPoint.forward;
        numCheckpoints = 0;
        trackCheckpoints.ResetCheckpoints(transform);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = trackCheckpoints.GetNextCheckpoint(transform).transform.position - transform.position;
        Vector3 obs = new Vector3(diff.x, transform.position.y, diff.z) / 10f;
        
        sensor.AddObservation(obs);     // give as state input the top-down-view vector between the kart and next correct checkpoint
        sensor.AddObservation(kartController.realSpeed);    // give as the state input the kart's speed

        if (lesson.speedReward)
        {
            AddReward((kartController.realSpeed - 15)/4000);    // give small negative/positive reward w.r.t. speed
        }      
        AddReward(lesson.existenceReward);      // give small negative reward each time step to promote finishing faster
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
    
        switch (actions.DiscreteActions[0])     // convert between horizontal (lft/rght) keyboard input and kart movement 
        {
            case 0: forwardAmount = -1f;
                AddReward(lesson.reverseReward);    // give negative reward for going backwards
                break;
            case 1: forwardAmount = 0f; 
                break;
            case 2: forwardAmount = 1f;
                break;
        }
    
        switch (actions.DiscreteActions[1])     // convert between vertical (fwd/bck) keyboard input and kart movement 
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
        // method to allow kart to be manually controlled for testing
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
        // method to collect and store analytics data when episode is ended (either through finishing lap or through kart making a mistake)
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
        
        EndEpisode();   // end episode to reset agent
    }
}

public enum GameMode    // allow for different mechanics for if player is controlling kart, or if the neural network is
{
    Training,
    Player
}
