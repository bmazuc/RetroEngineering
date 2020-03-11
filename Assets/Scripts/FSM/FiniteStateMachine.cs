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

    private Dictionary<State, List<TransitionStatePair>> transitionMap;

    public FiniteStateMachine()
    {
        states = new List<State>();
        transitionMap = new Dictionary<State, List<TransitionStatePair>>();
    }

    public State AddState(bool isFirstState = false)
    {
        State state = new State();
        states.Add(state);

        if (isFirstState)
            current = state;

        return state;
    }

    public void AddTransition(State from, State to, Func<bool> condition)
    {
        Transition transition = new Transition(condition);
        TransitionStatePair pair = new TransitionStatePair(transition, to);
        if (!transitionMap.ContainsKey(from))
            transitionMap.Add(from, new List<TransitionStatePair>());
        transitionMap[from].Add(pair);
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

        if (!transitionMap.ContainsKey(current))
            return;

        List<TransitionStatePair> pairList = transitionMap[current];
        int length = pairList.Count;
        for (int i = 0; i < length; ++i)
        {
            if (pairList[i].Key.CheckCondition())
            {
                current.Exit();
                current = pairList[i].Value;
                current.Enter();
                break;
            }
        }
    }
}
