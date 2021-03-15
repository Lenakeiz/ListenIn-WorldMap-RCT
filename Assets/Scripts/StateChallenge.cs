using UnityEngine;
using System;
using System.Collections;

public class StateChallenge : State
{
    #region singleton
    private static readonly StateChallenge instance = new StateChallenge();
    public static StateChallenge Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    GameObject m_GO;
	public ChallengeAnimator challengeanim;
	public bool cheatActivated = false;

    //private TimeSpan therapyTime = new TimeSpan();
    private double totalTherapyTime = 0;
    private double todayTherapyTime = 0;

    // Use this for initialization
    public override void Init()
    {
		GameObject go;
		if(StatePinball.Instance.ID == 0)
		{
			go = GameObject.Instantiate(Resources.Load("Prefabs/States/ChallengeTutorial")) as GameObject;
			Debug.Log("Starting Tutorial");
		}
		else
		{
			go = GameObject.Instantiate(Resources.Load("Prefabs/States/Challenge")) as GameObject;
		}
        
		m_GO = go;
		challengeanim = go.GetComponent<ChallengeAnimator>();
        Debug.Log("StateChallenge: Init therapy");

		challengeanim.currMargin = 0;
		challengeanim.margins[challengeanim.currMargin].SetActive(true);

        coins = 0;
        questions = 0;
        correctAnswers = 0;

        DatabaseXML.Instance.SetTimerState(DatabaseXML.TimerType.Therapy, true);
    }

    // Update is called once per frame
    public override void Update()
    {
        //Debug.Log("Coins : [" + coins + "];");
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddCoin(10);
			cheatActivated = true;
            StatePinball.Instance.initialize = false;
            GameController.Instance.ChangeState(GameController.States.StatePinball);
            StatePinball.Instance.InitLevelPinball();
        }
#endif
    }

    public override void Exit()
    {
        Debug.Log("StateChallenge: exiting challenge");
		if(cheatActivated)
		{
			UnityEngine.GameObject.Destroy(m_GO);
		}

        StatePinball.Instance.initialize = false;
		cheatActivated = false;

        DatabaseXML.Instance.SetTimerState(DatabaseXML.TimerType.Therapy, false);
    }

    private int coins ;
    private int questions ;
    private int correctAnswers ;

    public void AddCoin(int amount)
    {
        //Debug.Log("Add Coin : [" + amount + "|" + coins + "];");
        coins += amount;
    }

    public void ResetCoins()
    {
        //Debug.Log("ResetCoins : [" + coins + "];");
        coins = 0;
    }

    public int GetCoinsEarned()
    {
        //Debug.Log("GetCoinsEarned : [" + coins + "];");
        return coins;
    }

    public void SetQuestionAmount(int amount)
    {
        questions = amount;
    }

    public int GetQuestionAmount()
    {
        return questions;
    }
    public void ResetQuestions()
    {
        questions = 0;
    }

    public void CorrectAnswer()
    {
        correctAnswers++;
    }

    public int GetCorrectAnswers()
    {
        return correctAnswers;
    }
    public void ResetCorrectAnswers()
    {
        correctAnswers = 0;
    }

	public void UnlockChallenged()
	{
		m_GO.GetComponentInChildren<GameControlScript>().PlayAudio();
	}

    
    public void SetTotalTherapyTime(double time)
    {
        Debug.Log("StateChallenge: SetTherapyTimeMin = " + time);
        //therapyTime = TimeSpan.FromMinutes(time);
        totalTherapyTime = time;
    }
    
    public string GetTotalTherapyTime()
    {
        int intHour = (int) (totalTherapyTime / 60);
        int intMin = (int)(totalTherapyTime % 60);
        string time = string.Format("{0:D2}:{1:D2}", intHour, intMin);
        return time;

        /*int minutes = therapyTime.Minutes;
        if (therapyTime.Seconds >= 30)
        {
            minutes += 1;
        }
        //string time = string.Format("{0:D2}:{1:D2}", therapyTime.Hours, minutes);
        string time = therapyTime.Hours.ToString() + ":" + therapyTime.Minutes.ToString();
        return time;*/
    }

    public void SetTodayTherapyTime(double time)
    {        
        todayTherapyTime = time;
    }

    public string GetTodayTherapyTime()
    {
        int intHour = (int)(todayTherapyTime / 60);
        int intMin = (int)(todayTherapyTime % 60);
        string time = string.Format("{0:D2}:{1:D2}", intHour, intMin);
        return time;
    }

}
