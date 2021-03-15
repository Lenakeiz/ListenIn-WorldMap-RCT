////----------------------------------------------------------------------------------------------------
//// IGNORE THIS SCRIPT
////----------------------------------------------------------------------------------------------------

//using UnityEngine;
//using System.Collections;

//public class RectangleRedScript : MonoBehaviour {

//	//Control Script
//	GameControlScript m_gameControlScript;

//	// Use this for initialization
//	void Start () {

//		//Because this is instantiated, we must find the game control at run time
//		m_gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
	
//	}
	
//	// Update is called once per frame
//	void Update () {
	
//	}

//	//if a meteor hits the player
//	void OnTriggerEnter2D(Collider2D other)
//	{
//		GetComponent<Renderer>().enabled = true;
//		m_gameControlScript.ShowNextTrial ();
//		Destroy(other.gameObject);
//	}
//}
