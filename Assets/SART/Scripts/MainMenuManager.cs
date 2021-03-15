using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	public static bool testCompleted = false;
	public static bool practiceCompleted = false;

	public GameObject testTick;
	public GameObject practiceTick;

	// Use this for initialization
	void Start () {

		if(testCompleted == true){
			testTick.gameObject.SetActive (true);
		} else {
			testTick.gameObject.SetActive (false);
		}

		if(practiceCompleted == true){
			practiceTick.gameObject.SetActive (true);
		} else {
			practiceTick.gameObject.SetActive (false);
		}
	}

	public void LoadTest(){
		SceneManager.LoadScene("SART");
	}
}
