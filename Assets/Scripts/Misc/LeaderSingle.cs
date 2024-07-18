using System;
using TMPro;
using UnityEngine;

public class LeaderSingle : MonoBehaviour
{
    public SaveData MyData;
    public TextMeshProUGUI pn, t1, t2, t3;

    private void Start()
    {
        pn.text = MyData.PlayerName;
        t1.text = MyData.Time1;
        t2.text = MyData.Time2;
        t3.text = MyData.Time3;
    }
}