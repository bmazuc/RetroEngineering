using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Agent : MonoBehaviour
{
    [SerializeField] SwarmBrain brain;
    public SwarmBrain Brain { set { brain = value; } get { return brain; } }

    Vector3 desiredVelocity;
    Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } }
    Vector3 acceleration;

    [SerializeField] private float maxSpeed;
    public float MaxSpeed { get { return maxSpeed; } }
    [SerializeField] private float maxForce;
    public float MaxForce { get { return maxForce; } }

    bool isWandering = true;

    private enum States
    {
        Wandering,
        Detecting,
        Targeting,
        Scared
    }

    [SerializeField] private Behaviours.Wander wander;
    [SerializeField] private Behaviours.FlowFieldPathfinder flowFieldPathfinder;
    [SerializeField] private Behaviours.Flee flee;
    [SerializeField] private Behaviours.Seek seek;
    [SerializeField] private Behaviours.Flock flock;

    bool isScare = false;
    Transform fearSource;
    public Transform FearSource { get { return fearSource; } }
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
        wander.Owner = this;
        flowFieldPathfinder.Owner = this;
        flee.Owner = this;
        seek.Owner = this;
        flock.Owner = this;
        InitFSM();
    }

    void InitFSM()
    {
        fsm = new FiniteStateMachine();

        State wanderingState = fsm.AddState(true);
        wanderingState.OnUpdate += wander.Execute;

        State detectingState = fsm.AddState();
        detectingState.OnEnter += () => { isDetecting = false; };
        detectingState.OnUpdate += Detecting;

        State targetingState = fsm.AddState();
        targetingState.OnUpdate += flowFieldPathfinder.Execute;
        //targetingState.OnUpdate += seek.Execute;
        targetingState.OnUpdate += flock.Execute;

        State scaredState = fsm.AddState();
        scaredState.OnUpdate += flee.Execute;

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

    public void Compute()
    {
        fsm.Execute();
        velocity.y = 0f;
        transform.position += velocity * Time.deltaTime;
        if (!isDetecting)
            transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        CheckWordBound();

        acceleration *= 0f;
    }

    public void ApplyForce(Vector3 force)
    {
        force = Vector3.ClampMagnitude(force, maxForce);
        acceleration += force;
    }

    public void CheckWordBound()
    {
        Vector3 position = transform.position;
        if (position.x > brain.worldExtents.x || position.x < -brain.worldExtents.x)
            velocity.x *= -1f;

        if (position.y > brain.worldExtents.y || position.y < -brain.worldExtents.y)
            velocity.y *= -1f;

        if (position.z > brain.worldExtents.z || position.z < -brain.worldExtents.z)
            velocity.z *= -1f;
    }
}
