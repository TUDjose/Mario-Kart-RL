using System;
using UnityEngine;

public class ObstacleSingle : MonoBehaviour
{
    [NonSerialized] public TrackObstacles trackObstacles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out KartController kart))
        {
            Debug.Log("Hit pylon");
            trackObstacles.KartHit(other.transform);
        }
    }
}