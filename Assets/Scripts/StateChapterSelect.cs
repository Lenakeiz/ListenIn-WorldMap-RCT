//#define CHECK_STATECHAPTERSELECT_USENESS

using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Chapter
{
    public ChapterIconMono Mono;
    public string Name;

	public string TitleImage;
	public Color TitleColor;

    public int LevelNumber;
    public string PictureName;
	public string nextLvlPictureName;

	public float[] JigsawPeicesUnlocked = new float[12];
	public bool[] JigsawPiecesToUnlock = new bool[12];

    public bool UnLocked = false;
	public bool Completed = false;

	public int cheatCount = 0;

	public bool autoScrollOnComplete = false;

	public TimeSpan CurrPlayedTime;

    public int UnlockPerc
    {
        get
        {
            int count=0;
            for (int i = 0; i < JigsawPeicesUnlocked.Length; i++)
            {
                if (JigsawPeicesUnlocked[i] == 1)
                {
                    count += 1;
                }
            }
            float perc = ((float)count / (float)JigsawPeicesUnlocked.Length) * 100f;
            Debug.Log("Percentage : [" + perc + "]; Count : [" + count+"];");
            return (int)perc;
        }
    }

    public Chapter(string name, int levelNumber, string picture, string nextLvlPicture, Color? titleColor, bool unLocked = false, string titleImage = "")
    {
        Name = name;
		TitleImage = titleImage;
		TitleColor = titleColor == null ? Color.green : titleColor.Value;
        LevelNumber = levelNumber;
        PictureName = picture;
		nextLvlPictureName = nextLvlPicture;

        UnLocked = unLocked;
		Completed = false;

		//Initializes variables
		for (int i = 0; i < JigsawPiecesToUnlock.Length; i++) {
			JigsawPiecesToUnlock[i] = false;
		}
    }

    public bool AllPiecesUnlocked()
    {
        //OH MY GOD
        for (int i = 0; i < JigsawPeicesUnlocked.Length; i++)
        {
            if (!Mathf.Approximately(JigsawPeicesUnlocked[i],1.0f))
            {
                return false;
            }
        }
        return true;
    }

	public ChapterSaveState ToChapterSaveState()
	{
		ChapterSaveState state =  new ChapterSaveState();

		state.LevelNumber = this.LevelNumber;

		for (int i = 0; i < this.JigsawPeicesUnlocked.Length; i++) {
			state.JigsawPeicesUnlocked[i] = this.JigsawPeicesUnlocked[i];
		}

		return state;
	}
}


public class StateChapterSelect : State
{

    #region singleton
    private static readonly StateChapterSelect instance = new StateChapterSelect();
    public static StateChapterSelect Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
#if CHECK_STATECHAPTERSELECT_USENESS
    private Chapter[] m_Chapters = new Chapter[]{
		new Chapter("*Tutorial*", 0,"Background_004_deprecated", "Background_009", new Color(1f, 1f, 1f),true, ""),

		new Chapter("Candy Madness",   1,"Background_009", "Background_002", new Color32(255,0,162,255),false, ""),
		new Chapter("Space Travel",    2,"Background_002", "Background_008", Color.black, false, ""),
		new Chapter("Cheeky Jungle",   3,"Background_008", "Background_011", Color.red ,false, ""),
		new Chapter("Cookie Monster",  4,"Background_011", "Background_014", new Color32(255,221,0,255),false, ""),
		new Chapter("Tricky Lights",   5,"Background_014", "Background_015", Color.black ,false, ""),
		new Chapter("Tibet",           6, "Background_015", "Background_003", Color.white ,false, ""),		
		new Chapter("Halloween",       7,"Background_003", "Background_016", new Color(0.686275f, 0.933333f, 0.933333f),false, ""),
		new Chapter("Fun Fair",        8,"Background_016", "Background_004", Color.white ,false, ""),
		new Chapter("Far Islands",     9,"Background_004", "Background_005", Color.yellow, false, ""),
		new Chapter("Great Canyon",    10,"Background_005", "Background_006", new Color(0.498039f, 1f, 0.831373f),false, ""),
		new Chapter("Underworld",      11,"Background_006", "Background_007", Color.cyan ,false, ""),
		new Chapter("Artic View",      12,"Background_007", "Background_010", Color.green ,false, ""),
		new Chapter("Wonder Land",     13,"Background_010", "Background_012", new Color32(36,145,0,255),false, ""),

		new Chapter("Fuji Mountain",   14,"Background_012", "Background_013", new Color32(255,0,221,255),false, ""),
		new Chapter("Moscow",          15,"Background_013", "Background_017", new Color32(175,0,0,255),false, ""),

		new Chapter("Dodgeball",       16,"Background_017", "Background_018", Color.blue ,false, ""),
		new Chapter("Great Coliseum",  17,"Background_018", "Background_019", Color.magenta ,false, ""),
		new Chapter("Egypt",           18,"Background_019", "Background_021", Color.yellow, false, ""),
		new Chapter("Fearless Ski",    19,"Background_021", "Background_001", Color.blue, false, ""),
     	new Chapter("Pencil Board",    20,"Background_001", "Background_004_deprecated", new Color(0f, 0f, 0.803922f),false, ""),

    };
    public Chapter[] Chapters { get { return m_Chapters; } }

    ChapterSelectMono m_Mono;

    private bool m_Init = false;

    private bool /*This really*/ m_Dragging;
    private bool m_IsScrolling;
    private Vector2 m_ScrollTo;

    private RectTransform m_ContentRect;

    private float m_Intertia;
    private int m_LastChecked = 0; //this mark the chapter that we are currently playing

	private GameObject jigsawUnlocked;
	private List<GameObject> lerpers = new List<GameObject>();
	private ParticleSystem endSystemEffect;
	private bool activateJigsawAnimation;
	private bool animating;
	private bool rewardAnimation;

	private GameObject stateChapterSelectPrefab;

	private void CleanLerpers()
	{
		foreach (var lerp in lerpers) {
			GameObject.Destroy(lerp);
		}
		lerpers.Clear();
	}

	private void InitializeChapters(){

		for (int i = 0; i < m_Chapters.Length; i++)
		{
			GameObject chapterGO = GameObject.Instantiate(Resources.Load("Prefabs/Chapter")) as GameObject;

			//Loading Chapter Select
			m_Chapters[i].Mono = chapterGO.GetComponent<ChapterIconMono>();
			m_Chapters[i].Mono.transform.SetParent(m_Mono.Grid.transform);
			
			m_Chapters[i].Mono.transform.localScale = Vector2.one;
			
			//Setting current level number
			m_Chapters[i].Mono.ID = m_Chapters[i].LevelNumber;

			//Setting Images
			m_Chapters[i].Mono.Image.sprite = Resources.Load<Sprite>("PinballBackgrounds/" + m_Chapters[i].PictureName);
			m_Chapters[i].Mono.m_nextLvl_Image = Resources.Load<Sprite>("PinballBackgrounds/" + m_Chapters[i].nextLvlPictureName);
			m_Chapters[i].Mono.m_nextLvl_Image_small = Resources.Load<Sprite>("PinballBackgrounds/" + m_Chapters[i].nextLvlPictureName + "_small");
			m_Chapters[i].Mono.Completed_Image.sprite = Resources.Load<Sprite>("PinballBackgrounds/" + m_Chapters[i].nextLvlPictureName);

			//Setting Name
			m_Chapters[i].Mono.Title.text = m_Chapters[i].Name;
			
			{
				m_Chapters[i].Mono.Title.GetComponent<Text>().color = m_Chapters[i].TitleColor;
			}
						
			if(m_Chapters[i].UnLocked)
			{
				m_Chapters[i].Mono.PlayButton.GetComponent<Image>().sprite = m_Chapters[i].Mono.PlaySprite;
				m_Chapters[i].Mono.PlayButton.GetComponent<PlayButtonAnimator>().ActivePlay =  true;
			}
			else
			{
				m_Chapters[i].Mono.PlayButton.GetComponent<Image>().sprite = m_Chapters[i].Mono.PlayLockedSprite;
			}
			
			if(m_Chapters[i].Completed)
			{				
				m_Chapters[i].Mono.JigsawPieceRoot.SetActive(false);
				m_Chapters[i].Mono.Completed_Image.gameObject.SetActive(true);
				if(m_Chapters[i].Mono.ID != 0)
					m_Chapters[i].Mono.PlayButton.SetActive(true);
			}
			else
			{
				m_Chapters[i].Mono.JigsawPieceRoot.SetActive(true);
				m_Chapters[i].Mono.Completed_Image.gameObject.SetActive(false);
				
				for (int j = 0; j < m_Chapters[i].JigsawPeicesUnlocked.Length; j++)
				{
					m_Chapters[i].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().sprite = m_Chapters[i].Mono.m_nextLvl_Image;
					Color temp = m_Chapters[i].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().color;
					temp.a = 0.4f;
					
					{
						if(m_Chapters[i].JigsawPeicesUnlocked[j] == 1.0f && !m_Chapters[i].JigsawPiecesToUnlock[j])
						{
							temp.a = 1.0f;
						}
					}
					
					m_Chapters[i].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().color = temp;
					
				}
			}
		}
	}

	private void GameInitialization(){

		stateChapterSelectPrefab = GameObject.Instantiate(Resources.Load("Prefabs/States/ChapterSelect")) as GameObject;
		stateChapterSelectPrefab.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>() as Camera;
		m_Mono = stateChapterSelectPrefab.GetComponent<ChapterSelectMono>();

		InitializeChapters();
		
		float preferred = m_Mono.Grid.cellSize.x * m_Chapters.Length;
		
		m_ContentRect = m_Mono.Grid.GetComponent<RectTransform>();
		
		m_Intertia = m_Mono.ScrollRect.decelerationRate;
		m_Mono.ScrollRect.inertia = false;

	}

#endif
	// Use this for initialization
	public override void Init () {
#if CHECK_STATECHAPTERSELECT_USENESS
        if(!m_Init)
		{
			try {
				GameStateSaver.Instance.LoadGameProgress();
			} catch (System.Exception ex) {

				Debug.LogError(ex.Message);
				GameStateSaver.Instance.ResetListenIn();			
			}

			GameInitialization();

			m_Init = true;
		}

		if(lerpers.Count != 0)
		{
			CleanLerpers();
		}

		//Resetting animation variables
        activateJigsawAnimation = false;
		animating = false;
		rewardAnimation = false;
		endSystemEffect = null;
		
		stateChapterSelectPrefab.SetActive(true);

		float gridSnap = getGridPosAt((m_Mono.Grid.transform.childCount-1) - m_LastChecked);		
		m_ScrollTo = new Vector2(gridSnap, m_Mono.Grid.transform.localPosition.y);

		m_Mono.Grid.transform.localPosition = m_ScrollTo;

		//Unlocking arrows
		int nearest = NearestChild();
		m_Mono.LeftArrow.gameObject.SetActive(nearest < m_Mono.Grid.transform.childCount - 1);
		m_Mono.RightArrow.gameObject.SetActive(nearest > 0);

		jigsawUnlocked = GameObject.Find("JiggsawEarned(Clone)");
		if(jigsawUnlocked != null && jigsawUnlocked.transform.childCount != 0)
		{
			activateJigsawAnimation = true;
			m_Chapters[m_LastChecked].Mono.PlayButton.SetActive(false);
		}
#endif
	}
#if CHECK_STATECHAPTERSELECT_USENESS
    //TODO not used anymore
    public void CheckScrollRight()
	{
		//Trying to scroll to right automatically
		bool scroll = true;

		for (int j = 0; j < m_Chapters[StatePinball.Instance.ID].JigsawPeicesUnlocked.Length; j++)
		{
			if(m_Chapters[StatePinball.Instance.ID].JigsawPeicesUnlocked[j] != 1.0f)
			{
				scroll = false;
				break;
			}
		}
		if(scroll && !m_Chapters[StatePinball.Instance.ID].autoScrollOnComplete)
		{
			m_Chapters[StatePinball.Instance.ID].autoScrollOnComplete = true;
			RightArrow();
		}
	}

	bool CheckLerperAnimationEnded()
	{
		if(lerpers.Count == 0)
			return false;
		
		foreach (GameObject item in lerpers) {
			if(!item.GetComponent<TransformLerper>().animationEnded)
			{
				return false;
			}
		}
		return true;
	}

	private void StartAnimation()
	{
		for (int i = 0; i < jigsawUnlocked.transform.childCount; i++) {
			GameObject jigCanvas = jigsawUnlocked.transform.GetChild(i).gameObject;

			GameObject lerp = new GameObject("Lerper");
			TransformLerper tr = lerp.AddComponent<TransformLerper>();
			tr.Init();
			tr.usingRectTransform = true;
			tr.isLocal = true;
			tr.slerpingIsActive = true;
			tr.singleStepDuration = 0.5f;
			tr.ObjectToLerp = jigCanvas.GetComponentInChildren<Canvas>().gameObject.transform.GetChild(0).gameObject;

			RectTransform jig = jigCanvas.GetComponentInChildren<Canvas>().gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
			tr.AddPosition (jig.localPosition);
			tr.AddScaler(jig.localScale);
			Vector3 finalPos = m_Chapters[m_LastChecked].Mono.JigsawPieces[jigCanvas.GetComponentInChildren<JigsawInfo>().Index].GetComponent<RectTransform>().localPosition;
			tr.AddPosition(finalPos);
			tr.AddScaler(new Vector3(1.0f,1.0f));

			tr.StartAnimation();

			lerpers.Add(lerp);
		}
	}

	private void FinishJigsawTransition()
	{
		for (int j = 0; j < m_Chapters[m_LastChecked].JigsawPiecesToUnlock.Length; j++)
		{
			if(m_Chapters[m_LastChecked].JigsawPiecesToUnlock[j])
			{
				m_Chapters[m_LastChecked].JigsawPiecesToUnlock[j] = false;
				m_Chapters[m_LastChecked].JigsawPeicesUnlocked[j] = 1.0f;
				m_Chapters[m_LastChecked].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().sprite = m_Chapters[m_LastChecked].Mono.m_nextLvl_Image;
				Color tempColor = m_Chapters[m_LastChecked].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().color;
				tempColor.a = 1.0f;
				m_Chapters[m_LastChecked].Mono.JigsawPieces[j].transform.GetChild(0).GetComponent<Image>().color = tempColor;
			}
		}

		UnityEngine.Object.Destroy(jigsawUnlocked);

		if(m_Chapters[m_LastChecked].AllPiecesUnlocked())
		{
			m_Chapters[m_LastChecked].Mono.PlayButton.SetActive(false);
			m_Mono.EndPuzzleEffect.SetActive(true);
			m_Mono.EndPuzzleEffect.GetComponent<Animator>().SetTrigger("StartTrophyAnim");

			GameObject.Find("Main Camera").GetComponent<SoundManager>().Play("TrophyWin");

			endSystemEffect = GameObject.FindGameObjectWithTag("ParticleSystemEnd").GetComponent<ParticleSystem>();

			rewardAnimation = true;

		}
		else
		{
			m_Chapters[m_LastChecked].Mono.PlayButton.SetActive(true);
		}
	}

	//this is used in combination with the background image animator and EndRewardAnimation function
	public void SwapBackgroundImage()
	{
		m_Chapters[m_LastChecked].Mono.JigsawPieceRoot.SetActive(false);
		m_Chapters[m_LastChecked].Mono.Completed_Image.gameObject.SetActive(true);
	}

	private void EndRewardAnimation()
	{
		m_Chapters[m_LastChecked].Completed = true;
		m_Mono.EndPuzzleEffect.SetActive(false);

		if(m_Chapters[m_LastChecked].Mono.ID != 0)
		{
			m_Chapters[m_LastChecked].Mono.PlayButton.SetActive(true);
		}

		if(m_LastChecked + 1 < m_Chapters.Length)
		{
			m_Chapters[m_LastChecked + 1].UnLocked = true;
			m_Chapters[m_LastChecked + 1].Mono.PlayButton.GetComponent<Image>().sprite = m_Chapters[m_LastChecked + 1].Mono.PlaySprite;
			m_Chapters[m_LastChecked + 1].Mono.PlayButton.GetComponent<PlayButtonAnimator>().ActivePlay =  true;
			//automatially scrolling to the right
			RightArrow();
		}

	}
#endif
	// Update is called once per frame
    public override void Update()
    {
#if CHECK_STATECHAPTERSELECT_USENESS

        if (activateJigsawAnimation)
		{
			activateJigsawAnimation = false;
			animating = true;
			m_Mono.LeftArrow.gameObject.SetActive(false);
			m_Mono.RightArrow.gameObject.SetActive(false);
	        StartAnimation();
		}
		else if (!m_Dragging && !animating && !rewardAnimation)
        {
            if (m_IsScrolling)
            {
                m_Mono.Grid.transform.localPosition = Vector2.Lerp(m_Mono.Grid.transform.localPosition, m_ScrollTo, m_Intertia);
            }
            else
            {
                float dist = Mathf.Infinity;
                float snapTo = 0;

                for (int i = 0; i < m_Mono.Grid.transform.childCount; i++)
                {
                    float gridSnap = getGridPosAt(i);
                    float curDist = Mathf.Abs(m_Mono.Grid.transform.localPosition.x - gridSnap);
                    if (curDist < dist)
                    {
                        dist = curDist;
                        snapTo = gridSnap;
                    }
                }
                Vector2 newPos = new Vector2(snapTo, m_Mono.Grid.transform.localPosition.y);
                m_Mono.Grid.transform.localPosition = Vector2.Lerp(m_Mono.Grid.transform.localPosition, newPos, m_Intertia);
            }
			int nearest = NearestChild();
			m_Mono.LeftArrow.gameObject.SetActive(nearest < m_Mono.Grid.transform.childCount - 1);
			m_Mono.RightArrow.gameObject.SetActive(nearest > 0);
        }

		if(animating && CheckLerperAnimationEnded())
		{
			FinishJigsawTransition();
			CleanLerpers();
			animating = false;
		}
		else if(endSystemEffect != null && rewardAnimation && !endSystemEffect.isPlaying)
		{
			animating = false;
			EndRewardAnimation();
			rewardAnimation = false;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameStateSaver.Instance.SaveGameProgress();
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			GameStateSaver.Instance.LoadGameProgress();
		}
			
#endif
    }
#if CHECK_STATECHAPTERSELECT_USENESS
    private float getGridPosAt(int i ){
        float width = m_Mono.Grid.cellSize.x;
        float pos = ((float)width * i) - ((float)width * ((float)(m_Mono.Grid.transform.childCount - 1) * 0.5f));
        return pos;
    }

    private int NearestChild()
    {
        float dist = Mathf.Infinity;
        float width = m_Mono.Grid.cellSize.x;
        int valI = -1;
        for (int i = 0; i < m_Mono.Grid.transform.childCount; i++)
        {
            float gridSnap = ((float)width * i) - ((float)width * ((float)(m_Mono.Grid.transform.childCount - 1) * 0.5f));
            float curDist = Mathf.Abs(m_Mono.Grid.transform.localPosition.x - gridSnap);
            if (curDist < dist)
            {
                dist = curDist;
                valI = i;
            }
        }

        //Debug.Log("Nearest Child is : [" + valI + "];");
        return valI;
    }

    private void ScrollToPos(int i){
        float gridSnap = getGridPosAt(i);

        m_ScrollTo = new Vector2(gridSnap, m_Mono.Grid.transform.localPosition.y);

        m_IsScrolling = true;
    }

    public void LeftArrow()
    {
        int nearest = NearestChild();
        if (nearest >= m_Mono.Grid.transform.childCount-1)
        {
            Debug.Log("LEFT : At End");
            return;
        }
        ScrollToPos(nearest + 1);
    }

    public void RightArrow()
    {
        int nearest = NearestChild();
        if (nearest <= 0)
        {
            Debug.Log("RIGHT : At End");
            return;
        }
        ScrollToPos(nearest - 1);
    }

    public void Drag()
    {
        m_Dragging = true;
        m_IsScrolling = false;
    }

    public void DragEnd()
    {
        m_Dragging = false;
    }
#endif
    public override void Exit()
    {
#if CHECK_STATECHAPTERSELECT_USENESS
        //UnityEngine.Object.Destroy(m_Mono.gameObject);
        stateChapterSelectPrefab.SetActive(false);
#endif
    }
#if CHECK_STATECHAPTERSELECT_USENESS
    public void PressButton(int ID)
    {
		if(animating)
		{
			return;	
		}

        if (m_Chapters[ID].UnLocked)
        {
			m_LastChecked = m_Chapters[ID].LevelNumber;
			StatePinball.Instance.ID = m_Chapters[ID].LevelNumber;
			GameController.Instance.ChangeState(GameController.States.StateChallenge);
        }
		else
		{
			m_Chapters[ID].cheatCount++;
			if(m_Chapters[ID].cheatCount > 8)
			{
				m_Chapters[ID].UnLocked = true;
				m_Chapters[ID].Mono.PlayButton.GetComponent<Image>().sprite = m_Chapters[ID].Mono.PlaySprite;
				m_Chapters[ID].Mono.PlayButton.GetComponent<PlayButtonAnimator>().ActivePlay =  true;
				//m_Chapters[ID].Mono.PlayText.text = "Play";
				m_Chapters[ID].cheatCount = 0;
			}
		}
	}
#endif
}