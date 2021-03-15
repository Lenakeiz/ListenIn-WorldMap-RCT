using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using MadLevelManager;

public class SART_Test_Practice : MonoBehaviour {

    private enum PracticeStructure { Practice, Go, NoGO };
    private enum TrialType { Go, NoGo };
    private enum ButtonPromptType { Demo, Prompt1, Prompt2, Prompt3};

	public GameObject door;
	public GameObject go;
	public GameObject noGo;
    public GameObject hand;
    public GameObject rewardHand;
    public GameObject button;
    
	public Text counter;

    public Sprite GoHand;
    public Sprite NoGoHand;
    public Sprite OkHand;
    public Sprite NotOkHand;

    public ButtonSpriteAnimator bsa;

    private SpriteRenderer goHandRenderer;
    private SpriteRenderer rewardHandRenderer;

    private Button buttonB;

    private PracticeStructure m_currStep;

    /// <summary>
    /// GO SESSION
    /// </summary>
    private List<TrialType> currGOTrials;
    public int currCorrectGoTrial = 0;
    /// <summary>
    /// NO GO SESSION
    /// </summary>
    private List<List<TrialType>> NoGoTrials;
    private TrialType currNOGOTrialType;
    private ButtonPromptType currPromptType;
    public int currCorrectNOGoSession = 0;

    private bool isButtonEnabled = false;
    private bool canStopCoroutine = false;
    private bool showPositiveFeedback = false;
    private bool practicefail = false;

    IEnumerator GOCoroutine;
    IEnumerator NOGOCoroutine;

    public Vector3 handInitialiPosition;

	void Awake(){
        
	}

	void Start(){
        InitializeNoGOTrials();
		Reset ();
        m_currStep = PracticeStructure.Practice;
        buttonB = button.GetComponent<Button>();
        StartCoroutine(PracticeLoop());
	}

    private void InitializeNoGOTrials()
    {
        currGOTrials = new List<TrialType>(new TrialType[] { TrialType.Go, TrialType.Go, TrialType.Go, TrialType.Go, TrialType.Go, TrialType.Go, });

        NoGoTrials = new List<List<TrialType>>();
        List<TrialType> currList = new List<TrialType>(new TrialType[] {TrialType.NoGo});
        NoGoTrials.Add(currList);
        currList = new List<TrialType>(new TrialType[] { TrialType.NoGo, TrialType.Go });
        NoGoTrials.Add(currList);
        currList = new List<TrialType>(new TrialType[] { TrialType.NoGo, TrialType.Go, TrialType.NoGo});
        NoGoTrials.Add(currList);
        currList = new List<TrialType>(new TrialType[] { TrialType.Go, TrialType.NoGo, TrialType.Go, TrialType.NoGo });
        NoGoTrials.Add(currList);
        currList = new List<TrialType>(new TrialType[] { TrialType.NoGo, TrialType.Go, TrialType.Go, TrialType.Go, TrialType.NoGo, TrialType.Go});
        NoGoTrials.Add(currList);
    }
    
    void Reset(){

        rewardHandRenderer = rewardHand.GetComponent<SpriteRenderer>();
        goHandRenderer = hand.GetComponent<SpriteRenderer>();

        go.SetActive(false);
        button.SetActive(false);
        noGo.SetActive (false);
		door.SetActive (true);
        hand.SetActive(false);
        rewardHand.SetActive(false);
    }

    private void CleanTrial()
    {
        isButtonEnabled = false;
        hand.gameObject.transform.position = handInitialiPosition;
        go.SetActive(false);
        noGo.SetActive(false);
        door.SetActive(true);
        hand.SetActive(false);
        rewardHand.SetActive(false);
    }

    private void SetCharacter(bool isGo)
    {
        go.SetActive(isGo);
        noGo.SetActive(!isGo);
        door.SetActive(false);
    }
    private void ShowRewardHand(bool isOK, bool isOn)
    {
        rewardHandRenderer.sprite = isOK ? OkHand : NotOkHand;
        rewardHand.SetActive(isOn);
    }
    private IEnumerator ShowHand(bool isActive, bool isNoGo, ButtonPromptType buttonPrompt)
    {
        goHandRenderer.sprite = isNoGo? NoGoHand: GoHand;
        hand.SetActive(isActive);
        //Andrea need to add animation
        if (isActive)
        {
            switch (buttonPrompt)
            {
                case ButtonPromptType.Demo:
                    //Move to button
                    iTween.Init(hand);
                    iTween.MoveTo(hand, iTween.Hash("position", new Vector3(2, -3.5f, -15), "time", 1.0f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutCubic));
                    yield return new WaitForSeconds(4.0f);
                    hand.transform.position = handInitialiPosition;
                    buttonB.interactable = true;
                    iTween.Init(hand);
                    iTween.MoveTo(hand, iTween.Hash("position", new Vector3(1, -3.5f, -15), "time", 2.0f, "looptype", iTween.LoopType.none, "easetype", iTween.EaseType.linear));
                    yield return new WaitForSeconds(2.0f);
                    Debug.Log("SART - Pressing the button");
                    
                    yield return new WaitForEndOfFrame();
                    button.GetComponent<Button>().onClick.Invoke();
                    break;
                case ButtonPromptType.Prompt1:
                    hand.transform.position = handInitialiPosition;
                    iTween.Init(hand);
                    iTween.MoveTo(hand, iTween.Hash("position", new Vector3(2,-3.5f,-15), "time", 1.0f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutCubic));
                    yield return null;
                    break;
                case ButtonPromptType.Prompt2:
                    button.GetComponent<ColorLerper>().StartLerp();
                    //iTween.Init(button);
                    //iTween.ColorTo(button, iTween.Hash("color", new Color(220.0f/255.0f, 220.0f / 255.0f, 220.0f / 255.0f), "time", 1.0f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutCubic));
                    yield return null;
                    break;
                case ButtonPromptType.Prompt3:
                    hand.transform.position = handInitialiPosition;
                    iTween.Init(hand);
                    button.GetComponent<ColorLerper>().StopLerp();
                    iTween.MoveTo(hand, iTween.Hash("position", new Vector3(1, -3.5f, -15), "time", 2.0f, "looptype", iTween.LoopType.none, "easetype", iTween.EaseType.linear));
                    yield return new WaitForSeconds(2.0f);
                    break;
            }
        }

        yield return null;

    }
    private IEnumerator ShowRewardHandFeedback(bool isPositive)
    {
        ShowRewardHand(isPositive, true);
        yield return new WaitForSeconds(2);
        ShowRewardHand(isPositive, false);
        yield return new WaitForEndOfFrame();
    }

    public void GoButtonPressed()
    {
        if (m_currStep == PracticeStructure.Practice || isButtonEnabled == false)
            return;

        if (m_currStep == PracticeStructure.Go)
        {
            if (GOCoroutine != null)
            {
                ResetButton();
                isButtonEnabled = false;
                showPositiveFeedback = true;
                currCorrectGoTrial++;
                canStopCoroutine = true;
                //StopCoroutine(GOCoroutine);
            }
        }
        else if (m_currStep == PracticeStructure.NoGO)
        {
            if (currNOGOTrialType == TrialType.NoGo)
            {
                ResetButton();
                isButtonEnabled = false;
                showPositiveFeedback = false;
                canStopCoroutine = true;
            }
            else if (currNOGOTrialType == TrialType.Go)
            {
                ResetButton();
                isButtonEnabled = false;
                showPositiveFeedback = true;
                
                canStopCoroutine = true;
            }
        }
    }

    /// <summary>
    /// Entire loop divided in more sections
    /// </summary>
    /// <returns></returns>
    private IEnumerator PracticeLoop()
    {
        yield return null;

        for (int i = 3; i > 0; i--)
        {
            counter.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        counter.gameObject.SetActive(false);

        yield return StartCoroutine(InitialPracticeShow());
        yield return StartCoroutine(GoSessionLoop());
        yield return StartCoroutine(NoGoSessionLoop());

        yield return StartCoroutine(TherapyLIROManager.Instance.SaveCurrentSARTPractice(currCorrectGoTrial > 2 && currCorrectNOGoSession > 2));
        yield return new WaitForSeconds(1);
        MadLevel.LoadLevelByName("MainHUB");
    }
    /// <summary>
    /// GO trials loop
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoSessionLoop()
    {
        //Demo part
        yield return StartCoroutine(DemoSession());

        m_currStep = PracticeStructure.Go;
        for (int i = 0; i < currGOTrials.Count; i++)
        {
            GOCoroutine = GoTrial();
            yield return StartCoroutine(GOCoroutine);
            CleanTrial();
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(ShowRewardHandFeedback(showPositiveFeedback));             
        }

    }
    /// <summary>
    /// NoGo trials loop
    /// </summary>
    /// <returns></returns>
    private IEnumerator NoGoSessionLoop()
    {
        m_currStep = PracticeStructure.NoGO;

        yield return StartCoroutine(NoGoDemoSession());

        foreach (var currTrial in NoGoTrials)
        {
            for (int i = 0; i < currTrial.Count; i++)
            {
                currNOGOTrialType = currTrial[i];
                switch (currNOGOTrialType)
                {
                    case TrialType.Go:
                        GOCoroutine = GoTrial();
                        yield return StartCoroutine(GOCoroutine);
                        CleanTrial();
                        yield return new WaitForEndOfFrame();
                        yield return StartCoroutine(ShowRewardHandFeedback(showPositiveFeedback));
                        break;
                    case TrialType.NoGo:
                        GOCoroutine = NoGoTrial();
                        yield return StartCoroutine(GOCoroutine);
                        CleanTrial();
                        yield return new WaitForEndOfFrame();
                        yield return StartCoroutine(ShowRewardHandFeedback(showPositiveFeedback));
                        break;
                }
                yield return null;
            }
        }

    }

    private IEnumerator InitialPracticeShow()
    {
        //Start Practice
        SetCharacter(true);
        yield return new WaitForSeconds(3);
        Reset();
        yield return new WaitForSeconds(2);
        SetCharacter(false);
        yield return new WaitForSeconds(3);
        Reset();
        yield return new WaitForSeconds(2);
    }
    /// <summary>
    /// Demo for one go trial
    /// </summary>
    /// <returns></returns>
    private IEnumerator DemoSession()
    {
        m_currStep = PracticeStructure.Practice;
        SetCharacter(true);
        yield return new WaitForSeconds(2.0f);
        button.SetActive(true);
        buttonB.interactable = false;
        bsa.SetAnimationEnabled(false);
        isButtonEnabled = false;
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ShowHand(true, false, ButtonPromptType.Demo));
        CleanTrial();
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ShowRewardHandFeedback(true));
        bsa.SetAnimationEnabled(true);

    }
    /// <summary>
    /// Demo for one NoGo trial
    /// </summary>
    /// <returns></returns>
    private IEnumerator NoGoDemoSession()
    {
        yield return null;
        SetCharacter(false);
        button.SetActive(true);
        buttonB.interactable = false;
        isButtonEnabled = false;
        bsa.SetAnimationEnabled(false);
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ShowHand(true, true, ButtonPromptType.Prompt1));
        yield return new WaitForSeconds(3.0f);
        CleanTrial();
        buttonB.interactable = true;
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ShowRewardHandFeedback(true));
        bsa.SetAnimationEnabled(true);
    }
    /// <summary>
    /// Single Go trial
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoTrial()
    {
        SetCharacter(true);
        button.SetActive(true);
        isButtonEnabled = true;
        canStopCoroutine = false;
        for (int i = 1; i < 6; i++)
        {
            yield return new WaitForSeconds(1);
            if (canStopCoroutine)
            {
                yield break;
            }
        }
        yield return ShowHand(true, false, ButtonPromptType.Prompt1);
        for (int i = 1; i < 6; i++)
        {
            yield return new WaitForSeconds(1);
            if (canStopCoroutine)
            {
                yield break;
            }
        }
        //AndreaLIRO: show enanched animation
        yield return ShowHand(true, false, ButtonPromptType.Prompt2);
        for (int i = 1; i < 6; i++)
        {
            yield return new WaitForSeconds(1);
            if (canStopCoroutine)
            {
                yield break;
            }
        }
        isButtonEnabled = false;
        yield return ShowHand(true, false, ButtonPromptType.Prompt3);
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// Single No Go Trial
    /// </summary>
    /// <returns></returns>
    private IEnumerator NoGoTrial()
    {
        SetCharacter(false);
        button.SetActive(true);
        isButtonEnabled = true;
        canStopCoroutine = false;

        for (int i = 1; i < 6; i++)
        {
            yield return new WaitForSeconds(1);
            if (canStopCoroutine)
            {
                yield break;
            }
        }
        //CorrectAnswer
        isButtonEnabled = false;
        showPositiveFeedback = true;
        currCorrectNOGoSession++;
    }

    private void ResetButton()
    {
        button.GetComponent<ColorLerper>().StopLerp();
    }
}
