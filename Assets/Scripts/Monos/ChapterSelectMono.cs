using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChapterSelectMono : MonoBehaviour {


    [SerializeField]
    private ScrollRect m_ScrollRect;
    public ScrollRect ScrollRect { get { return m_ScrollRect; } }

    [SerializeField]
    private GridLayoutGroup m_Grid;
    public GridLayoutGroup Grid
    {
        get { return m_Grid; }
    }
    [SerializeField]
    private Scrollbar m_Bar;
    public Scrollbar Scrollbar { get { return m_Bar; } }

    [SerializeField]
    private Image m_LeftArrow, m_RightArrow;
    public Image LeftArrow { get { return m_LeftArrow; } }
    public Image RightArrow { get { return m_RightArrow; } }

	[SerializeField]
	private GameObject m_endPuzzleEffect;
	public GameObject EndPuzzleEffect{ get { return m_endPuzzleEffect; } }

    //AndreaLIRO: eliminating demo from levels
    //[SerializeField]
    //private GameObject m_DemoButton;
    //public GameObject DemoButton { get { return m_DemoButton; } }
    
    public void SavePinballGame()
    {
        StartCoroutine(SaveGameState());
    }

    private IEnumerator SaveGameState()
    {
        yield return new WaitForEndOfFrame();
        GameStateSaver.Instance.SaveGameProgress();
    }

    //DEPRECATED
    public void Drag()
    {
        //StateChapterSelect.Instance.Drag();
    }
    public void DragEnd()
    {
        //StateChapterSelect.Instance.DragEnd();
    }
    public void LeftArrowButton()
    {
        //StateChapterSelect.Instance.LeftArrow();
    }
    public void RightArrowButton()
    {
        //StateChapterSelect.Instance.RightArrow();
    }

	public void OpenMenu()
	{
        GameObject menu = GameObject.FindGameObjectWithTag("MenuUI");
        if (menu != null)
        {
            ShowPanels sp = menu.GetComponentInChildren<ShowPanels>();
            if(sp != null)
                sp.ShowInitialMenu(false);
            else
                Debug.LogError("MENU UI tag found but not showpanels script attached");
        }
        else
        {
            Debug.LogError("Open Menu has been but not Menu has been found");
        }
        
	}

    public void OpenUploadScreen()
    {
        try
        {
            GameObject menu = GameObject.FindGameObjectWithTag("MenuUI");
            if (menu != null)
            {
                ShowPanels sp = menu.GetComponentInChildren<ShowPanels>();
                if (sp != null)
                    sp.ShowUploadUI();
                else
                    Debug.LogError("MENU UI tag found but not showpanels script attached");
            }
            else
            {
                Debug.LogError("Open Menu has been but not Menu has been found");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ChapterSelectMono: " + ex.Message);
        }

    }

}
