using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AgentSettings
{
    public Transform target;
    public RuleData separation;
    public RuleData cohesion;
    public RuleData alignment;
    public float maxSpeed;
    public float maxForce;
    public float mass;
    public float stopRadius;
    public float seekWeight;
    public float fleeWeight;
    public Vector3 worldExtents;
}

[System.Serializable]
public struct RuleData
{
    public Vector3 steer;
    public int neighbour;
    public float radius;
    public float weight;

    public void Reset()
    {
        steer = Vector3.zero;
        neighbour = 0;
    }
}

public enum AgentState
{
    Wandering,
    Seeking
}

public class Agent : MonoBehaviour
{
    [SerializeField] AgentSettings settings;
    public AgentSettings Settings { set { settings = value; } }
    Vector3 desiredVelocity;
    Vector3 velocity;
    Vector3 acceleration;

    bool isWandering = true;

    private enum States
    {
        Wandering,
        Detecting,
        Targeting,
        Scared
    }

    //Wander
    private Vector3 circleCenter;
    private Vector3 displacement;
    [SerializeField] private float circleDistance = 1f;
    [SerializeField] private float circleRadius = 5f;
    [SerializeField] private float wanderAngle = 0f;
    [SerializeField] private float angleChange = 15f;
    [SerializeField] private float wanderWeight = 1f;

    FlowField flowField;
    [SerializeField] private float flowFieldWeight = 1f;

    bool isScare = false;
    Transform fearSource;
    bool isDetecting = false;
    public Transform interestSource;

    private float detectionTimer;
    [SerializeField] private float detectionDuration = 2f;

    FiniteStateMachine fsm;

    public void Detect()
    {
        isDetecting = true;
        isWandering = false;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        detectionTimer = 0f;
    }

    public void SwitchIsWandering()
    {
        isWandering = !isWandering;
    }

    public void Scare(Transform inFearSource)
    {
        isScare = true;
        fearSource = inFearSource;
    }

    public void Unscare()
    {
        isScare = false;
    }

    void Start()
    {
        flowField = FlowField.Instance;
        Vector3 min = -Vector3.one;
        Vector3 max = Vector3.one;
        velocity = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        InitFSM();
    }

    void InitFSM()
    {
        fsm = new FiniteStateMachine();

        State wanderingState = fsm.AddState(true);
        wanderingState.OnUpdate += Wander;

        State detectingState = fsm.AddState();
        detectingState.OnEnter += () => { isDetecting = false; };
        detectingState.OnUpdate += Detecting;

        State targetingState = fsm.AddState();
        targetingState.OnUpdate += Target;

        State scaredState = fsm.AddState();
        scaredState.OnUpdate += Flee;

        fsm.AddTransition(wanderingState, detectingState, () => { return isDetecting; });
        fsm.AddTransition(detectingState, targetingState, () => { return (detectionTimer >= detectionDuration); });
        fsm.AddTransition(wanderingState, scaredState, () => { return isScare; });
        fsm.AddTransition(targetingState, scaredState, () => { return isScare; });
        fsm.AddTransition(scaredState, wanderingState, () => { return !isScare && isWandering; });
        fsm.AddTransition(scaredState, targetingState, () => { return !isScare && !isWandering; });

        fsm.Start();
    }

    private void Detecting()
    {
        transform.LookAt(interestSource);
        detectionTimer += Time.deltaTime;
    }

    private void Target()
    {
        FollowFlowFieldPath();
        Seek();
       // Flock(agents);
    }

    public void Compute(List<Agent> agents)
    {
        fsm.Execute();
        velocity.y = 0f;
        transform.position += velocity * Time.deltaTime;
        if (!isDetecting)
            transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, settings.maxSpeed);

        CheckWordBound();

        acceleration *= 0f;
    }

    void FollowFlowFieldPath()
    {
        Cell cell = flowField.Grid.GetCellFromWorld(transform.position);
        if (cell == null)
            return;
        Vector2 dir = cell.direction * settings.maxForce;
        ApplyForce(new Vector3(dir.x, 0f, dir.y) * flowFieldWeight);
    }

    public void Wander()
    {
        circleCenter = velocity.normalized * circleDistance;
        displacement = Vector3.right * circleRadius;

        wanderAngle += (Random.Range(-1f, 1f) * angleChange) - (angleChange * 0.5f);
        displacement = Quaternion.AngleAxis(wanderAngle, Vector3.up) * displacement;

        ApplyForce((circleCenter + displacement).normalized * wanderWeight);
    }

    void Align(List<Agent> boids)
    {
        settings.alignment.Reset();

        Agent current;
        int length = boids.Count;
        for (int i = 0; i < length; ++i)
        {
            current = boids[i];
            if (current == this)
                continue;

            float dist = (current.transform.position - transform.position).magnitude;

            if (dist <= 0)
                continue;

            if (dist <= settings.alignment.radius)
            {
                settings.alignment.steer += current.velocity;
                ++settings.alignment.neighbour;
            }
        }

        if (settings.alignment.neighbour > 0)
        {
            settings.alignment.steer /= settings.alignment.neighbour;
            settings.alignment.steer = SteerTowards(settings.alignment.steer);
            //alignment.steer -= velocity;
            //alignment.steer.Normalize();
        }

        ApplyForce(settings.alignment.steer * settings.alignment.weight);
    }

    void Cohere(List<Agent> boids)
    {
        settings.cohesion.Reset();

        Agent current;
        int length = boids.Count;
        for (int i = 0; i < length; ++i)
        {
            current = boids[i];
            if (current == this)
                continue;

            float dist = (current.transform.position - transform.position).magnitude;

            if (dist <= 0)
                continue;

            if (dist <= settings.cohesion.radius)
            {
                settings.cohesion.steer += current.transform.position;
                ++settings.cohesion.neighbour;
            }
        }

        if (settings.cohesion.neighbour > 0)
        {
            settings.cohesion.steer /= settings.cohesion.neighbour;
            settings.cohesion.steer -= transform.position;
            settings.cohesion.steer = SteerTowards(settings.cohesion.steer);
            //cohesion.steer.Normalize();
        }

        ApplyForce(settings.cohesion.steer * settings.cohesion.weight);
    }

    void Separe(List<Agent> boids)
    {
        settings.separation.Reset();

        Agent current;
        int length = boids.Count;
        for (int i = 0; i < length; ++i)
        {
            current = boids[i];
            if (current == this)
                continue;

            float dist = (current.transform.position - transform.position).magnitude;

            if (dist <= 0)
                continue;

            if (dist <= settings.separation.radius)
            {
                Vector3 offset = (current.transform.position - transform.position);
                float distanceSqr = dist * dist;// Vector3.Scale(offset, offset).magnitude;
                settings.separation.steer += offset / -distanceSqr;
                ++settings.separation.neighbour;
            }
        }

        if (settings.separation.neighbour > 0)
        {
            settings.separation.steer /= settings.separation.neighbour;
            settings.separation.steer = SteerTowards(settings.separation.steer);
            //separation.steer.Normalize();
        }

        ApplyForce(settings.separation.steer * settings.separation.weight);
    }

    void Flock(List<Agent> boids)
    {
        Align(boids);
        Cohere(boids);
        Separe(boids);
    }

    void Seek()
    {
        if (settings.target)
            ApplyForce(SteerTowards(settings.target.position - transform.position) * settings.seekWeight);
    }

    void Flee()
    {
        ApplyForce(SteerTowards(transform.position - fearSource.transform.position) * settings.fleeWeight);
    }

    Vector3 SteerTowards(Vector3 target)
    {
        Vector3 steering = target.normalized * settings.maxSpeed; // compute desired velocity
        steering -= velocity; // desired velocity - velocity
        return steering;
    }

    void ApplyForce(Vector3 force)
    {
        force = Vector3.ClampMagnitude(force, settings.maxForce);
        acceleration += force;
    }

    public void CheckWordBound()
    {
        Vector3 position = transform.position;
        if (position.x > settings.worldExtents.x || position.x < -settings.worldExtents.x)
            velocity.x *= -1f;

        if (position.y > settings.worldExtents.y || position.y < -settings.worldExtents.y)
            velocity.y *= -1f;

        if (position.z > settings.worldExtents.z || position.z < -settings.worldExtents.z)
            velocity.z *= -1f;
    }
}
