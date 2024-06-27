using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform kart;
    public Transform target;
    public Vector3 offset = new Vector3(-5f, 5f, 0f);
    public bool trackKart = false;
    
    void FixedUpdate()
    {
        if (trackKart)
        {
            transform.LookAt(kart);
            float move = Mathf.Abs(Vector3.Distance(transform.position, target.position) * 5f);
            transform.position = Vector3.MoveTowards(transform.position, target.position, move * Time.deltaTime);
        }
    }

    public void ClickCamera()
    {
        trackKart = !trackKart;
    }
}
