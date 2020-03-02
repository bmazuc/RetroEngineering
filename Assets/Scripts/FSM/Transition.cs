using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Transition
{
    private Func<bool> conditionFunction;

    public Transition(Func<bool> inConditionFunction)
    {
        conditionFunction = inConditionFunction;
    }

    public bool CheckCondition()
    {
        return conditionFunction == null || conditionFunction();
    }
}
