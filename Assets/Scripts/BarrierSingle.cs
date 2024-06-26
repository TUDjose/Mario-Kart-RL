using System;
using UnityEngine;

public class BarrierSingle : MonoBehaviour
{
    [NonSerialized] public TrackBarrier trackBarrier;
    [SerializeField] private bool disableCollisions;

    private void OnTriggerEnter(Collider other)
    {
        if (!disableCollisions && other.gameObject.TryGetComponent(out KartController kart))
        {
            trackBarrier.KartHit(other.transform);
        }
    }
}