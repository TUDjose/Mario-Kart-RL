using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackObstacles : MonoBehaviour
{ 
    [SerializeField] private Transform obstacles;
    public event EventHandler<OnHitObstacleArgs> OnHitObstacle;
     

    public class OnHitObstacleArgs : EventArgs
    {
        public Transform kartT;
    }
    
    private void Awake()
    {
        foreach (Transform obstacle in obstacles)
        {
            ObstacleSingle obs = obstacle.GetComponent<ObstacleSingle>();
            obs.trackObstacles = this;
        }
    }

    public void KartHit(Transform kart)
    {
        OnHitObstacle?.Invoke(this, new OnHitObstacleArgs{kartT = kart});
    }

    public void SpawnObstacles()
    {
        foreach (Transform obstacle in obstacles)
        {
            // obstacle.GetComponent<MeshRenderer>().enabled = true;
            obstacle.GetComponent<MeshCollider>().enabled = true;
        }
    }

    // public void MoveObstacle()
    // {
    //     foreach (Transform obstacle in obstacles)
    //     {
    //         obstacle.position = obstacles.position +
    //                             new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(50f, -35f));
    //     }
    // }
}