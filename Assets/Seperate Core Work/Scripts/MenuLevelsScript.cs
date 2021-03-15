//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;

//public class MenuLevelsScript : MonoBehaviour {

//	// link to GameControlScript
//	public static GameControlScript m_gameControlScript;

//	// list of button level game object
//	List<Button> m_lsBtnLevel = new List<Button>();

//	List<Text> m_lsBtnLevelText = new List<Text>();

//	private Color baseColor;
//	private Color highColor;
//	private int previousHighlighted;

//	//----------------------------------------------------------------------------------------------------
//	// Use this for initialization
//	//----------------------------------------------------------------------------------------------------
//	void ForcedStart () 
//	{
//		m_lsBtnLevel.Clear();

//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel1").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel2").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel3").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel4").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel5").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel6").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel7").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel8").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel9").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel10").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel11").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel12").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel13").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel14").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel15").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel16").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel17").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel18").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel19").GetComponent<Button>());
//		m_lsBtnLevel.Add(GameObject.Find ("BtnLevel20").GetComponent<Button>());

//		for (int j = 0; j < m_lsBtnLevel.Count; j++) 
//		{
//			Text[] texts = m_lsBtnLevel[j].GetComponentsInChildren<Text>();
//			m_lsBtnLevelText.Add (texts [0]);
//		}

//		baseColor = new Color(0.0f, 0.835f, 1.0f);
//		highColor = Color.green;
//	}

//	//----------------------------------------------------------------------------------------------------
//	// Update is called once per frame
//	//----------------------------------------------------------------------------------------------------
//	void Update () 
//	{

//	}

//	//----------------------------------------------------------------------------------------------------
//	// SetGameControlScript - to be called from GameControlScript
//	//----------------------------------------------------------------------------------------------------
//	public void SetGameControlScript (GameControlScript gcs) 
//	{
//		m_gameControlScript = gcs;
//	}


//	public void Initialize()
//	{
//		ForcedStart();
//	}

//	//----------------------------------------------------------------------------------------------------
//	// SetAdminMode - to be called from GameControlScript
//	//----------------------------------------------------------------------------------------------------
//	public void SetAdminMode (bool bAdmin) 
//	{
//		//Text[] texts = m_lsBtnLevel[0].GetComponentsInChildren<Text>();
//		//texts [0].text = "V1";

//		if (bAdmin) 
//		{
//			m_lsBtnLevelText [0].text = "N2";
//			m_lsBtnLevelText [1].text = "N1";
//			m_lsBtnLevelText [2].text = "N4";
//			m_lsBtnLevelText [3].text = "V3";
//			m_lsBtnLevelText [4].text = "PRN2";
//			m_lsBtnLevelText [5].text = "N5";
//			m_lsBtnLevelText [6].text = "N6";
//			m_lsBtnLevelText [7].text = "V2";
//			m_lsBtnLevelText [8].text = "V1";
//			m_lsBtnLevelText [9].text = "A1";
//			m_lsBtnLevelText [10].text = "A2";
//			m_lsBtnLevelText [11].text = "V4";
//			m_lsBtnLevelText [12].text = "PREP1";
//			m_lsBtnLevelText [13].text = "PREP2";
//			m_lsBtnLevelText [14].text = "PHN_N4";
//			m_lsBtnLevelText [15].text = "PHN_V4";
//			m_lsBtnLevelText [16].text = "NS3_A2";
//			m_lsBtnLevelText [17].text = "NS4_N6";
//			m_lsBtnLevelText [18].text = "NS1_V2";
//			m_lsBtnLevelText [19].text = "NS2_V3";
//			SetAllBtnLevelDisabled (true);
//		} 
//		else 
//		{
//			m_lsBtnLevelText [0].text = "Level 1";
//			m_lsBtnLevelText [1].text = "Level 2";
//			m_lsBtnLevelText [2].text = "Level 3";
//			m_lsBtnLevelText [3].text = "Level 4";
//			m_lsBtnLevelText [4].text = "Level 5";
//			m_lsBtnLevelText [5].text = "Level 6";
//			m_lsBtnLevelText [6].text = "Level 7";
//			m_lsBtnLevelText [7].text = "Level 8";
//			m_lsBtnLevelText [8].text = "Level 9";
//			m_lsBtnLevelText [9].text = "Level 10";
//			m_lsBtnLevelText [10].text = "Level 11";
//			m_lsBtnLevelText [11].text = "Level 12";
//			m_lsBtnLevelText [12].text = "Level 13";
//			m_lsBtnLevelText [13].text = "Level 14";
//			m_lsBtnLevelText [14].text = "Level 15";
//			m_lsBtnLevelText [15].text = "Level 16";
//			m_lsBtnLevelText [16].text = "Level 17";
//			m_lsBtnLevelText [17].text = "Level 18";
//			m_lsBtnLevelText [18].text = "Level 19";
//			m_lsBtnLevelText [19].text = "Level 20";
//			SetAllBtnLevelDisabled (false);
//		}
//	}
		
//	//----------------------------------------------------------------------------------------------------
//	// SetBtnLevelDisabled - to be called from GameControlScript
//	//----------------------------------------------------------------------------------------------------
//	public void SetAllBtnLevelDisabled (bool bStatus) 
//	{
//		for (int j = 0; j < m_lsBtnLevel.Count; j++)
//			m_lsBtnLevel[j].interactable = bStatus;
			
//	}

//	//----------------------------------------------------------------------------------------------------
//	// SetBtnLevelDisabled - to be called from GameControlScript
//	//----------------------------------------------------------------------------------------------------
//	public void SetBtnLevelDisabled (int intLevel, bool bStatus) 
//	{
//		m_lsBtnLevel[intLevel].interactable = bStatus;
//	}

//	//----------------------------------------------------------------------------------------------------
//	// SetBtnLevelHighlight - to be called from GameControlScript
//	//----------------------------------------------------------------------------------------------------
//	public void SetBtnLevelHighlight (int intLevel) 
//	{
//		//baseColor = m_lsBtnLevel[intLevel].colors.normalColor;
//		previousHighlighted = intLevel;

//		SetColor(highColor, intLevel);
//	}

//	private void SetColor(Color col, int lvl)
//	{
//		ColorBlock cb = m_lsBtnLevel[lvl].colors;
//		cb.normalColor = col;
		
//		m_lsBtnLevel[lvl].colors = cb;
//	}

//	public void Reset()
//	{
//		SetColor(baseColor,previousHighlighted);
//	}

//	//----------------------------------------------------------------------------------------------------
//	// OnClickButtonLevel - to be called prefab inspector
//	//----------------------------------------------------------------------------------------------------
//	public void OnClickButtonLevel (int intLevel) 
//	{
//		//TODO change this part into something pretty
//		SetColor(baseColor, previousHighlighted);
//		//SetColor(highColor, intLevel);

//		Debug.Log ("*** OnClickButtonLevel - intLevel = " + intLevel);
//		if (intLevel == 100)
//			m_gameControlScript.OnClickButtonAdmin ();
//		else
//			m_gameControlScript.OnClickButtonLevel (intLevel);
//	}
//}
