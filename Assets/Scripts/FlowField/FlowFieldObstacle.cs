using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldObstacle : MonoBehaviour
{
    void Awake()
    {
        FlowField.Instance.AddObstacle(transform.position);
    }
}
