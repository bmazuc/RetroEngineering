using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwarmBrain
{
    public Transform target;
    public Transform Character;
    public Transform agentManager;
    public FlowField flowField;
    public List<Agent> agents;
    public Vector3 worldCenter;
    public Vector3 worldExtents;

    public void Detect()
    {
        int agentCount = agents.Count;
        for (int i = 0; i < agentCount; ++i)
            agents[i].Act();
    }

    public void Compute()
    {
        int agentCount = agents.Count;
        for (int i = 0; i < agentCount; ++i)
            agents[i].Compute();
    }
}