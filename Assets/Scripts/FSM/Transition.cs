using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Transition
{
    State from;
    State to;
    private Func<bool> conditionFunction;

    public Transition(State inFrom, State inTo, Func<bool> inConditionFunction)
    {
        from = inFrom;
        to = inTo;
        conditionFunction = inConditionFunction;
    }

    public bool CheckCondition()
    {
        return conditionFunction == null || conditionFunction();
    }
}
