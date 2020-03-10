using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwarmBrain
{
    public Transform target;

    public FlowField flowField;
    public List<Agent> agents;
    public Vector3 worldExtents;

    public void Detect()
    {
        int agentCount = agents.Count;
        for (int i = 0; i < agentCount; ++i)
            agents[i].Detect();
    }

    public void Compute()
    {
        int agentCount = agents.Count;
        for (int i = 0; i < agentCount; ++i)
            agents[i].Compute();
    }
}