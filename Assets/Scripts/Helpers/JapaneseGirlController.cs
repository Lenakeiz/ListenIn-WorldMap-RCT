using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JapaneseGirlController : MonoBehaviour {

	public bool isActive = false;
	public int index = -1;
	float yExtent;

	private int direction = -1;

	private float currSpeed = 0.0f;
	public float minSpeed = 350.0f;
	public float maxSpeed = 440.0f;

	private float jumpStrengh = 50.0f;
	public float minJumpStrengh = 250.0f;
	public float maxJumpStrengh = 380.0f;

	private float jumptimer = 0.0f;
	private float jumptimerthreshold = 0.0f;
	public float minjumptimer = 0.3f;
	public float maxjumptimer = 1.3f;

	private bool enableJump = false;
	private bool isjumping = false;
	private bool canJump = false;
	public float ySpawnPos = 0.0f;
	public float xHalfWidth;
	public float xOffset = 2.0f;
	public float xOutOfBoundOffset = 2.5f;

	private Rigidbody2D body;
	private LevelTwelveManager controller;

	private List<GameObject> grabbedCoins;
	private List<Transform> hands;

	public string soundFolderPath;
	public string[] StompSounds;
	public float thresholdJumpForSound = 7;

	// Use this for initialization
	void Start () {
		grabbedCoins = new List<GameObject>();
		hands = new List<Transform>();

		Transform childleft = transform.Find("Left");
		if(childleft != null)
		{
			hands.Add(childleft);
		}

		Transform childright = transform.Find("Right");		
		if(childright != null)
		{
			hands.Add(childright);
		}

		Vector3 screenRes = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		xHalfWidth = screenRes.x;

		body = gameObject.GetComponent<Rigidbody2D>();

		yExtent = GetComponent<Collider2D>().bounds.extents.y;

		//TODO this is not scalable, find a better way to do that
		controller = gameObject.transform.parent.gameObject.GetComponent<LevelTwelveManager>();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "Coin" && hands.Count != 0 && grabbedCoins.Count < 2)
		{
			other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
			other.gameObject.GetComponent<Rigidbody2D>().mass = 0.0f;
			other.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

			if(grabbedCoins.Count == 1)
			{
				other.gameObject.transform.SetParent(hands[0],true);
			}
			else
			{
				other.gameObject.transform.SetParent(hands[1],true);
			}

			other.gameObject.transform.localPosition = Vector2.zero;
			other.gameObject.transform.localScale = Vector2.one * 0.3f;
			other.gameObject.transform.localRotation = Quaternion.identity;


			grabbedCoins.Add(other.gameObject);
		}
	}

	public void SpawnRandomPosition()
	{
		//float yPos = Random.Range
		float xPos = 0.0f;
		float yPos = ySpawnPos;
		currSpeed = Random.Range(minSpeed,maxSpeed);
		
		if(Random.Range(0,101) > 49)
		{
			//spawnRight
			xPos = xHalfWidth + xOffset;
			direction = -1;
		}
		else
		{
			//spawnLeft
			xPos = -xHalfWidth - xOffset; 
			direction = 1;
		}

		jumptimerthreshold = Random.Range(minjumptimer,maxjumptimer);
		jumptimer = 0.0f;


		Vector3 scale = gameObject.transform.localScale;
		scale.x = direction * Mathf.Abs(scale.x);

		gameObject.transform.position = new Vector3(xPos,yPos,0);
		gameObject.transform.localScale = scale;

		body.isKinematic = false;
		canJump = Random.Range(0,101) > 49;
		isActive = true;

	}

	void FixedUpdate()
	{
		if(isActive)
		{
			body.AddForce(Vector2.right * direction * Time.fixedDeltaTime * currSpeed);
			//body.MovePosition(new Vector2(transform.position.x, transform.position.y) + Vector2.right * direction * Time.fixedDeltaTime * speed);
			if(enableJump && canJump)
			{
				enableJump = false;
				isjumping = true;
				body.AddForce(Vector2.up * Time.fixedDeltaTime * jumpStrengh,ForceMode2D.Impulse);
				jumpStrengh = Random.Range(minJumpStrengh,maxJumpStrengh);
			}
		}
	}

	private void DestroyGrabbedCoins()
	{
		if(grabbedCoins.Count != 0)
		{
			foreach (var item in grabbedCoins) {
				GameObject.DestroyImmediate(item);
			}
		}
		grabbedCoins.Clear();
	}

	private void ReturnObjectToPool()
	{
		controller.ReturnToPool(index);
		body.velocity = Vector2.zero;
		body.angularVelocity = 0;
		body.isKinematic = true;
		isActive = false;
		DestroyGrabbedCoins();
	}

	void LateUpdate()
	{
		if(grabbedCoins.Count != 0)
		{
			foreach (var item in grabbedCoins) {
				item.transform.localPosition = Vector2.zero;
				item.transform.localRotation = Quaternion.identity;
			}
		}
	}

	private void PlaySound()
	{
		AudioClipInfo aci;
		aci.delayAtStart = 0.0f;
		aci.isLoop = false;
		aci.useDefaultDBLevel = false;
		aci.clipTag = string.Empty;
		
		string strAudio = soundFolderPath + "/" + StompSounds[Random.Range(0,StompSounds.Length)].ToString();
		Camera.main.GetComponent<SoundManager>().SetChannelLevel(ChannelType.LevelEffects, -5.0f);
		Camera.main.GetComponent<SoundManager>().Play((Resources.Load(strAudio) as AudioClip), ChannelType.LevelEffects,aci);
	}

	// Update is called once per frame
	void Update () {

		if(isActive)
		{
			jumptimer += Time.deltaTime;

			//RaycastHit2D ray = Physics2D.Raycast(transform.position - (new Vector3(0,yExtent + 0.03f,0) ), -Vector2.up, 0.05f);

			int layerMask = 1 << 18;

			RaycastHit2D ray = Physics2D.Raycast(transform.position, -Vector2.up, yExtent + 0.5f, layerMask);
			if(jumptimer > jumptimerthreshold && ray.collider != null)
			{
				jumptimer = 0.0f;
				if(isjumping)
				{
					isjumping = false;
					if(Mathf.Abs(body.velocity.y) > thresholdJumpForSound)
					{
						PlaySound();
					}
						
				}
				enableJump = true;
			}
		}

		if(transform.position.x > xHalfWidth + xOutOfBoundOffset || transform.position.x < - xHalfWidth - xOutOfBoundOffset)
		{
			ReturnObjectToPool();
		}


	}
}
