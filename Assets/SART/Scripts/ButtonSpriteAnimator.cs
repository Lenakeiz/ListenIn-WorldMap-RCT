using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonSpriteAnimator : MonoBehaviour {

    public Sprite[] buttonSprites;

    public Image buttonImage;

    private bool isAnimationEnabled = true;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        if(isAnimationEnabled)
            buttonImage.sprite  = buttonSprites[1];
    }

    void OnMouseUp()
    {
        if(isAnimationEnabled)
            buttonImage.sprite = buttonSprites[0];
    }

    public void SetAnimationEnabled(bool status)
    {
        isAnimationEnabled = status;;
    }
}
