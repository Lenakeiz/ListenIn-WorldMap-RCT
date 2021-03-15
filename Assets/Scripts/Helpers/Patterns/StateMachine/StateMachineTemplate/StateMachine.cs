using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StateMachine<T>
{
    T agent;
    //tracking current state machine
    AIState<T> currState = null;

    public AIState<T> GetState {
        get { return currState; } set { currState = value; } }

    //contructor
    public StateMachine(T _agent)
    {
        agent = _agent;
    }

    private void SwitchState(AIState<T> state)
    {
        if (currState == state)
        {
            Debug.LogWarning("Switching to the same state: " + state.Name);
        }

        if (currState != null)
        {
            currState.ExitState(agent);
        }            
        currState = state;
        currState.EnterState(agent);
    }

    public void AIUpdate()
    {
        if (currState != null)
        {
            currState.ExecuteState(agent);
        }
    }

}

