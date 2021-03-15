using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class Therapy_UI : MonoBehaviour {

    private string CycleFormat = "Cycle {0} progress";
    private string TherapyTotalFormat = "Total: {0}h  {1}m";
    private string TherapyDailyFormat = "Daily: {0}h  {1}m";
    //private string GameFormat = "Total Game Time: {0}h {1}m";
    private string PercentageFormat = "{0}%";
    public Text cycleText;
    public Image clockImage;
    public Text completedText; 
    public Text therapyText;
    public Text therapyDailyText;
    public Image backgroundFill;
    public Image filledProgressBar;
    public Text percentageText;
    public Text completedTherapyText;

    public float textSpeed = 0.1f;

    private char[] cycleTextS;
    private char[] therapyTextS;
    private char[] dailyTherapyTextS;
    //private char[] gameTextS;
    private char[] percentageTextS;

    private float perc;

    void Awake()
    {
        //Resetting the appearance of the cycle therapy information screen.
        cycleText.text = string.Empty;
        completedText.text = string.Empty;
        therapyText.text = string.Empty;
        therapyDailyText.text = string.Empty;
        percentageText.text = string.Empty;
        completedTherapyText.text = string.Empty;

        HideImageAlpha(clockImage);
        HideImageAlpha(backgroundFill);
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDisable()
    {

    }

    public void UpdateUserStats(UserProfileManager profile)
    {
        cycleTextS = (string.Format(CycleFormat, profile.m_userProfile.m_cycleNumber)).ToCharArray();

        int hours, mins;
        hours = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes / 60;
        mins = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes % 60;
        therapyTextS = (string.Format(TherapyTotalFormat, hours, mins)).ToCharArray();

        hours = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes / 60;
        mins = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes % 60;
        //gameTextS = (string.Format(GameFormat, hours, mins)).ToCharArray();

        hours = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes / 60;
        mins = profile.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes % 60;
        dailyTherapyTextS = (string.Format(TherapyDailyFormat, hours, mins)).ToCharArray();

        //Calculating percentage
        perc = (float)(profile.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock - 1) / (float)profile.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks * 100.0f;
        if (perc > 100.0f || perc == -Mathf.Infinity || profile.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock > profile.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks)
            perc = 0.0f;

        int roundPer = Mathf.RoundToInt(perc);

        percentageTextS = (string.Format(PercentageFormat, roundPer.ToString())).ToCharArray();
        StartCoroutine(UpdateUI());
    }

    private void HideImageAlpha(Image im)
    {
        Color currColor = im.color;
        Color targetColor = currColor;
        targetColor.a = 0.0f;
        im.color = targetColor;
    }

    private IEnumerator UpdateUI()
    {
        //Title
        yield return StartCoroutine(PrintText(cycleText,cycleTextS));
        //Timers
        StartCoroutine(PrintText(completedText, ("Time").ToCharArray()));
        yield return StartCoroutine(ShowImageAlpha(clockImage, 1.5f, 0.85f));

        StartCoroutine(PrintText(therapyText, therapyTextS));
        yield return StartCoroutine(PrintText(therapyDailyText, dailyTherapyTextS));

        //Cycle animation
        StartCoroutine(PrintText(completedTherapyText, ("Completed").ToCharArray()));
        yield return StartCoroutine(ShowImageAlpha(backgroundFill, 1.8f, 0.85f));
        StartCoroutine(PrintText(percentageText, percentageTextS));
        yield return StartCoroutine(ShowProgressFill(filledProgressBar, 1.5f));

    }

    private IEnumerator ShowImageAlpha(Image mImage, float speed, float desiredAlpha)
    {
        Color currColor = mImage.color;
        Color targetColor = currColor;
        targetColor.a = desiredAlpha;
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime;
            mImage.color = Color.Lerp(currColor, targetColor, t * speed);
            yield return new WaitForEndOfFrame();
        }
        mImage.color = targetColor;
    }

    private IEnumerator ShowProgressFill(Image mImage, float speed)
    {
        
        float t = 0.0f;
        float finalPercentage = perc / 100.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime;
            mImage.fillAmount = Mathf.Lerp(0.0f, finalPercentage, t * speed);
            yield return new WaitForEndOfFrame();
        }
        mImage.fillAmount = finalPercentage;
    }

    private IEnumerator PrintText(Text targetText, char[] targetString)
    {
        yield return null;
        targetText.text = string.Empty;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < targetString.Length; i++)
        {
            sb.Append(targetString[i]);
            targetText.text = sb.ToString();
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
