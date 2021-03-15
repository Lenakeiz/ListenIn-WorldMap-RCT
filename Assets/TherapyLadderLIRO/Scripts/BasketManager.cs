using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using MadLevelManager;

public class BasketManager : MonoBehaviour {

    private List<BasketUI> m_SelectedBaskets;
    //TO BE ADJUSTED IN EDITOR
    public int max_number_of_selected_basket = 1;

    [SerializeField]
    private Button m_startButton;
    [SerializeField]
    private GameObject m_progressScreen;
    [SerializeField]
    private Text m_progressTherapy;
    [SerializeField]
    private Image[] tickImages;
    [SerializeField]
    private Text[] m_basketCounterTexts;

    public BasketSelectionHandHelper m_handHelper;

    private string m_stringProgressBarFormat = "Loading Therapy... {0}%";

	// Use this for initialization
	void Start () {

        m_SelectedBaskets = new List<BasketUI>();
        if (m_startButton == null || m_progressScreen == null || m_progressTherapy == null)
            Debug.LogError("Please attach the components to BasketManager");
        m_startButton.interactable = false;
        m_progressScreen.SetActive(false);
        m_handHelper.gameObject.SetActive(false);
        UpdateTicks(0);
        UpdateBasketCounters();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_SelectedBaskets.Clear();
            m_SelectedBaskets.Add(new BasketUI() { basketId = 1, hardMode = false });
            m_SelectedBaskets.Add(new BasketUI() { basketId = 2, hardMode = false });
            PrepareTherapy();
        }
#endif
    }

    void OnDestroy()
    {
        if (TherapyLIROManager.Instance != null)
            TherapyLIROManager.Instance.m_onUpdateProgress -= UpdateProgressBar;
    }

    public void RegisterBasket(BasketController bc)
    {
        BasketUI basketInList = m_SelectedBaskets.FirstOrDefault(x => x.basketId == bc.m_basketNumber);
        if (basketInList != null)
        {
            //Remove
            bc.SetHighlightColor(false);
            m_SelectedBaskets.Remove(basketInList);
            UpdateTicks(m_SelectedBaskets.Count);
            m_startButton.interactable = false;
            m_handHelper.gameObject.SetActive(false);
        }
        else
        {
            if (m_SelectedBaskets.Count < max_number_of_selected_basket)
            {
                bc.SetHighlightColor(true);
                BasketUI newBasket = new BasketUI();
                newBasket.basketId = bc.m_basketNumber;
                newBasket.hardMode = bc.m_hardMode;
                m_SelectedBaskets.Add(newBasket);
                UpdateTicks(m_SelectedBaskets.Count);
                if (m_SelectedBaskets.Count == max_number_of_selected_basket)
                {
                    m_startButton.interactable = true;
                    m_handHelper.gameObject.SetActive(true);
                }
                    
            }                    
        }
    }

    public void UpdateBasketType(BasketController bc)
    {
        BasketUI basketInList = m_SelectedBaskets.FirstOrDefault(x => x.basketId == bc.m_basketNumber);
        if (basketInList != null)
        {
            //Change current hard mode
            basketInList.hardMode = bc.m_hardMode;
        }
    }

    public void PrepareTherapy()
    {
        m_progressScreen.SetActive(true);
        TherapyLIROManager.Instance.m_onUpdateProgress += UpdateProgressBar;
        TherapyLIROManager.Instance.StartCoreTherapy(m_SelectedBaskets);        
    }

    private void UpdateProgressBar(int amount)
    {
        m_progressTherapy.text = string.Format(m_stringProgressBarFormat, amount);
        if (amount >= 100)
        {
            //Escaping to the world map select
            StartCoroutine(BackToWorldMap());
        }
    }

    private void UpdateTicks(int count)
    {
        for (int i = 0; i < 4; i++)
        {
                tickImages[i].enabled = i < count;
        }
    }

    private void UpdateBasketCounters()
    {
        int[] currNumbers = TherapyLIROManager.Instance.GetUserProfile.m_userProfile.m_BasketTracking.m_basketTrackingCounters;
        for (int i = 0; i < 8; i++)
        {
            m_basketCounterTexts[i].text = currNumbers[i].ToString();
        }
    }

    private IEnumerator BackToWorldMap()
    {
        yield return StartCoroutine(TherapyLIROManager.Instance.SaveBasketCompletedInfo());
        yield return new WaitForSeconds(2);
        MadLevel.LoadLevelByName("MainHUB");
    }

}
