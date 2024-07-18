using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public KartAgent kart;
    public bool isTiming;
    public event EventHandler OnStartTrack;
    public event EventHandler OnEndTrack;
    public SaveData Data;

    private void Start()
    {
        Data = new SaveData("Anon", "00:00:000", "00:00:000","00:00:000");
    }

    public void InvokeStart()
    {
        OnStartTrack?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeEnd()
    {
        // OnEndTrack?.Invoke(this, EventArgs.Empty);
    }
}
