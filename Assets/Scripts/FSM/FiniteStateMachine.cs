using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TransitionStatePair = System.Collections.Generic.KeyValuePair<Transition, State>;

public class FiniteStateMachine
{
    State current;
    State previous;

    private List<State> states;

    private Dictionary<State, List<Transition>> transitionMap;

    public FiniteStateMachine()
    {
        states = new List<State>();
    }

    public State AddState(bool isFirstState = false)
    {
        State state = new State();
        states.Add(state);

        if (isFirstState)
            current = state;

        return state;
    }

    public void Start()
    {
        if (current == null)
            return;

        current.Enter();
    }

    public void Execute()
    {
        if (current == null)
            return;

        current.Update();
    }
}
