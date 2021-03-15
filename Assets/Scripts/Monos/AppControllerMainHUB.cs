using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using MadLevelManager;

public class AppControllerMainHUB : MonoBehaviour {

    public Button m_continueButtonACT;
    public Button m_continueButtonTherapy;
    public Button m_continueInstructionTherapy;

    public Button[] m_continueButtonSart;
    public Button m_questionaireStart;
    //public Text m_currentSectionText;
    public ACT_UI m_ACT_ui;
    public Therapy_UI m_Therapy_ui;
    public SART_UI m_SART_ui;
    public Questionaire_UI m_Questionaire_ui;
    public Therpay_Instruction_UI m_Therapy_Instruction_UI;

    private TherapyLadderStep currLadderStep;
    private string currLevelToLoad = "";

    void Awake()
    {
        //AndreaLIRO: goes into the main hub. This get the feedback from the therapyLIROManager to understand what to do in the current section
        //The first event is used when detecting switches
        TherapyLIROManager.Instance.m_OnUpdatingCurrentSection += UpdateCurrentSection;
        TherapyLIROManager.Instance.m_OnSwitchingSection += SwitchingSection;
        TherapyLIROManager.Instance.m_OnEndingSection += OnEndingSection;
    }

    void Start()
    {

    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("MainHUB: Level Loaded calling TherapyLiroManager");
        HideAllMenus();
        TherapyLIROManager.Instance.CheckCurrentSection();
    }

    void OnDestroy()
    {
        if (TherapyLIROManager.Instance != null)
        {
            TherapyLIROManager.Instance.m_OnUpdatingCurrentSection -= UpdateCurrentSection;
            TherapyLIROManager.Instance.m_OnSwitchingSection -= SwitchingSection;
            TherapyLIROManager.Instance.m_OnEndingSection -= OnEndingSection;
        }
    }

    #region Button Events
    public void mainButtonClickedTherapy()
    {
        MadLevel.LoadLevelByName(currLevelToLoad);
    }
    public void mainButtonClickedACT()
    {
        currLevelToLoad = "ACT";
        MadLevel.LoadLevelByName(currLevelToLoad);
    }
    /// <summary>
    /// This event is launched when the user press continue button after having seen the score
    /// </summary>
    public void actScoreButtonClicked()
    {
        StartCoroutine(CloseActScore());
    }

    public void sartPracticeTestClicked()
    {
        currLevelToLoad = "SART_PRACTICE";
        MadLevel.LoadLevelByName(currLevelToLoad);
    }
    public void sartTestClicked()
    {
        currLevelToLoad = "SART";
        MadLevel.LoadLevelByName(currLevelToLoad);

    }

    public void questionaireButtonClicked()
    {
        currLevelToLoad = "Questionaire";
        MadLevel.LoadLevelByName(currLevelToLoad);
        //AndreaLIRO: need to implement
    }

    public void sartEndAllClicked()
    {
        m_SART_ui.DisableCloseButtons();
        TherapyLIROManager.Instance.GoToNextSection();
    }
    #endregion

    #region DelegateMethods
    void UpdateCurrentSection(UserProfileManager currProfile, int amount)
    {
        switch (currProfile.LIROStep)
        {
            case TherapyLadderStep.BASKET:
                //m_currentSectionText.text = "Therapy";
                m_Therapy_Instruction_UI.gameObject.SetActive(true);
                //m_Therapy_ui.PrepareBasketScreen(currProfile);
                m_continueInstructionTherapy.enabled = true;
                currLevelToLoad = "Basket Selection";
                m_continueInstructionTherapy.interactable = true;
                break;
            case TherapyLadderStep.CORE:
                //m_currentSectionText.text = "Therapy";
                m_Therapy_ui.gameObject.SetActive(true);
                m_Therapy_ui.UpdateUserStats(currProfile);
                m_continueButtonTherapy.enabled = true;
                currLevelToLoad = "World Map Select";
                m_continueButtonTherapy.interactable = true;
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                //m_currentSectionText.text = "ACT";
                m_ACT_ui.gameObject.SetActive(true);
                m_ACT_ui.SetACTStardardBoard();
                m_ACT_ui.UpdateIcon(2,currProfile.m_userProfile.m_ACTLiroUserProfile.m_currentBlock);
                //currLevelToLoad = "ACT";
                m_continueButtonACT.interactable = true;
                break;
            case TherapyLadderStep.SART_PRACTICE_1_:
            case TherapyLadderStep.SART_PRACTICE_2_:
                //m_currentSectionText.text = "SART";
                m_SART_ui.gameObject.SetActive(true);
                if (currProfile.m_userProfile.m_SartLiroUserProfile.attempts == 1)
                {
                    m_SART_ui.SetText("Practice FAILED, try again");
                }
                m_continueButtonSart[0].interactable = true;
                m_continueButtonSart[1].interactable = false;
                //currLevelToLoad = "SART";
                break;
            case TherapyLadderStep.SART_TEST_1_:
            case TherapyLadderStep.SART_TEST_2_:
                //m_currentSectionText.text = "SART";
                m_SART_ui.gameObject.SetActive(true);
                m_continueButtonSart[0].interactable = false;
                m_continueButtonSart[1].interactable = true;
                //currLevelToLoad = "SART";
                break;
            case TherapyLadderStep.QUESTIONAIRE_1:
            case TherapyLadderStep.QUESTIONAIRE_2:
                //m_currentSectionText.text = "Questionnaire";
                m_Questionaire_ui.gameObject.SetActive(true);
                m_questionaireStart.interactable = true;
                break;
            default:
                break;
        }

        VideoPlayerController.Instance.SetVideo(currProfile.LIROStep);

    }

    void SwitchingSection(UserProfileManager currProfile, int amount)
    {
        if (amount == 0)
        {
            HideAllMenus();

            //Activate the current UI
            switch (currProfile.LIROStep)
            {
                case TherapyLadderStep.BASKET:
                    //m_currentSectionText.text = "Therapy";
                    m_Therapy_Instruction_UI.gameObject.SetActive(true);
                    //m_Therapy_ui.PrepareBasketScreen(currProfile);
                    currLevelToLoad = "Basket Selection";
                    m_continueInstructionTherapy.enabled = true;
                    currLevelToLoad = "Basket Selection";
                    m_continueInstructionTherapy.interactable = true;
                    break;
                case TherapyLadderStep.CORE:
                    //m_currentSectionText.text = "Therapy";
                    m_Therapy_ui.gameObject.SetActive(true);
                    m_Therapy_ui.UpdateUserStats(currProfile);
                    currLevelToLoad = "World Map Select";
                    m_continueButtonTherapy.interactable = true;
                    break;
                case TherapyLadderStep.ACT_1_:
                case TherapyLadderStep.ACT_2_:
                    //m_currentSectionText.text = "ACT";
                    m_ACT_ui.gameObject.SetActive(true);
                    m_ACT_ui.SetACTStardardBoard();
                    currLevelToLoad = "ACT";
                    break;
                case TherapyLadderStep.SART_PRACTICE_1_:
                case TherapyLadderStep.SART_PRACTICE_2_:
                    //m_currentSectionText.text = "SART";
                    m_SART_ui.gameObject.SetActive(true);
                    m_continueButtonSart[0].interactable = true;
                    m_continueButtonSart[1].interactable = false;
                    break;
                case TherapyLadderStep.SART_TEST_1_:
                case TherapyLadderStep.SART_TEST_2_:
                    //m_currentSectionText.text = "SART";
                    m_SART_ui.gameObject.SetActive(true);
                    m_continueButtonSart[0].interactable = false;
                    m_continueButtonSart[1].interactable = true;
                    break;
                case TherapyLadderStep.QUESTIONAIRE_1:
                case TherapyLadderStep.QUESTIONAIRE_2:
                    //m_currentSectionText.text = "Questionnaire";
                    m_Questionaire_ui.gameObject.SetActive(true);
                    m_questionaireStart.interactable = true;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (currProfile.LIROStep)
            {
                case TherapyLadderStep.CORE:
                    break;
                case TherapyLadderStep.ACT_1_:
                case TherapyLadderStep.ACT_2_:
                    m_ACT_ui.UpdateText(amount);
                    break;
                default:
                    break;
            }
        }
    }

    void OnEndingSection(UserProfileManager currProfile)
    {
        switch (currProfile.LIROStep)
        {
            case TherapyLadderStep.BASKET:
            case TherapyLadderStep.CORE:
                //m_currentSectionText.text = "Therapy";
                m_Therapy_ui.gameObject.SetActive(true);
                StartCoroutine(EndTherapy(currProfile));
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                //m_currentSectionText.text = "ACT";
                m_ACT_ui.gameObject.SetActive(true);
                StartCoroutine(EndACT(currProfile));
                break;
            case TherapyLadderStep.SART_PRACTICE_1_:
            case TherapyLadderStep.SART_PRACTICE_2_:
                //m_currentSectionText.text = "SART";
                m_SART_ui.gameObject.SetActive(true);
                m_continueButtonSart[0].interactable = false;
                m_continueButtonSart[1].interactable = false;
                StartCoroutine(EndSART(currProfile));
                break;
            case TherapyLadderStep.SART_TEST_1_:
            case TherapyLadderStep.SART_TEST_2_:
                //m_currentSectionText.text = "SART";
                m_SART_ui.gameObject.SetActive(true);
                m_continueButtonSart[0].interactable = false;
                m_continueButtonSart[1].interactable = false;
                m_SART_ui.PrepareEndingScreen();
                break;
            case TherapyLadderStep.QUESTIONAIRE_1:
                //m_currentSectionText.text = "Questionaire";
                m_Questionaire_ui.gameObject.SetActive(false);
                m_questionaireStart.interactable = false;
                StartCoroutine(EndQuestionaire(currProfile));
                break;
            case TherapyLadderStep.QUESTIONAIRE_2:
                //m_currentSectionText.text = "Questionaire";
                m_Questionaire_ui.gameObject.SetActive(false);
                m_questionaireStart.interactable = false;
                StartCoroutine(EndQuestionaire(currProfile));
                break;
            default:
                break;
        }
    }

    private IEnumerator EndTherapy(UserProfileManager currProfile)
    {
        yield return new WaitForSeconds(2);
        TherapyLIROManager.Instance.GoToNextSection();
    }
    private IEnumerator EndACT(UserProfileManager currProfile)
    {
        //m_ACT_ui.UpdateIcon(2, currProfile.m_userProfile.m_ACTLiroUserProfile.m_currentBlock);
        //yield return new WaitForSeconds(2);
        bool firstTime = currProfile.m_userProfile.m_cycleNumber == 0;
        //Hardcode the percentage
        // There are 228 ACT at the moment
        yield return StartCoroutine(m_ACT_ui.SetScore((float)(currProfile.m_userProfile.m_ACTLiroUserProfile.m_currScore/228.0f)*100.0f, (float)(currProfile.m_userProfile.m_ACTLiroUserProfile.m_previousScore / 228.0f) * 100.0f, firstTime));
        //AndreaLIRO: add other animations here... then back to the Therapy Manager to change section
        //Maybe wait for a button to be pressed in order to go back to the 
    }
    public IEnumerator EndACTScore()
    {
        yield return StartCoroutine(TherapyLIROManager.Instance.SaveACTPreviousScore());
        TherapyLIROManager.Instance.GoToNextSection();
    }
    private IEnumerator EndSART(UserProfileManager currProfile)
    {
        yield return new WaitForEndOfFrame();
        TherapyLIROManager.Instance.GoToNextSection();
    }
    private IEnumerator EndQuestionaire(UserProfileManager currProfile)
    {
        yield return new WaitForSeconds(2);
        TherapyLIROManager.Instance.GoToNextSection();
    }
    private IEnumerator CloseActScore()
    {
        yield return StartCoroutine(m_ACT_ui.HideBoard());
        yield return TherapyLIROManager.Instance.SaveACTPreviousScore();
        TherapyLIROManager.Instance.GoToNextSection();
    }
    #endregion

    private void HideAllMenus()
    {
        m_ACT_ui.gameObject.SetActive(false);
        m_Therapy_ui.gameObject.SetActive(false);
        m_SART_ui.gameObject.SetActive(false);
        m_Questionaire_ui.gameObject.SetActive(false);
        m_Therapy_Instruction_UI.gameObject.SetActive(false);
    }

    public void PlayVideo()
    {
        VideoPlayerController.Instance.PlayVideo();
    }
}
