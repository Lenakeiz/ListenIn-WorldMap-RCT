using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class TaskTrialAI : MonoBehaviour
{
    protected StateMachine<TaskTrialAI> m_fsm;
    public string CurrentState { get; set; }

    #region ControllerMethods
    public virtual void SwitchState(TaskTrialState state)
    {
        if (m_fsm.GetState != null && state.Name == m_fsm.GetState.Name)
        {
            Debug.LogWarning("Switching to the same state: " + state.Name);
        }

  
        if (m_fsm.GetState != null)
        {
            var exitState = m_fsm.GetState;
            exitState.ExitState(this);
            //Destroy
            var exitType = exitState.GetType();
            Destroy(gameObject.GetComponent(exitType));
        }

        var currtype = state.GetType();
        var currState = gameObject.AddComponent(currtype) as TaskTrialState;
        m_fsm.GetState = currState;
        currState.EnterState(this);
            
    }
    #endregion

    #region Unity_Methods

    public TaskTrialAI()
    {
        m_fsm = new StateMachine<TaskTrialAI>(this);
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        m_fsm.AIUpdate();
        if (m_fsm.GetState != null)
            CurrentState = m_fsm.GetState.Name;
    }
    #endregion

    #region Events
    public event EventHandler flowerReachedEvent;
    public event EventHandler evaluationeTimeoutEvent;
        
    public virtual void FlowerReachedTrigger()
    {
        if (flowerReachedEvent != null)
            flowerReachedEvent.Invoke(this, null);
    }

    public virtual void EvaluationTimeoutTrigger()
    {
        if (evaluationeTimeoutEvent != null)
            evaluationeTimeoutEvent.Invoke(this, null);
    }
    #endregion 

}

