using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardBucketController : MonoBehaviour {

    GameObject m_Bucket, m_Particles;

	public GameObject textReward;

    public int threshold;

	public bool isUnlocked;

    private int timesEntered;
    private float alphaValue;
    private float increaseAmount;
    private SpriteRenderer sprite;
    private Transform progressionColour;
    private Image progressionFiller; 
    private float progressionStart;
    private float progressionMoveAmount;

	private bool rewardSound = false;

	[SerializeField]
    private int m_JigsawPeiceToUnlock = -1;
	public int JigsawPeiceToUnlock
    {
        get { return m_JigsawPeiceToUnlock; }
        set { m_JigsawPeiceToUnlock = value; }
    }
	[SerializeField]
    private int m_LevelToUnlock = -1;
    public int LevelToUnlock
    {
        get { return m_LevelToUnlock; }
        set { m_LevelToUnlock = value; }
    }

    private int initVal;

	public void Init (int peiceToUnlock, float currentProgress, int puzzleToUnlock, Image image) {
        JigsawPeiceToUnlock = peiceToUnlock;
        LevelToUnlock = puzzleToUnlock;
        progressionFiller = image;
        initVal = (int)(currentProgress * threshold);
    }

    void Start()
    {
        GetChildComponents();

        timesEntered = 0;
        alphaValue = 0;
        //set the amount to increase alpha based on the threshold (alpha between 0 and 1)
        increaseAmount = 1 / (float)threshold;

        //calculate amount to move our progression by
        progressionMoveAmount = Mathf.Abs(progressionStart/(float)threshold);

        if (m_Particles != null)
        {
            m_Particles.SetActive(false);
        }

        for (int i = 0; i < initVal; i++)
        {
            timesEntered++;
            MoveProgression();
        }

        SetFillerAmount();

		isUnlocked = false;
	}

    void GetChildComponents()
    {
        foreach (Transform child in transform)
        {
             if (child.CompareTag("colour")) sprite = child.GetComponent<SpriteRenderer>();
             if (child.CompareTag("progression"))
             {
                 progressionColour = child;
                 progressionStart = child.localPosition.y;
             }
        }
    }

	private void PlaySound(string resource, bool loop)
	{
		AudioClipInfo aci;
		aci.delayAtStart = 0.0f;
		aci.isLoop = loop;
		aci.useDefaultDBLevel = true;
		aci.clipTag = string.Empty;
		
		Camera.main.GetComponent<SoundManager>().Play((Resources.Load(resource) as AudioClip), ChannelType.LevelEffects,aci);

	}

    public void UnlockJigsawPiece()
    {
        if (m_JigsawPeiceToUnlock != -1)
        {
            StatePinball.Instance.m_PinballMono.EarnedJigsaw.Add(transform.Find("JigsawPlaceholder").gameObject as GameObject);
            StatePinball.Instance.m_PinballMono.EarnedJigsawTransforms.Add(transform);

            textReward.GetComponent<Animator>().SetTrigger("activateTextReward");
            transform.Find("JigsawPlaceholder").GetComponent<Animator>().SetTrigger("triggerReward");// GetComponentInChildren<Animator>().SetTrigger("triggerReward");

            StateJigsawPuzzle.Instance.RecordPieceToUnlock(m_JigsawPeiceToUnlock);

            gameObject.GetComponentInChildren<JigsawInfo>().Index = m_JigsawPeiceToUnlock;

            isUnlocked = true;

            if (!rewardSound)
            {
                rewardSound = true;
                PlaySound("Sounds/Pinball/Pinball_Chips_Stack_Highlight_Loop_01", true);
            }

            if(progressionFiller != null)
                progressionFiller.fillAmount = 0.0f;

        }
    }

    private void SetFillerAmount()
    {
        if (progressionFiller != null)
        {
            float amount = Mathf.Clamp01((float)timesEntered / threshold);
            progressionFiller.fillAmount = amount;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //destroy the coin once it's fallen into the bucket and add to our counter

        CoinMono mono = other.GetComponent<CoinMono>();
        if (mono != null)
        {
            mono.BeginDestroy();
        }
        else
        {
            Destroy(other.gameObject);
        }

        timesEntered++;
        MoveProgression();

        if (timesEntered < threshold)
        {
            SetFillerAmount();
            if (m_JigsawPeiceToUnlock != -1)
            {
                StateJigsawPuzzle.Instance.SetCapToJigsawProgression(m_JigsawPeiceToUnlock, threshold);
            }
        }
        else if (timesEntered == threshold)
        {
            UnlockJigsawPiece();
        }
    }

    void MoveProgression()
    {
        //move up our progression colour
		if(progressionColour.localPosition.y < -progressionMoveAmount) progressionColour.position = progressionColour.position + new Vector3(0, progressionMoveAmount, 0);
    }

}
