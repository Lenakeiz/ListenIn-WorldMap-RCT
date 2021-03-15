using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CreditsController : MonoBehaviour {

    public CanvasGroup cg;

    [Header("Assigned in the inspector")]
    public ScrollRect sc;
    public Scrollbar sb;
    [Range(10.0f,50.0f)]
    public float scrollSpeed = 10.0f;
    public float waitTime = 5.0f;
    bool autoScrolling = false;

	// Use this for initialization
	void Start () {
        cg = gameObject.GetComponent<CanvasGroup>();
        if (cg == null)
            Debug.LogError("Please assign canvas group to the credits button");
        if(sb == null)
            Debug.LogError("Please assign vertical scroll group to the credits button");

        cg.alpha = 0.0f;
        cg.blocksRaycasts = false;
	}

    public void HideCanvas(bool canvasStatus)
    {
        cg.alpha = canvasStatus ? 1.0f : 0.0f;
        cg.blocksRaycasts = canvasStatus;
        if (canvasStatus)
        {
            MainHubAnimatorController.Instance.AnimateCharacter(MainHubAnimatorController.animatorMoves.Throw);
            StartCoroutine(AutoScrolling());            
        } 
        else StopScrolling(true);
    }

    public void StopScrolling(bool reset)
    {
        if (autoScrolling)
        {
            StopAllCoroutines();
            autoScrolling = false;
        }        
        if(reset)
            sb.value = 1f;
    }

    private IEnumerator AutoScrolling()
    {
        autoScrolling = true;
        yield return new WaitForSeconds(waitTime);
        while (autoScrolling)
        {
            sc.velocity = new Vector2(0.0f, scrollSpeed);
            if (sb.value < 0.01f)
                autoScrolling = false;
            yield return new WaitForEndOfFrame();
        }
        autoScrolling = false;
    }

}
