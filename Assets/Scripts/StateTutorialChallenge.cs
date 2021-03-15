using UnityEngine;
using System.Collections;
using MadLevelManager;

public class StateTutorialChallenge : State {

    #region singleton
    private static readonly StateTutorialChallenge instance = new StateTutorialChallenge();
    public static StateTutorialChallenge Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    ChallengeAnimator m_challenge_animator;
    GameObject m_pinball_go;
    VideoManager videoManager;
    PinballMono pm;

    private void BackToWorldMenuSelect()
    {
        //Unsubscribing from the event
        m_challenge_animator.outroAnimationCompleted -= BackToWorldMenuSelect;
        MadLevel.LoadLevelByName("MainHUB");
    }

    public override void Init()
    {
        Debug.Log("Entering Fake Tutorial Challenge Transition");

        GameObject m_challenge_go = GameObject.FindGameObjectWithTag("Challenge");
        if (m_challenge_go == null)
        {
            Debug.LogError("Challenge not found");
        }

        m_challenge_animator = m_challenge_go.GetComponent<ChallengeAnimator>();
        m_challenge_animator.outroAnimationCompleted += BackToWorldMenuSelect;

        videoManager = m_challenge_go.GetComponent<VideoManager>();

        m_pinball_go = GameObject.FindGameObjectWithTag("PinballPrefab");
        pm = m_pinball_go.GetComponent<PinballMono>();

        if (m_challenge_go == null)
        {
            Debug.LogError("Challenge Not Found");
        }

        if (m_pinball_go == null)
        {
            Debug.LogError("Pinball Not Found");
        }

        if (pm == null)
        {
            Debug.LogError("Pinball mono not Found");
        }

        m_challenge_animator.ActivateOutroAnimation();
        pm.SetToAlphaFading(1.0f, false, false);
        videoManager.StartVideoProduction();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        
    }

}
