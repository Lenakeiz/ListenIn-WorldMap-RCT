using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class GameControlScriptTutorial : MonoBehaviour 
{

	private enum TutorialStructure{IDLE, STIMULUS1_CORRECT, STIMULUS2_WRONG, STIMULUS2_CORRECT, STIMULUS3_CORRECT, ENDING}
	private TutorialStructure m_structure = TutorialStructure.IDLE;
	private TutorialStructure m_next_state = TutorialStructure.IDLE;

	public GameObject m_pointingHand;
	TransformLerper lerp = null;
	private int currNextPointingIndex; //can be also wrong // 0 is always true answer
	private int currPointingIndex;
	public float offsetToTarget = 2.0f;
	private bool activateAnimation = false;
	private bool restingAnimation = true;
	private bool acquiringInput = false;
	private bool restingCoroutine = false;

	public int numberOfTrials;
	private int trialsCounter;
	private bool enable_input = true;
	AnimationInterface ai;
	
	// index of the current displayed trial/challenge
	int m_intCurIdx = -1;
	
	// current trial
	CTrial m_curTrial = new CTrial();
	
	// response to current trial
	CResponse m_curResponse = new CResponse();
	
	// structure represent stimulus game object 
	public struct stStimulusGO {
		public GameObject stimulusGO;  // stimulus gameobject
		public StimulusScript stimulusScript;   // reference to stimulus's script
	}
	// array of 6 stimulus game object 
	stStimulusGO[] m_arrStimulusGO = new stStimulusGO[6];
	
	// feedback (ticks) to show correct answer
	ParticleSystem m_particleSyst;
	
	// feedback (cross) to show incorrect answer
	ParticleSystem m_particleSystIncorrect;
	
	// flags to control the visibility of btnRepeat
	bool m_bShowBtnRepeat = false;
	
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
	
	//Transforms to set the stimuli
	public Transform m_root_3;
	public Transform m_root_4;
	public Transform m_root_5;
	public Transform m_root_6;
	
	// index of the selected stimulus
	int m_intSelectedStimulusIdx = -1;
	
	// flags to check if the coroutines running
	bool m_bIsCoroutineIncorrectRunning = false;
	bool m_bIsCoroutineCorrectRunning = false;

	GameObject repeatButton;
	SoundManager m_sound_manager;	
	
	// is currently in admin mode?
	bool m_bIsAdminMode = true;	
	
	// list of trials / challenges
	List<CTrial> m_lsTrial = new List<CTrial>();
	
	//----------------------------------------------------------------------------------------------------
	// Use this for initialization
	//----------------------------------------------------------------------------------------------------
	void Start () 
	{
		repeatButton = GameObject.FindGameObjectWithTag("RepeatButton") as GameObject;

        m_pointingHand = GameObject.FindGameObjectWithTag("PointingHand") as GameObject;
        lerp = m_pointingHand.GetComponent<TransformLerper>();

		m_sound_manager = GameObject.Find("Main Camera").GetComponent<SoundManager>() as SoundManager;				
		
		// retrieve stimuli 1 - 6's gameobject and scripts
		m_arrStimulusGO [0].stimulusGO = GameObject.Find ("Stimulus1");
		m_arrStimulusGO [0].stimulusScript = GameObject.Find("Stimulus1").GetComponent<StimulusScript>();
		m_arrStimulusGO [1].stimulusGO = GameObject.Find ("Stimulus2");
		m_arrStimulusGO [1].stimulusScript = GameObject.Find("Stimulus2").GetComponent<StimulusScript>();
		m_arrStimulusGO [2].stimulusGO = GameObject.Find ("Stimulus3");
		m_arrStimulusGO [2].stimulusScript = GameObject.Find("Stimulus3").GetComponent<StimulusScript>();
		m_arrStimulusGO [3].stimulusGO = GameObject.Find ("Stimulus4");
		m_arrStimulusGO [3].stimulusScript = GameObject.Find("Stimulus4").GetComponent<StimulusScript>();
		m_arrStimulusGO [4].stimulusGO = GameObject.Find ("Stimulus5");
		m_arrStimulusGO [4].stimulusScript = GameObject.Find("Stimulus5").GetComponent<StimulusScript>();
		m_arrStimulusGO [5].stimulusGO = GameObject.Find ("Stimulus6");
		m_arrStimulusGO [5].stimulusScript = GameObject.Find("Stimulus6").GetComponent<StimulusScript>();
		
		// each trial will have 3 / 4 / 5 / 6 stimuli
		//SetupStimuliPosMap ();
		SetupStimuliPosMapTransforms();
		
		// set stimulus's layer orders - for images overlapping
		GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 4;
		GameObject.Find("Stimulus2").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 3; 
		
		GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 6; 
		GameObject.Find("Stimulus3").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 5;
		
		GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 8; 
		GameObject.Find("Stimulus4").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 7;
		
		GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 10; 
		GameObject.Find("Stimulus5").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 9;
		
		GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").transform.FindChild("FrameShadow").gameObject.GetComponent<Renderer>().sortingOrder = 12; 
		GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder = 11;
		
		
		// set particlesystem layer's order
		m_particleSyst = GameObject.Find("ParticleFeedback").GetComponent<ParticleSystem>();
		m_particleSyst.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
		m_particleSyst.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;
		
		m_particleSystIncorrect = GameObject.Find("ParticleFeedbackIncorrect").GetComponent<ParticleSystem>();
		m_particleSystIncorrect.GetComponent<Renderer>().sortingLayerID = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingLayerID;
		m_particleSystIncorrect.GetComponent<Renderer>().sortingOrder = GameObject.Find("Stimulus6").transform.FindChild("PictureFrame").gameObject.GetComponent<Renderer>().sortingOrder + 1;

		//GETTING THE Animator script
		GameObject lepr =  GameObject.FindGameObjectWithTag("Leprechaun");
		if(lepr != null)
		{
			ai = lepr.GetComponent<AnimationInterface>();
			Vector3 throwPosition = lepr.transform.FindChild("ThrowPos").position;
			SetStimuliThrowPos(throwPosition);
		}

		// restart game
		RestartGame ();		
		
	}

	void SetStimuliThrowPos(Vector3 pos){
		for (int i = 0; i < m_arrStimulusGO.Length; i++) {
			m_arrStimulusGO[i].stimulusScript.SetThrowPosition(pos);
		}
	}
	
	void ResetStimulThrowPos()
	{
		for (int i = 0; i < m_arrStimulusGO.Length; i++) {
			m_arrStimulusGO[i].stimulusScript.ResetPosition();
		}
	}
	
	//----------------------------------------------------------------------------------------------------
	// SetupStimuliPosMapTransforms: set up positions using transform set in the scene
	//----------------------------------------------------------------------------------------------------
	void SetupStimuliPosMapTransforms()
	{
		if(m_root_3 != null)
		{
			for (int i = 0; i < m_root_3.childCount; i++) {
				m_arr3StimuliPos[i] = m_root_3.GetChild(i).position;
			}
		}
		if(m_root_4 != null)
		{
			for (int i = 0; i < m_root_4.childCount; i++) {
				m_arr4StimuliPos[i] = m_root_4.GetChild(i).position;
			}
		}
		if(m_root_5 != null)
		{
			for (int i = 0; i < m_root_5.childCount; i++) {
				m_arr5StimuliPos[i] = m_root_5.GetChild(i).position;
			}
		} 
		if(m_root_6 != null)
		{
			for (int i = 0; i < m_root_6.childCount; i++) {
				m_arr6StimuliPos[i] = m_root_6.GetChild(i).position;
			}
		} 
	}
	
	
	
	//----------------------------------------------------------------------------------------------------
	// LoadTrials: load from xml file all stimuli's images, audio and target index for all trials/challenges
	//----------------------------------------------------------------------------------------------------
	void LoadTrials()
	{
		// if xml has already been loaded, then return
		//if (m_lsTrial.Count > 0) return;
		
		m_lsTrial.Clear ();
		
		// load xml
		//XElement root = XElement.Load(strXmlFile);
		TextAsset textAsset = Resources.Load("Doc/demo-xml") as TextAsset;
		XElement root = XElement.Parse(textAsset.text);     
		m_lsTrial = (
			from el in root.Elements("node")
			select new CTrial  
			{
			m_lsStimulus = (
				from el2 in el.Elements("stimulus")
				select new CStimulus
				{
				m_strName = (string)el2.Element("name"),
				m_strImage = (string)el2.Element("image"),
				m_strType = (string)el2.Element("type"),
			}
			).ToList(),
			m_strTargetAudio = (string)el.Element("targetAudio"),	
			m_intTargetIdx = (int)el.Element("targetIdx")	
		}
		).ToList(); 
		
		//tell the score master how many questions we're asking
		ScoreMaster.SetQuestionAmount(m_lsTrial.Count);
		
	}		
	
	//----------------------------------------------------------------------------------------------------
	// RestartGame: restart game
	//----------------------------------------------------------------------------------------------------
	void RestartGame()
	{		
		repeatButton.SetActive(false);
		m_pointingHand.SetActive(false);

		CleanPreviousTrial();

		trialsCounter = numberOfTrials;		
		
		m_bIsAdminMode = true;		
		
		LoadTrials ();
		PrepareNextTrial ();

	}

	void AcquirePlayerInput()
	{
		if (Input.touchCount > 0) {
			for (var i = 0; i < Input.touchCount; ++i) {
				if (Input.GetTouch (i).phase == TouchPhase.Began) {
					RaycastHit2D hitInfo = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (i).position), Vector2.zero);
					// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
					if (hitInfo) 
					{
						ShowFeedback(hitInfo);										
					}
				}
			}
		}
		// mouse click
		else if (Input.GetMouseButtonDown (0)) 
		{
			Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
			if(hitInfo)
			{
				ShowFeedback(hitInfo);	
			}
		}
	}

	private void SettingCurrentStep(TutorialStructure next, int curridx, int nextidx)
	{
		m_next_state = next;
		currPointingIndex = curridx;
		if(activateAnimation)
		{
			activateAnimation = false;
			lerp.StartAnimation();
		}
		else if(lerp.animationEnded)
		{
			if(!restingCoroutine)
			{
				StartCoroutine(StartRestingAnimation());
			}
		}

		if(acquiringInput)
		{
			currNextPointingIndex = nextidx;
			AcquirePlayerInput();
		}
	}

	//----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	//----------------------------------------------------------------------------------------------------
	void Update () 
	{
		switch (m_structure) {
			case TutorialStructure.IDLE:
				m_next_state = TutorialStructure.STIMULUS1_CORRECT;
				currPointingIndex = 0;
				currNextPointingIndex = 0;
				break;
			case TutorialStructure.STIMULUS1_CORRECT:
                SettingCurrentStep(TutorialStructure.STIMULUS2_CORRECT, 0, 0);
                //SettingCurrentStep(TutorialStructure.STIMULUS2_WRONG,0,1);
                break;
			case TutorialStructure.STIMULUS2_WRONG:
				SettingCurrentStep(TutorialStructure.STIMULUS2_CORRECT,1,0);
				break;
			case TutorialStructure.STIMULUS2_CORRECT:
				SettingCurrentStep(TutorialStructure.STIMULUS3_CORRECT,0,3);
				break;
			case TutorialStructure.STIMULUS3_CORRECT:
				SettingCurrentStep(TutorialStructure.ENDING,3,0);
				break;
			case TutorialStructure.ENDING:
				break;
			default:
				break;
			}
	}
	

	//----------------------------------------------------------------------------------------------------
	// CleanPreviousTrial
	//----------------------------------------------------------------------------------------------------
	private void CleanPreviousTrial()
	{
		// stop background noise
		m_sound_manager.Stop (ChannelType.BackgroundNoise);
		
		// hide all stimuli
		for (var i = 0; i < m_arrStimulusGO.Count(); i++) 
			m_arrStimulusGO [i].stimulusScript.ShowStimulus (false);
		
		m_bShowBtnRepeat = false;

		m_bIsCoroutineIncorrectRunning = false;
		m_intCurIdx = -1;
	}	
	
	//----------------------------------------------------------------------------------------------------
	// ShowFeedback
	//----------------------------------------------------------------------------------------------------
	void ShowFeedback(RaycastHit2D hitInfo)
	{
		//TODO : put it back!!
//		if (!m_bShowBtnRepeat)
//			return;	
		
		// user has selected another pic, stop coroutine "WaitIncorrect"
		if (m_bIsCoroutineIncorrectRunning || m_bIsCoroutineCorrectRunning)
			return;
//			StopCoroutine("WaitIncorrect"); 
		
		m_bShowBtnRepeat = false;

		//taking current hit
		m_intSelectedStimulusIdx = ConvertStimulusTagToIdx (hitInfo.collider.gameObject.tag);
		
		if(m_intSelectedStimulusIdx == currPointingIndex)
		{
			m_curResponse.m_lsSelectedStimulusIdx.Add(m_intSelectedStimulusIdx);

			if (m_intSelectedStimulusIdx == m_curTrial.m_intTargetIdx) 
			{
				int intCoinNum = 2;
				if (m_curResponse.m_lsSelectedStimulusIdx.Count > 1)
					intCoinNum = 1;
				StateChallenge.Instance.AddCoin(intCoinNum);
				StateChallenge.Instance.CorrectAnswer();
				PlayFeedbackSnd(true);
				
				m_particleSyst.transform.position = m_arrStimulusGO [m_intSelectedStimulusIdx].stimulusGO.transform.position; //hitInfo.collider.gameObject.transform.position;
				m_particleSyst.Play ();
				ai.Play("Happy");
				StartCoroutine (WaitCorrect());
			}
			else
			{
				PlayFeedbackSnd(false);
				// show feedback if user hasn't got a right answer
				m_particleSystIncorrect.transform.position = m_arrStimulusGO [m_intSelectedStimulusIdx].stimulusGO.transform.position;  //hitInfo.collider.gameObject.transform.position;
				m_particleSystIncorrect.Play();
				ai.Play("Sad");
				StartCoroutine(WaitIncorrect()); // remain till user has got a right answer
			}
		}
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
	
	//----------------------------------------------------------------------------------------------------
	// WaitCorrect: user has a correct answer, move on to next trial
	//----------------------------------------------------------------------------------------------------
	IEnumerator WaitCorrect ()
	{		
		m_bIsCoroutineCorrectRunning = true;

		float animationDuration = ai.AnimationLength("Happy");
		yield return new WaitForSeconds(animationDuration);
		
		// set all stimuli to invisible
		ShowAllStimuli (false);
		m_pointingHand.SetActive(false);
		// continue next trial/challenge
		PrepareNextTrial ();

		m_bIsCoroutineCorrectRunning = false;
	}
	
	//----------------------------------------------------------------------------------------------------
	// WaitIncorrect: user has an incorrect answer, stay on current trial until user has got a correct answer
	//----------------------------------------------------------------------------------------------------
	IEnumerator WaitIncorrect ()
	{	
		m_bIsCoroutineIncorrectRunning = true;

		m_pointingHand.SetActive(false);
		
		float animationDuration = ai.AnimationLength("Sad");
		yield return new WaitForSeconds(animationDuration);
		
		// set selected stimuli to invisible
		m_arrStimulusGO [m_intSelectedStimulusIdx].stimulusScript.ShowStimulus (false);
		PlaySound("Sounds/Challenge/PicturesDisappear");
		// Wait for 3 sec, if "repeat" is not pressed, the word is automatically played following hte pause
		yield return new WaitForSeconds(2f);
		
		PlayAudio ();

		m_bShowBtnRepeat = true;

		ShowNextMovement();

		m_bIsCoroutineIncorrectRunning = false;
	}
	
	//----------------------------------------------------------------------------------------------------
	// PrepareNextTrial: continue next trial/challenge
	//----------------------------------------------------------------------------------------------------
	void PrepareNextTrial()
	{
		m_pointingHand.SetActive(false);

		m_intCurIdx++;

		if (m_intCurIdx >= m_lsTrial.Count) 
		{
			EndTherapySession();
			return;
		}

		// fetch the next trial
		m_curResponse.Reset ();
		m_curTrial = m_lsTrial [m_intCurIdx];  //CTrialList.Instance.GetNextTrial ();
		
		trialsCounter--;
		
		ShowAllStimuli (false);
		
		// show stimuli's images and play target audio
		Invoke("ShowNextTrial", 2.0f);
	}
	
	//----------------------------------------------------------------------------------------------------
	// EndTherapySession
	//----------------------------------------------------------------------------------------------------
	void EndTherapySession()
	{	
		m_sound_manager.Stop(ChannelType.BackgroundNoise);
        StateChallenge.Instance.ResetCoins();
		GameController.Instance.ChangeState(GameController.States.StateTutorialChallenge);
	}

	void PlaySound(string resource)
	{
		AudioClipInfo aci;
		aci.delayAtStart = 0.0f;
		aci.isLoop = false;
		aci.useDefaultDBLevel = false;
		aci.clipTag = string.Empty;
		
		Camera.main.GetComponent<SoundManager>().Play((Resources.Load(resource) as AudioClip), ChannelType.LevelEffects,aci);
		
	}

	//----------------------------------------------------------------------------------------------------
	// ShowAllStimuli: set all stimuli to visible / invisible
	//----------------------------------------------------------------------------------------------------
	void ShowAllStimuli(bool bShow)
	{	
		for (var i = 0; i < m_curTrial.m_lsStimulus.Count; ++i)
		{
			m_arrStimulusGO [i].stimulusScript.ShowStimulus (bShow);
		}

		if(bShow){
			PlaySound("Sounds/Challenge/PicturesAppear");
		}

	}
	
	//----------------------------------------------------------------------------------------------------
	// SetEnable
	//----------------------------------------------------------------------------------------------------
	public void SetEnable(bool enable)
	{
		enable_input = enable;
	}	
	
	//----------------------------------------------------------------------------------------------------
	// ShowNextTrial: show stimuli's images and play target audio
	//----------------------------------------------------------------------------------------------------
	public void ShowNextTrial()
	{
		ai.Play("Throw");

		
		for (var i = 0; i < m_curTrial.m_lsStimulus.Count; ++i) 
		{
			switch (m_curTrial.m_lsStimulus.Count)
			{
			case 3:
				m_arrStimulusGO [i].stimulusScript.SetFinalPosition(m_arr3StimuliPos [i]);
				break;
			case 4:
				m_arrStimulusGO [i].stimulusScript.SetFinalPosition(m_arr4StimuliPos [i]);
				break;
			case 5:
				m_arrStimulusGO [i].stimulusScript.SetFinalPosition(m_arr5StimuliPos [i]);
				break;
			case 6:
				m_arrStimulusGO [i].stimulusScript.SetFinalPosition(m_arr6StimuliPos [i]);
				break;				
			default:
				print ("Incorrect intelligence level.");
				break;
			}
			//m_arrStimulusGO [i].stimulusScript.SetStimulusImage ("Images/" + m_lsTrial [m_intCurIdx].m_lsStimulus[i].m_strImage);
			m_arrStimulusGO [i].stimulusScript.SetStimulusImage ("Images/phase1/" + m_curTrial.m_lsStimulus[i].m_strImage);
			
		}

		ShowAllStimuli (true);
		float delay = ai.AnimationLength("Throw");
		PlayAudio (delay);
		ResetStimulThrowPos();

		Invoke("ShowNextMovement", 2.0f);
	}

	private void ChangeState()
	{
		activateAnimation = true;
		acquiringInput = true;
		m_structure = m_next_state;
	}

	private void ShowNextMovement()
	{

		lerp.ResetAll();

		lerp.AddPosition(m_pointingHand.transform.position);
		int target = currNextPointingIndex;

		Vector3 currDirection = (m_arrStimulusGO[target].stimulusGO.transform.position - m_pointingHand.transform.position).normalized;

		lerp.AddPosition(m_arrStimulusGO[target].stimulusGO.transform.position - currDirection * offsetToTarget);
		lerp.AddLookingTarget((m_arrStimulusGO[target].stimulusGO.transform.position));

		lerp.usingRectTransform = false;
		lerp.isLocal = false;
		lerp.slerpingIsActive = false;

		m_pointingHand.SetActive(true);

		lerp.singleStepDuration = 0.5f;

		ChangeState();
	}

	IEnumerator StartRestingAnimation()
	{
		restingCoroutine = true;
		lerp.ResetAll();
		int target = currPointingIndex;
		
		Vector3 currDirection = (m_arrStimulusGO[target].stimulusGO.transform.position - m_pointingHand.transform.position).normalized;
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position + currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position - currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position + currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position - currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position + currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position - currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position + currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.AddPosition(m_pointingHand.transform.position - currDirection * 0.2f);
		lerp.AddPosition(m_pointingHand.transform.position);
		lerp.singleStepDuration = 0.25f;
		lerp.slerpingIsActive = false;

		yield return new WaitForSeconds(1.5f);

		lerp.StartAnimation();
		restingCoroutine = false;
	}

	//----------------------------------------------------------------------------------------------------
	// PlayAudio: play target audio 
	//----------------------------------------------------------------------------------------------------
	public float PlayAudio(float fDelay = 0)
	{
		// stop coroutine "WaitIncorrect" when user presses the "repeat" button or audio is auto played after a pause
		if (m_bIsCoroutineIncorrectRunning)
			StopCoroutine("WaitIncorrect"); 
		
		//string strAudio = "Audio/" + m_lsTrial [m_intCurIdx].m_strTargetAudio;
		//string strFolder = CTrialList.Instance.getCurAudioFolder ();
		string strAudio = "Audio/N2_MALE/" + m_curTrial.m_strTargetAudio;
		
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
			
			clip = Resources.Load(strAudio) as AudioClip;
			
			m_sound_manager.Play(clip,ChannelType.VoiceText,aci);
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
			strAudio = "Audio/snd_correct";
		else
			strAudio = "Audio/snd_wrong";
		
		AudioClipInfo aci;
		aci.delayAtStart = 0.0f;
		aci.isLoop = false;
		aci.useDefaultDBLevel = true;
		aci.clipTag = string.Empty;
		
		m_sound_manager.Play(Resources.Load(strAudio) as AudioClip,ChannelType.SoundFx,aci);	
		
	}
}
