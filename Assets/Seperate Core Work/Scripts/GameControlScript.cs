//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Xml.Linq;
//using System.Linq;
//using System;

///// <summary>
///// THIS IS THE VERY ORIGINAL ONE FROM THE CLINICAL TRIAL, NOW IT HAS BEEN CHANGED TO GAMECONTROLSCRIPTSTANDARD AND GAMECONTROLSCRIPTACT
///// </summary>
//public class GameControlScript : MonoBehaviour 
//{
//    // list of trials / challenges
//    //List<CTrial> m_lsTrial = new List

//    public int numberOfTrials;
//    private int trialsCounter;
//    private bool enable_input = true;
//    AnimationInterface ai;

//    // index of the current displayed trial/challenge
//    int m_intCurIdx = -1;

//    // current trial
//    CTrial m_curTrial = new CTrial();

//    // response to current trial
//    CResponse m_curResponse = new CResponse();

//    // structure represent stimulus game object 
//    public struct stStimulusGO
//    {
//        public GameObject stimulusGO;  // stimulus gameobject
//        public StimulusScript stimulusScript;   // reference to stimulus's script
//    }
//    // array of 6 stimulus game object 
//    stStimulusGO[] m_arrStimulusGO = new stStimulusGO[6];

//    // feedback (ticks) to show correct answer
//    ParticleSystem m_particleSyst;

//    // feedback (cross) to show incorrect answer
//    ParticleSystem m_particleSystIncorrect;

//    // flags to control the visibility of btnRestart
//    bool m_bShowBtnRestart = false;

//    // flags to control the visibility of btnRepeat
//    bool m_bShowBtnRepeat = false;

//    struct StimulusPosScale
//    {
//        Vector3 pos;
//        Vector3 localScale;
//    }

//    // vector positions for 3/4/5/6 stimuli
//    Vector3[] m_arr3StimuliPos = new Vector3[3];
//    Vector3[] m_arr4StimuliPos = new Vector3[4];
//    Vector3[] m_arr5StimuliPos = new Vector3[5];
//    Vector3[] m_arr6StimuliPos = new Vector3[6];

//    // array of audio sources
//    public AudioSource[] m_arrAudioSource;

//    // audio source for target
//    public AudioSource m_audioTarget;

//    // audio source for background noise
//    public AudioSource m_audioBackgroundNoise;

//    // audio source for feedback
//    public AudioSource m_audio_feedback;

//    //Transforms to set the stimuli
//    public Transform m_root_3;
//    public Transform m_root_4;
//    public Transform m_root_5;
//    public Transform m_root_6;

//    // flags to control the visibility of main menu
//    bool m_bShowMainMenu = false;

//    // index of the selected stimulus
//    int m_intSelectedStimulusIdx = -1;

//    // flags to check if the coroutine "WaitIncorrect" is running
//    bool m_bIsCoroutineIncorrectRunning = false;

//    GameObject repeatButton;
//    SoundManager m_sound_manager;

//    // structure represent menu levels
//    struct stMenuLevel
//    {
//        public GameObject goMenuLevels;  // menu levels gameobject
//        public MenuLevelsScript scriptMenuLevels;   // reference to stimulus's script
//    }
//    stMenuLevel m_menuLevels = new stMenuLevel();

//    // is currently in admin mode?
//    bool m_bIsAdminMode = false;

//    // tap the admin btn three times and the it will turn into admin mode
//    int m_intClickButtonAdminCtr = 0;

//    // cheat code - tap the side panel 5 times and the therapy session will be terminated and move to pinball session
//    int m_intCheatCtr = 0;

//    // show phone mode icon on the top right corner during levels with phone voice
//    GameObject goPhoneModeIcon;

//    // trial start time, this is to keep track how long does the patient take to get a correct response 
//    DateTime m_dtCurTrialStartTime;


//    //----------------------------------------------------------------------------------------------------
//    // Awake
//    //----------------------------------------------------------------------------------------------------
//    void Awake()
//    {

//    }

//    //----------------------------------------------------------------------------------------------------
//    // Use this for initialization
//    //----------------------------------------------------------------------------------------------------
//    void Start()
//    {
//        repeatButton = GameObject.FindGameObjectWithTag("RepeatButton") as GameObject;

//        m_sound_manager = GameObject.Find("Main Camera").GetComponent<SoundManager>() as SoundManager;
//        goPhoneModeIcon = GameObject.FindGameObjectWithTag("PhoneModeIcon") as GameObject;

//        // retrieve audio sources
//        m_arrAudioSource = GetComponents<AudioSource>();
//        m_audioTarget = m_arrAudioSource[0];
//        m_audioBackgroundNoise = m_arrAudioSource[1];
//        m_audio_feedback = m_arrAudioSource[2];

//        // retrieve stimuli 1 - 6's gameobject and scripts
//        m_arrStimulusGO[0].stimulusGO = GameObject.Find("Stimulus1");
//        m_arrStimulusGO[0].stimulusScript = GameObject.Find("Stimulus1").GetComponent<StimulusScript>();
//        m_arrStimulusGO[1].stimulusGO = GameObject.Find("Stimulus2");
//        m_arrStimulusGO[1].stimulusScript = GameObject.Find("Stimulus2").GetComponent<StimulusScript>();
//        m_arrStimulusGO[2].stimulusGO = GameObject.Find("Stimulus3");
//        m_arrStimulusGO[2].stimulusScript = GameObject.Find("Stimulus3").GetComponent<StimulusScript>();
//        m_arrStimulusGO[3].stimulusGO = GameObject.Find("Stimulus4");
//        m_arrStimulusGO[3].stimulusScript = GameObject.Find("Stimulus4").GetComponent<StimulusScript>();
//        m_arrStimulusGO[4].stimulusGO = GameObject.Find("Stimulus5");
//        m_arrStimulusGO[4].stimulusScript = GameObject.Find("Stimulus5").GetComponent<StimulusScript>();
//        m_arrStimulusGO[5].stimulusGO = GameObject.Find("Stimulus6");
//        m_arrStimulusGO[5].stimulusScript = GameObject.Find("Stimulus6").GetComponent<StimulusScript>();

//        // each trial will have 3 / 4 / 5 / 6 stimuli
//        //SetupStimuliPosMap ();
//        SetupStimuliPosMapTransforms();

//        Vector3 scale = new Vector3(2, 2, 2);
//        GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").transform.localScale = scale;
//        GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.localScale = scale;
//        GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.localScale = scale;
//        GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.localScale = scale;
//        GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.localScale = scale;
//        GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").transform.localScale = scale;

//        // set stimulus's layer orders - for images overlapping
//        GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 4;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
//        GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 3;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 6;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
//        GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 5;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 8;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
//        GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 7;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 10;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
//        GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 9;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 12;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
//        GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 11;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;


//        // set particlesystem layer's order
//        m_particleSyst = GameObject.Find("ParticleFeedback").GetComponent<ParticleSystem>();
//        m_particleSyst.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
//        m_particleSyst.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        m_particleSystIncorrect = GameObject.Find("ParticleFeedbackIncorrect").GetComponent<ParticleSystem>();
//        m_particleSystIncorrect.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
//        m_particleSystIncorrect.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

//        // load from xml file all stimuli's images, audio and target index for all trials/challenges 
//        //LoadTrials ();

//        //GETTING THE Animator script
//        GameObject lepr = GameObject.FindGameObjectWithTag("Leprechaun");
//        if (lepr != null)
//        {
//            ai = lepr.GetComponent<AnimationInterface>();
//            Vector3 throwPosition = lepr.transform.FindChild("ThrowPos").position;
//            SetStimuliThrowPos(throwPosition);
//        }

//        // restart game
//        RestartGame();

//        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<CircleCollider2D>().enabled = true;
//        Camera.main.GetComponent<SoundManager>().SetChannelLevel(ChannelType.LevelEffects, 5.0f);

//        Dictionary<string, string> block_start = new Dictionary<string, string>();

//        int patient = UploadManager.Instance.PatientId;
//        DateTime now = System.DateTime.Now;

//        block_start.Add("patient", patient.ToString());
//        block_start.Add("date", now.ToString("yyyy-MM-dd HH:mm:ss"));

//        //AndreaLIRO: removing writing to database xml
//        //DatabaseXML.Instance.WriteDatabaseXML(block_start, DatabaseXML.Instance.therapy_block_insert);
//    }

//    void SetStimuliThrowPos(Vector3 pos)
//    {
//        for (int i = 0; i < m_arrStimulusGO.Length; i++)
//        {
//            m_arrStimulusGO[i].stimulusScript.SetThrowPosition(pos);
//        }
//    }

//    void ResetStimulThrowPos()
//    {
//        for (int i = 0; i < m_arrStimulusGO.Length; i++)
//        {
//            m_arrStimulusGO[i].stimulusScript.ResetPosition();
//        }
//    }

//    //----------------------------------------------------------------------------------------------------
//    // SetupStimuliPosMapTransforms: set up positions using transform set in the scene
//    //----------------------------------------------------------------------------------------------------
//    void SetupStimuliPosMapTransforms()
//    {
//        if (m_root_3 != null)
//        {
//            for (int i = 0; i < m_root_3.childCount; i++)
//            {
//                m_arr3StimuliPos[i] = m_root_3.GetChild(i).position;
//            }
//        }
//        if (m_root_4 != null)
//        {
//            for (int i = 0; i < m_root_4.childCount; i++)
//            {
//                m_arr4StimuliPos[i] = m_root_4.GetChild(i).position;
//            }
//        }
//        if (m_root_5 != null)
//        {
//            for (int i = 0; i < m_root_5.childCount; i++)
//            {
//                m_arr5StimuliPos[i] = m_root_5.GetChild(i).position;
//            }
//        }
//        if (m_root_6 != null)
//        {
//            for (int i = 0; i < m_root_6.childCount; i++)
//            {
//                m_arr6StimuliPos[i] = m_root_6.GetChild(i).position;
//            }
//        }
//    }

//    //----------------------------------------------------------------------------------------------------
//    // RestartGame: restart game
//    //----------------------------------------------------------------------------------------------------
//    void RestartGame()
//    {

//        CleanPreviousTrial();

//        // load patient profile
//        //CTrialList.Instance.LoadUserProfile();

//        // avoid long time holding, call this function in GameController-Init() instead
//        //CUserTherapy.Instance.LoadUserProfile();
        
//        trialsCounter = numberOfTrials;

//        //m_bShowMainMenu = true;

//        m_bIsAdminMode = false;
//        m_intClickButtonAdminCtr = 0;
//        m_intCheatCtr = 0;

//        //OnClickButtonLevel(CTrialList.Instance.getCurLevel());
//        StartTherapy();
//    }    

//    //----------------------------------------------------------------------------------------------------
//    // Update is called once per frame
//    //----------------------------------------------------------------------------------------------------
//    void Update()
//    {
//        if (Input.GetKey("up"))
//            DoCheatCodes();

//        if (enable_input)
//        {
//            repeatButton.SetActive(m_bShowBtnRepeat);
//            // when user touches / clicks on one of the stimuli
//            // touch screen
//            if (Input.touchCount > 0)
//            {
//                for (var i = 0; i < Input.touchCount; ++i)
//                {
//                    if (Input.GetTouch(i).phase == TouchPhase.Began)
//                    {
//                        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero);
//                        // RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
//                        if (hitInfo)
//                        {
//                            ShowFeedback(hitInfo);
//                        }
//                    }
//                }
//            }
//            // mouse click
//            else if (Input.GetMouseButtonDown(0))
//            {
//                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
//                if (hitInfo)
//                {
//                    ShowFeedback(hitInfo);
//                }
//            }
//        }
//    }

//    //----------------------------------------------------------------------------------------------------
//    // DoCheatCodes
//    //----------------------------------------------------------------------------------------------------
//    void DoCheatCodes()
//    {
//        Debug.Log(" *** DoCheatCodes ***");
//        m_intCheatCtr++;
//        // if m_intCheatCtr >= 5, terminate the game
//        if (m_intCheatCtr >= 50)
//        {
//            m_intCheatCtr = 0;
//            StateChallenge.Instance.AddCoin(5);
//            StateChallenge.Instance.CorrectAnswer();
//            EndTherapySession();
//        }
//    }

//    //----------------------------------------------------------------------------------------------------
//    // OnGUI
//    //----------------------------------------------------------------------------------------------------
//    void OnGUI()
//    {
//        //ShowMainMenu ();
//        if (m_bShowMainMenu)
//        {
//            m_menuLevels.goMenuLevels.SetActive(true);

//            if (m_bIsAdminMode)
//            {
//                // if in admin mode, enable all button level
//                m_menuLevels.scriptMenuLevels.SetAdminMode(true);
//            }
//            else
//            {
//                // disabled all btn levels
//                m_menuLevels.scriptMenuLevels.SetAdminMode(false);

//                // enabled all completed levels
//                List<int> lsCompletedLevel = CTrialList.Instance.getLsCompletedLevel();
//                for (int j = 0; j < lsCompletedLevel.Count; j++)
//                {
//                    //Debug.Log("OnGUI - lsCompletedLevel[j] = " + lsCompletedLevel[j]);
//                    m_menuLevels.scriptMenuLevels.SetBtnLevelDisabled(lsCompletedLevel[j], true);
//                }
//                if (lsCompletedLevel.Count == 0)
//                {
//                    //Debug.Log("OnGUI - lsCompletedLevel.count = 0" );
//                    m_menuLevels.scriptMenuLevels.SetBtnLevelDisabled(0, true);
//                }

//                // enable & highlight cur level
//                m_menuLevels.scriptMenuLevels.SetBtnLevelDisabled(CTrialList.Instance.getCurLevel(), true);
//                m_menuLevels.scriptMenuLevels.SetBtnLevelHighlight(CTrialList.Instance.getCurLevel());
//            }
//        }

//        // set up font size based on screen resolution
//        float fDefaultResolutionHeight = 800;
//        float fDefaultFontSize = 22;
//        int intFontSize = (int)(Screen.height / fDefaultResolutionHeight * fDefaultFontSize);

//        // button style
//        GUIStyle styleBtn = new GUIStyle(GUI.skin.button);
//        styleBtn.fontSize = intFontSize;

//        // set up button width & height relative to screen size
//        float fBtnWidth = Screen.width / 12;
//        float fBtnHeight = Screen.height / 20;

//        // show restart button at the end of game
//        if (m_bShowBtnRestart)
//        {
//            Rect rectBtn = new Rect(Screen.width - Screen.width / 10, Screen.height / 30, fBtnWidth, fBtnHeight);
//            if (GUI.Button(rectBtn, "Restart", styleBtn))
//            {
//                RestartGame();
//            }

//        }

//    }

//    //----------------------------------------------------------------------------------------------------
//    // OnClickButtonAdmin - to be called from MenuLevelsScript
//    //----------------------------------------------------------------------------------------------------
//    public void OnClickButtonAdmin()
//    {
//        // tap the admin btn three times and the it will turn into admin mode
//        m_intClickButtonAdminCtr++;
//        if (m_intClickButtonAdminCtr >= 3)
//        {
//            m_bIsAdminMode = !m_bIsAdminMode;
//            m_intClickButtonAdminCtr = 0;
//        }

//    }

//    private void CleanPreviousTrial()
//    {
//        // stop background noise
//        m_sound_manager.Stop(ChannelType.BackgroundNoise);

//        //if (m_audioBackgroundNoise.isPlaying)
//        //	m_audioBackgroundNoise.Stop();

//        // hide all stimuli
//        for (var i = 0; i < m_arrStimulusGO.Count(); i++)
//            m_arrStimulusGO[i].stimulusScript.ShowStimulus(false);

//        //m_bShowBtnRestart = true;		
//        m_bShowBtnRepeat = false;
//        m_bIsCoroutineIncorrectRunning = false;
//        m_intCurIdx = -1;
//    }

//    //----------------------------------------------------------------------------------------------------
//    // OnClickButtonLevel - to be called from MenuLevelsScript
//    //----------------------------------------------------------------------------------------------------
//    public void OnClickButtonLevel(int intLevel)
//    {
//        // hide main menu
//        //m_menuLevels.goMenuLevels.SetActive (false);
//        //m_bShowMainMenu = false;

//        CleanPreviousTrial();

//        goPhoneModeIcon.SetActive(false);

//        // levels 14 & 15
//        if ((intLevel >= 14) && (intLevel <= 15))
//        {
//            goPhoneModeIcon.SetActive(true);
//            AudioClipInfo aci;
//            aci.delayAtStart = 0.0f;
//            aci.isLoop = false;
//            aci.useDefaultDBLevel = true;
//            aci.clipTag = string.Empty;
//            m_sound_manager.Play(Resources.Load("Audio/telephone_ring") as AudioClip, ChannelType.SoundFx, aci);
//        }

//        // levels 16, 17, 18, 19 with background noise
//        if (intLevel >= 16)
//        {
//            float fVoiceChannelDBlevel = m_sound_manager.GetChannelLevel(ChannelType.VoiceText);
//            string strAudio = "";
//            if (intLevel == 16)
//            {
//                strAudio = "cafe_short";  //"NOISE1_V2";
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 20);
//            }
//            else if (intLevel == 17)
//            {
//                strAudio = "darts_short"; //"NOISE2_V3";
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 15);
//            }
//            else if (intLevel == 18)
//            {
//                strAudio = "race_short";  //"NOISE3_A2";
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 10);
//            }
//            else if (intLevel == 19)
//            {
//                strAudio = "rugby_short"; //"NOISE4_N6";
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 5);
//            }

//            m_sound_manager.Stop(ChannelType.BackgroundNoise);

//            AudioClipInfo aci;
//            aci.delayAtStart = 0.0f;
//            aci.isLoop = true;
//            aci.useDefaultDBLevel = false;
//            aci.clipTag = string.Empty;

//            m_sound_manager.Play(Resources.Load("Audio/" + strAudio) as AudioClip, ChannelType.BackgroundNoise, aci);

//            //			if (!m_audioBackgroundNoise.isPlaying) 
//            //			{
//            //				m_audioBackgroundNoise.clip = Resources.Load ("Audio/" + strAudio) as AudioClip;
//            //				m_audioBackgroundNoise.Play ();
//            //			}
//        }

//        //CTrialList.Instance.ConvertCsvToXml();

//        //CTrialList.Instance.LoadTrials(intLevel);
//        //PrepareNextTrial();
//    }

//    //----------------------------------------------------------------------------------------------------
//    // StartTherapy
//    //----------------------------------------------------------------------------------------------------
//    public void StartTherapy()
//    {
//        CleanPreviousTrial();

//        goPhoneModeIcon.SetActive(false);

//        CUserTherapy.Instance.LoadTrials();

//        // set background noise
//        int intNoiseLevel = CUserTherapy.Instance.GetCurNoiseLevel();
//        //intNoiseLevel = 5;
//        //Debug.Log("*** intNoiseLevel = " + intNoiseLevel);

//        if (intNoiseLevel == 1)
//        {
//            goPhoneModeIcon.SetActive(true);
//            AudioClipInfo aci;
//            aci.delayAtStart = 0.0f;
//            aci.isLoop = false;
//            aci.useDefaultDBLevel = true;
//            aci.clipTag = string.Empty;
//            m_sound_manager.Play(Resources.Load("Audio/telephone_ring") as AudioClip, ChannelType.SoundFx, aci);
//        }
//        else if (intNoiseLevel > 1)
//        {
//            List<string> lsNoiseFile = new List<string>();
//            lsNoiseFile.Add("bar_pub");
//            lsNoiseFile.Add("birds_forest");
//            lsNoiseFile.Add("city_traffic");
//            lsNoiseFile.Add("office");
//            lsNoiseFile.Add("shopping_mall");
//            lsNoiseFile.Add("supermarket");
//            lsNoiseFile.Add("swings_playground");
//            lsNoiseFile.Add("train_station");
//            System.Random rnd = new System.Random();
//            int intIdx = rnd.Next(0, lsNoiseFile.Count);
//            string strAudio = lsNoiseFile[intIdx];

//            float fVoiceChannelDBlevel = m_sound_manager.GetChannelLevel(ChannelType.VoiceText);
//            Debug.Log("fVoiceChannelDBlevel = '" + fVoiceChannelDBlevel + "'");

//            if (intNoiseLevel == 2)
//            {
//                //strAudio = "cafe_short";  
//                //m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 20);
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 15);  // noise1 = -5db
//            }
//            else if (intNoiseLevel == 3)
//            {
//                //strAudio = "darts_short"; 
//                //m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 15);
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 10); // noise2 = 0db
//            }
//            else if (intNoiseLevel == 4)
//            {
//                //strAudio = "race_short";  
//                //m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 10);
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 5); // noise3 = 5db
//            }
//            else if (intNoiseLevel == 5)
//            {
//                //strAudio = "rugby_short"; 
//                //m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 5);
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel - 0); // noise4 = 10db               
//            }
//            else if (intNoiseLevel >= 6)
//            {                
//                m_sound_manager.SetChannelLevel(ChannelType.BackgroundNoise, fVoiceChannelDBlevel + 5); // noise5 = 15db
//            }
//            m_sound_manager.Stop(ChannelType.BackgroundNoise);

//            AudioClipInfo aci;
//            aci.delayAtStart = 0.0f;
//            aci.isLoop = true;
//            aci.useDefaultDBLevel = false;
//            aci.clipTag = string.Empty;
//            m_sound_manager.Play(Resources.Load("Audio/" + strAudio) as AudioClip, ChannelType.BackgroundNoise, aci);
//        }

//        //CUserTherapy.Instance.LoadTrials();
//        PrepareNextTrial();
//    }

//    //----------------------------------------------------------------------------------------------------
//    // ShowFeedback
//    //----------------------------------------------------------------------------------------------------
//    void ShowFeedback(RaycastHit2D hitInfo)
//    {
//        // check if user has already selected a pic
//        if (!m_bShowBtnRepeat)
//            return;

//        if (hitInfo.collider.gameObject.tag == "RepeatButton")
//            return;

//        if (hitInfo.collider.gameObject.tag == "Coin")
//            return;

//        if (hitInfo.collider.gameObject.tag == "CoinSpawner")
//            return;
//        // user has selected another pic, stop coroutine "WaitIncorrect"
//        if (m_bIsCoroutineIncorrectRunning)
//            StopCoroutine("WaitIncorrect");

//        m_bShowBtnRepeat = false;

//        //if (ConvertStimulusTagToIdx (hitInfo.collider.gameObject.tag) == m_arrTrial [m_intCurIdx].intTargetIdx) {
//        m_intSelectedStimulusIdx = ConvertStimulusTagToIdx(hitInfo.collider.gameObject.tag);
//        m_curResponse.m_lsSelectedStimulusIdx.Add(m_intSelectedStimulusIdx);

//        //if (m_intSelectedStimulusIdx == m_lsTrial [m_intCurIdx].m_intTargetIdx) {
//        if (m_intSelectedStimulusIdx == m_curTrial.m_intTargetIdx)
//        {

//            m_curResponse.m_fRTSec = (float)Math.Round((DateTime.Now - m_dtCurTrialStartTime).TotalSeconds, 4);

//            if (m_curResponse.m_lsSelectedStimulusIdx.Count == 1)
//                m_curResponse.m_intScore = 1;
//            //int intCoinNum = m_curTrial.m_lsStimulus.Count - m_curResponse.m_lsSelectedStimulusIdx.Count;
//            int intCoinNum = 2;
//            if (m_curResponse.m_lsSelectedStimulusIdx.Count > 1)
//                intCoinNum = 1;
//            m_curResponse.m_intCoinNum = intCoinNum;

//            StateChallenge.Instance.AddCoin(intCoinNum);
//            StateChallenge.Instance.CorrectAnswer();

//            //ScoreMaster.CorrectAnswer();

//            PlaySound("Sounds/Challenge/AnswerCorrect");

//            m_particleSyst.transform.position = hitInfo.collider.gameObject.transform.position;
//            m_particleSyst.Play();
//            ai.Play("Happy");
//            StartCoroutine(WaitCorrect());
//        }
//        else {
//            PlayFeedbackSnd(false);
//            // show feedback if user hasn't got a right answer
//            m_particleSystIncorrect.transform.position = hitInfo.collider.gameObject.transform.position;
//            m_particleSystIncorrect.Play();
//            //StartCoroutine("WaitCorrect");  // move on straight to next trial
//            ai.Play("Sad");
//            StartCoroutine(WaitIncorrect()); // remain till user has got a right answer

//        }

//    }

//    //----------------------------------------------------------------------------------------------------
//    // ConvertStimulusTagToIdx
//    //----------------------------------------------------------------------------------------------------
//    int ConvertStimulusTagToIdx(string strTag)
//    {
//        int intIdx = -1;
//        if (strTag == "Stimulus1")
//            intIdx = 0;
//        else if (strTag == "Stimulus2")
//            intIdx = 1;
//        else if (strTag == "Stimulus3")
//            intIdx = 2;
//        else if (strTag == "Stimulus4")
//            intIdx = 3;
//        else if (strTag == "Stimulus5")
//            intIdx = 4;
//        else if (strTag == "Stimulus6")
//            intIdx = 5;

//        return (intIdx);
//    }

//    //----------------------------------------------------------------------------------------------------
//    // WaitCorrect: user has a correct answer, move on to next trial
//    //----------------------------------------------------------------------------------------------------
//    IEnumerator WaitCorrect()
//    {
//        // Wait for 2 sec
//        float animationDuration = ai.AnimationLength("Happy");
//        yield return new WaitForSeconds(animationDuration);

//        // set all stimuli to invisible
//        ShowAllStimuli(false);
//        PlaySound("Sounds/Challenge/PicturesDisappear");
//        // continue next trial/challenge
//        PrepareNextTrial();
//    }

//    //----------------------------------------------------------------------------------------------------
//    // WaitIncorrect: user has an incorrect answer, stay on current trial until user has got a correct answer
//    //----------------------------------------------------------------------------------------------------
//    IEnumerator WaitIncorrect()
//    {
//        m_bIsCoroutineIncorrectRunning = true;

//        // Wait for 2 sec
//        float animationDuration = ai.AnimationLength("Sad");
//        yield return new WaitForSeconds(animationDuration);

//        // set selected stimuli to invisible

//        m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.ShowStimulus(false);

//        m_bShowBtnRepeat = true;
//        PlaySound("Sounds/Challenge/PicturesDisappear");

//        // Wait for 3 sec, if "repeat" is not pressed, the word is automatically played following hte pause
//        yield return new WaitForSeconds(2f);

//        //PlayAudio ();

//        m_bIsCoroutineIncorrectRunning = false;

//    }

//    //----------------------------------------------------------------------------------------------------
//    // PrepareNextTrial: continue next trial/challenge
//    //----------------------------------------------------------------------------------------------------
//    void PrepareNextTrial()
//    {
//        m_intSelectedStimulusIdx = -1;

//        // update CTrialList with current response
//        //CTrialList.Instance.UpdateResponseList (m_curResponse);
//        CUserTherapy.Instance.UpdateResponseList(m_curResponse);

//        // check end of level
//        //if (CTrialList.Instance.IsEndOfLevel(m_bIsAdminMode) || trialsCounter == 0)
//        if (CUserTherapy.Instance.IsEndOfLevel(m_bIsAdminMode) || trialsCounter < 1)
//        {
//            EndTherapySession();
//            return;
//        }

//        // fetch the next trial
//        m_curResponse.Reset();
//        //m_curTrial = CTrialList.Instance.GetNextTrial ();
//        m_curTrial = CUserTherapy.Instance.GetNextTrial();
//        trialsCounter--;

//        ShowAllStimuli(false);

//        // show stimuli's images and play target audio
//        Invoke("ShowNextTrial", 2.0f);
//    }

//    //----------------------------------------------------------------------------------------------------
//    // EndTherapySession
//    //----------------------------------------------------------------------------------------------------
//    void EndTherapySession()
//    {
//        m_sound_manager.Stop(ChannelType.BackgroundNoise);
//        //		if (m_audioBackgroundNoise.isPlaying)
//        //			m_audioBackgroundNoise.Stop();
        
//        StateChallenge.Instance.SetTotalTherapyTime(CUserTherapy.Instance.getTotalTherapyTimeMin());
//        StateChallenge.Instance.SetTodayTherapyTime(CUserTherapy.Instance.getTodayTherapyTimeMin());
//        UploadManager.Instance.ForcedTimerState = true;
//        //Andrea: starting to change the animation
//        StartCoroutine(FinishChallenge());        
//    }

//    private IEnumerator FinishChallenge()
//    {
//        yield return new WaitForSeconds(4);
//        ai.Play("JumpIn");
//        yield return new WaitForSeconds(3);
//        GameController.Instance.ChangeState(GameController.States.StateInitializePinball);
//    }

//    //----------------------------------------------------------------------------------------------------
//    // ShowAllStimuli: set all stimuli to visible / invisible
//    //----------------------------------------------------------------------------------------------------
//    void ShowAllStimuli(bool bShow)
//    {
//        for (var i = 0; i < m_curTrial.m_lsStimulus.Count; ++i)
//        {
//            m_arrStimulusGO[i].stimulusScript.ShowStimulus(bShow);
//        }

//        if (bShow)
//        {
//            PlaySound("Sounds/Challenge/PicturesAppear");
//        }
//    }

//    public void SetEnable(bool enable)
//    {
//        enable_input = enable;
//    }

//    public void SetLevelMenu(bool isAdmin)
//    {
//        m_menuLevels.goMenuLevels = GameObject.FindGameObjectWithTag("LevelMenu") as GameObject;

//        if (m_menuLevels.goMenuLevels == null)
//        {
//            Debug.LogError("LevelMenu Not Found");
//            return;
//        }

//        m_menuLevels.scriptMenuLevels = m_menuLevels.goMenuLevels.GetComponent<MenuLevelsScript>();
//        m_menuLevels.scriptMenuLevels.Initialize();
//        m_menuLevels.scriptMenuLevels.SetGameControlScript(this);
//        m_menuLevels.scriptMenuLevels.SetAdminMode(isAdmin);
//        m_menuLevels.scriptMenuLevels.SetBtnLevelHighlight(CTrialList.Instance.getCurLevel());
//    }
//    //----------------------------------------------------------------------------------------------------
//    // ShowNextTrial: show stimuli's images and play target audio
//    //----------------------------------------------------------------------------------------------------
//    public void ShowNextTrial()
//    {
//        ai.Play("Throw");

//        for (var i = 0; i < m_curTrial.m_lsStimulus.Count; ++i)
//        {
//            switch (m_curTrial.m_lsStimulus.Count)
//            {
//                case 3:
//                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr3StimuliPos[i]);
//                    break;
//                case 4:
//                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr4StimuliPos[i]);
//                    break;
//                case 5:
//                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr5StimuliPos[i]);
//                    break;
//                case 6:
//                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr6StimuliPos[i]);
//                    break;
//                default:
//                    print("Incorrect intelligence level.");
//                    break;
//            }
//            //m_arrStimulusGO [i].stimulusScript.SetStimulusImage ("Images/" + m_lsTrial [m_intCurIdx].m_lsStimulus[i].m_strImage);
//            m_arrStimulusGO[i].stimulusScript.SetStimulusImage("Images/CorePhotos/" + m_curTrial.m_lsStimulus[i].m_strImage);

//        }

//        ShowAllStimuli(true);
//        float delay = ai.AnimationLength("Throw");
//        PlayAudio(delay);
//        ResetStimulThrowPos();
//        // to keep track reaction time
//        m_dtCurTrialStartTime = DateTime.Now;

//    }

//    void PlaySound(string resource)
//    {
//        AudioClipInfo aci;
//        aci.delayAtStart = 0.0f;
//        aci.isLoop = false;
//        aci.useDefaultDBLevel = false;
//        aci.clipTag = string.Empty;

//        Camera.main.GetComponent<SoundManager>().Play((Resources.Load(resource) as AudioClip), ChannelType.LevelEffects, aci);

//    }

//    static bool nie = false;

//    //----------------------------------------------------------------------------------------------------
//    // PlayAudio: play target audio 
//    //----------------------------------------------------------------------------------------------------
//    public float PlayAudio(float fDelay = 0)
//    {
//        // stop coroutine "WaitIncorrect" when user presses the "repeat" button or audio is auto played after a pause
//        if (m_bIsCoroutineIncorrectRunning)
//            StopCoroutine("WaitIncorrect");

//        string strAudio = "Audio/CorePhotos/" + m_curTrial.m_strTargetAudio;
//        Debug.Log(String.Format("GameControlScript: target audio = {0}", strAudio));
//        /*
//        string strFolder = CTrialList.Instance.getCurAudioFolder ();
//		string strAudio = "Audio/" + strFolder + m_curTrial.m_strTargetAudio;
//	    */
//        AudioClip clip = null;

//        if (goPhoneModeIcon.activeSelf)       
//		{
//            // play phone voice
//            m_sound_manager.SetChannelLevel(ChannelType.PhoneVoice, 0.0f);
//            AudioClipInfo aci;
//			aci.isLoop = false;
//			aci.delayAtStart = fDelay;
//			aci.useDefaultDBLevel = true;
//			aci.clipTag = string.Empty;

//			clip = Resources.Load(strAudio) as AudioClip;
//			m_sound_manager.Play (clip, ChannelType.PhoneVoice, aci);
//		} 
//		else 
//        {
//            // play normal voice
//            //This is an example of use
//            m_sound_manager.SetChannelLevel(ChannelType.VoiceText, 0.0f);

//            AudioClipInfo aci;
//            aci.isLoop = false;
//            aci.delayAtStart = fDelay;
//            aci.useDefaultDBLevel = true;
//            aci.clipTag = string.Empty;

//            clip = Resources.Load(strAudio) as AudioClip;
//            m_sound_manager.Play(clip, ChannelType.VoiceText, aci);
//        }

//        m_bShowBtnRepeat = true;

//        return clip == null ? 0.0f : clip.length;

//    }

//    //----------------------------------------------------------------------------------------------------
//    // PlayFeedbackSnd: play feedback sound (correct / wrong) using target audio 
//    //----------------------------------------------------------------------------------------------------
//    void PlayFeedbackSnd(bool bCorrect)
//    {
//        string strAudio = "Audio/";
//        if (bCorrect)
//            strAudio = "Sounds/Challenge/AnswerCorrect";
//        else
//            strAudio = "Audio/snd_wrong";

//        AudioClipInfo aci;
//        aci.delayAtStart = 0.0f;
//        aci.isLoop = false;
//        aci.useDefaultDBLevel = true;
//        aci.clipTag = string.Empty;

//        m_sound_manager.Play(Resources.Load(strAudio) as AudioClip, ChannelType.SoundFx, aci);

//        //		m_audio_feedback.clip = Resources.Load(strAudio) as AudioClip;
//        //		m_audio_feedback.Play();
//    }

//    //----------------------------------------------------------------------------------------------------
//    // OnClickReplayButton - called from ReplaySound.cs
//    //----------------------------------------------------------------------------------------------------
//    public float OnClickReplayButton()
//    {
//        m_curResponse.m_intReplayBtnCtr++;
//        return PlayAudio(0);
//    }

//    //----------------------------------------------------------------------------------------------------
//    // getTotalTherapyTime
//    //----------------------------------------------------------------------------------------------------
//    public double getTotalTherapyTimeMin()
//    {
//        return CUserTherapy.Instance.getTotalTherapyTimeMin();
//    }
//}
