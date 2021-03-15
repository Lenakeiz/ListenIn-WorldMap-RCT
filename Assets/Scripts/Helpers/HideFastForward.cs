using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HideFastForward : MonoBehaviour {

    [SerializeField]
    private GameObject challengeButtons;
    [SerializeField]
    private GameObject pinballButtons;

    // Use this for initialization
    void OnEnable() {

        if (challengeButtons == null || pinballButtons == null)
        {
            Debug.LogError("Must menu buttons button in editor");
            return;
        }

        Scene currScene = SceneManager.GetActiveScene();

        //Andrea
        if (currScene.name == "GameLoop")
        {
            GameObject challengeTherapy = GameObject.FindGameObjectWithTag("Challenge");
            if (challengeTherapy != null)
            {
                MenuManager mm = challengeTherapy.GetComponentInChildren<MenuManager>();
                if (mm != null)
                {
                    Debug.Log("Opening menu in therapy: hiding fast forward button");
                    challengeButtons.SetActive(true);
                    pinballButtons.SetActive(false);
                    return;
                }
            }

            GameObject jigsawPuzzle = GameObject.FindGameObjectWithTag("PinballPrefab");
            if (jigsawPuzzle != null)
            {
                MenuManager csm = jigsawPuzzle.GetComponentInChildren<MenuManager>();
                if (csm != null)
                {
                    Debug.Log("Opening menu in pinball game: enabling");
                    challengeButtons.SetActive(false);
                    pinballButtons.SetActive(true);
                }
            }
        }
     }
}
