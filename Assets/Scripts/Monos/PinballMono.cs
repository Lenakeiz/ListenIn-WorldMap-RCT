using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PinballMono : MonoBehaviour {

    [SerializeField]
    private CardBucketController[] m_Buckets;
    public CardBucketController[] Buckets { get { return m_Buckets; } }

    [SerializeField]
    private RectTransform[] m_JigsawParents;
    public RectTransform[] JigsawParents { get { return m_JigsawParents; } }

    [SerializeField]
    private GameObject[] m_Levels;
    public GameObject[] Levels { get { return m_Levels; } }
	
	[SerializeField]
	private GameObject m_Cannon;
	public GameObject Cannon { get { return m_Cannon; } } 

	[SerializeField]
	private GameObject m_Coin_Spawner;
	public GameObject Coin_Spawner { get { return m_Coin_Spawner; } } 

	[SerializeField]
	private GameObject m_frame;
	public GameObject Frame { get { return m_frame; } } 

	[SerializeField]
	private GameObject firebutton;
	public GameObject FireButton { get { return firebutton; } }

	[SerializeField]
	private GameObject uiHolder;
	public GameObject UIHolder { get { return uiHolder; } } 

	[SerializeField]
	private GameObject fadingPlane;
	public GameObject FadingPlane { get { return fadingPlane; } }

    [SerializeField]
    private GameObject bucketsHolder;
    public GameObject BucketsHolder { get { return bucketsHolder; } }

    public float fadingSpeed;
    private float targetAlpha;
	private bool startFading;
	private bool startAnimationIn;
	private bool startLoadingCannon;
	private bool changeState;
    private bool canDetroy;
    private bool triggerStartPinball;

    [SerializeField]
	public List<GameObject> EarnedJigsaw;
	public List<Transform> EarnedJigsawTransforms;

	public float singleStepMovement;
	private Vector3 finalPos;
	private Vector3 midPos; 
	private Vector3 startPos;

    private bool isApproximate(float a, float b, float tol)
    {
        return Mathf.Abs(a - b) < tol;
    }

    IEnumerator FadePlane()
	{
		float astart = fadingPlane.GetComponent<SpriteRenderer>().color.a;
        float newAlpha = astart;
        float currLerpTime = 0.0f;
        Color updateColor;

        while (!isApproximate(newAlpha, targetAlpha, 0.01f))
        {
            currLerpTime += Time.deltaTime * fadingSpeed;
            currLerpTime = Mathf.Clamp(currLerpTime, 0.0f, 1.0f);

            newAlpha = Mathf.Lerp(astart, targetAlpha, currLerpTime);
            updateColor = fadingPlane.GetComponent<SpriteRenderer>().color;
            updateColor.a = newAlpha;
            fadingPlane.GetComponent<SpriteRenderer>().color = updateColor;
            yield return null;
        }

        updateColor = fadingPlane.GetComponent<SpriteRenderer>().color;
        updateColor.a = targetAlpha;
        fadingPlane.GetComponent<SpriteRenderer>().color = updateColor;

        if (triggerStartPinball)
        {
            changeState = true;
        }
       
        if(canDetroy)
		    Destroy(gameObject);
	}

	IEnumerator FadePinballIn()
	{
		float percentage = 0;
		
		while(percentage <= 1.0f + singleStepMovement)
		{
			gameObject.transform.localPosition = Vector3.Lerp(startPos, finalPos, percentage);
			percentage += singleStepMovement;
			yield return null;
		}

		gameObject.transform.localPosition = finalPos;
		yield return new WaitForSeconds(0.5f);
		startLoadingCannon = true;

	}

	IEnumerator LoadCannon()
	{
		int n_coins = StateChallenge.Instance.GetCoinsEarned();
		
		if(m_Cannon != null)
		{
			Vector3 initialPos = m_Cannon.transform.localPosition;
			Vector3 midPos = initialPos - Vector3.up;

			Vector3 coinSpawner = m_Coin_Spawner.transform.position + Vector3.up * 4.0f;

			float percentage = 0;
			//float slowMovement = 0.005f;
			float mediumMovement = 0.015f;

			while(percentage <= 1.0f + mediumMovement)
			{
				m_Cannon.transform.localPosition = Vector3.Lerp(initialPos, midPos, percentage);
				percentage += mediumMovement;
				yield return null;
			}

			while(n_coins > 0)
			{
				GameObject coin = GameObject.Instantiate(Resources.Load("Prefabs/Coin"),coinSpawner,Quaternion.identity) as GameObject;
				coin.transform.SetParent(m_Coin_Spawner.transform,true);
				Vector3 currLocalScale = coin.transform.localScale;
				coin.transform.localScale = currLocalScale * 0.17f;
				coin.GetComponent<Rigidbody2D>().AddTorque(5);
				//Ignoring collision with upper bar
				Physics2D.IgnoreCollision(coin.GetComponent<Collider2D>(),m_frame.GetComponent<Collider2D>());
				n_coins--;
				yield return new WaitForSeconds(0.3f);
			}

			yield return new WaitForSeconds(0.5f);
			percentage = 0.0f;

			while(percentage <= 1.0f + mediumMovement)
			{
				m_Cannon.transform.localPosition = Vector3.Lerp(midPos, initialPos, percentage);
				percentage += mediumMovement;
				yield return null;
			}

			m_Cannon.transform.localPosition = initialPos;
			SetFrameParent(true);

			for (int i = 0; i < m_Coin_Spawner.transform.childCount; i++) {
				GameObject coin = m_Coin_Spawner.transform.GetChild(i).gameObject;
				coin.GetComponent<Rigidbody2D>().isKinematic = true;
				Physics2D.IgnoreCollision(coin.GetComponent<Collider2D>(),m_frame.GetComponent<Collider2D>(),false);
				//m_Coin_Spawner.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
			} 

			yield return new WaitForSeconds(0.2f);

	
			SetCannonState(true);
			OrderCoinInSpawner();

			m_frame.GetComponent<Collider2D>().enabled = false;

			changeState = true;
		}
	}

    public void EnterCannon()
    {
        //iTween.MoveTo();
        //StartCoroutine(WaitForPinballEntering());
        CoinSpawnerB2_Final spawner = m_Cannon.GetComponent<CoinSpawnerB2_Final>();
        spawner.AnimateEnterCannon();
    }

    private IEnumerator WaitForPinballEntering()
    {
        yield return new WaitForSeconds(4);
    }

    public void SetEarnedCoins()
    {
        gameObject.GetComponentInChildren<CoinSpawnerB2_Final>().SetCoinsEarned();
    }

    public void SetSpwanerTriggerState(bool state)
    {
        Collider2D spawnerCollider = m_Cannon.GetComponent<Collider2D>();
        spawnerCollider.enabled = state;
        Collider2D upperBarCollider = m_frame.GetComponent<Collider2D>();
        upperBarCollider.enabled = state;
    }

    public void SetToAlphaFading(float alpha, bool destroyAtTheEnd, bool startGame)
    {
        targetAlpha = alpha;
        startFading = true;
        canDetroy = destroyAtTheEnd;
        triggerStartPinball = startGame;
    }

    //public void SetFade (){
    //	startFading = true;
    //}

    public void ActivateAnimationIn()
	{
		finalPos = gameObject.transform.localPosition;
		midPos = finalPos - Vector3.up * 10;
		startPos = finalPos - Vector3.up * 20;
		gameObject.transform.localPosition = startPos;
		startAnimationIn = true;
	}

	public void SetCannonState(bool state)
	{
		gameObject.GetComponentInChildren<CoinSpawnerB2_Final>().ActivateCannon = state;
    }

    public void EnableCannonGraphics()
    {
        gameObject.GetComponentInChildren<CoinSpawnerB2_Final>().SetCannonSprite();
    }

    public void OrderCoinInSpawner()
	{
		gameObject.GetComponentInChildren<CoinSpawnerB2_Final>().SetListOfCoins();
	}

	public void SetFrameParent(bool state)
	{
		if(state)
		{
			m_frame.transform.SetParent(gameObject.transform,true);
		}
		else 
		{
			m_frame.transform.SetParent(m_Coin_Spawner.transform,true);
		}
	}

	public void DuplicateEarnedJigsaw()
	{
		if(EarnedJigsaw.Count != 0)
		{
			for (int i = 0; i < EarnedJigsaw.Count; i++) {
				GameObject copy = GameObject.Instantiate(EarnedJigsaw[i],Vector3.zero,Quaternion.identity) as GameObject;
				copy.GetComponent<Animator>().enabled = false;
				Vector3 pos = EarnedJigsawTransforms[i].position;
				//pos.y += 0.5f; //maybe if working with localposition this is not necassary
				pos.z = -2.0f;
				copy.transform.position = pos;
			}
		}
	}

	public void DisableOriginals()
	{
		for (int i = 0; i < EarnedJigsaw.Count; i++) {
			EarnedJigsaw[i].SetActive(false);
		}
	}

	void Awake()
	{
		startFading = false;
		EarnedJigsaw = new List<GameObject>();
		EarnedJigsawTransforms = new List<Transform>();
	}

	// Use this for initialization
	void Start () {
		
	}

    public void UnlockAndFinishPinballGame(bool unlockAll)
    {
        CardBucketController[] cbc = GameObject.FindObjectsOfType<CardBucketController>();
        if (cbc != null && cbc.Length != 0)
        {
            for (int i = 0; i < cbc.Length; i++)
            {
                cbc[i].UnlockJigsawPiece();
                if (!unlockAll)
                    break;
            }
        }

        CoinSpawnerB2_Final cb2F = GameObject.FindObjectOfType<CoinSpawnerB2_Final>();
        if (cb2F != null)
        {
            cb2F.PinballGameOver();
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                UnlockAndFinishPinballGame(true);
            }
        }

		if(startFading)
		{
			startFading = false;
			StartCoroutine("FadePlane");
		}
		else if(changeState)
		{
			changeState = false;
            GameController.Instance.ChangeState(GameController.States.StatePinball);
        }
	}

	public void SkipPinball()
	{
		GameController.Instance.ChangeState(GameController.States.StateReward);
	}
}
