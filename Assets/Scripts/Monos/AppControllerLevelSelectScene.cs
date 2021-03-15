using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MadLevelManager;

public class AppControllerLevelSelectScene : MonoBehaviour {

    public GameObject IniatilizeTherapyCore;

    private int previousAmount = 0;
    private int currAmount = 0;

	// Use this for initialization
	void Awake () {
        GameController.Instance.ChangeState(GameController.States.Idle);
        if (IniatilizeTherapyCore == null)
        {
            Debug.LogError("Please assign the components to AppControllerLevelSelectScene");
        }
    }

    void Start()
    {
        
    }

    void OnLevelWasLoaded(int level)
    {

    }

    void OnDestroy()
    {
     
    }

    // Update is called once per frame
    void Update () {
        GameController.Instance.Update();
    }
}
