using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using ListenIn;
using MadLevelManager;

public class StatePinball : State
{
    #region singleton
    private static readonly StatePinball instance = new StatePinball();
    public static StatePinball Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    public int ID;
	public GameObject go;
	public PinballMono m_PinballMono;
    public LevelDifficulty currDifficulty;
	public Chapter thisChapter;
	public DateTime startGame;
	public DateTime endGame;
    public bool initialize = false;

	private void SetBucketPositions()
	{
		//THIS IS MEANT JUST FOR THE TUTORIAL
		List<int> intList = new List<int>();

		for (int i = 0; i < thisChapter.JigsawPeicesUnlocked.Length; i++)
		{
			if (thisChapter.JigsawPeicesUnlocked[i] != 1)
			{
				intList.Add(i);
			}
		}

        //THERE WILL BE JUST A VALUE
        m_PinballMono.Buckets[2]
        .Init(intList[0], thisChapter.JigsawPeicesUnlocked[intList[0]], ID + 1, null);
        //m_PinballMono.Buckets[2]
		//.Init(intList[0], StateChapterSelect.Instance.Chapters[ID].JigsawPeicesUnlocked[intList[0]], ID + 1);

		int div = intList[0] / 4;
		int mod = intList[0] % 4;
		string prefabstring = "Prefabs/JigsawPieces/" + mod + "_" + div;
		
		GameObject jigsawPieceGO = GameObject.Instantiate(Resources.Load(prefabstring)) as GameObject;
		RectTransform jigsawPieceTransform = jigsawPieceGO.GetComponent<RectTransform>();
		
		if (jigsawPieceTransform.childCount != 0)
		{
			Image img = jigsawPieceTransform.GetChild(0).GetComponent<Image>();
			if (img != null)
			{
				img.sprite = Resources.Load<Sprite>("PinballBackgrounds/" + thisChapter.nextLvlPictureName);
			}
		}
		
		jigsawPieceTransform.SetParent(m_PinballMono.JigsawParents[2].transform,false);
		//jigsawPieceTransform.parent = m_PinballMono.JigsawParents[BucketsShuffled[i]].transform;
		jigsawPieceTransform.localPosition = Vector2.zero;//m_PinballMono.JigsawParents[BucketsShuffled[i]].GetComponent<RectTransform>().localPosition;
		jigsawPieceTransform.localScale = Vector2.one * 0.015f;
	}
	
	private void RandomizeBucketPosition()
	{

        Debug.Log(String.Format("StatePinball: Randomizing jigsaw pieces"));
		List<int> intList = new List<int>();
		
		//TODO control the game logic differely, don t like to return like this.
		for (int i = 0; i < thisChapter.JigsawPeicesUnlocked.Length; i++)
		{
			if (thisChapter.JigsawPeicesUnlocked[i] != 1)
			{
				intList.Add(i);
			}
		}
		while (intList.Count > 5)
		{
			intList.RemoveAt(UnityEngine.Random.Range(0, intList.Count - 1));
		}
		//intList now ordered list of 5 elements or less
		
		int[] BucketsShuffled;
		
		{
			List<int> BucketIs = new List<int>(new int[] { 0, 1, 2, 3, 4 });
			BucketsShuffled = new int[BucketIs.Count];
			
			int count = BucketIs.Count;
			for (int i = 0; i < count; i++)
			{
				int iToRemove = UnityEngine.Random.Range(0, BucketIs.Count);
                //ListenIn.Logger.Instance.Log(String.Format("Jigsaw Removed : [ {0}  |  {1} ]", iToRemove, BucketIs[iToRemove]), LoggerMessageType.Info);
                //Debug.Log("Jigsae Removed : [" + iToRemove + "|"+BucketIs[iToRemove]+"];");
				BucketsShuffled[i] = BucketIs[iToRemove];
				BucketIs.RemoveAt(iToRemove);
			}
		}
		//BucketsShuffled now randomised array of 1 through 5
		
		for (int i = 0; i < intList.Count; i++)
		{
			
			//m_PinballMono.Buckets[BucketsShuffled[i]].JigsawPeiceToUnlock = intList[i];
			//m_PinballMono.Buckets[BucketsShuffled[i]].LevelToUnlock = ID + 1;
					
			int div = intList[i] / 4;
			int mod = intList[i] % 4;
			string prefabstring = "Prefabs/JigsawPieces/" + mod + "_" + div;

            //ListenIn.Logger.Instance.Log(String.Format(" {0} | Shuffle: [ {1} ]; Piece : [ {2} ]; Jigsaw Prefab Path : [ {3} ] ", i, BucketsShuffled[i], intList[i], prefabstring), LoggerMessageType.Info);
            //Debug.Log(i + "| Shuffle : [" + BucketsShuffled[i] + "]; Piece : [" + intList[i] + "]; Jigsaw Prefab Path : [" + prefabstring + "];");
			
			GameObject jigsawPieceGO = GameObject.Instantiate(Resources.Load(prefabstring)) as GameObject;
			RectTransform jigsawPieceTransform = jigsawPieceGO.GetComponent<RectTransform>();
			
			if (jigsawPieceTransform.childCount != 0)
			{
				Image img = jigsawPieceTransform.GetChild(0).GetComponent<Image>();
				if (img != null)
				{
					img.sprite = Resources.Load<Sprite>("PinballBackgrounds/" + thisChapter.nextLvlPictureName);
				}
			}
			
			jigsawPieceTransform.SetParent(m_PinballMono.JigsawParents[BucketsShuffled[i]].transform,false);
			//jigsawPieceTransform.parent = m_PinballMono.JigsawParents[BucketsShuffled[i]].transform;
			jigsawPieceTransform.localPosition = Vector2.zero;//m_PinballMono.JigsawParents[BucketsShuffled[i]].GetComponent<RectTransform>().localPosition;
			jigsawPieceTransform.localScale = Vector2.one * 0.015f;

            Image currFiller = jigsawPieceTransform.FindChild("Progression").gameObject.GetComponent<Image>();

            //Initialize cardbucketcontroller
            m_PinballMono.Buckets[BucketsShuffled[i]].Init(intList[i], thisChapter.JigsawPeicesUnlocked[intList[i]], ID, currFiller);

        }
	}
	
	// Use this for initialization
	public override void Init()
	{
        //Debug.Log("Enteering Pinball state");
        if (go == null)
        {
            go = GameObject.FindGameObjectWithTag("PinballPrefab");
        }

        if (go == null)
        {
            ListenIn.Logger.Instance.Log("Pinball gameobject not found", LoggerMessageType.Error);
            //Debug.LogError("Pinball state not found");
        }

        //Avoid this being called twice
        if (go != null && !initialize)
        {
            //Getting level from resources
            string currLevel = String.Concat("LevelPrefabs/",MadLevel.currentLevelName);
            ListenIn.Logger.Instance.Log(String.Format("StatePinball: loading {0}", currLevel), LoggerMessageType.Info);
            GameObject loadLevel = GameObject.Instantiate(Resources.Load(currLevel, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            loadLevel.transform.SetParent(go.transform, true);
            loadLevel.transform.SetAsFirstSibling();
            //go = GameObject.Instantiate(Resources.Load("Prefabs/States/Pinball")) as GameObject;

            initialize = true;
            thisChapter = StateJigsawPuzzle.Instance.CurrChapter;//StateChapterSelect.Instance.Chapters[ID];
            			
            m_PinballMono = go.GetComponent<PinballMono>();
            m_PinballMono.SetEarnedCoins();
            //m_PinballMono.Levels[ID].SetActive(true);

            //Should never be zero since tutorialis being removed
            if (ID == 0)
            {
                ListenIn.Logger.Instance.Log("ID zero as tutorial level is deprecated", LoggerMessageType.Error);
                //Debug.LogError("ID zero as tutorial level is deprecated");
                //SetBucketPositions();
            }
            else
            {
                RandomizeBucketPosition();
            }

        }

        //ILevel lvlManager = m_PinballMono.Levels[ID].GetComponent<ILevel>();

        ILevel lvlManager = m_PinballMono.transform.GetChild(0).gameObject.GetComponent<ILevel>();

        lvlManager.currDifficulty = this.currDifficulty;

        DatabaseXML.Instance.SetTimerState(DatabaseXML.TimerType.Pinball, true);

    }
	
	public void ExitLevelPinball()
	{
		m_PinballMono.FireButton.SetActive(false);
		m_PinballMono.UIHolder.SetActive(false);
		m_PinballMono.SetCannonState(false);

        ILevel lvlManager = m_PinballMono.transform.GetChild(0).gameObject.GetComponent<ILevel>();
        //ILevel lvlManager = m_PinballMono.Levels[ID].GetComponent<ILevel>();
        lvlManager.endingLevel = true;
		Camera.main.GetComponent<SoundManager>().Stop(ChannelType.LevelMusic);
		Camera.main.GetComponent<SoundManager>().Stop(ChannelType.LevelEffects);
		endGame = DateTime.UtcNow;

        // insert db entry
        TimeSpan span = endGame - startGame;
        double dGameTimeMin = Math.Round(span.TotalMinutes, 4);

        Debug.Log("StatePinball: ExitLevelPinball() adding game_time_insert query");

        Dictionary<string, string> time_insert = new Dictionary<string, string>();
        time_insert.Add("patientid", DatabaseXML.Instance.PatientId.ToString());
        time_insert.Add("date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        time_insert.Add("totaltime", dGameTimeMin.ToString());

        DatabaseXML.Instance.WriteDatabaseXML(time_insert, DatabaseXML.Instance.game_time_insert);

        //Andrea: 30/10 move it for later

        //if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        //{
        //    //read the xml
        //    DatabaseXML.Instance.ReadDatabaseXML();
        //}
    }

	public void InitLevelPinball()
	{
        //Debug.Log("");
        DatabaseXML.Instance.ResetTimer(DatabaseXML.TimerType.Idle);
        m_PinballMono.UIHolder.SetActive(true);
        //m_PinballMono.FireButton.SetActive(true);
        m_PinballMono.Frame.SetActive(true);
        m_PinballMono.BucketsHolder.SetActive(true);
        m_PinballMono.SetSpwanerTriggerState(true);
        m_PinballMono.SetCannonState(true);

        rotatingCogs[] cogs = m_PinballMono.Cannon.GetComponentsInChildren<rotatingCogs>();
        for (int i = 0; i < cogs.Length; i++)
        {
            cogs[i].enabled = true;
        }

        ILevel lvlManager = m_PinballMono.transform.GetChild(0).gameObject.GetComponent<ILevel>();
        //ILevel lvlManager = m_PinballMono.Levels[ID].GetComponent<ILevel>();

        if (lvlManager != null)
		{
			lvlManager.startingLevel = true;
            ListenIn.Logger.Instance.Log(String.Format("StatePinball: InitLevelPinball() initializing {0} pinball level",lvlManager.name), LoggerMessageType.Info);
        }
		else
		{
            ListenIn.Logger.Instance.Log("StatePinball: InitLevelPinball() missing level manager", LoggerMessageType.Warning);
            //Debug.LogWarning("Missing level manager");
		}
		startGame = endGame = DateTime.UtcNow;
	}
	
    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Exit()
    {
        ListenIn.Logger.Instance.Log("StatePinball: Exit() exiting pinball level", LoggerMessageType.Info);
        //Debug.Log("Exiting Pinball state"); 

		//preparing the scene by removing elements that will create problems.
		ExitLevelPinball();
        //Create a copy of the earned objects
		m_PinballMono.DuplicateEarnedJigsaw();
		m_PinballMono.DisableOriginals();
        m_PinballMono.SetToAlphaFading(1.0f, true, false);
        //m_PinballMono.SetFade();

        //UnityEngine.Object.Destroy(m_PinballMono.gameObject);
        //ListenIn.Logger.Instance.Log(String.Format("Mono : [ {0} ]; Name : [ {1} ]" ,m_PinballMono, m_PinballMono.name), LoggerMessageType.Info);
        //Debug.Log("Mono : [" + m_PinballMono + "]; Name : [" + m_PinballMono.name + "];");
		
		StateChallenge.Instance.ResetCoins();
        StateChallenge.Instance.ResetCorrectAnswers();
        StateChallenge.Instance.ResetQuestions();

        if (!thisChapter.Completed)
        {
            thisChapter.CurrPlayedTime += endGame - startGame;
        }

        //Debug.Log(thisChapter.CurrPlayedTime.ToString());
	}
}
