using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrackBarrier : MonoBehaviour
{
    private bool hide;
    [SerializeField] private Transform barriers;
    // private List<BarrierSingle> barrierList = new List<BarrierSingle>();
    public event EventHandler<OnHitBarrierArgs> OnHitBarrier;
    public class OnHitBarrierArgs : EventArgs
    {
        public Transform kartT;
        public bool stay = false;
    }
    
    private void Awake()
    {
        foreach (Transform barrier in barriers)
        {
            BarrierSingle bs = barrier.GetComponent<BarrierSingle>();
            bs.trackBarrier = this;
            // barrierList.Add(bs);
        }
    }

    public void KartHit(Transform kart)
    {
        OnHitBarrier?.Invoke(this, new OnHitBarrierArgs{kartT = kart});
    }
    
    public void ClickHide()
    {
        foreach (Transform cp in barriers)
        {
            cp.GetComponent<MeshRenderer>().enabled = hide;
        }
    }
}
