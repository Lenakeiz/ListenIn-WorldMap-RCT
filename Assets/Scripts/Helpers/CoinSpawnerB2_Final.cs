using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinSpawnerB2_Final : MonoBehaviour {

    public GameObject coin;
    public Vector3[] enteringPositions;
    public float speed = 5.0f;
    public Transform spawnPoint;
	public bool isDebugging = false;

    private float maxWidth;
    [SerializeField]
    private float OFFSET_FROM_CORNER = 0.3f;
    private int coinsEarned;
    private int totalNumberSpawned = 0;

	private bool activate = false;
    public bool ActivateCannon { get { return activate; } set { activate = value; } }
    private bool canFire = false;

    private bool dirRight = true;
    private bool gameOver = false;
	private bool endPinball = false;
	private bool startFinalTimer = false;
	private float startTime;

    private float fireRate = 0.01f;
    private float nextFire;

	private Vector2 translation;
	private Vector2 smoothVelocity;

	private List<GameObject> loadedCoins = new List<GameObject>();

	public bool stopDropper = false;

    private bool previousDirection = false;

    [SerializeField] private SpriteRenderer sprite;

    void Start()
    {
        //Time.timeScale = 0;

        //nice little piece of code that finds the maximum width of the screen
        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0);
        Vector3 targetWidth = Camera.main.ScreenToWorldPoint(upperCorner);
        //which we use to set out max spawn range (so objects don't spawn off screen)
        float coinWidth = coin.GetComponent<Renderer>().bounds.size.x;
        maxWidth = targetWidth.x - coinWidth;

        //get the coin score from the previous screen
        
        //if (coinsEarned == 0) coinsEarned = 14; // default debug value used when testing scene directly, should be removed from final builds

    }

    void MoveDropper()
    {
        //move the dropper back and forth
		//we have a rigidbody attached to it so we need to use the fixed update
		Vector3 traslationTarget;
		if (dirRight) traslationTarget = Vector3.right * speed; //transform.Translate(Vector2.right * speed * Time.deltaTime);
		else traslationTarget = -Vector3.right * speed;

		translation =  Vector2.SmoothDamp(translation, traslationTarget, ref smoothVelocity, 0.10f, 1, 1);
        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + traslationTarget, Time.deltaTime);
        //gameObject.transform.Translate(traslationTarget * Time.deltaTime);

        if (transform.position.x >= (maxWidth - OFFSET_FROM_CORNER))
        {
            dirRight = false;
        }
        if (transform.position.x <= -1*(maxWidth - OFFSET_FROM_CORNER))
        {
            dirRight = true;
        }

        if (previousDirection != dirRight)
        {
            Vector3 currScale = transform.localScale;
            currScale.x *= -1;
            transform.localScale = currScale;
        }

        previousDirection = dirRight;
        //MoveSpawnPoint(0.18f);
    }

	void FixedUpdate()
	{
		if(!stopDropper)
			gameObject.transform.Translate(translation * Time.fixedDeltaTime);
	}

    void MoveSpawnPoint(float amount)
    {
        //hacky code to have the spawnpoint behind the dropper so visually it looks better
        if (dirRight) spawnPoint.localPosition = new Vector3(amount, spawnPoint.localPosition.y ,spawnPoint.localPosition.z);
        else spawnPoint.localPosition = new Vector3(-amount, spawnPoint.localPosition.y, spawnPoint.localPosition.z);
    }

    void Update()
    {
		if(!activate && !isDebugging) return;

		if (!gameOver && !endPinball)
        {
			if(!stopDropper)
				MoveDropper();
            CoinFiring();
            GameOverTrigger();
        }
		else if(!endPinball && gameOver)
        {
            PinballGameOver();
        }
   	}

    public void SetCoinsEarned()
    {
        coinsEarned = StateChallenge.Instance.GetCoinsEarned();
    }

    public void PinballGameOver()
    {
        endPinball = true;
        GameObject[] leftCoins = GameObject.FindGameObjectsWithTag("Coin");
        if (leftCoins != null && leftCoins.Length != 0)
        {
            for (int i = 0; i < leftCoins.Length; i++)
            {
                CoinMono cm = leftCoins[i].GetComponent<CoinMono>();
                cm.ImmediateDestroy(0.0f);
            }
        }
        GameController.Instance.ChangeState(GameController.States.StateReward);
    }

	void CoinFiring()
    {
        //when the mouse button is pressed fire a coin, you can also hold to fire
        if (canFire && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			SpawnCoin ();
		}
        // when touched
        else if (Input.GetMouseButtonDown(0) && isDebugging)
        {
            SpawnCoinInPosition();
        }

        if(!startFinalTimer && ReturnCoinsLeft() == 0)
		{
			startFinalTimer = true;
			startTime = Time.time;
		}
		canFire = false;
    }

	bool CheckCoinMoving()
	{
		GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
		if(coins != null && coins.Length != 0)
		{
			for (int i = 0; i < coins.Length; i++) {
				if(!coins[i].GetComponent<CoinMono>().isDeleting && coins[i].GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f)
					return true;
			}
		}
		return false;
	}

    void GameOverTrigger()
    {
        //if all coins have finished falling end the game and show the score
        
		if ((GameObject.FindWithTag("Coin") == null && ReturnCoinsLeft() <= 0) || (startFinalTimer && (Time.time - startTime > 10.0f)))
        {
			if(!isDebugging)
			{
                //Andrea: insert here the forced stop timer
                UploadManager.Instance.ForcedTimerState = true;
                gameOver = true;
			}
        }
    }

    public void SpawnCoin()
    {
        if (ReturnCoinsLeft() > 0)
        {
            //spawn a coin and keep track of it in our counter
            GameObject coinInstance = null;

            if (loadedCoins.Count != 0)
            {
                coinInstance = loadedCoins[0];
                loadedCoins.RemoveAt(0);
            }

            if (coinInstance == null)
            {
                coinInstance = Instantiate(coin, spawnPoint.transform.position, Quaternion.identity) as GameObject;
                coinInstance.transform.localScale = new Vector3(0.47f, 0.47f);
            }
            else
            {
                coinInstance.transform.SetParent(StatePinball.Instance.m_PinballMono.transform, true);
                coinInstance.transform.position = spawnPoint.transform.position;
                Vector3 currLocalScale = coinInstance.transform.localScale;
                coinInstance.transform.localScale = new Vector3(0.47f, 0.47f);//currLocalScale * 2.9f;
            }
            coinInstance.GetComponent<Rigidbody2D>().isKinematic = false;
            coinInstance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            coinInstance.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            coinInstance.GetComponent<Rigidbody2D>().AddForce(-Vector2.up * 10);
            totalNumberSpawned++;

        }
    }

    public void SpawnCoinInPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject coinInstance = null;
        //coinInstance = Instantiate(coin, ray.origin, Quaternion.identity) as GameObject;
        coinInstance = Instantiate(coin, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        coinInstance.transform.localScale = new Vector3(0.47f, 0.47f);
        coinInstance.GetComponent<Rigidbody2D>().isKinematic = false;
        coinInstance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        coinInstance.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        coinInstance.GetComponent<Rigidbody2D>().AddForce(-Vector2.up * 10);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    GameObject coinInstance = null;
        //    coinInstance = Instantiate(coin, hit.transform.position, Quaternion.identity) as GameObject;
        //    coinInstance = Instantiate(coin, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        //    coinInstance.transform.localScale = new Vector3(0.47f, 0.47f);
        //    coinInstance.GetComponent<Rigidbody2D>().isKinematic = false;
        //    coinInstance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //    coinInstance.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        //    coinInstance.GetComponent<Rigidbody2D>().AddForce(-Vector2.up * 10);
        //}
    }

    public void SetFiringCannon()
	{
		canFire = true;
	}

	public void SetActivateCannon(bool act)
	{
		activate = act;
	}

	public void SetListOfCoins()
	{
		loadedCoins.Clear();

		List<Component> components = new List<Component>(spawnPoint.GetComponentsInChildren(typeof(Transform)));
		List<Transform> transforms = components.ConvertAll(c => (Transform)c);
		
		transforms.Remove(spawnPoint.transform);
		transforms.Sort(
			delegate (Transform t1, Transform t2)
			{
			return (t1.transform.localPosition.y.CompareTo(t2.transform.localPosition.y));
		}
		);

		for (int j = transforms.Count - 1; j > -1; j--) {
			loadedCoins.Add(transforms[j].gameObject);
		}

	}

    //Functions to be referenced by the UI
	public int ReturnCoinsLeft()
    {
        return coinsEarned - totalNumberSpawned;
    }

    public void AnimateEnterCannon()
    {
        sprite.enabled = true;
        iTween.Init(this.gameObject);
        iTween.MoveTo(this.gameObject, iTween.Hash("path", enteringPositions, "time", 3.5, "easetype", iTween.EaseType.easeOutCubic, "oncomplete", "FinishedTransition"));
    }

    public void SetCannonSprite()
    {
        if (sprite != null)
            sprite.enabled = true;
    }

    public void FinishedTransition()
    {
        Debug.Log("Cannon in postion");
        StateInitializePinball.Instance.StartPinball();
    }

}
