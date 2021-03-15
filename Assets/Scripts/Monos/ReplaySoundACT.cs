using UnityEngine;
using System.Collections;

public class ReplaySoundACT : MonoBehaviour {


	public GameObject gameController;
	private float lastPlayedTime = 0.0f;
	private float nextThreshold = 0.0f;

	void OnMouseDown() {
		if(gameController != null && Time.time - lastPlayedTime > nextThreshold)
		{
			nextThreshold = gameController.GetComponent<GameControlScriptACT>().OnClickReplayButton(); //.PlayAudio(0);
			lastPlayedTime = Time.time;
		}
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
