using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryChecker : MonoBehaviour {

    private Text m_innerText;
    private Image m_innerImage;
    //private bool lateInitialization = false;
    private float level = 100;
    private Color warningcolor = Color.red;
    private Color basecolor = Color.white;
    private Vector3 startingScale = new Vector3(1,1);
    private Vector3 finalTextScale = new Vector3(2, 2);
    private Vector3 finalImageScale = new Vector3(1.5f, 1.5f);


    // Use this for initialization
    void Start () {
        m_innerText = GetComponentInChildren<Text>();
        m_innerImage = GetComponentInChildren<Image>();
        //lateInitialization = false;
#if UNITY_ANDROID
        try
        {
            level = UploadManager.Instance.GetBatteryLevel();
            m_innerText.text = ((int)level).ToString() + "%";
            Debug.Log(string.Format("BatteryChecker: battery level {0}", ((int)level).ToString()));
            if (level < 15.0f)
            {
                StopAllCoroutines();
                StartCoroutine(Enlarge());
            }
            else
            {
                m_innerText.color = basecolor;
                m_innerImage.color = basecolor;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(string.Format("BatteryChecker: {0}", ex.Message));
        }
#endif

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate() {
//        if (!lateInitialization)
//        {
//            lateInitialization = true;

//#if UNITY_ANDROID
//            level = UploadManager.Instance.GetBatteryLevel();
//            m_innerText.text = ((int)level).ToString() + "%";
//            Debug.Log(string.Concat("Current battery level: ", ((int)level).ToString() ));
//            if (level < 15.0f)
//            {
//                StopAllCoroutines();
//                StartCoroutine(Enlarge());
//            }
//            else
//            {
//                m_innerText.color = basecolor;
//                m_innerImage.color = basecolor;
//            }     
//#endif        
//        }
    }

    private IEnumerator Enlarge()
    {
        float timer = 0;
        float fixedtotaltime = 1.5f;
        float proportion = timer / fixedtotaltime;

        while (true)
        {
            m_innerText.color = Color.Lerp(basecolor, warningcolor, Mathf.PingPong(Time.time, 1.0f));
            m_innerText.rectTransform.localScale = Vector3.Lerp(startingScale, finalTextScale, Mathf.PingPong(Time.time, 1.0f));
            m_innerImage.color = Color.Lerp(basecolor, warningcolor, Mathf.PingPong(Time.time, 1.0f));
            m_innerImage.rectTransform.localScale = Vector3.Lerp(startingScale, finalImageScale, Mathf.PingPong(Time.time, 1.0f));
            yield return new WaitForEndOfFrame();
        }
    }

    void OnEnable()
    {
//        if (lateInitialization)
//        {
//#if UNITY_ANDROID
//            level = UploadManager.Instance.GetBatteryLevel();
//            m_innerText.text = ((int)level).ToString() + "%";
//            if (level < 15.0f)
//            {
//                StopAllCoroutines();
//                StartCoroutine(Enlarge());
//            }
//            else
//            {
//                m_innerText.color = basecolor;
//                m_innerImage.color = basecolor;
//            }
//#endif
//        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDetroy() {
        StopAllCoroutines();
    }
}
