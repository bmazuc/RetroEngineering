using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    public Action OnEnter;
    public Action OnUpdate;
    public Action OnExit;

    public void Enter()
    {
        OnEnter?.Invoke();
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }
}
