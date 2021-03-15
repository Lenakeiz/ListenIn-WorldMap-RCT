using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChallengeAnimator : MonoBehaviour {

	public bool activateOutro;
	public bool margineSlide;
	public float xStrenght;
	public float yStrength;
	public float movementSpeed;
	public float waitTimeAfterLastCoin;

    public Vector3[] positions;
    public Vector3[] avatarPositions;
    public Vector3[] avatarScale;
	public GameObject[] coinSpawners;
	public GameObject[] margins;
    public GameObject m_fullAvatar;
	public int currMargin = 0;

    public Action outroAnimationCompleted;

	public void ActivateOutroAnimation()
	{
		activateOutro = true;
	}

	public void ActivateMarginSlinding()
	{
		margineSlide = true;
	}

	IEnumerator OutroAnimation()
	{
		yield return new WaitForSeconds(1.5f);

        //Andrea: starting animation of balloon

        //DestroyAllOfTheCoins
        List<Component> components = new List<Component>(coinSpawners[currMargin].GetComponentsInChildren(typeof(Transform)));
        List<Transform> transforms = components.ConvertAll(c => (Transform)c);

        transforms.Remove(coinSpawners[currMargin].transform);
        transforms.Sort(
                  delegate (Transform t1, Transform t2)
                  {
                      return (t2.transform.localPosition.y.CompareTo(t1.transform.localPosition.y));
                  }
              );

        foreach (var item in transforms)
        {
            GameObject.DestroyObject(item.gameObject);
            yield return new WaitForEndOfFrame();
        }

        //List<Component> components = new List<Component>(coinSpawners[currMargin].GetComponentsInChildren(typeof(Transform)));
        //List<Transform> transforms = components.ConvertAll(c => (Transform)c);

        //transforms.Remove(coinSpawners[currMargin].transform);
        //      transforms.Sort(
        //          delegate (Transform t1, Transform t2)
        //          {
        //              return (t1.transform.localPosition.y.CompareTo(t2.transform.localPosition.y));
        //          }
        //      );
        //      for (int j = transforms.Count - 1; j > -1; j--)
        //{

        //          transforms[j].gameObject.GetComponent<CoinLoader>().strenghtXComponent = xStrenght;
        //          transforms[j].gameObject.GetComponent<CoinLoader>().strenghtYComponent = yStrength;

        //          transforms[j].gameObject.GetComponent<CoinLoader>().ChargeIntoCannon(positions);
        //          yield return new WaitForSeconds(0.1f);
        //}
        //yield return new WaitForSeconds(waitTimeAfterLastCoin);
        ActivateMarginSlinding();
	}

	IEnumerator MarginAnimation()
	{
        //Vector3 initialPosition = margins[currMargin].transform.position;
        //Vector3 finalposition = initialPosition + Vector3.left * 8;

        //float percentage = 0;

        //while(percentage <= 1.0f + movementSpeed)
        //{
        //	margins[currMargin].transform.position = Vector3.Lerp(initialPosition, finalposition, percentage);
        //	percentage += movementSpeed;
        //	yield return null;
        //}

        //Andrea: new animation for gentleman avatar
        iTween.Init(m_fullAvatar);
        yield return new WaitForEndOfFrame();

        iTween.MoveTo(m_fullAvatar, iTween.Hash("path", avatarPositions, "time", 4, "easetype", iTween.EaseType.easeOutCubic));
        iTween.ScaleTo(m_fullAvatar, iTween.Hash("scale", new Vector3(0.5f,0.5f,0.5f), "time", 4, "easetype", iTween.EaseType.easeOutCubic));
        StartCoroutine(FinishedAvatarTransition());

    }

    private IEnumerator FinishedAvatarTransition()
    {
        yield return new WaitForSeconds(4);
        Debug.Log("Finished transition on the avatar");
        //OutroAnimation will be used also in challegetutorial. We can proceed only if we are not in that case
        if (!gameObject.name.Contains("Tutorial"))
        {
            //Andrea: starting a new animation
            StateInitializePinball.Instance.StartCannonEnteringAnimation();
            //StateInitializePinball.Instance.StartPinball();
        }            
        else if (outroAnimationCompleted != null)
        {
            outroAnimationCompleted();
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(activateOutro)
		{
			activateOutro = false;
			StartCoroutine(OutroAnimation());
		}
		else if(margineSlide)
		{
			margineSlide = false;
			StartCoroutine(MarginAnimation());
		}
	}
}
