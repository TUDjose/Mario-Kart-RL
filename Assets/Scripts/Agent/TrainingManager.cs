using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using System.IO;
using Debug = UnityEngine.Debug;


public class TrainingManager : MonoBehaviour
{
    public int BestLapTime = 100_000;
    [NonSerialized] public int envs;
    
    private MapData[] environments;
    private KartAgent[] agents;
    private TrackCheckpoints[] trackCheckpoints;

    [Header("Curriculum Learning")]
    public List<Lesson> Curriculum;
    public bool ApplyCurriculum;
    public int currLesson;
    [SerializeField] private int stepRecorder = 0;
    
    private string path;
    public bool recordAnalytics = true;


    private void Start()
    {
        if (recordAnalytics)
        {
            path = "MKRL/Unity/analytics_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            File.WriteAllText(path, "");
        }
        
        agents = (KartAgent[])FindObjectsOfType(typeof(KartAgent));
        trackCheckpoints = (TrackCheckpoints[])FindObjectsOfType(typeof(TrackCheckpoints));
        environments = (MapData[])FindObjectsOfType(typeof(MapData));

        envs = environments.Length;

        if (!ApplyCurriculum)
        {
            SetLesson(3);
        }
        else
        {
            SetLesson(0);
        }
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
        stepRecorder = Academy.Instance.TotalStepCount * envs;
        if(ApplyCurriculum) ChooseLesson();
    }

    private void ChooseLesson()
    {
        switch (stepRecorder)
        {
            case < (int)1e5:
                // Debug.Log("lesson 0 - no braking");
                SetLesson(0);  
                break;
            case < (int)2e6 and > (int)1e5:
                // Debug.Log("lesson 1 - allow braking");
                SetLesson(1);  
                break;
            case < (int)5e6 and > (int)2e6:
                // Debug.Log("lesson 2 - add reward for speed (must have completed most of track)");
                SetLesson(2); 
                break;
            case < (int)1.5e7 and > (int)2e6:
                // Debug.Log("lesson 3 - add obstacles and offroad");
                SetLesson(3);
                break;
            // case > (int)1.5e7:
            //     // Debug.Log("lesson 4 - add boosts");
            //     SetLesson(4);  
            //     break;
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

    public void StoreAnalytics(AnalyticsData data)
    {
        if(recordAnalytics) File.AppendAllText(path, data.ToCSV());
    }
}

