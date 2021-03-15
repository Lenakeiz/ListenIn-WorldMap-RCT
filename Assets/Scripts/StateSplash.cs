using UnityEngine;
using System.Collections;

using MadLevelManager;

public class StateSplash : State
{
    #region singleton
    private static readonly StateSplash instance = new StateSplash();
    public static StateSplash Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private SplashMono m_Mono;
	private GameObject m_intro_go;
    private float m_Timer;
    private bool m_Intro;
	private bool m_gameIntro;
	private bool m_mainmenu;

    // Use this for initialization
    public override void Init()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/States/SplashScreen")) as GameObject;
        m_Mono = go.GetComponent<SplashMono>();
        m_Intro = true;
		m_gameIntro = false;
		m_mainmenu = false;
        m_Timer = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (m_Intro)
        {
            m_Timer += Time.deltaTime;

            if (0 <= m_Timer && m_Timer < 3f)
            {
                //PlayIntroJingle();
				GoToSplashSreen();
            }
            else if (3f <= m_Timer)
            {
				//GoToIntro();
				m_Mono.SplashScreen.GetComponent<Animator>().SetTrigger("StartFading");
				m_Intro = false;
            }
        }

		if(m_gameIntro)
		{
			m_gameIntro = false;
			GoToGameIntro();
			m_intro_go = GameObject.Instantiate(Resources.Load("Prefabs/IntroNew")) as GameObject;
		}
		else if(m_mainmenu)
		{
			m_mainmenu = false;
			StartGame();
		}
    }

    private void PlayIntroJingle()
    {
        //AudioClip introJingle = Resources.Load("Sounds/Ding") as AudioClip; //TODO: insert name of asset when it's provided

        //m_Mono.audio.clip = introJingle;
        //m_Mono.audio.Play ();
        //m_Mono.audio.loop = false;
    }

    public override void Exit()
    {
        //TODO with the new logic there is no necessity anymore for destroying since we are loading different scenes
        //UnityEngine.Object.Destroy(m_Mono.gameObject);
		//UnityEngine.Object.Destroy(m_intro_go);
    }
    public void StartGame()
    {
        MadLevel.LoadLevelByName("SetupScreen");
        //GameController.Instance.ChangeState(GameController.States.ChapterSelect);
        //GameController.Instance.ChangeState(GameController.States.ChapterSelect);
    }
    public void GoToMainMenu()
	{
		//m_Mono.Intro.SetActive(false);
        m_Mono.DoctorsScreen.SetActive(false);
        m_Mono.OptionsScreen.SetActive(false);
        m_Mono.MainMenu.SetActive(false);
    }

	public void GoToGameIntro(){
		//m_Mono.Intro.SetActive(true);
		m_Mono.SplashScreen.SetActive(false);
		m_Mono.DoctorsScreen.SetActive(false);
		m_Mono.OptionsScreen.SetActive(false);
		m_Mono.MainMenu.SetActive(false);

	}

	public void GoToSplashSreen()
	{
		//m_Mono.Intro.SetActive(false);
		m_Mono.SplashScreen.SetActive(true);
		m_Mono.DoctorsScreen.SetActive(false);
		m_Mono.OptionsScreen.SetActive(false);
		m_Mono.MainMenu.SetActive(false);
	}

	public void GoToOptionMenu()
    {
        m_Mono.DoctorsScreen.SetActive(false);
        m_Mono.OptionsScreen.SetActive(true);
        m_Mono.MainMenu.SetActive(false);
    }

    public void GoToDoctor()
    {
        m_Mono.DoctorsScreen.SetActive(true);
        m_Mono.OptionsScreen.SetActive(false);
        m_Mono.MainMenu.SetActive(false);
    }

	public void ActivateGameIntro()
	{
		m_gameIntro = true;
	}

	public void ActivateStartGame()
	{
		m_mainmenu = true;
	}
}
