using UnityEngine;
using System;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 

	public GameObject challengePanel;
    public GameObject UploadingMessageUI;
	private bool locked = false;

	public bool inChallengeMenu = false;

	//Call this function to activate and display the Options panel during the main menu
	public void ShowOptionsPanel()
	{
		menuPanel.GetComponent<CanvasGroup>().interactable = false;
		optionsPanel.SetActive(true);
		gameObject.GetComponentInChildren<CheatLoader>().ShowCurrentCoins();
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		menuPanel.GetComponent<CanvasGroup>().interactable = true;
		optionsTint.SetActive(true);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		pausePanel.SetActive (false);
		//optionsTint.SetActive(false);

	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePanels()
	{
		menuPanel.SetActive(false);
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);		
	}

	public void ShowDebugMenu()
	{
		gameObject.GetComponent<CanvasGroup>().alpha = 1;
		menuPanel.GetComponent<CanvasGroup>().alpha = 1;
		challengePanel.SetActive(false);
		menuPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	public void ShowInitialMenu(bool inChallengeState)
	{
        Time.timeScale = 0.0f;
        DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
        DatabaseXML.Instance.SetIsMenu = true;
		gameObject.GetComponent<CanvasGroup>().alpha = 1;
		challengePanel.GetComponent<CanvasGroup>().alpha = 1;
		challengePanel.SetActive(true);
		optionsTint.SetActive(true);

		gameObject.GetComponent<CheatLoader>().ResetCounter();

		inChallengeMenu = inChallengeState;
		
	}

	public void HideInitialMenu()
	{
        DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
        DatabaseXML.Instance.SetIsMenu = false;
        Time.timeScale = 1.0f;
		challengePanel.SetActive(true);
		menuPanel.SetActive(false);
	}

    public void ShowUploadUI()
    {
        //DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
        DatabaseXML.Instance.SetIsMenu = false;
        Time.timeScale = 1.0f;
        challengePanel.SetActive(false);
        menuPanel.SetActive(false);
        optionsTint.SetActive(false);
        UploadingMessageUI.SetActive(true);
    }


    
	IEnumerator BackToChallenge(float waitTime)
	{
        //Making this time scale independent
        while (waitTime > 0.0f)
        {
            waitTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        try
        {
            Time.timeScale = 1.0f;
            DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
            DatabaseXML.Instance.SetIsMenu = false;
            GameObject challenge = GameObject.Find("Challenge(Clone)");

            if (challenge != null)
            {
                challenge.GetComponentInChildren<GameControlScript>().SetEnable(true);
                ReplaySound replayButton = challenge.GetComponentInChildren<ReplaySound>();

                if (replayButton != null)
                {
                    replayButton.GetComponent<CircleCollider2D>().enabled = true;
                }
            }
            else
            {
                Debug.LogWarning("Challenge not found when going back to game");
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(String.Format("ShowPanels: {0}", ex.Message));
        }
        finally
        {
            challengePanel.SetActive(false);
            menuPanel.SetActive(false);
            optionsTint.SetActive(false);
            locked = false;
        }


	}

	public void BackToChallengeGame(float waitTime)
	{
		if(!locked)
		{
			locked = true;
			StartCoroutine(BackToChallenge(waitTime));
		}

	}

    IEnumerator BackToChallengeAndCheat(float waitTime)
    {
                //Making this time scale independent
        while (waitTime > 0.0f)
        {
            waitTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        //Adjusting Timers
        try
        {
            Time.timeScale = 1.0f;
            DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
            DatabaseXML.Instance.SetIsMenu = false;
        }
        catch (Exception ex)
        {
            Debug.Log(String.Format("ShowPanels: {0}", ex.Message));
        }
        finally
        {
            challengePanel.SetActive(false);
            menuPanel.SetActive(false);
            optionsTint.SetActive(false);
            locked = false;

            StatePinball.Instance.m_PinballMono.UnlockAndFinishPinballGame(false);

        }

    }

    public void BackToChallengeGameAndCheat(float waitTime)
    {
        if (!locked)
        {
            locked = true;
            StartCoroutine(BackToChallengeAndCheat(waitTime));
        }
    }

    public void OnDestroy()
    {
        //Close everything
        try
        {
            Time.timeScale = 1.0f;
            DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
            DatabaseXML.Instance.SetIsMenu = false;
            challengePanel.SetActive(false);
            menuPanel.SetActive(false);
            optionsTint.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.Log(String.Format("Show panels: {0}", ex.Message));
        }

    }

}
