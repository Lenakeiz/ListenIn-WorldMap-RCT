using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenInStateMachine : TaskTrialAI
{
    public StateMachine<TaskTrialAI> GetFSM()
    {
        return m_fsm;
    }

    protected override void Awake()
    {
        base.Awake();
    }
    
}
