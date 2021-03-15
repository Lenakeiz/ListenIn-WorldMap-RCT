using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.IO;
using System.Linq;

/// <summary>
/// USED FOR STANDARD THERAPY ANDREA
/// </summary>
public class GameControlScriptStandard : MonoBehaviour 
{
    // list of trials / challenges
    //List<CTrial> m_lsTrial = new List

    public int numberOfTrials;
    private int trialsCounter;
    private bool enable_input = true;
    AnimationInterface ai;

    // index of the current displayed trial/challenge
    int m_intCurIdx = -1;

    // structure represent stimulus game object 
    public struct stStimulusGO
    {
        public GameObject stimulusGO;  // stimulus gameobject
        public StimulusScript stimulusScript;   // reference to stimulus's script
    }
    // array of 6 stimulus game object 
    stStimulusGO[] m_arrStimulusGO = new stStimulusGO[6];

    struct StimulusPosScale
    {
        Vector3 pos;
        Vector3 localScale;
    }

    // vector positions for 3/4/5/6 stimuli
    Vector3[] m_arr3StimuliPos = new Vector3[3];
    Vector3[] m_arr4StimuliPos = new Vector3[4];
    Vector3[] m_arr5StimuliPos = new Vector3[5];
    Vector3[] m_arr6StimuliPos = new Vector3[6];

    #region External references
    // array of audio sources
    public AudioSource[] m_arrAudioSource;
    // audio source for target
    public AudioSource m_audioTarget;
    // audio source for background noise
    public AudioSource m_audioBackgroundNoise;
    // audio source for feedback
    public AudioSource m_audio_feedback;

    //Transforms to set the stimuli
    public Transform m_root_3;
    public Transform m_root_4;
    public Transform m_root_5;
    public Transform m_root_6;

    // feedback (ticks) to show correct answer
    ParticleSystem m_particleSyst;
    // feedback (cross) to show incorrect answer
    ParticleSystem m_particleSystIncorrect;

    // show phone mode icon on the top right corner during levels with phone voice
    GameObject repeatButton;
    SoundManager m_sound_manager;
    #endregion

    // flags to control the visibility of main menu
    bool m_bShowMainMenu = false;
    // index of the selected stimulus
    int m_intSelectedStimulusIdx = -1;
    // flags to check if the coroutine "WaitIncorrect" is running
    bool m_bIsCoroutineIncorrectRunning = false;
    // flags to control the visibility of btnRestart
    bool m_bShowBtnRestart = false;
    // flags to control the visibility of btnRepeat
    bool m_bShowBtnRepeat = false;

    // structure represent menu levels
    //AndreaLIRO: to check for further delete
    //struct stMenuLevel
    //{
    //    public GameObject goMenuLevels;  // menu levels gameobject
    //    public MenuLevelsScript scriptMenuLevels;   // reference to stimulus's script
    //}

    //stMenuLevel m_menuLevels = new stMenuLevel();

    #region CurrentBlock Variables
    private string m_loadedFile = String.Empty; //AndreaLIRO: added as an escape for the therapy if there is a mismatch between current recorded stats
    CoreItemReader cir = new CoreItemReader();
    List<Challenge> m_currListOfChallenges = new List<Challenge>();
    private int m_curChallengeIdx = -1;
    private int m_currBlockNumberFromManager = -1;
    private int m_currCycleNumber = -1;
    private Challenge m_currChallenge;
    private string m_currAudio;
    private ChallengeResponse m_challengeResponse;
    private List<ChallengeResponse> m_responseList = new List<ChallengeResponse>();
    // trial start time, this is to keep track how long does the patient take to get a correct response 
    DateTime m_dtCurTrialStartTime;
    DateTime m_dtStartingBlock;
    DateTime m_dtEndingBlock;
    float m_totalPlayedMinutes = 0;

    [SerializeField]
    private bool m_cheatOn = true;

    LayerMask currMask;

    private CoreItemWriter m_coreWriter = new CoreItemWriter();
    #endregion

    public void SetEnable(bool enable)
    {
        enable_input = enable;
    }

    //----------------------------------------------------------------------------------------------------
    // SetupStimuliPosMapTransforms: set up positions using transform set in the scene
    //----------------------------------------------------------------------------------------------------
    void SetupStimuliPosMapTransforms()
    {
        if (m_root_3 != null)
        {
            for (int i = 0; i < m_root_3.childCount; i++)
            {
                m_arr3StimuliPos[i] = m_root_3.GetChild(i).position;
            }
        }
        if (m_root_4 != null)
        {
            for (int i = 0; i < m_root_4.childCount; i++)
            {
                m_arr4StimuliPos[i] = m_root_4.GetChild(i).position;
            }
        }
        if (m_root_5 != null)
        {
            for (int i = 0; i < m_root_5.childCount; i++)
            {
                m_arr5StimuliPos[i] = m_root_5.GetChild(i).position;
            }
        }
        if (m_root_6 != null)
        {
            for (int i = 0; i < m_root_6.childCount; i++)
            {
                m_arr6StimuliPos[i] = m_root_6.GetChild(i).position;
            }
        }
    }
    void SetStimuliThrowPos(Vector3 pos)
    {
        for (int i = 0; i < m_arrStimulusGO.Length; i++)
        {
            m_arrStimulusGO[i].stimulusScript.SetThrowPosition(pos);
        }
    }
    void ResetStimulThrowPos()
    {
        for (int i = 0; i < m_arrStimulusGO.Length; i++)
        {
            m_arrStimulusGO[i].stimulusScript.ResetPosition();
        }
    }

    //----------------------------------------------------------------------------------------------------
    // StartTherapy
    //----------------------------------------------------------------------------------------------------
    private void StartTherapy()
    {
        LoadCurrentBlock();
        m_dtStartingBlock = DateTime.UtcNow;    
    }
    //----------------------------------------------------------------------------------------------------
    // RestartGame: restart game
    //----------------------------------------------------------------------------------------------------
    void InitializeGameLIRO()
    {
        CleanPreviousTrial();

        trialsCounter = numberOfTrials;

        StartTherapy();
    }

    private void LoadCurrentBlock()
    {
        m_currBlockNumberFromManager = TherapyLIROManager.Instance.GetCurrentBlockNumber()+GlobalVars.TherapyFilesOffset;
        m_currCycleNumber = TherapyLIROManager.Instance.GetCurrentTherapyCycle();

        string verga = String.Format(
                            "THERAPY_{1}_Cycle_{2}",
                            TherapyLIROManager.Instance.GetCurrentLadderStep().ToString(),
                            m_currBlockNumberFromManager,
                            TherapyLIROManager.Instance.GetCurrentTherapyCycle()
                        );

        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", String.Format(
                            "THERAPY_{1}_Cycle_{2}",
                            TherapyLIROManager.Instance.GetCurrentLadderStep().ToString(),
                            m_currBlockNumberFromManager,
                            TherapyLIROManager.Instance.GetCurrentTherapyCycle()
                        ));
        form.AddField("folder_name", GlobalVars.SectionFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetCurrentBlockFile, LoadCurrentBlockCallback);

    }

    private void LoadCurrentBlockCallback(string response)
    {
        if (response == "error")
        {
            Debug.LogError("THERAPY FILE NOT FOUND");
            return;
        }
        else
        {
            m_currListOfChallenges = cir.ParseCsvFromContent(response).ToList();
            UploadManager.Instance.ResetTimer(TimerType.Therapy);
            UploadManager.Instance.SetTimerState(TimerType.Therapy, true);
            PrepareNextTrialLIRO();
        }
    }

    void PrepareNextTrialLIRO()
    {
        m_intSelectedStimulusIdx = -1;
        m_curChallengeIdx++;

        if (CheckBlockEnding())
        {
            EndTherapySessionLIRO();
            return;
        }
        // fetch the next trial
        m_currChallenge = m_currListOfChallenges[m_curChallengeIdx];
        // preparing response
        m_challengeResponse = new ChallengeResponse();
        m_challengeResponse.m_challengeID = m_currChallenge.ChallengeID;
        m_challengeResponse.m_block = m_currBlockNumberFromManager;
        m_challengeResponse.m_cycle = m_currCycleNumber;
        m_challengeResponse.m_presentationNumber = m_currChallenge.PresentationNumber;
        m_challengeResponse.m_lexicalPresentationNumber = m_currChallenge.LexicalPresentationNumber;
        m_challengeResponse.m_basketNumber = m_currChallenge.BasketNumber;
        m_challengeResponse.m_accuracy = 1;
        RandomizeFoils(m_currChallenge);
        m_currAudio = GetRandomizedAudio(m_currChallenge);
        trialsCounter--;

        //ShowAllStimuli(false);

        // show stimuli's images and play target audio
        ShowNextTrialLIRO();
        //Invoke("ShowNextTrial", 2.0f);
    }
    public void ShowNextTrialLIRO()
    {
        //yield return new WaitForSeconds(2.0f);
        List<long> availableFoils = new List<long>();

        availableFoils = m_currChallenge.Foils.Where(x => x != 0).ToList();

        for (var i = 0; i < availableFoils.Count; ++i)
        {
            switch (availableFoils.Count)
            {
                case 3:
                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr3StimuliPos[i]);
                    break;
                case 4:
                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr4StimuliPos[i]);
                    break;
                case 5:
                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr5StimuliPos[i]);
                    break;
                case 6:
                    m_arrStimulusGO[i].stimulusScript.SetFinalPosition(m_arr6StimuliPos[i]);
                    break;
                default:
                    print("Incorrect intelligence level.");
                    break;
            }
            try
            {
                Debug.Log("Loading image: " + availableFoils[i].ToString());

                ///Up to 3000 is reserved for ACT items
                if (m_currChallenge.ChallengeID < 3000)
                {
                    Debug.LogError("Detected ACT ID in normal therapy");
                    m_arrStimulusGO[i].stimulusScript.SetStimulusImage("Images/LIRO/ACT/" + m_currChallenge.ChallengeID.ToString() + "/" + availableFoils[i].ToString());
                    m_arrStimulusGO[i].stimulusScript.m_registeredID = availableFoils[i];
                }
                //Normal core items
                else
                {
                    
                    m_arrStimulusGO[i].stimulusScript.SetStimulusImage("Images/CorePhotos/" + availableFoils[i].ToString());
                    m_arrStimulusGO[i].stimulusScript.m_registeredID = availableFoils[i];
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(String.Format("Challenge ID: {0}; Cannot load: {0}", m_currChallenge.ChallengeID.ToString(),availableFoils[i].ToString()));
            }

        }
        ai.Play("Throw");
        ShowAllStimuli(true);
        float delay = ai.AnimationLength("Throw");
        PlayAudioLIRO(delay);
        ResetStimulThrowPos();
        // to keep track reaction time
        m_dtCurTrialStartTime = DateTime.UtcNow;
        m_challengeResponse.m_dateTimeStart = DateTime.UtcNow;

    }
    private void CleanPreviousTrial()
    {
        // stop background noise
        //m_sound_manager.Stop(ChannelType.BackgroundNoise);

        // hide all stimuli
        for (var i = 0; i < m_arrStimulusGO.Count(); i++)
            m_arrStimulusGO[i].stimulusScript.ShowStimulus(false);

        //m_bShowBtnRestart = true;		
        m_bShowBtnRepeat = false;
        m_bIsCoroutineIncorrectRunning = false;
        m_intCurIdx = -1;
    }

    //----------------------------------------------------------------------------------------------------
    // ConvertStimulusTagToIdx
    //----------------------------------------------------------------------------------------------------
    int ConvertStimulusTagToIdx(string strTag)
    {
        int intIdx = -1;
        if (strTag == "Stimulus1")
            intIdx = 0;
        else if (strTag == "Stimulus2")
            intIdx = 1;
        else if (strTag == "Stimulus3")
            intIdx = 2;
        else if (strTag == "Stimulus4")
            intIdx = 3;
        else if (strTag == "Stimulus5")
            intIdx = 4;
        else if (strTag == "Stimulus6")
            intIdx = 5;

        return (intIdx);
    }
    void ShowFeedbackLIRO(RaycastHit2D hitInfo)
    {
        // check if user has already selected a pic
        if (!m_bShowBtnRepeat)
            return;

        if (hitInfo.collider.gameObject.tag == "RepeatButton")
            return;

        if (hitInfo.collider.gameObject.tag == "Coin")
            return;

        if (hitInfo.collider.gameObject.tag == "CoinSpawner")
            return;
        // user has selected another pic, stop coroutine "WaitIncorrect"
        if (m_bIsCoroutineIncorrectRunning)
            StopCoroutine("WaitIncorrect");

        m_bShowBtnRepeat = false;

        //if (ConvertStimulusTagToIdx (hitInfo.collider.gameObject.tag) == m_arrTrial [m_intCurIdx].intTargetIdx) {
        m_intSelectedStimulusIdx = ConvertStimulusTagToIdx(hitInfo.collider.gameObject.tag);

        //if (m_intSelectedStimulusIdx == m_lsTrial [m_intCurIdx].m_intTargetIdx) {
        if (m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.m_registeredID == m_currChallenge.CorrectImageID)
        {

            m_challengeResponse.m_dateTimeEnd = DateTime.UtcNow;

            int coinsEarned = 1;
            if (m_challengeResponse.m_accuracy == 1)
                coinsEarned++;

            StateChallenge.Instance.AddCoin(coinsEarned);
            StateChallenge.Instance.CorrectAnswer();

            PlaySound("Sounds/Challenge/AnswerCorrect");

            m_particleSyst.transform.position = hitInfo.collider.gameObject.transform.position;
            m_particleSyst.Play();
            ai.Play("Happy");
            StartCoroutine(WaitCorrect());
        }
        else {
            PlayFeedbackSnd(false);
            // show feedback if user hasn't got a right answer
            m_particleSystIncorrect.transform.position = hitInfo.collider.gameObject.transform.position;
            m_particleSystIncorrect.Play();
            //AndreaLIRO: adjust the accuracy with the 
            m_challengeResponse.m_accuracy++;
            //StartCoroutine("WaitCorrect");  // move on straight to next trial
            ai.Play("Sad");
            StartCoroutine(WaitIncorrect()); // remain till user has got a right answer

        }

    }
    //----------------------------------------------------------------------------------------------------
    // WaitCorrect: user has a correct answer, move on to next trial
    //----------------------------------------------------------------------------------------------------
    IEnumerator WaitCorrect()
    {
        // Wait for 2 sec
        float animationDuration = ai.AnimationLength("Happy");
        if (animationDuration == 0.0f)
            Debug.LogError("Animation found with zero lenght");
        yield return new WaitForSeconds(animationDuration);

        // set all stimuli to invisible
        ShowAllStimuli(false);
        PlaySound("Sounds/Challenge/PicturesDisappear");
        SaveCurrentChallenge();
        // continue next trial/challenge
        yield return new WaitForSeconds(2.0f);
        PrepareNextTrialLIRO();
    }
    //----------------------------------------------------------------------------------------------------
    // WaitIncorrect: user has an incorrect answer, stay on current trial until user has got a correct answer
    //----------------------------------------------------------------------------------------------------
    IEnumerator WaitIncorrect()
    {
        m_bIsCoroutineIncorrectRunning = true;
        m_challengeResponse.incorrectPicturesIDs.Add(m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.m_registeredID.ToString());
        // Wait for 2 sec
        float animationDuration = ai.AnimationLength("Sad");
        PlaySound("Sounds/Challenge/PicturesDisappear");
        m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.ShowStimulus(false);
        yield return new WaitForSeconds(animationDuration);

        // set selected stimuli to invisible
        m_bShowBtnRepeat = true;

        // Wait for 3 sec, if "repeat" is not pressed, the word is automatically played following hte pause
        yield return new WaitForSeconds(0.5f);
        //PlayAudio ();

        m_bIsCoroutineIncorrectRunning = false;

    }

    private bool CheckBlockEnding()
    {
        return (m_curChallengeIdx == m_currListOfChallenges.Count);
    }
    private IEnumerator FinishTherapyBlock()
    {
        m_dtEndingBlock = DateTime.UtcNow;
        //m_totalPlayedMinutes = Mathf.CeilToInt((float)(m_dtEndingBlock - m_dtStartingBlock).TotalMinutes);
        UploadManager.Instance.SetTimerState(TimerType.Therapy, false);
        m_totalPlayedMinutes = (UploadManager.Instance.GetTimerState(TimerType.Therapy))/60.0f;
        yield return new WaitForSeconds(3);
        ai.Play("JumpIn");
        SaveCurrentBlockResponse();
        StartCoroutine(TherapyLIROManager.Instance.AddTherapyMinutes(Mathf.CeilToInt((float)m_totalPlayedMinutes)));
        yield return StartCoroutine(UploadManager.Instance.EndOfTherapyClean(0, m_loadedFile));
        yield return new WaitForSeconds(0.2f);
        GameController.Instance.ChangeState(GameController.States.StateInitializePinball);
    }
    void EndTherapySessionLIRO()
    {
        m_sound_manager.Stop(ChannelType.BackgroundNoise);

        UploadManager.Instance.ForcedTimerState = true;
        //Andrea: starting to change the animation
        StartCoroutine(FinishTherapyBlock());
    }
    private void SaveCurrentChallenge()
    {
        //adding zeros to pictures
        while (m_challengeResponse.incorrectPicturesIDs.Count < 5)
        {
            m_challengeResponse.incorrectPicturesIDs.Add("0");
        }
        m_responseList.Add(m_challengeResponse);

        //AndreaLIRO: TIMERS are counted in UPLOAD MANAGER
        //m_totalPlayedMinutes += (float)(m_challengeResponse.m_dateTimeEnd - m_challengeResponse.m_dateTimeStart).TotalMinutes;
    }
    private void SaveCurrentBlockResponse()
    {
        string filename = String.Format("THERAPY_{0}_Cycle_{1}.csv", m_challengeResponse.m_block.ToString(), m_challengeResponse.m_cycle.ToString());
        string pathFolder = GlobalVars.GetPathToLIROOutput(NetworkManager.IdUser);

#if SAVE_LOCALLY
        //#ERASE
        m_coreWriter.WriteCsv(pathFolder, filename, m_responseList);
        //#ERASE
#endif

        List<string> listString = new List<string>();

        //Adding header
        listString.Add(String.Join(",", new string[] {
                
                "ChallengeID",
                "CycleNumber",
                "Block",
                "PresentationNumber",
                "LexicalPresentationNumber",
                "BasketNumber",

                "DayStart",
                "TimeStart",
                //"DayEnd",
                //"TimeEnd",

                "Accuracy",
                "SoundRepetition",

                "IncorrectPicture1",
                "IncorrectPicture2",
                "IncorrectPicture3",
                "IncorrectPicture4",
                "IncorrectPicture5",

        }));


        foreach (var item in m_responseList)
        {
            listString.Add(String.Join(",", new string[] {

                  item.m_challengeID.ToString(),
                  item.m_cycle.ToString(),
                  item.m_block.ToString(),
                  item.m_presentationNumber.ToString(),
                  item.m_lexicalPresentationNumber.ToString(),
                  item.m_basketNumber.ToString(),

                  item.m_dateTimeStart.ToString("dd/MM/yyyy"),
                  item.m_dateTimeStart.ToString("HH:mm:ss"),
                  //item.m_dateTimeEnd.ToString("dd/MM/yyyy"),
                  //item.m_dateTimeEnd.ToString("HH:mm:ss"),

                  item.m_accuracy.ToString(),
                  item.m_repeat.ToString(),

                  item.incorrectPicturesIDs[0],
                  item.incorrectPicturesIDs[1],
                  item.incorrectPicturesIDs[2],
                  item.incorrectPicturesIDs[3],
                  item.incorrectPicturesIDs[4]

            }));
        };

        //SEND TO SERVER
        byte[] dataAsBytes = listString.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", filename);
        form.AddField("file_size", dataAsBytes.Length);
        form.AddField("folder_name", GlobalVars.OutputFolderName);
        form.AddBinaryData("file_data", dataAsBytes, filename);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);
    }

    void ShowAllStimuli(bool bShow)
    {
        List<long> availableFoils = new List<long>();

        availableFoils = m_currChallenge.Foils.Where(x => x != 0).ToList();

        for (var i = 0; i < availableFoils.Count; ++i)
        {
            m_arrStimulusGO[i].stimulusScript.ShowStimulus(bShow);
        }

        if (bShow)
        {
            PlaySound("Sounds/Challenge/PicturesAppear");
        }
    }
    private void RandomizeFoils(Challenge m_currChallenge)
    {
        for (int i = 0; i < m_currChallenge.Foils.Count; i++)
        {
            long temp = m_currChallenge.Foils[i];
            int intRandomIndex = UnityEngine.Random.Range(i, m_currChallenge.Foils.Count);
            m_currChallenge.Foils[i] = m_currChallenge.Foils[intRandomIndex];
            m_currChallenge.Foils[intRandomIndex] = temp;
        }
    }
    private string GetRandomizedAudio(Challenge m_currChallenge)
    {
        string[] audios = m_currChallenge.FileAudioIDs.Where(x => x.Length > 1).ToArray();
        return audios[UnityEngine.Random.Range(0,audios.Count())];
    }
    private void RandomizeChallenges()
    {
        for (int i = 0; i < m_currListOfChallenges.Count; i++)
        {
            Challenge temp = m_currListOfChallenges[i];
            int intRandomIndex = UnityEngine.Random.Range(i, m_currListOfChallenges.Count);
            m_currListOfChallenges[i] = m_currListOfChallenges[intRandomIndex];
            m_currListOfChallenges[intRandomIndex] = temp;
        }
    }

    //----------------------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------------------
    IEnumerator DoCheatCodes()
    {
        SaveCurrentBlockResponse();
        //TherapyLIROManager.Instance.AddTherapyMinutes(m_totalPlayedMinutes);
        yield return StartCoroutine(UploadManager.Instance.EndOfTherapyClean(0, m_loadedFile));
        //AndreaLIRO: when cheating for starting the timer. Do not remove
        UploadManager.Instance.ResetTimer(TimerType.Pinball);
        UploadManager.Instance.SetTimerState(TimerType.Pinball, true);
        StateChallenge.Instance.AddCoin(40);
        StateChallenge.Instance.cheatActivated = true;
        StatePinball.Instance.initialize = false;
        GameController.Instance.ChangeState(GameController.States.StatePinball);
        StatePinball.Instance.InitLevelPinball(true);        
    }

#region Audio API

    void PlaySound(string resource)
    {
        AudioClipInfo aci;
        aci.delayAtStart = 0.0f;
        aci.isLoop = false;
        aci.useDefaultDBLevel = false;
        aci.clipTag = string.Empty;

        Camera.main.GetComponent<SoundManager>().Play((Resources.Load(resource) as AudioClip), ChannelType.LevelEffects, aci);

    }
    public float PlayAudioLIRO(float fDelay = 0)
    {
        // stop coroutine "WaitIncorrect" when user presses the "repeat" button or audio is auto played after a pause
        if (m_bIsCoroutineIncorrectRunning)
            StopCoroutine("WaitIncorrect");
        string strAudio;
        if (m_currChallenge.ChallengeID < 3000)
        {
            strAudio = "Images/LIRO/ACT/" + m_currChallenge.ChallengeID.ToString() + "/" + m_currAudio;
        }
        else
        {
            strAudio = "Audio/CoreAudios/" + m_currAudio;            
        }
        strAudio = strAudio.Replace(".wav", "");

        Debug.Log(String.Format("GameControlScript: target audio = {0}", strAudio));
        AudioClip clip = null;
                

        try
        {
            m_sound_manager.SetChannelLevel(ChannelType.VoiceText, 0.0f);

            AudioClipInfo aci;
            aci.isLoop = false;
            aci.delayAtStart = fDelay - 0.2f;
            aci.useDefaultDBLevel = true;
            aci.clipTag = string.Empty;

            clip = Resources.Load<AudioClip>(strAudio);

            if (clip != null)
            {
                m_sound_manager.Play(clip, ChannelType.VoiceText, aci);
            }
            else
            {
                Debug.LogWarning(String.Format("Challenge ID: {0}; Cannot load audio: {1}", m_currChallenge.ChallengeID.ToString(), m_currAudio.ToString()));
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("GameControlScriptStandard: unable to load " + strAudio);
        }



        m_bShowBtnRepeat = true;

        return clip == null ? 0.0f : clip.length;

    }

    //----------------------------------------------------------------------------------------------------
    // PlayFeedbackSnd: play feedback sound (correct / wrong) using target audio 
    //----------------------------------------------------------------------------------------------------
    void PlayFeedbackSnd(bool bCorrect)
    {
        string strAudio = "Audio/";
        if (bCorrect)
            strAudio = "Sounds/Challenge/AnswerCorrect";
        else
            strAudio = "Audio/snd_wrong";

        AudioClipInfo aci;
        aci.delayAtStart = 0.0f;
        aci.isLoop = false;
        aci.useDefaultDBLevel = true;
        aci.clipTag = string.Empty;

        m_sound_manager.Play(Resources.Load(strAudio) as AudioClip, ChannelType.SoundFx, aci);

        //		m_audio_feedback.clip = Resources.Load(strAudio) as AudioClip;
        //		m_audio_feedback.Play();
    }
#endregion

    public float OnClickReplayButton()
    {
        //m_curResponse.m_intReplayBtnCtr++;
        m_challengeResponse.m_repeat++;
        return PlayAudioLIRO(0);
    }

    //----------------------------------------------------------------------------------------------------
    // getTotalTherapyTime
    //----------------------------------------------------------------------------------------------------
    //public double getTotalTherapyTimeMin()
    //{
    //    return CUserTherapy.Instance.getTotalTherapyTimeMin();
    //}

#region Unity functions
    //----------------------------------------------------------------------------------------------------
    // Awake
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {

    }

    //----------------------------------------------------------------------------------------------------
    // Use this for initialization
    //----------------------------------------------------------------------------------------------------
    void Start()
    {
        repeatButton = GameObject.FindGameObjectWithTag("RepeatButton") as GameObject;

        m_sound_manager = GameObject.Find("Main Camera").GetComponent<SoundManager>() as SoundManager;

        // retrieve audio sources
        m_arrAudioSource = GetComponents<AudioSource>();
        m_audioTarget = m_arrAudioSource[0];
        m_audioBackgroundNoise = m_arrAudioSource[1];
        m_audio_feedback = m_arrAudioSource[2];

        // retrieve stimuli 1 - 6's gameobject and scripts
        m_arrStimulusGO[0].stimulusGO = GameObject.Find("Stimulus1");
        m_arrStimulusGO[0].stimulusScript = GameObject.Find("Stimulus1").GetComponent<StimulusScript>();
        m_arrStimulusGO[1].stimulusGO = GameObject.Find("Stimulus2");
        m_arrStimulusGO[1].stimulusScript = GameObject.Find("Stimulus2").GetComponent<StimulusScript>();
        m_arrStimulusGO[2].stimulusGO = GameObject.Find("Stimulus3");
        m_arrStimulusGO[2].stimulusScript = GameObject.Find("Stimulus3").GetComponent<StimulusScript>();
        m_arrStimulusGO[3].stimulusGO = GameObject.Find("Stimulus4");
        m_arrStimulusGO[3].stimulusScript = GameObject.Find("Stimulus4").GetComponent<StimulusScript>();
        m_arrStimulusGO[4].stimulusGO = GameObject.Find("Stimulus5");
        m_arrStimulusGO[4].stimulusScript = GameObject.Find("Stimulus5").GetComponent<StimulusScript>();
        m_arrStimulusGO[5].stimulusGO = GameObject.Find("Stimulus6");
        m_arrStimulusGO[5].stimulusScript = GameObject.Find("Stimulus6").GetComponent<StimulusScript>();

        // each trial will have 3 / 4 / 5 / 6 stimuli
        SetupStimuliPosMapTransforms();

        Vector3 scale = new Vector3(2, 2, 2);
        GameObject.Find("Stimulus1").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus2").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus3").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus4").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus5").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus6").transform.Find("PictureFrame").transform.localScale = scale;

        // set stimulus's layer orders - for images overlapping
        GameObject.Find("Stimulus1").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 52;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus1").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 51;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus2").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 54;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus2").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 53;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus3").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 56;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus3").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 55;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus4").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 58;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus4").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 57;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus5").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 60;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus5").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 59;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus6").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 62;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 61;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        
        // set particlesystem layer's order
        m_particleSyst = GameObject.Find("ParticleFeedback").GetComponent<ParticleSystem>();
        m_particleSyst.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
        m_particleSyst.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        m_particleSystIncorrect = GameObject.Find("ParticleFeedbackIncorrect").GetComponent<ParticleSystem>();
        m_particleSystIncorrect.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
        m_particleSystIncorrect.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        //GETTING THE Animator script
        GameObject lepr = GameObject.FindGameObjectWithTag("Leprechaun");
        if (lepr != null)
        {
            ai = lepr.GetComponent<AnimationInterface>();
            Vector3 throwPosition = lepr.transform.Find("ThrowPos").position;
            SetStimuliThrowPos(throwPosition);
        }
        
        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<CircleCollider2D>().enabled = true;
        Camera.main.GetComponent<SoundManager>().SetChannelLevel(ChannelType.LevelEffects, 5.0f);

        //AndreaLIRO: DatabaseXML will have to be scrapped
        //Dictionary<string, string> block_start = new Dictionary<string, string>();

        //int patient = DatabaseXML.Instance.PatientId;
        //DateTime now = System.DateTime.Now;

        //block_start.Add("patient", patient.ToString());
        //block_start.Add("date", now.ToString("yyyy-MM-dd HH:mm:ss"));
        
        // restart game
        InitializeGameLIRO();

    }
    void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        //AndreaLIRO: putting this on the GameControlScriptStandard to control normal process of closing the challenge therapy
        if (Input.GetKeyDown(KeyCode.Space) && m_cheatOn)
        {
            StartCoroutine(DoCheatCodes());
        }
#endif

        if (enable_input)
        {
            repeatButton.SetActive(m_bShowBtnRepeat);
            // when user touches / clicks on one of the stimuli
            // touch screen
            if (Input.touchCount > 0)
            {
                for (var i = 0; i < Input.touchCount; ++i)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        currMask = LayerMask.NameToLayer("Stimulus");
                        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero, Mathf.Infinity, 1 << currMask);
                        // RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
                        if (hitInfo)
                        {
                            ShowFeedbackLIRO(hitInfo);
                        }
                    }
                }
            }
            // mouse click
            else if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                currMask = LayerMask.NameToLayer("Stimulus");
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero, Mathf.Infinity, 1 << currMask);
                if (hitInfo)
                {
                    ShowFeedbackLIRO(hitInfo);
                }
            }
        }
    }
#endregion
}
