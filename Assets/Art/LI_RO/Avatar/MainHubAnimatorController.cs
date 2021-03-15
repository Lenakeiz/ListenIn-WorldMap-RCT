using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainHubAnimatorController : MonoBehaviour {

    public enum animatorMoves {Throw, Happy, Sad, JumpIn}

    public static MainHubAnimatorController instance;
    public static MainHubAnimatorController Instance
    {
        get
        {
            if (SceneManager.GetActiveScene().name == "MainHUB")
            {
                return instance;
            }
            else return null;
        }
        private set
        {
            if (instance == null)
            {
                instance = value;
            }
        }
    }

    public Animator anim;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            AnimateCharacter(animatorMoves.Throw);
#endif
    }

    public void AnimateCharacter(animatorMoves movetype)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PRESENT_CARDS_Baked"))
            return;
        anim.SetBool(movetype.ToString(), true);
    }

}
