using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using MadLevelManager;

public class GameController {

    #region singleton
    private static readonly GameController instance = new GameController();
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion


    private State[] m_States = new State[]{
        StateIdle.Instance,
        StateSplash.Instance,
        StateJigsawPuzzle.Instance,
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
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }

    }

    //DEPRECATED
    public void Init()
    {
		if (!m_Initializtion)
        {
            m_Initializtion = true;
            Application.targetFrameRate = 60;

            //Setting the logger
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI_Canvas_Debug"));
            Text debug_text = go.GetComponentInChildren<Text>();
            ListenIn.Logger.Instance.SetLoggerUIFrame(debug_text);
            ListenIn.Logger.Instance.SetLoggerLogToExternal(true);
            ListenIn.Logger.Instance.Log("Log started", ListenIn.LoggerMessageType.Info);

            try
            {
                Debug.LogError("GameController: initializing database (is this a double initialization?)");
                DatabaseXML.Instance.InitializeDatabase();
                UploadManager.Instance.Initialize();
                CUserTherapy.Instance.LoadDataset_UserProfile();
                StateJigsawPuzzle.Instance.OnGameLoadedInitialization();                               

                IMadLevelProfileBackend backend = MadLevelProfile.backend;
                string profile = backend.LoadProfile(MadLevelProfile.DefaultProfile);
                ListenIn.Logger.Instance.Log(string.Format("GameController: Loaded profile: {0}", profile), ListenIn.LoggerMessageType.Info);
                //Debug.Log(profile);
                
                GameStateSaver.Instance.Load();
                
                ChangeState(States.Idle);
                //ChangeState(States.Splash);
            }
            catch (System.Exception ex)
            {
                ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
            }
            
        }
        else
        {
            ChangeState(States.Idle);
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
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }

    }

    

}
