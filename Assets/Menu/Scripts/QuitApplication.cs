using UnityEngine;
using System.Collections;
using MadLevelManager;

public class QuitApplication : MonoBehaviour {

	public void Quit()
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

    /// <summary>
    /// DEPRECATED
    /// </summary>
	public void ResetSavedGameState()
	{
		GameStateSaver.Instance.ResetGameProgress();
        MadLevelProfile.Reset();
        Application.Quit();
		#if UNITY_EDITOR
		//Stop playing the scene
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}

    public void SimpleQuit()
    {
        //If we are running in a standalone build of the game
        Application.Quit();

        //If we are running in the editor
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
