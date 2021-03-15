using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MadLevelManager;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {

    [Header("Assign in inspector")]
    public List<Image> logos = new List<Image>();
    public float updateTimeForLogo = 3.0f;
    private int counter = 0;
    private bool isChanging = false;
	// Use this for initialization
	void Start () {
        foreach (var im in logos)
        {
            Color c = im.color;
            c.a = 0.0f;
            im.color = c;
        }
        StartCoroutine(ShowLogos(counter));

    }

    public void UpdateColor(Color c)
    {
        if(counter<logos.Count)
            logos[counter].color = c;
    }

    public void OnCompletedFadeIn()
    {
        Debug.Log("Fade in completed " + counter);
        Color c = logos[counter].color;
        Color d = c;
        d.a = 0.0f;
        iTween.ValueTo(this.gameObject, iTween.Hash("from", c, "to", d, "time", updateTimeForLogo, "easetype", iTween.EaseType.easeOutCubic, "onupdate", "UpdateColor", "onupdatetarget", this.gameObject, "oncomplete", "OnCompletedFadeOut", "oncompletetarget", this.gameObject));

    }

    public void OnCompletedFadeOut()
    {
        Debug.Log("Fade out completed " + counter);

        counter++;
        StartCoroutine(ShowLogos(counter));
    }

    private IEnumerator ShowLogos(int counter)
    {
        yield return new WaitForEndOfFrame();
        if (counter < logos.Count)
        {
            Color c = logos[counter].color;
            Color d = c;
            d.a = 1.0f;
            iTween.ValueTo(this.gameObject, iTween.Hash("from", c, "to", d, "time", updateTimeForLogo, "easetype", iTween.EaseType.easeOutCubic, "onupdate", "UpdateColor", "onupdatetarget", this.gameObject, "oncomplete", "OnCompletedFadeIn","oncompletetarget",this.gameObject));
        }
        else
        {
            isChanging = true;
            yield return new WaitForSeconds(1.0f);
            SwitchScene();
        }
    }

    public void Update()
    {
        //get fingers on screen android only 
        int fingerCount = 0;

#if UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        fingerCount = Input.GetMouseButtonDown(0) ? 1 : 0;
#endif

        if (fingerCount > 0)
        {
            StopAnimation();
            SwitchScene();
        }
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("GoogleSignIn");
    }

    private void StopAnimation()
    {
        if (!isChanging)
        {
            StopAllCoroutines();
            iTween.Stop(this.gameObject);
        }        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
