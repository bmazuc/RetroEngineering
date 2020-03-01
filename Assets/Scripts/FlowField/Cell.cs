using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Cell
{
    public Vector3 position;
    public Vector2 direction;
    public int distance;  // distance from goal (in cell count)
    public bool isMarked = false;
    public bool unpassable = false;

    public Cell(Vector3 inPosition)
    {
        distance = int.MaxValue;
        position = inPosition;
    }
}
