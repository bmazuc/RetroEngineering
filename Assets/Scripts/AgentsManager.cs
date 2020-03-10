using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int agentCount = 10;
    [SerializeField] private float spawnRadius = 2f;

    [SerializeField] SwarmBrain swarmBrain;
    [SerializeField] private Color boundColor = Color.white;
    [SerializeField] private Color radiusColor = Color.red;
    [SerializeField] private bool drawGizmos = false;
    Vector3 spawnposition;
    List<Agent> agents = new List<Agent>();

    private delegate void DebugDelegate();
    private DebugDelegate OnGetTarget;
    private DebugDelegate OnDetectTarget;

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = boundColor;
        Gizmos.DrawCube(transform.position, swarmBrain.worldExtents * 2f);
        Gizmos.color = radiusColor;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    void Start()
    {
        swarmBrain.flowField = FlowField.Instance;

        for (int i = 0; i < agentCount; ++i)
        {
            spawnposition = transform.position + (Random.insideUnitSphere * spawnRadius);
            spawnposition.y = 0.15f;
            Agent agent = Instantiate(agentPrefab, spawnposition, Quaternion.identity).GetComponent<Agent>();
            agent.transform.parent = transform;
            agent.Brain = swarmBrain;
            swarmBrain.agents = agents;
            agent.interestSource = swarmBrain.target.transform;
            OnDetectTarget += agent.Detect;
            OnGetTarget += agent.SwitchIsWandering;
            agents.Add(agent);
        }
    }

    private void Update()
    {
        int agentCount = agents.Count;

        if (Input.GetKeyDown(KeyCode.T))
            FlowField.Instance.GeneratePathTo(swarmBrain.target.position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlowField.Instance.GeneratePathTo(swarmBrain.target.position);       
            for (int i = 0; i < agentCount; ++i)
                agents[i].Detect();
        }

        for (int i = 0; i < agentCount; ++i)
            agents[i].Compute();
    }
}
