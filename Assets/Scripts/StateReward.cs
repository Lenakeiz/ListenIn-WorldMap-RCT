using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StateReward : State
{
    #region singleton
    private static readonly StateReward instance = new StateReward();
    public static StateReward Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

	GameObject m_splines;
	GameObject m_jigsaw_earned;
	GameObject pinballLerper;
	List<GameObject> jigsawLerpers = new List<GameObject>();
	private bool fadeChapterSelect;
	private bool jigsawDisappear;
	private bool jigsawAppear;

    // Use this for initialization
    public override void Init()
    {
		GameObject splineMovements = Resources.Load("Prefabs/States/RewardAnimation") as GameObject;
		m_splines = GameObject.Instantiate(splineMovements);
		GameObject JiggsawEarned = Resources.Load("Prefabs/JiggsawEarned") as GameObject;
		m_jigsaw_earned = GameObject.Instantiate(JiggsawEarned);

		//Praparing the outro of pinball State:
		pinballLerper = new GameObject("PinballLerper");
		TransformLerper trLerp = pinballLerper.AddComponent<TransformLerper>();
        GameObject toLerp = GameObject.FindGameObjectWithTag("PinballPrefab");
		trLerp.Init();
		trLerp.ObjectToLerp = toLerp;
		trLerp.AddPosition(toLerp.transform.position);
		float fScreenHeight = Screen.height;
		float fScreenCenterWidth = Screen.width/2;
		Vector3 finalPosition = toLerp.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(fScreenCenterWidth,fScreenHeight * 2));
		trLerp.AddPosition(finalPosition);
		trLerp.singleStepDuration = 3.0f;
		trLerp.StartAnimation();

		foreach (var gameObj in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]) {

			if(gameObj.name.Equals("JigsawPlaceholder(Clone)"))
			{
				gameObj.transform.SetParent(m_jigsaw_earned.transform,true);
				gameObj.AddComponent<SplineInterpolator>();
				SplineController sp = gameObj.AddComponent<SplineController>() as SplineController;
				sp.Duration = 3.0f;
				sp.WrapMode = eWrapMode.ONCE;
				sp.AutoClose = false;
				sp.HideOnExecute = false;
				int movementIndex = gameObj.GetComponent<JigsawInfo>().BucketIndex;
				sp.SplineRoot = m_splines.transform.GetChild(movementIndex).gameObject;
				sp.AutoStart = true;
				sp.ExecuteMotion();
			}

		}

		m_splines.SetActive(false);

		fadeChapterSelect = false;
		jigsawDisappear = false;
		jigsawAppear = false;

		jigsawLerpers.Clear ();
    }

	//not used
	void SetEarnedJigsawDisappear()
	{
		jigsawLerpers.Clear();
		for (int i = 0; i < m_jigsaw_earned.transform.childCount; i++) {
			GameObject jigCanvas = m_jigsaw_earned.transform.GetChild(i).gameObject;
			GameObject lerp = new GameObject("LerpScalerDown");

			TransformLerper tr = lerp.AddComponent<TransformLerper>();
			tr.Init();
			tr.usingRectTransform = false;
			tr.singleStepDuration = 2.0f;
			tr.ObjectToLerp = jigCanvas;
			tr.AddScaler(jigCanvas.transform.localScale);
			tr.AddScaler(new Vector3(0.01f,0.01f));
			tr.StartAnimation();

			jigsawLerpers.Add(lerp);
		}
		jigsawAppear = true;
	}

	bool CheckLerperAnimationEnded()
	{
		if(jigsawLerpers.Count == 0)
			return false;

		foreach (GameObject item in jigsawLerpers) {
			if(!item.GetComponent<TransformLerper>().animationEnded)
			{
				return false;
			}
		}
		return true;
	}

	void SetEarnedJigsawToCanvas()
	{
		if(jigsawLerpers.Count!=0)
			CleanUpJigsawLerpers();

		for (int i = 0; i < m_jigsaw_earned.transform.childCount; i++) {

			GameObject jigCanvas = m_jigsaw_earned.transform.GetChild(i).gameObject;
			jigCanvas.transform.position = Vector3.zero;
			jigCanvas.GetComponentInChildren<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			jigCanvas.GetComponentInChildren<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

			RectTransform jig = jigCanvas.GetComponentInChildren<Canvas>().gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

			jigCanvas.transform.position = Vector3.zero;
			jigCanvas.transform.localScale = Vector3.one;
			jig.localPosition = new Vector3(340,200);
			jig.localScale = new Vector3(0.7f,0.7f);

		}

		fadeChapterSelect = true;

	}

	private void CleanUpJigsawLerpers()
	{
		foreach (GameObject item in jigsawLerpers) {
			GameObject.Destroy(item);
		}
		jigsawLerpers.Clear();
	}

	private void CleanUpScene()
	{
		float durationTime = 0.5f;
		UnityEngine.Object.Destroy(m_splines, durationTime);
		CleanUpJigsawLerpers();
	}

    // Update is called once per frame
    public override void Update()
    {
		if(pinballLerper != null && pinballLerper.GetComponent<TransformLerper>().animationEnded)
		{			
			UnityEngine.Object.Destroy(pinballLerper);
			jigsawDisappear = true;
		}
		else if(jigsawDisappear)
		{
			jigsawDisappear = false;
			SetEarnedJigsawToCanvas();
		}
		else if(jigsawAppear && CheckLerperAnimationEnded())
		{
			jigsawAppear = false;
		}
		else if(fadeChapterSelect)
		{
			fadeChapterSelect = false;
            CloseRewardTransition();
        }
    }

    private void CloseRewardTransition()
    {
        CleanUpScene();
        StateJigsawPuzzle.Instance.ActivateReward = true;
        GameController.Instance.ChangeState(GameController.States.JigsawPuzzle);
    }

    public override void Exit()
    {
		if(m_jigsaw_earned.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(m_jigsaw_earned);
		}
        //UploadManager.Instance.SetTimerState(TimerType.Pinball, false);
    }
}
