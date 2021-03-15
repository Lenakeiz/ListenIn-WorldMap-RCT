using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SART_UI : MonoBehaviour {

    public Text practiceFailed;
    public GameObject sartStandardScreen;
    public GameObject endSARTTotal;

    public Button[] sartcloseButtons;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PrepareEndingScreen()
    {
        sartStandardScreen.SetActive(false);
        endSARTTotal.SetActive(true);
    }

    public void SetText(string textToSet)
    {
        practiceFailed.text = textToSet;
    }

    public void DisableCloseButtons()
    {
        for (int i = 0; i < sartcloseButtons.Length; i++)
        {
            sartcloseButtons[i].interactable = false;
        }
    }

}
