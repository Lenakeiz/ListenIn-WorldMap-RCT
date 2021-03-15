using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject gameController;
	public GameObject ui_element;

	public bool inChallengeState = true;

	void OnMouseDown() {
        OpenMenu();
	}

    public void OpenMenu()
    {
        if (ui_element != null)
        {
            if (inChallengeState && gameController != null)
            {
                //gameObject.GetComponent<CircleCollider2D>().enabled = false;
                GameObject go = GameObject.FindGameObjectWithTag("RepeatButton");
                if (go != null)
                {
                    go.GetComponent<CircleCollider2D>().enabled = false;
                }

                gameController.GetComponent<GameControlScriptStandard>().SetEnable(false);

                ui_element.GetComponent<ShowPanels>().ShowInitialMenu(true);
                //gameController.GetComponent<GameControlScript>().SetLevelMenu(true);
            }
            else
            {
                ui_element.GetComponent<ShowPanels>().ShowInitialMenu(false);
                //Time.timeScale = 0.0f;
            }
        }
    }

	// Use this for initialization
	void Start () {
		if(ui_element == null)
		{
			ui_element = GameObject.FindGameObjectWithTag("MenuUI");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
