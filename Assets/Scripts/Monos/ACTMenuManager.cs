using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACTMenuManager : MonoBehaviour {
    
    public CanvasGroup ui_element;

    public void OpenMenu()
    {
        if (ui_element != null)
        {
            ui_element.alpha = 1.0f;
            ui_element.blocksRaycasts = true;
        }
    }

    public void CloseMenu()
    {
        if (ui_element != null)
        {
            ui_element.alpha = 0.0f;
            ui_element.blocksRaycasts = false;
        }
    }

    public void QuitListenInACT()
    {
        //If we are running in a standalone build of the game
        Debug.Log("Starting LISTEN IN quit routine");
        GameStateSaver.Instance.SaveGameProgress();
        Application.Quit();

        //If we are running in the editor
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Use this for initialization
    void Start()
    {
        if (ui_element == null)
        {
            ui_element = GameObject.FindGameObjectWithTag("MenuUI").GetComponent<CanvasGroup>();
            ui_element.blocksRaycasts = false;
            ui_element.alpha = 1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
