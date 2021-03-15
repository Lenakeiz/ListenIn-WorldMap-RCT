////----------------------------------------------------------------------------------------------------
//// IGNORE THIS SCRIPT
////----------------------------------------------------------------------------------------------------

//using UnityEngine;
//using System.Collections;

//public class RectangleSmallRedScript : MonoBehaviour {

//	float speed = -3f;
	
//	//random rotation
//	float rotation;

//	//Control Script
//	GameControlScript m_gameControlScript;

//	// Use this for initialization
//	void Start () {
//		//Because this is instantiated, we must find the game control at run time
//		m_gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();

//		rotation = Random.Range(-40, 40);
//	}
	
//	// Update is called once per frame
//	void Update () {
//		//transform.Translate(0f, speed * Time.deltaTime, 0f);
//		//transform.Rotate(0f, rotation * Time.deltaTime, 0f);
//	}

//	void FixedUpdate() 
//	{
//		float fSpinMin = -100f;
//		float fSpinMax = 100f;
//		//rigidbody2D.fixedAngle = false;
//		GetComponent<Rigidbody2D>().AddTorque(3);
//		//rigidbody2D.AddTorque(Random.Range(fSpinMin, fSpinMax));
//	}

//	//if a meteor hits the player
//	void OnTriggerEnter2D(Collider2D other)
//	{
//		//rigidbody2D.isKinematic = true;
//		//renderer.enabled = false;
//		//m_gameControlScript.showNextTrial ();
//		//Destroy(other.gameObject);
//		//control.PlayerDied();
//		//Destroy(this.gameObject);
//	}
//}
