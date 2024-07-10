using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;


public class TrainingManager : MonoBehaviour
{
    public int BestLapTime = 100_000;
    private int envs;
    
    private MapData[] environments;
    private KartAgent[] agents;
    private TrackCheckpoints[] trackCheckpoints;

    [Header("Curriculum Learning")]
    public List<Lesson> Curriculum;
    public bool ApplyCurriculum;
    public int currLesson;

    private void Start()
    {
        agents = (KartAgent[])FindObjectsOfType(typeof(KartAgent));
        trackCheckpoints = (TrackCheckpoints[])FindObjectsOfType(typeof(TrackCheckpoints));
        environments = (MapData[])FindObjectsOfType(typeof(MapData));

        envs = environments.Length;
        
        if(!ApplyCurriculum) SetLesson(3);
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

    private void FixedUpdate()
    {
        if(ApplyCurriculum) ChooseLesson();
    }

    private void ChooseLesson()
    {
        int step = Academy.Instance.TotalStepCount * envs;

        switch (step)
        {
            case 0:
                Debug.Log("lesson 0 - no braking");
                SetLesson(0);  
                break;
            case (int)5e5:
                Debug.Log("lesson 1 - allow braking");
                SetLesson(1);  
                break;
            case (int)2e6:
                Debug.Log("lesson 2 - add reward for speed (must have completed most of track)");
                SetLesson(2); 
                break;
            case(int)5e6:
                Debug.Log("lesson 3 - add obstacles and offroad");
                SetLesson(3);
                break;
            case(int)1.5e7:
                Debug.Log("lesson 4 - add boosts");
                SetLesson(4);  
                break;
        }
    }

    private void SetLesson(int i)
    {
        currLesson = i;
        foreach (KartAgent agent in agents)
        {
            agent.lesson = Curriculum[i];
        }

        foreach (string spawn in Curriculum[i].spawner)
        {
            foreach (MapData map in environments)
            {
                map.SpawnObjects(spawn);
            }
        }
    }
    
}
