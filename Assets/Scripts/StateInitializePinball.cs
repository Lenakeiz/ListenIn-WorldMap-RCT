using UnityEngine;
using System.Collections;

public class StateInitializePinball : State
{
	#region singleton
	private static readonly StateInitializePinball instance = new StateInitializePinball();
	public static StateInitializePinball Instance
	{
		get
		{
			return instance;
		}
	}
	#endregion

	GameObject m_challenge_go;
	GameObject m_pinball_go;
	PinballMono pm;

	// Use this for initialization
	public override void Init()
	{
        UploadManager.Instance.ResetTimer(TimerType.Pinball);
        UploadManager.Instance.SetTimerState(TimerType.Pinball, true);

        Debug.Log("StateInitializePinball: Init() Starting pinball transition");
        m_challenge_go = GameObject.FindGameObjectWithTag("Challenge");
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

        pm.SetSpwanerTriggerState(false);
        m_challenge_go.GetComponent<ChallengeAnimator>().ActivateOutroAnimation();
        pm.SetToAlphaFading(1.0f, false, false);

	}

    public void StartCannonEnteringAnimation()
    {
        PinballMono pinballmonocomponent = m_pinball_go.GetComponent<PinballMono>();
        if (pinballmonocomponent != null){
            pinballmonocomponent.EnterCannon();
        }
        else {
            Debug.LogError("Coud not find Pinball Mono");
        }
    }


    public void StartPinball()
	{
		if(m_challenge_go != null)
		{
			UnityEngine.GameObject.Destroy(m_challenge_go);
			StatePinball.Instance.Init();
            pm.SetToAlphaFading(0.0f, false, true);
		}
	}
	
	// Update is called once per frame
	public override void Update()
	{

	}
	
	public override void Exit()
	{
        Debug.Log("StateInitializePinball: Exit() Finishing transiton to pinball");
        StatePinball.Instance.InitLevelPinball(false);
	}
}
