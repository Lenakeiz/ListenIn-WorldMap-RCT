using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using MadLevelManager;
using System.IO;

public class UploadManager : Singleton<UploadManager> {


    private string PatientId = "1";

    public float currentBatteryLevel = 0.0f;
    private bool backToLevelSelection = false;
    private float _currDeltaTime = 0;
    private float startUploadTime = 0;
    private float timeoutTimer = 0.0f;
    private float waitTime = 3.0f;
    private bool startTimer = false;

    private static int minutesToPause = 1;
    private static int minutesToQuit = 1;

    #region Timers
    //idle time
    float idle_time = 0;
    float therapy_time = 0;
    float therapy_pinball_time = 0;
    float therapy_worldmap_time = 0;
    //helpers for counting
    bool count_idle_time = false;
    bool count_therapy_time = false;
    bool count_pinball_time = false;
    bool count_worldmap_time = false;

    bool isMenuPaused = false;
    bool m_stop_forcetimer_routine = false;
    public bool SetIsMenu { get { return isMenuPaused; } set { isMenuPaused = value; } }

    #endregion Timers

    public void Initialize()
    {
        //check if directory doesn't exit -- FIRST INITIALIZATION
        if (!Directory.Exists(Application.persistentDataPath + @"/ListenIn/"))
        {
            //if it doesn't, create it
            Debug.Log("UploadManager: first initialization - creating directories"); // 2016-12-06
            Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/LIRO");
        }

        OnPatientChange();

    }

    /// <summary>
    /// Every time this function is called it checks if it has the folder structure for the current patient ID, if not it will create directories.
    /// </summary>
    public void OnPatientChange()
    {
        PatientId = GetPatient();
        string patientPath = Application.persistentDataPath + @"/ListenIn/LIRO";
        patientPath = Path.Combine(patientPath, PatientId);
        if (!Directory.Exists(patientPath))
        {
            Directory.CreateDirectory(patientPath + @"/Output");
            Directory.CreateDirectory(patientPath + @"/Section");
            Directory.CreateDirectory(patientPath + @"/ACT");
            //Resetting JigsawState
            //Used to communicate inside the initialization of the setup controller
            GlobalVars.isProfileNewOrChanged = true;
        }
    }

    //return lenght
    public string GetPatient()
    {
        //AndreaLIRO: need to get this fro the network manager
        return NetworkManager.IdUser;
        return "1";
    }

    #region GAME
    private void CheckAppTimeoutTimers()
    {
        //get fingers on screen android only 
        int fingerCount = 0;

#if UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        fingerCount = Input.GetMouseButtonDown(0) ? 1 : 0;
#endif

        if (fingerCount > 0)
        {
            //if finger, reset the timer
            count_idle_time = false;
            idle_time = 0;
        }
        else
        {
            //if no finger, then run the counter
            count_idle_time = true;
        }

        //block timer
        if (count_therapy_time)
        {
            therapy_time += Time.deltaTime;
        }

        if (count_pinball_time)
        {
            therapy_pinball_time += Time.deltaTime;
        }

        if (count_worldmap_time)
        {
            therapy_worldmap_time += Time.deltaTime;
        }

        //iddle timer
        if (count_idle_time)
        {
            idle_time += Time.unscaledDeltaTime;
        }

        //TODO: unify the menu system
        #region TimeoutGame
        if (isMenuPaused && idle_time > 60 * minutesToQuit)
        {
            Debug.Log("Forcing ListenIn to quit due to timeout (99)");
            //reasonToExit = 99;
            Application.Quit();

            //If we are running in the editor
#if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (!isMenuPaused && idle_time > 60 * minutesToPause && !m_stop_forcetimer_routine)
        {
            if (OpenPauseMenu())
            {
                ResetTimer(TimerType.Idle);
            }
            else
            {
                Debug.LogWarning("Preventing force idle menu. Could be setup screen, uploading screen or a transition");
                ResetTimer(TimerType.Idle);
            }
        }

        #endregion
    }

    private bool OpenPauseMenu()
    {
        Scene currScene = SceneManager.GetActiveScene();
        if (currScene.name == "SetupScreen")
        {
            //Debug.Log("DatabaseXML: OpenPauseMenu() preventing screen to pause in setupscreen");
            return false;
        }

        //Works only on the WorldMapScene
        if (currScene.name == "LevelSelectScene")
        {
            GameObject menuUI = GameObject.FindGameObjectWithTag("MenuUI");
            if (menuUI != null)
            {
                LevelSelectManager lsm = menuUI.GetComponent<LevelSelectManager>();
                if (lsm != null)
                {
                    lsm.OpenPauseMenu();

                    Debug.Log("DatabaseXML: OpenPauseMenu() Forcing menu after idle timeout - case WorldMap");
                    return true;
                }
            }
        }

        //Andrea
        if (currScene.name == "GameLoop")
        {
            GameObject challengeTherapy = GameObject.FindGameObjectWithTag("Challenge");
            if (challengeTherapy != null)
            {
                MenuManager mm = challengeTherapy.GetComponentInChildren<MenuManager>();
                if (mm != null)
                {
                    mm.OpenMenu();
                    Debug.Log("DatabaseXML: OpenPauseMenu() Forcing menu after idle timeout - case Therapy Challenge");
                    return true;
                }
            }

            GameObject jigsawPuzzle = GameObject.FindGameObjectWithTag("PinballPrefab");
            if (jigsawPuzzle != null)
            {
                MenuManager csm = jigsawPuzzle.GetComponentInChildren<MenuManager>();
                if (csm != null)
                {
                    csm.OpenMenu();
                    Debug.Log("Forcing menu after idle timeout - case Pinball");
                    return true;
                }
            }
        }

        return false;

    }

    public void SetTimerState(TimerType tymerType, bool state)
    {
        switch (tymerType)
        {
            case TimerType.Idle:
                count_idle_time = state;
                break;
            case TimerType.WorldMap:
                count_worldmap_time = state;
                break;
            case TimerType.Therapy:
                count_therapy_time = state;
                break;
            case TimerType.Pinball:
                count_pinball_time = state;
                break;
            default:
                break;
        }
    }

    public float GetTimerState(TimerType tymerType)
    {
        float currTime = 0;
        switch (tymerType)
        {
            case TimerType.Idle:
                currTime = idle_time;
                break;
            case TimerType.WorldMap:
                currTime = therapy_worldmap_time;
                break;
            case TimerType.Therapy:
                currTime = therapy_time;
                break;
            case TimerType.Pinball:
                currTime = therapy_pinball_time;
                break;
            default:
                break;
        }
        if (currTime == 0)
        {
            Debug.LogError("UploadManager: asked for a timer, but currently is zero");
        }
        return currTime;
    }

    public void ResetTimer(TimerType tymerType)
    {
        switch (tymerType)
        {
            case TimerType.Idle:
                ForcedTimerState = false;
                idle_time = 0.0f;
                break;
            case TimerType.WorldMap:
                therapy_worldmap_time = 0.0f;
                break;
            case TimerType.Therapy:
                therapy_time = 0.0f;
                break;
            case TimerType.Pinball:
                therapy_pinball_time = 0.0f;
                break;
            default:
                break;
        }
    }

    //This bool is set to prevent that in certain part of the game the timer could cause the game to quit abruptly (i.e when finishing the pinball wait until the end of the uploading screen)
    public bool ForcedTimerState { set { m_stop_forcetimer_routine = value; } }

    #endregion GAME

    #region THERAPY
    public IEnumerator EndOfTherapyClean(int correctAnswer = 0, string fileToDelete = "")
    {
        backToLevelSelection = false;
        yield return StartCoroutine(CleanUp(correctAnswer, fileToDelete));
    }
    private IEnumerator CleanUp(int correctAnswer, string fileToDelete = "")
    {
        startUploadTime = Time.time;
        //AndreaLIRO: adding the LIRO therapy update
        yield return StartCoroutine(TherapyLIROManager.Instance.AdvanceCurrentBlockInSection(fileToDelete));
        Debug.Log("UploadManager: " + startUploadTime + " saving the LIRO therapy");
    }
    #endregion THERAPY

    #region ACT
    public IEnumerator EndOfACTClean(int correctAnswer = 0)
    {
        backToLevelSelection = false;
        yield return StartCoroutine(CleanUpACT(correctAnswer));
    }
    private IEnumerator CleanUpACT(int correctAnswer)
    {
        startUploadTime = Time.time;
        Debug.Log("UploadManager: " + startUploadTime + " saving ACT");
        //AndreaLIRO: adding the LIRO therapy update
        yield return StartCoroutine(TherapyLIROManager.Instance.AdvanceCurrentBlockInSection());
        yield return StartCoroutine(TherapyLIROManager.Instance.UpdateACTScore(correctAnswer));

        _currDeltaTime = Time.time - startUploadTime;
        Debug.Log("UploadManager: " + _currDeltaTime + " collecting memory");

        CollectMemory();
        backToLevelSelection = true;

    }
    #endregion ACT

    #region Battery Level
    public float GetBatteryLevel()
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                if (unityPlayer != null)
                {
                    using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        if (currActivity != null)
                        {
                            using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }))
                            {
                                using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter }))
                                {
                                    int level = batteryIntent.Call<int>("getIntExtra", new object[] { "level", -1 });
                                    int scale = batteryIntent.Call<int>("getIntExtra", new object[] { "scale", -1 });

                                    // Error checking.
                                    if (level == -1 || scale == -1)
                                    {
                                        currentBatteryLevel = 0.0f;
                                        return currentBatteryLevel;
                                    }
                                    currentBatteryLevel = ((float)level / (float)scale) * 100.0f;
                                    return currentBatteryLevel;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception ex) { }
        currentBatteryLevel = 101.0f;
        return currentBatteryLevel;
    }
    #endregion

    protected void Update()
    {
        if (backToLevelSelection)
        {
            //Waiting some extra time
            timeoutTimer += Time.deltaTime;
            if (timeoutTimer > waitTime)
            {
                _currDeltaTime = Time.time - startUploadTime;
                Debug.Log("UploadManager: " + _currDeltaTime + " returning to main screen.");
                backToLevelSelection = false;
                timeoutTimer = 0.0f;
                GameStateSaver.Instance.SaveGame();
                MadLevel.LoadLevelByName("MainHUB");
            }
        }
        else
        {
            CheckAppTimeoutTimers();
        }
    }

    public void CollectAndBackToMainHub()
    {
        CollectMemory();
        StartCoroutine(UpdateUserProfile());        
    }

    public void SaveGame()
    {
        GameStateSaver.Instance.SaveGame();
    }

    private void CollectMemory()
    {
        //Collecting memory
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    private IEnumerator UpdateUserProfile()
    {
        UploadManager.Instance.SetTimerState(TimerType.Pinball, false);
        yield return StartCoroutine(TherapyLIROManager.Instance.AddGameMinutes(Mathf.CeilToInt(therapy_pinball_time/60.0f)));
        backToLevelSelection = true;
    }
}
