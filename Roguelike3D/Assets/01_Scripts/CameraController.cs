using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offSet;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = target.position + offSet;
    }
}