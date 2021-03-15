using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BasketController : MonoBehaviour {

    public int m_basketNumber;
    public bool m_hardMode = false;

    public Color m_NormalColor;
    public Color m_HighlightColor;

    public Image m_Image;

    public GameObject m_slider;
    public GameObject m_needle;
    public float speed = 1.0f;
    public Vector3 positionEasy;
    public Vector3 positionHard;

    public AnimationCurve animationCurve;

    // Use this for initialization
    void Start () {
        if (m_Image == null)
        {
            Debug.LogWarning("Need to add the highlight image to the image controller");
        }
        if(m_needle)
            StartCoroutine(ShakeNeedle(true));
    }

    public void SetHighlightColor(bool highlight)
    {
        if (highlight)
        {
            m_Image.enabled = true;
        }
        else
        {
            m_Image.enabled = false;
        }
    }

    public void SetHardMode()
    {
        StopAllCoroutines();
        if (!m_hardMode)
        {
            m_hardMode = true;
            if(m_slider != null)
                m_slider.GetComponent<RectTransform>().localPosition = positionHard;
            if (m_needle)
                StartCoroutine(ShakeNeedle(false));            
        }
        else
        {
            m_hardMode = false;
            if(m_slider != null)
                m_slider.GetComponent<RectTransform>().localPosition = positionEasy;
            if(m_needle)
                StartCoroutine(ShakeNeedle(true));
        }
    }

    private IEnumerator ShakeNeedle(bool isEasy)
    {
        float t = 0.0f;
        Quaternion initialRotation = m_needle.transform.rotation;
        Quaternion finalRotation = isEasy ? Quaternion.Euler(0, 0, 45) : Quaternion.Euler(0, 0, -45);
        while (t < 1.0f)
        {
            t += Time.deltaTime * speed;
            m_needle.transform.rotation = Quaternion.SlerpUnclamped(initialRotation, finalRotation, animationCurve.Evaluate(t));
            yield return new WaitForEndOfFrame();
        }
    }

}
