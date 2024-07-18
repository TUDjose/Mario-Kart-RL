using System;
using UnityEngine;

public class ObstacleSingle : MonoBehaviour
{
    [NonSerialized] public TrackObstacles trackObstacles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out KartController kart))
        {
            trackObstacles.KartHit(other.transform);
        }
    }
}