using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using MadLevelManager;

public class GameController : Singleton<GameController> {

    //Insert FSM in this

    private State[] m_States = new State[]{
        StateIdle.Instance,
        StateSplash.Instance,
        StateJigsawPuzzle.Instance,
        //AndreaLIRO_TB: should look if this is actually used
        StateChapterSelect.Instance,

        StateChallenge.Instance,
        StateTutorialChallenge.Instance,
        StateInitializePinball.Instance,
        StatePinball.Instance,
        StateReward.Instance
    };
    public enum States
    {
        Idle = 0,
        Splash,
        JigsawPuzzle,
        ChapterSelect,
        StateChallenge,
        StateTutorialChallenge,
		StateInitializePinball,
        StatePinball,
		StateReward
    };

    private State m_CurState;
    private bool m_Initializtion = false;

    public void ChangeState(States state)
    {
        try
        {
            // Debug.Log("Change State");
            if (m_CurState != null)
            {
                m_CurState.Exit();
            }
            m_CurState = m_States[(int)state];
            m_CurState.Init();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("ERROR while changing state from {0}: {1}", m_CurState.ToString(), ex.Message));
        }

    }

    public void Update()
    {
        try
        {
            if (m_CurState != null)
            {
                m_CurState.Update();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("ERROR while updating state {0}: {1}", m_CurState.ToString(), ex.Message));
        }

    }

    

}
