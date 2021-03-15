using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class ACT_UI : MonoBehaviour {

    [Header("Initialization, Score and Current progression")]
    public Text m_baseText;
    //public GameObject m_Icon;
    public GameObject standardAct;
    public GameObject scoreAct;
    public Text currScoreText;
    public Text prevScoreText;
    //public List<RectTransform> m_iconTransformList = new List<RectTransform>();
    public Button m_continueButton;
    public Button m_closeScoreButton;

    [Header("Score")]
    public Text ResultText;
    public GameObject ResultsIcon;
    public GameObject todayIcon;
    public GameObject lastIcon;
    public Text todayText;
    public Text lastText;
    public AnimationCurve animationIconsScale;
    public AnimationCurve animationIconsRotation;
    public AnimationCurve animationTranslate;
    public float AnimationSpeedMultiplier;
    public float initialRotation;

    [Header("Current Progression")]
    public Image m_circularProgression;
    public Image[] m_filledPieces;
    public Text m_scoreText;

    [Header("Helpers in section")]
    public Button CloseButton;
    public Button InfoButton;
    public GameObject InitialInfoAfterLoading;

    private string text_format = "Loading... {0}%";

    private int currAmount = 0;
    private int previousAmount = 0;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void HideInteralUI()
    {
        standardAct.gameObject.SetActive(false);
        standardAct.gameObject.SetActive(false);
        CloseButton.interactable = false;
    }

    public void SetACTStardardBoard()
    {
        HideInteralUI();
        standardAct.gameObject.SetActive(true);
    }

    public IEnumerator HideBoard()
    {
        m_closeScoreButton.interactable = false;
        CloseButton.interactable = false;
        yield return new WaitForSeconds(1);
        standardAct.SetActive(false);
        scoreAct.SetActive(false);
        yield return new WaitForSeconds(2);
    }

    public IEnumerator SetScore(float currScore, float previousScore, bool firstTime)
    {
        HideInteralUI();
        m_closeScoreButton.interactable = false;
        scoreAct.SetActive(true);

        string formatCurrent = "<color=aqua>{0}%</color>";
        string formatLast = "<color=lime>{0}%</color>";

        yield return null;
        yield return StartCoroutine(PrintText(ResultText, "RESULTS".ToCharArray(), 0.2f));
        yield return StartCoroutine(fastAnimationGO(ResultsIcon, AnimationSpeedMultiplier));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(fastAnimationGOTranslate(todayIcon, 1.5f, new Vector3(-150,0,0)));
        yield return StartCoroutine(PrintText(todayText, "Today's score".ToCharArray(), 0.08f));
        yield return new WaitForSeconds(1.0f);
        currScoreText.text = string.Format(formatCurrent, ((int)Mathf.Ceil(currScore)).ToString());
        yield return null;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fastAnimationGOTranslate(lastIcon, 1.5f, new Vector3(150, 0, 0)));
        yield return StartCoroutine(PrintText(lastText, "Last score".ToCharArray(), 0.08f));
        yield return new WaitForSeconds(1.0f);
        prevScoreText.text = firstTime ? "N/A" : string.Format(formatLast, ((int)Mathf.Ceil(previousScore)).ToString());

        yield return new WaitForSeconds(3);

        CloseButton.interactable = true;
        m_closeScoreButton.interactable = true;
    }

    private IEnumerator fastAnimationGO(GameObject go, float speed)
    {

        Transform tf = go.transform;
        Vector3 initialScale = new Vector3(4, 4, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);
        //Quaternion initialRotation = Quaternion.Euler(0, 0, 360+270);
        //Quaternion finalRotation = Quaternion.Euler(0, 0, 0);
        float initialRotation = this.initialRotation;
        float finalRotation = 0;
        float currRoation = 0;

        go.SetActive(true);
        tf.localScale = initialScale;
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed;

            tf.localScale = Vector3.LerpUnclamped(initialScale, finalScale, animationIconsScale.Evaluate(t));
            currRoation = Mathf.Lerp(initialRotation, finalRotation, animationIconsRotation.Evaluate(t));
            tf.localRotation = Quaternion.AngleAxis(currRoation, Vector3.forward);

            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator fastAnimationGOTranslate(GameObject go, float speed, Vector3 InitialPos)
    {
        Transform tf = go.transform;
        Vector3 initialPos = InitialPos;
        Vector3 fialPosition = new Vector3(0, 0, 0);

        go.SetActive(true);
        tf.localPosition = InitialPos;
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed;

            tf.localPosition = Vector3.LerpUnclamped(initialPos, fialPosition, animationIconsScale.Evaluate(t));
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator PrintText(Text targetText, char[] targetString, float textSpeed)
    {
        targetText.text = string.Empty;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < targetString.Length; i++)
        {
            sb.Append(targetString[i]);
            targetText.text = sb.ToString();
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void StartAnimationEditor()
    {
        StartCoroutine(SetScore(64, 25, false));
    }

    public void UpdateText(int amount)
    {
        StopCoroutine(UpdateProgressInUI());
        currAmount = amount;
        StartCoroutine(UpdateProgressInUI());
    }

    public void UpdateIcon(float duration, int currStep)
    {
        StartCoroutine(SetCircularBar(1.3f, currStep));

        if (currStep > 0)
        {
        }
        //    m_Icon.transform.localPosition = m_iconTransformList[currStep - 1].localPosition;
        //    if (currStep == 1)
        //        return;

        //    iTween.Init(m_Icon);
        //    iTween.MoveTo(m_Icon, iTween.Hash("position", m_iconTransformList[currStep].localPosition, "islocal",  true, "time", 2, "easetype", iTween.EaseType.easeOutCubic));
    }

    private void SetIcon(int currStep)
    {
        Debug.LogError("Method not implemented");
        if (currStep > 0)
        {
            //m_Icon.transform.localPosition = m_iconTransformList[currStep].position;
        }
    }

    private IEnumerator SetCircularBar(float speed, int step)
    {
        yield return new WaitForEndOfFrame();
        Color startColor = new Color(1, 1, 1, 0);
        Color endColor = new Color(1, 1, 1, 1);
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed * 1.5f;
            m_circularProgression.color = Color.Lerp(startColor, endColor, t);
            yield return new WaitForEndOfFrame();
        }

        m_circularProgression.color = endColor;
        yield return new WaitForSeconds(0.4f);

        t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed * 1.5f;
            for (int i = 0; i < step - 1; i++)
            {
                m_filledPieces[i].color = Color.Lerp(startColor, endColor, t);
            }
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < step -1; i++)
        {
            m_filledPieces[i].color = endColor;
        }
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(0.5f);
        m_scoreText.text = string.Format("{0}/8", (step - 1).ToString());

        yield return new WaitForSeconds(1.0f);
        m_baseText.text = "Press PLAY to continue";
        CloseButton.interactable = true;
        InfoButton.interactable = true;
        yield return new WaitForEndOfFrame();

    }

    IEnumerator UpdateProgressInUI()
    {
        int amount = previousAmount;
        while (amount <= currAmount)
        {
            m_baseText.text = string.Format(text_format, amount);
            amount++;
            previousAmount = amount;
            yield return new WaitForSeconds(0.02f);
        }
        previousAmount = currAmount;

        if (previousAmount == 100)
        {
            //StartCoroutine(SetCircularBar(1.3f,1));
            yield return new WaitForSeconds(1.0f);
            m_baseText.text = "";
            yield return new WaitForEndOfFrame();
            InitialInfoAfterLoading.SetActive(true);
            m_continueButton.interactable = true;
            CloseButton.interactable = true;
            InfoButton.interactable = true;
            yield return new WaitForEndOfFrame();
        }

    }

}
