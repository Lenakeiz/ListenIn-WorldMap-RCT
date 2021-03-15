using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskTrialState : MonoBehaviour, AIState<TaskTrialAI>
{

    public string Name { get; set; }

    public virtual void EnterState(TaskTrialAI agent)
    {            
        if (Application.isEditor)
        {
            Debug.Log("Entering state: " + Name);
        }

        agent.flowerReachedEvent += FlowerReachedEventHandler;
        agent.evaluationeTimeoutEvent += EvaluationTimeoutEventHandler;
        
    }

    public virtual void ExecuteState(TaskTrialAI agent)
    {

    }

    public virtual void ExitState(TaskTrialAI agent)
    {
        agent.flowerReachedEvent -= FlowerReachedEventHandler;
        agent.evaluationeTimeoutEvent -= EvaluationTimeoutEventHandler;
    }

    #region Unity_Methods
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void Awake()
    {

    }
    #endregion Unity_Methods

    #region Events
    public virtual void FlowerReachedEventHandler(System.Object sender, EventArgs e)
    {
        Debug.Log("Flower reached event received in father");
    }

    public virtual void EvaluationTimeoutEventHandler(System.Object sender, EventArgs e)
    {
        Debug.Log("Flower reached event received in father");
    }
    #endregion

    public virtual string ToString()
    {
        return "State: " + Name;
    }
}

