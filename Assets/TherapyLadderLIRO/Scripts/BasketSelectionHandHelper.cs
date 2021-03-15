using UnityEngine;
using System.Collections;

public class BasketSelectionHandHelper : MonoBehaviour {

    public AnimationCurve animationCurve;
    private Vector2 final_position = new Vector2(-100, 0);
    private Vector2 m_originalPosition ;
    private RectTransform currTransform;
    public float m_speed = 0.9f;
    bool isInitialized = false;

    void Start()
    {
        if (!isInitialized)
        {
            currTransform = GetComponent<RectTransform>();
            m_originalPosition = currTransform.anchoredPosition;
            isInitialized = true;
            StartCoroutine(DoMovement());
        }        
    }

    void OnEnable()
    {
        if (isInitialized)
        {
            StopAllCoroutines();
            currTransform.anchoredPosition = m_originalPosition;
            StartCoroutine(DoMovement());
        }        
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DoMovement()
    {
        float t = 0;
        while (true)
        {
            currTransform.anchoredPosition = Vector3.LerpUnclamped(m_originalPosition, final_position, animationCurve.Evaluate(t)) * m_speed;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

}
