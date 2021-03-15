using UnityEngine;
using System.Collections;

public class BridgeOpeningController : MonoBehaviour {

    public BridgeControllerLdn[] sides;
    public float timeBetweenOpenings = 7.0f;
    public float openingTime = 4.0f;
    private float timer = 0.0f;

    // Use this for initialization
    void Start () {
        sides = GetComponentsInChildren<BridgeControllerLdn>();
        StartCoroutine(GateTransition(true));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator GateTransition(bool isOpening)
    {
        float waitTime = isOpening ? timeBetweenOpenings : openingTime;
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < sides.Length; i++)
        {
            sides[i].SetBridgeOpenState(isOpening);
        }
        yield return new WaitForSeconds(waitTime);

        isOpening = !isOpening;
        StartCoroutine(GateTransition(isOpening));
    }

    // Update is called once per frame
    void Update () {

        //timer += Time.deltaTime;

        //if (isOpen && timer > openingTime)
        //{
        //    timer = 0.0f;
        //    isOpen = false;
        //    for (int i = 0; i < sides.Length; i++)
        //    {
        //        sides[i].SetBridgeOpenState(false);
        //    }
        //}
        //else if (timer > timeBetweenOpenings)
        //{
        //    timer = 0.0f;
        //    isOpen = true;
        //    for (int i = 0; i < sides.Length; i++)
        //    {
        //        sides[i].SetBridgeOpenState(true);
        //    }
        //}
        
            
	}
}
