using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int agentCount = 10;
    [SerializeField] private float spawnRadius = 2f;

    [SerializeField] AgentSettings agentSettings;
    [SerializeField] private Color color = Color.white;
    Vector3 spawnposition;
    List<Agent> agents = new List<Agent>();

    [SerializeField] private Transform target;

    private delegate void DebugDelegate();
    private DebugDelegate OnGetTarget;
    private DebugDelegate OnDetectTarget;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, (agentSettings.worldExtents - transform.position) * 2f);
    }

    void Start()
    {
        for (int i = 0; i < agentCount; ++i)
        {
            spawnposition = transform.position + (Random.insideUnitSphere * spawnRadius);
            spawnposition.y = 0.15f;
            Agent agent = Instantiate(agentPrefab, spawnposition, Quaternion.identity).GetComponent<Agent>();
            agent.transform.parent = transform;
            agent.Settings = agentSettings;
            agent.interestSource = target.transform;
            OnDetectTarget += agent.Detect;
            OnGetTarget += agent.SwitchIsWandering;
            agents.Add(agent);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            FlowField.Instance.GeneratePathTo(target.position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlowField.Instance.GeneratePathTo(target.position);
            OnDetectTarget?.Invoke();
        }

        for (int i = 0; i < agentCount; ++i)
            agents[i].Compute(agents);
    }
}
