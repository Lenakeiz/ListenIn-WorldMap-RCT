using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.IO;
using MadLevelManager;


public class GameControlScriptACT : MonoBehaviour 
{
    // list of trials / challenges
    //List<CTrial> m_lsTrial = new List

    public int numberOfTrials;
    public int numberOfCorrectTrials = 0;
    private bool enable_input = true;

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

    public Text m_couterText;
    public Slider m_Slider;
    private string counterformat = "{0}";

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
    //struct stMenuLevel
    //{
    //    public GameObject goMenuLevels;  // menu levels gameobject
    //    public MenuLevelsScript scriptMenuLevels;   // reference to stimulus's script
    //}
    //stMenuLevel m_menuLevels = new stMenuLevel();

    // cheat code - tap the side panel 5 times and the therapy session will be terminated and move to pinball session
    int m_intCheatCtr = 0;

    #region CurrentBlock Variables
    ACTItemReader air = new ACTItemReader();
    List<ACTChallenge> m_currListOfChallenges = new List<ACTChallenge>();
    private int m_curChallengeIdx = -1;
    private ACTChallenge m_currACTChallenge;
    private ChallengeResponseACT m_challengeResponse;
    private List<ChallengeResponseACT> m_responseList = new List<ChallengeResponseACT>();
    private string m_currAudioFileName;
    // trial start time, this is to keep track how long does the patient take to get a correct response 
    DateTime m_dtCurTrialStartTime;
    LayerMask currMask;
    private bool isRepeatOn = true;

    private ACTItemWriter m_actWriter = new ACTItemWriter();
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
    public void StartTherapyACT()
    {
        //CleanPreviousTrial();
        LoadCurrentBlock();
    }
    //----------------------------------------------------------------------------------------------------
    // RestartGame: restart game
    //----------------------------------------------------------------------------------------------------
    void StartACT()
    {
        CleanPreviousTrial();
        StartTherapyACT();
    }

    private void LoadCurrentBlock()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", String.Format(
                            "{0}_{1}_Cycle_{2}",
                            TherapyLIROManager.Instance.GetCurrentLadderStep().ToString(),
                            TherapyLIROManager.Instance.GetCurrentBlockNumber(),
                            TherapyLIROManager.Instance.GetCurrentTherapyCycle()
                        ));
        form.AddField("folder_name", GlobalVars.SectionFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetFile, LoadCurrentBlockCallback);

        //try
        //{
        //    m_currListOfChallenges = air.ParseCsv(Path.Combine
        //            (GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.UserId),
        //    String.Format(
        //                    "{0}_{1}_Cycle_{2}", TherapyLIROManager.Instance.GetCurrentLadderStep().ToString(), TherapyLIROManager.Instance.GetCurrentBlockNumber(), TherapyLIROManager.Instance.GetCurrentTherapyCycle()
        //                )
        //            )
        //        ).ToList();
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError("GCTACT: " + ex.Message);
        //}
    }

    private void LoadCurrentBlockCallback(string response)
    {
        if(response == "error")
        {
            Debug.LogError("THERAPY FILE NOT FOUND");
            return;
        }
        else
        {
            m_currListOfChallenges = air.ParseCsvFromContent(response).ToList();
            PrepareNextTrialLIRO();
        }
    }

    void PrepareNextTrialLIRO()
    {
        isRepeatOn = true;
        m_intSelectedStimulusIdx = -1;
        m_curChallengeIdx++;

        if (CheckBlockEnding())
        {
            EndTherapySessionACT();
            return;
        }
        // fetch the next trial
        m_currACTChallenge = m_currListOfChallenges[m_curChallengeIdx];
        //Selecting curr audio to play
        m_currAudioFileName = (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) ? m_currACTChallenge.FileAudioID_F : m_currACTChallenge.FileAudioID_M;

        // preparing response
        m_challengeResponse = new ChallengeResponseACT();
        //Saving basic information
        m_challengeResponse.m_block = TherapyLIROManager.Instance.GetCurrentBlockNumber();
        m_challengeResponse.m_cycle = TherapyLIROManager.Instance.GetCurrentTherapyCycle();
        m_challengeResponse.m_challengeID = m_currACTChallenge.ChallengeID;
        m_challengeResponse.m_number = m_curChallengeIdx + 1;
        RandomizeFoils(m_currACTChallenge);

        //ShowAllStimuli(false);

        // show stimuli's images and play target audio
        ShowNextTrialLIRO();
        //Invoke("ShowNextTrial", 2.0f);
    }
    public void ShowNextTrialLIRO()
    {
        for (var i = 0; i < m_currACTChallenge.Foils.Count; ++i)
        {
            switch (m_currACTChallenge.Foils.Count)
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
                    print("Incorrect number of foils.");
                    break;
            }
            m_arrStimulusGO[i].stimulusScript.SetStimulusImage("Images/LIRO/ACT/" + m_currACTChallenge.ChallengeID.ToString() +  "/" + m_currACTChallenge.Foils[i].ToString());
            m_arrStimulusGO[i].stimulusScript.m_registeredID = m_currACTChallenge.Foils[i];

        }

        ShowAllStimuli(true);

        PlayAudioLIRO(2.5f);
        ResetStimulThrowPos();
        SetCurrentCounter();
        SetEnable(true);
        // to keep track reaction time
        m_challengeResponse.m_timeStamp = m_dtCurTrialStartTime = DateTime.Now;

    }
    void SetCurrentCounter()
    {
        if (m_couterText != null)
        {
            m_couterText.text = String.Format(counterformat, m_curChallengeIdx + 1);
            m_Slider.value = m_curChallengeIdx + 1;
        }
    }
    private void CleanPreviousTrial()
    {
        // stop background noise
        //m_sound_manager.Stop(ChannelType.BackgroundNoise);

        //if (m_audioBackgroundNoise.isPlaying)
        //	m_audioBackgroundNoise.Stop();

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
        SetEnable(false);

        m_intSelectedStimulusIdx = ConvertStimulusTagToIdx(hitInfo.collider.gameObject.tag);
        m_challengeResponse.m_pictureID = (int)m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.m_registeredID;
        m_challengeResponse.m_reactionTime = (float)Math.Round((DateTime.Now - m_dtCurTrialStartTime).TotalSeconds, 4);

        if (m_arrStimulusGO[m_intSelectedStimulusIdx].stimulusScript.m_registeredID == m_currACTChallenge.CorrectImageID)
        {
            //CORRECT            
            m_challengeResponse.m_accuracy = 1;
            numberOfCorrectTrials++;
            StartCoroutine(WaitCorrect());
        }
        else
        {
            //WRONG
            //PlayFeedbackSnd(false);
            m_challengeResponse.m_accuracy = 0;
            StartCoroutine(WaitIncorrect()); // remain till user has got a right answer

        }

    }
    //----------------------------------------------------------------------------------------------------
    // WaitCorrect: user has a correct answer, move on to next trial
    //----------------------------------------------------------------------------------------------------
    IEnumerator WaitCorrect()
    {
        // Wait for 2 sec
        //float animationDuration = ai.AnimationLength("Happy");
        //yield return new WaitForSeconds(animationDuration);
        yield return new WaitForSeconds(0.1f);

        // set all stimuli to invisible
        ShowAllStimuli(false);
        PlaySound("Sounds/Challenge/PicturesDisappear");
        // continue next trial/challenge
        SaveCurrentChallenge();
        yield return new WaitForSeconds(2.0f);
        PrepareNextTrialLIRO();
    }

    //----------------------------------------------------------------------------------------------------
    // WaitIncorrect: user has an incorrect answer, stay on current trial until user has got a correct answer
    //----------------------------------------------------------------------------------------------------
    IEnumerator WaitIncorrect()
    {

        yield return new WaitForSeconds(0.1f);

        ShowAllStimuli(false);
        PlaySound("Sounds/Challenge/PicturesDisappear");
        // continue next trial/challenge
        SaveCurrentChallenge();
        yield return new WaitForSeconds(2.0f);
        PrepareNextTrialLIRO();

    }

    private bool CheckBlockEnding()
    {
        return (m_curChallengeIdx >= m_currListOfChallenges.Count);
    }
    private void SaveCurrentChallenge()
    {
        m_responseList.Add(m_challengeResponse);
    }
    private void SaveCurrentBlockResponse()
    {
        TherapyLadderStep currentACTSection = TherapyLIROManager.Instance.GetUserProfile.LIROStep;
        string filename = String.Format(GlobalVars.ACTStringFormat, currentACTSection.ToString(), m_challengeResponse.m_block.ToString(), m_challengeResponse.m_cycle.ToString());
        string pathFolder = GlobalVars.GetPathToLIROOutput(NetworkManager.IdUser);

#if SAVE_LOCALLY
        //#ERASE
        m_actWriter.WriteCsv(pathFolder, filename, m_responseList);
        //#ERASE
#endif

        List<string> listString = new List<string>();
        foreach (var item in m_responseList)
        {
            listString.Add(String.Join(",", new string[] {

                item.m_challengeID.ToString(),
                item.m_timeStamp.ToString("dd/MM/yyyy"),
                item.m_timeStamp.ToString("HH:mm:ss"),
                item.m_number.ToString(),
                item.m_block.ToString(),
                item.m_cycle.ToString(),
                item.m_accuracy.ToString(),
                item.m_reactionTime.ToString(),
                item.m_repeat.ToString(),
                item.m_pictureID.ToString()
            }));
        }

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
    private IEnumerator FinishTherapyBlock()
    {
        SaveCurrentBlockResponse();
        yield return new WaitForEndOfFrame();
        StartCoroutine(UploadManager.Instance.EndOfACTClean(numberOfCorrectTrials));
    }
    void EndTherapySessionACT()
    {
        //m_sound_manager.Stop(ChannelType.BackgroundNoise);
        StartCoroutine(FinishTherapyBlock());
    }

    void ShowAllStimuli(bool bShow)
    {
        List<long> availableFoils = new List<long>();

        availableFoils = m_currACTChallenge.Foils.Where(x => x != 0).ToList();

        for (var i = 0; i < m_currACTChallenge.Foils.Count; ++i)
        {
            m_arrStimulusGO[i].stimulusScript.ShowStimulus(bShow);
        }

        if (bShow)
        {
            PlaySound("Sounds/Challenge/PicturesAppear");
        }
    }
    private void RandomizeFoils(ACTChallenge m_currChallenge)
    {
        for (int i = 0; i < m_currChallenge.Foils.Count; i++)
        {
            long temp = m_currChallenge.Foils[i];
            int intRandomIndex = UnityEngine.Random.Range(i, m_currChallenge.Foils.Count);
            m_currChallenge.Foils[i] = m_currChallenge.Foils[intRandomIndex];
            m_currChallenge.Foils[intRandomIndex] = temp;
        }
    }
    private void RandomizeChallenges()
    {
        for (int i = 0; i < m_currListOfChallenges.Count; i++)
        {
            ACTChallenge temp = m_currListOfChallenges[i];
            int intRandomIndex = UnityEngine.Random.Range(i, m_currListOfChallenges.Count);
            m_currListOfChallenges[i] = m_currListOfChallenges[intRandomIndex];
            m_currListOfChallenges[intRandomIndex] = temp;
        }
    }

    //----------------------------------------------------------------------------------------------------
    // OnClickButtonLevel - to be called from MenuLevelsScript
    //----------------------------------------------------------------------------------------------------
    
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

        //string audiofilelastpart = Path.Combine(m_currACTChallenge.ChallengeID.ToString(), m_currAudioFileName);
        
        string strAudio = "Images/LIRO/ACT/" + m_currACTChallenge.ChallengeID.ToString() + "/" + m_currAudioFileName;
        strAudio = strAudio.Replace(".wav", "");
        Debug.Log(String.Format("GameControlScript: target audio = {0}", strAudio));

        AudioClip clip = null;

        {
            // play normal voice
            //This is an example of use
            m_sound_manager.SetChannelLevel(ChannelType.VoiceText, 0.0f);

            AudioClipInfo aci;
            aci.isLoop = false;
            aci.delayAtStart = fDelay;
            aci.useDefaultDBLevel = true;
            aci.clipTag = string.Empty;
            try
            {
                clip = Resources.Load(strAudio) as AudioClip;
                m_sound_manager.Play(clip, ChannelType.VoiceText, aci);
            }
            catch (Exception ex)
            {
                Debug.Log("GameControlScriptACT: unable to load audio");
            }
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
        if (isRepeatOn)
        {
            isRepeatOn = false;
            m_challengeResponse.m_repeat++;
            return PlayAudioLIRO(0);
        }
        else return 0;
 
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
        //SetupStimuliPosMap ();
        SetupStimuliPosMapTransforms();

        Vector3 scale = new Vector3(2, 2, 2);
        GameObject.Find("Stimulus1").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus2").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus3").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus4").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus5").transform.Find("PictureFrame").transform.localScale = scale;
        GameObject.Find("Stimulus6").transform.Find("PictureFrame").transform.localScale = scale;

        // set stimulus's layer orders - for images overlapping
        GameObject.Find("Stimulus2").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 4;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus2").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 3;// GameObject.Find("Stimulus1").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus3").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 6;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus3").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 5;// GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus4").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 8;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus4").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 7;// GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus5").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 10;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus5").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 9;// GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        GameObject.Find("Stimulus6").transform.Find("PictureFrame").transform.Find("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 12;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder + 1;
        GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 11;// GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;


        // set particlesystem layer's order
        m_particleSyst = GameObject.Find("ParticleFeedback").GetComponent<ParticleSystem>();
        m_particleSyst.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
        m_particleSyst.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        m_particleSystIncorrect = GameObject.Find("ParticleFeedbackIncorrect").GetComponent<ParticleSystem>();
        m_particleSystIncorrect.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
        m_particleSystIncorrect.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.Find("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

        // restart game
        StartACT();

        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<CircleCollider2D>().enabled = true;
    }
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndTherapySessionACT();
            return;
        }

#endif
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTherapySessionACT();
            return;
        }
#endif
        if (enable_input)
        {
            repeatButton.SetActive(m_bShowBtnRepeat && isRepeatOn);
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
