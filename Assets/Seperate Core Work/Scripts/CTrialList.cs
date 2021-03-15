//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Xml.Linq;
//using System.Linq;
//using System.Xml;
//using System;

////************************************************************************************************************************
//// CTrialList
////************************************************************************************************************************
//public class CTrialList {
	
//	#region singleton
//	private static readonly CTrialList instance = new CTrialList();
//	public static CTrialList Instance
//	{
//		get
//		{
//			return instance;
//		}
//	}
//	#endregion
	
//	// list of trials / challenges
//	List<CTrial> m_lsTrial = new List<CTrial>();
	
//	// list of responses
//	List<CResponse> m_lsResponse = new List<CResponse>();
	
//	// index of the current displayed trial/challenge, zero-based
//	int m_intCurIdx = -1;
	
//	// end of game if user has achieved 60% (based on user's first response) correct of current level
//	int m_intTotalCorrect = 0;
	
//	// current level
//	int m_intCurLevel = 0;
	
//	// current level
//	int m_intCurLevelAttempt = 0;
	
//	// list of completed levels
//	List<CCompletedLevel> m_lsCompletedLevel = new List<CCompletedLevel>();
	
//	// list of completed levels
//	//List<int> m_lsCompletedLevel = new List<int>();
	
//	// list of completed levels
//	//List<float> m_lsCompletedLevelAccuracy = new List<float>();
	
//	// level start time, this is to keep track how long does the patient take to complete a level 
//	DateTime m_dtCurLevelStartTime; 
	
//	float m_fTotalDurationMin = 0f;

//	// list of statistics for dates
//	List<CStatisticDay> m_lsStatisticDay = new List<CStatisticDay>();
	
//	//----------------------------------------------------------------------------------------------------
//	// Init
//	//----------------------------------------------------------------------------------------------------
//	public void Init()
//	{
		
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// getCurLevel
//	//----------------------------------------------------------------------------------------------------
//	public int getCurLevel()
//	{
//		return m_intCurLevel;
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// getCurAudioFolder
//	//----------------------------------------------------------------------------------------------------
//	public string getCurAudioFolder()
//	{
//		string strFolder = "";
//		switch (m_intCurLevel) 
//		{
//		case 0:
//			strFolder = "N2_MALE/";
//			break;
//		case 1:
//			strFolder = "N1_FEMALE/";
//			break;
//		case 2:
//			strFolder = "N4_MALE/";
//			break;
//		case 3:
//			strFolder = "V3_FEMALE/";
//			break;
//		case 4:
//			strFolder = "PRN2_FEMALE/";
//			break;
//		case 5:
//			strFolder = "N5_MALE/";
//			break;
//		case 6:
//			strFolder = "N6_FEMALE/";
//			break;
//		case 7:
//			strFolder = "V2_MALE/";
//			break;
//		case 8:
//			strFolder = "V1_FEMALE/";
//			break;
//		case 9:
//			strFolder = "A1_FEMALE/";
//			break;
//		case 10:
//			strFolder = "A2_FEMALE/";
//			break;
//		case 11:
//			strFolder = "V4_MALE/";
//			break;
//		case 12:
//			strFolder = "PREP1_FEMALE/";
//			break;
//		case 13:
//			strFolder = "PREP2_FEMALE/";
//			break;
//			// two levels with phn voice
//		case 14:
//			strFolder = "N4_MALE/";
//			break;
//		case 15:
//			strFolder = "V4_MALE/";
//			break;
//			// four levels with background noise
//		case 16:
//			strFolder = "A2_FEMALE/";
//			break;
//		case 17:
//			strFolder = "N6_FEMALE/";
//			break;
//		case 18:
//			strFolder = "V2_MALE/";
//			break;
//		case 19:
//			strFolder = "V3_FEMALE/";
//			break;		
//		default:
//			break;
//		}
//		return strFolder;
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// getLsCompletedLevel
//	//----------------------------------------------------------------------------------------------------
//	public List<int> getLsCompletedLevel()
//	{
//		List<int> lsCompletedLevel = new List<int>();
		
//		for (int j = 0; j < m_lsCompletedLevel.Count; j++)
//			lsCompletedLevel.Add (m_lsCompletedLevel[j].m_intLevel);
		
//		return lsCompletedLevel;
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// LoadTrials: load from xml file all stimuli's images, audio and target index for all trials/challenges
//	//----------------------------------------------------------------------------------------------------
//	public void LoadTrials(int intLevel)
//	{
//		m_intCurLevel = intLevel;
		
//		// if xml has already been loaded, then return
//		//if (m_lsTrial.Count > 0) return;
		
//		m_lsTrial.Clear ();
//		m_lsResponse.Clear ();
		
//		// which triallist to load
//		string strTrialList = "";
//		switch (intLevel) 
//		{
//		case 0:
//			strTrialList = "level01-N2-xml";
//			break;
//		case 1:
//			strTrialList = "level02-N1-xml";
//			break;
//		case 2:
//			strTrialList = "level03-N4-xml";
//			break;
//		case 3:
//			strTrialList = "level04-V3-xml";
//			break;
//		case 4:
//			strTrialList = "level05-PRN2-xml";
//			break;
//		case 5:
//			strTrialList = "level06-N5-xml";
//			break;
//		case 6:
//			strTrialList = "level07-N6-xml";
//			break;
//		case 7:
//			strTrialList = "level08-V2-xml";
//			break;
//		case 8:
//			strTrialList = "level09-V1-xml";
//			break;
//		case 9:
//			strTrialList = "level10-A1-xml";
//			break;
//		case 10:
//			strTrialList = "level11-A2-xml";
//			break;
//		case 11:
//			strTrialList = "level12-V4-xml";
//			break;
//		case 12:
//			strTrialList = "level13-PREP1-xml";
//			break;
//		case 13:
//			strTrialList = "level14-PREP2-xml";
//			break;
//			// two levels with phn voice
//		case 14:
//			strTrialList = "level03-N4-xml";
//			break;
//		case 15:
//			strTrialList = "level12-V4-xml";
//			break;
//			// four levels with background noise
//		case 16:
//			strTrialList = "level11-A2-xml";
//			break;
//		case 17:
//			strTrialList = "level07-N6-xml";
//			break;
//		case 18:
//			strTrialList = "level08-V2-xml";
//			break;
//		case 19:
//			strTrialList = "level04-V3-xml";
//			break;
//		default:
//			break;
//		}
		
//		// load xml
//		//XElement root = XElement.Load(strXmlFile);
//		//TextAsset textAsset = Resources.Load("Doc/triallist") as TextAsset;
//		TextAsset textAsset = Resources.Load("Doc/" + strTrialList) as TextAsset;
//		XElement root = XElement.Parse(textAsset.text);     
		
//		m_lsTrial = (
//			from el in root.Elements("node")
//			select new CTrial  
//			{
//			m_lsStimulus = (
//				from el2 in el.Elements("stimulus")
//				select new CStimulus
//				{
//				m_strName = (string)el2.Element("name"),
//				m_strImage = (string)el2.Element("image"),
//				m_strType = (string)el2.Element("type"),
//			}
//			).ToList(),
//			m_strTargetAudio = (string)el.Element("targetAudio"),	
//			m_intTargetIdx = (int)el.Element("targetIdx")	
//		}
//		).ToList(); 
		
//		// shuffle stimuli in each trial
//		for (var i = 0; i < m_lsTrial.Count; ++i) 		
//			m_lsTrial [i].RandomizeStimuli ();		
		
//		// shuffle trials in m_lsTrial using Knuth shuffle algorithm :: courtesy of Wikipedia 
//		for (int t = 0; t < m_lsTrial.Count; t++ )
//		{
//			CTrial tmp = m_lsTrial[t];
//			int r = UnityEngine.Random.Range(t, m_lsTrial.Count);
//			m_lsTrial[t] = m_lsTrial[r];
//			m_lsTrial[r] = tmp;
//		}
		
//		//tell the score master how many questions we're asking
//		ScoreMaster.SetQuestionAmount(m_lsTrial.Count);
		
//		// add empty responses
//		for (var i = 0; i < m_lsTrial.Count; ++i) 
//		{
//			CResponse res = new CResponse();
//			m_lsResponse.Add(res);
//		}
		
//		// reset current idx
//		m_intCurIdx = -1;
//		m_intTotalCorrect = 0;
//	}		
	
//	//----------------------------------------------------------------------------------------------------
//	// UpdateResponseList
//	//----------------------------------------------------------------------------------------------------
//	public void UpdateResponseList(CResponse response)
//	{
//		if (m_intCurIdx < 0)
//			return; // no reponse to update 
		
//		if (response.m_intScore > 0)
//			m_intTotalCorrect++; 
		
//		m_lsResponse [m_intCurIdx].m_lsSelectedStimulusIdx = new List<int> (response.m_lsSelectedStimulusIdx);
//		m_lsResponse [m_intCurIdx].m_fRTSec = response.m_fRTSec;
//		m_lsResponse [m_intCurIdx].m_intReplayBtnCtr = response.m_intReplayBtnCtr;
//		m_lsResponse [m_intCurIdx].m_intScore = response.m_intScore;
//		m_lsResponse [m_intCurIdx].m_intCoinNum = response.m_intCoinNum;

//        Dictionary<string, string> challenge_insert = new Dictionary<string, string>();

//        challenge_insert.Add("patient", DatabaseXML.Instance.PatientId.ToString());
//        challenge_insert.Add("date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//        challenge_insert.Add("category", "1");
//        string foils = String.Empty;
//        for (int i = 0; i < response.m_lsSelectedStimulusIdx.Count; i++)
//        {
//            foils += " ";
//            foils += response.m_lsSelectedStimulusIdx[i].ToString();
//        }
//        challenge_insert.Add("foil_number", foils);
//        challenge_insert.Add("foil_type", "1");
//        challenge_insert.Add("image_list", "99");
//        challenge_insert.Add("accuracy", response.m_intScore.ToString());

//        //AndreaLIRO: removing writing to database xml
//        //DatabaseXML.Instance.WriteDatabaseXML(challenge_insert, DatabaseXML.Instance.therapy_challenge_insert);

//        /*for (int k = 0; k < m_lsResponse.Count; k++) 
//		{
//			for (int i = 0; i < m_lsResponse[k].m_lsSelectedStimulusIdx.Count; i++) 
//				Debug.Log ("*** CTrialList - UpdateReponseList - selectedIdx = " + m_lsResponse[k].m_lsSelectedStimulusIdx [i]);
//			Debug.Log ("*** CTrialList - UpdateReponseList - Score = " + m_lsResponse[k].m_intScore + " - coinNum = " + m_lsResponse[k].m_intCoinNum);
//		}*/
//    }
	
//	//----------------------------------------------------------------------------------------------------
//	// IsEndOfLevel
//	//----------------------------------------------------------------------------------------------------
//	public bool IsEndOfLevel(bool bIsAdminMode)
//	{
//		m_intCurIdx++;
		
//		// end of game if user has achieved 80% (based on user's first response) correct of current level
//		float fCorrect = (m_intTotalCorrect / (float)m_lsTrial.Count) * 100; // when using divide, one of the operands must be float
//		if (fCorrect > 80f) 
//		{
//			if (!bIsAdminMode)
//			{
//				AddCompletedLevel();
//				UpdateStatisticDay();
//				SaveTrials ("");
//				m_intCurLevel++;
//				if (m_intCurLevel >= 20)
//					m_intCurLevel = 0;
//				m_intCurLevelAttempt = 0;
//				SaveUserProfile();
//			}
//			return true;
//		}
//		else if (m_intCurIdx >= m_lsTrial.Count) 
//		{
//			if (!bIsAdminMode)
//			{
//				AddCompletedLevel();
//				UpdateStatisticDay();
//				SaveTrials ("");
//				// remain at current level as the patient has not achieved 80% 
//				m_intCurLevelAttempt++;
//				/*m_intCurLevel++;
//				if (m_intCurLevel >= 20)
//					m_intCurLevel = 0;*/
//				SaveUserProfile();
//			}
//			return true;
//		}
		
//		/*
//		m_intCurIdx++;
//		float fCorrect = (m_intTotalCorrect / (float)m_lsTrial.Count) * 100; // when using divide, one of the operands must be float

//		if (m_intCurIdx >= m_lsTrial.Count) 
//		{
//			if (!bIsAdminMode)
//			{
//				m_lsCompletedLevel.Add(m_intCurLevel);
//				m_lsCompletedLevelAccuracy.Add(fCorrect);
//				SaveTrials ("");
//				m_intCurLevel++;
//				if (m_intCurLevel >= 20)
//					m_intCurLevel = 0;
//				SaveUserProfile();
//			}
//			return true;
//		}

//		//Debug.Log("IsEndOfLevel - m_intTotalCorrect = " + m_intTotalCorrect + " - m_lsTrial.Count = " + m_lsTrial.Count);

//		// end of game if user has achieved 60% (based on user's first response) correct of current level
//		//float fCorrect = (m_intTotalCorrect / (float)m_lsTrial.Count) * 100; // when using divide, one of the operands must be float
//		if (fCorrect > 60f) 
//		{
//			if (!bIsAdminMode)
//			{
//				m_lsCompletedLevel.Add(m_intCurLevel);
//				m_lsCompletedLevelAccuracy.Add(fCorrect);
//				SaveTrials ("");
//				m_intCurLevel++;
//				if (m_intCurLevel >= 20)
//					m_intCurLevel = 0;
//				SaveUserProfile();
//			}
//			return true;
//		}*/
		
//		return false;
//	}

//	private double getStandardDeviation(List<double> doubleList)  
//	{  
//		double average = doubleList.Average();  
//		double sumOfDerivation = 0;  
//		foreach (double value in doubleList)  
//		{  
//			sumOfDerivation += (value) * (value);  
//		}  
//		double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count - 1);  
//		return Math.Sqrt(sumOfDerivationAverage - (average*average));  
//	}  

//	//----------------------------------------------------------------------------------------------------
//	// AddCompletedLevel
//	//----------------------------------------------------------------------------------------------------
//	private void AddCompletedLevel()
//	{
//		// calculate mean RT of all correct responses on first attempt
//		List<float> lsRTCorrectTrials = new List<float> ();
//		// calculate mean RT of all presented responses 
//		List<float> lsRTAllTrials = new List<float> ();

//		for (int i = 0; i < m_lsResponse.Count; i++) 
//		{
//			if (m_lsResponse[i].m_lsSelectedStimulusIdx.Count > 0)
//				lsRTAllTrials.Add(m_lsResponse[i].m_fRTSec);

//			if (m_lsResponse[i].m_intScore >= 1)
//				lsRTCorrectTrials.Add(m_lsResponse[i].m_fRTSec);
//		}

//		float fCorrectTrialsAverage = lsRTCorrectTrials.Average();
//		float sumOfSquaresOfDifferences = lsRTCorrectTrials.Select(val => (val - fCorrectTrialsAverage) * (val - fCorrectTrialsAverage)).Sum();
//		float fCorrectTrialsSD = (float)Math.Sqrt(sumOfSquaresOfDifferences / lsRTCorrectTrials.Count); 

//		float fAllTrialsAverage = lsRTAllTrials.Average();
//		sumOfSquaresOfDifferences = lsRTAllTrials.Select(val => (val - fAllTrialsAverage) * (val - fAllTrialsAverage)).Sum();
//		float fAllTrialsSD = (float)Math.Sqrt(sumOfSquaresOfDifferences / lsRTAllTrials.Count); 

//		CCompletedLevel completedLevel = new CCompletedLevel ();
//		completedLevel.m_intLevel = m_intCurLevel;
//		completedLevel.m_intCorrectTrials = m_intTotalCorrect;
//		completedLevel.m_intIncorrectTrials = m_intCurIdx - m_intTotalCorrect;
//		completedLevel.m_intTotalTrials = m_lsTrial.Count;
//		completedLevel.m_fAccuracy = (completedLevel.m_intCorrectTrials / (float)(completedLevel.m_intCorrectTrials + completedLevel.m_intIncorrectTrials)) * 100;
//		completedLevel.m_fCorrectTrialsRTMeanSec = fCorrectTrialsAverage;
//		completedLevel.m_fCorrectTrialsRTStdDSec = fCorrectTrialsSD;
//		completedLevel.m_fAllTrialsRTMeanSec = fAllTrialsAverage;
//		completedLevel.m_fAllTrialsRTStdDSec = fAllTrialsSD;
//		completedLevel.m_fDurationMin = (float)(DateTime.Now - m_dtCurLevelStartTime).TotalMinutes;
		
//		m_lsCompletedLevel.Add(completedLevel);
		
//		m_fTotalDurationMin = m_fTotalDurationMin + completedLevel.m_fDurationMin;
//	}

//	//----------------------------------------------------------------------------------------------------
//	// UpdateStatisticDay
//	//----------------------------------------------------------------------------------------------------
//	private void UpdateStatisticDay()
//	{
//		bool bFound = false;
//		int i = 0;
//		for (i = 0; i < m_lsStatisticDay.Count; i++) 
//		{
//			if (m_lsStatisticDay[i].m_strDate == DateTime.Now.ToString("yyyy_MM_dd"))
//			{
//				bFound = true;
//				break;
//			}
//		}
//		if (!bFound) 
//		{
//			// new date
//			CStatisticDay sd = new CStatisticDay ();
//			sd.m_strDate = DateTime.Now.ToString ("yyyy_MM_dd");
//			sd.m_fTherapyTimeMin = m_lsCompletedLevel [m_lsCompletedLevel.Count - 1].m_fDurationMin;
//			m_lsStatisticDay.Add (sd);
//		} 
//		else 
//		{
//			// existing date - accumulate therapy time
//			m_lsStatisticDay[i].m_fTherapyTimeMin += m_lsCompletedLevel [m_lsCompletedLevel.Count - 1].m_fDurationMin;
//		}
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// GetNextTrial
//	//----------------------------------------------------------------------------------------------------
//	public CTrial GetNextTrial()
//	{
//		if (m_intCurIdx >= m_lsTrial.Count) 
//			return null;
		
//		if (m_intCurIdx == 0)
//			m_dtCurLevelStartTime = DateTime.Now;
		
//		return m_lsTrial [m_intCurIdx];
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// SaveTrials
//	//----------------------------------------------------------------------------------------------------
//	private void SaveTrials(string strFilename)
//	{
//		/*string myFolderLocation = "/storage/emulated/0/DCIM/MyFolder/";
//		string myScreenshotLocation = myFolderLocation + myFilename;
		
//		//ENSURE THAT FOLDER LOCATION EXISTS
//		if(!System.IO.Directory.Exists(myFolderLocation)){
//			System.IO.Directory.CreateDirectory(myFolderLocation);
//		}*/
		
//		// add xml file name to m_lsCompletedLevel
//		string strXmlFilename = DateTime.Now.ToString ("yyyy_MM_dd") + "_level_" + m_intCurLevel + "_" + m_intCurLevelAttempt + ".xml";
//		m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_strXmlFile = strXmlFilename;
		
//		// Create the XmlDocument.
//		XmlDocument doc = new XmlDocument();
//		doc.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
//		            "<root>" +
//		            "</root>");
		
//		// Save the document to a file. White space is preserved (no white space).
//		string strFile = Application.persistentDataPath + "/" + strXmlFilename;
		
//		// save level accuracy & duration
//		XmlElement xmlNode = doc.CreateElement("correctTrials");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_intCorrectTrials.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);

//		xmlNode = doc.CreateElement("incorrectTrials");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_intIncorrectTrials.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);

//		xmlNode = doc.CreateElement("accuracy");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fAccuracy.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);

//		xmlNode = doc.CreateElement("correctTrialsRTMeanSec");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fCorrectTrialsRTMeanSec.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);

//		xmlNode = doc.CreateElement("correctTrialsRTStdDSec");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fCorrectTrialsRTStdDSec.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);

//		xmlNode = doc.CreateElement("allTrialsRTMeanSec");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fAllTrialsRTMeanSec.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);
		
//		xmlNode = doc.CreateElement("allTrialsRTStdDSec");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fAllTrialsRTStdDSec.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);
		
//		xmlNode = doc.CreateElement("durationMin");
//		xmlNode.InnerText = m_lsCompletedLevel[m_lsCompletedLevel.Count-1].m_fDurationMin.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);
		
//		for (int i = 0; i < m_lsTrial.Count; i++) 
//		{
//			XmlElement xmlTrial = doc.CreateElement("trial");
//			XmlAttribute attr = doc.CreateAttribute("idx");
//			attr.Value = i.ToString();
//			xmlTrial.SetAttributeNode(attr); 
			
//			// add stimuli
//			for (var j = 0; j < m_lsTrial[i].m_lsStimulus.Count; j++)
//			{
//				XmlElement xmlStimulus = doc.CreateElement("stimulus");
				
//				XmlElement xmlChild = doc.CreateElement("name");
//				xmlChild.InnerText = m_lsTrial[i].m_lsStimulus[j].m_strName;
//				xmlStimulus.AppendChild(xmlChild);
				
//				xmlChild = doc.CreateElement("type");
//				xmlChild.InnerText = m_lsTrial[i].m_lsStimulus[j].m_strType;
//				xmlStimulus.AppendChild(xmlChild);
				
//				xmlTrial.AppendChild(xmlStimulus);
//			}
			
//			XmlElement xmlChild2 = doc.CreateElement("targetIdx");
//			xmlChild2.InnerText = m_lsTrial[i].m_intTargetIdx.ToString();
//			xmlTrial.AppendChild(xmlChild2);
			
//			// add responses
//			XmlElement xmlSelectedIdx = doc.CreateElement("selectedStimulusIdx");
//			for (var j = 0; j < m_lsResponse[i].m_lsSelectedStimulusIdx.Count; j++)
//			{
//				XmlElement xmlChild = doc.CreateElement("idx");
//				xmlChild.InnerText = m_lsResponse[i].m_lsSelectedStimulusIdx[j].ToString();
//				xmlSelectedIdx.AppendChild(xmlChild);
//			}
//			xmlTrial.AppendChild(xmlSelectedIdx);

//			xmlChild2 = doc.CreateElement("rtSec");
//			xmlChild2.InnerText = m_lsResponse[i].m_fRTSec.ToString();
//			xmlTrial.AppendChild(xmlChild2);

//			xmlChild2 = doc.CreateElement("replayCtr");
//			xmlChild2.InnerText = m_lsResponse[i].m_intReplayBtnCtr.ToString();
//			xmlTrial.AppendChild(xmlChild2);
			
//			xmlChild2 = doc.CreateElement("score");
//			xmlChild2.InnerText = m_lsResponse[i].m_intScore.ToString();
//			xmlTrial.AppendChild(xmlChild2);
			
//			xmlChild2 = doc.CreateElement("coinNum");
//			xmlChild2.InnerText = m_lsResponse[i].m_intCoinNum.ToString();
//			xmlTrial.AppendChild(xmlChild2);
			
//			doc.DocumentElement.AppendChild(xmlTrial);
//		}
		
//		doc.PreserveWhitespace = true;
//		doc.Save(strFile);
//		//doc.Save(strPatientSt02Dir + "\\" + strPatientLabel + "_st02.xml");
		
//		Debug.Log ("SaveTrials - " + strFile);
		
//		/*#if UNITY_ANDROID
//		//REFRESHING THE ANDROID PHONE PHOTO GALLERY IS BEGUN
//		AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//		if (classPlayer != null)
//		{
//			AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");        
//			AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");        
//			AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2]{"android.intent.action.MEDIA_MOUNTED", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + strFile)});        
//			objActivity.Call ("sendBroadcast", objIntent);
//		}
//		#endif*/
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// LoadUserProfile 
//	//----------------------------------------------------------------------------------------------------
//	public void LoadUserProfile()
//	{
//		// init
//		m_intCurLevel = 0;
//		m_lsCompletedLevel.Clear();
//		//m_lsCompletedLevelAccuracy.Clear();
//		m_fTotalDurationMin = 0f;
		
//		//string strXmlFile = Application.persistentDataPath + "/" + DateTime.Now.ToString("yyyy_MM_dd") + "_user_profile.xml";
//		string strXmlFile = Application.persistentDataPath + "/user_profile.xml";
		
//		// check if file exists
//		if (!System.IO.File.Exists (strXmlFile))
//			return;
		
//		XElement root = XElement.Load(strXmlFile);
		
//		m_intCurLevel = (int)root.Element ("curLevel");
//		m_intCurLevelAttempt = 0;
//		Debug.Log("*** m_intCurLevelAttempt = " + m_intCurLevelAttempt);
//		if (root.Element ("curLevelAttempt") != null) 
//		{
//			m_intCurLevelAttempt = (int)root.Element ("curLevelAttempt");
//			Debug.Log("**** m_intCurLevelAttempt *** = " + m_intCurLevelAttempt);
//		}
		
//		m_fTotalDurationMin = (float)root.Element ("totalTherapyTimeMin");

//		// load statistics day
//		m_lsStatisticDay = (
//			from el in root.Element("statistic").Elements("date")
//			select new CStatisticDay
//			{
//			m_strDate = (string)el.Element("date"),
//			m_fTherapyTimeMin = (float)el.Element("therapyTimeMin"),
//		}
//		).ToList(); 
		
//		m_lsCompletedLevel = (
//			from el in root.Element("levelCompleted").Elements("level")
//			select new CCompletedLevel
//			{
//			m_intLevel = (int)el.Element("level"),
//			m_intCorrectTrials = (int)el.Element("correctTrials"),
//			m_intIncorrectTrials = (int)el.Element("incorrectTrials"),
//			m_intTotalTrials = (int)el.Element("totalTrials"),
//			m_fAccuracy = (float)el.Element("accuracy"),
//			m_fCorrectTrialsRTMeanSec = (float)el.Element("correctTrialsRTMeanSec"),
//			m_fCorrectTrialsRTStdDSec = (float)el.Element("correctTrialsRTStdDSec"),
//			m_fAllTrialsRTMeanSec = (float)el.Element("allTrialsRTMeanSec"),
//			m_fAllTrialsRTStdDSec = (float)el.Element("allTrialsRTStdDSec"),
//			m_fDurationMin = (float)el.Element("durationMin"),
//			m_strXmlFile = (string)el.Element("xmlfile"),
//		}
//		).ToList(); 
		
//		/*m_lsCompletedLevel = (
//			from el in root.Element("levelCompleted").Elements("level")
//			select (int)el
//		).ToList(); 	

//		m_lsCompletedLevelAccuracy = (
//			from el in root.Element("levelCompleted").Elements("accuracy")
//			select (float)el
//		).ToList(); 	

//		if (m_lsCompletedLevelAccuracy.Count == 0) 
//		{
//			for (int j = 0; j < m_lsCompletedLevel.Count; j++)
//			{
//				m_lsCompletedLevelAccuracy.Add(0f);
//			}
//		}*/		
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// SaveUserProfile
//	//----------------------------------------------------------------------------------------------------
//	private void SaveUserProfile()
//	{				
//		// Create the XmlDocument.
//		XmlDocument doc = new XmlDocument();
//		doc.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
//		            "<root>" +
//		            "</root>");
		
//		// Save the document to a file. White space is preserved (no white space).
//		//string strFile = Application.persistentDataPath + "/" + DateTime.Now.ToString("yyyy_MM_dd") + "_user_profile.xml";
//		string strFile = Application.persistentDataPath + "/user_profile.xml";
		
//		// current level
//		XmlElement xmlNode = doc.CreateElement("curLevel");
//		xmlNode.InnerText = m_intCurLevel.ToString();
//		doc.DocumentElement.AppendChild(xmlNode);
		
//		// current level attempt
//		XmlElement xmlNode2 = doc.CreateElement("curLevelAttempt");
//		xmlNode2.InnerText = m_intCurLevelAttempt.ToString();
//		doc.DocumentElement.AppendChild(xmlNode2);
		
//		// total therapy time
//		XmlElement xmlNode3 = doc.CreateElement("totalTherapyTimeMin");
//		xmlNode3.InnerText = m_fTotalDurationMin.ToString();
//		doc.DocumentElement.AppendChild(xmlNode3);

//		// list of completed levels
//		XmlElement xmlChild0 = doc.CreateElement("statistic");
//		for (int j = 0; j < m_lsStatisticDay.Count; j++)
//		{
//			XmlElement xmlChild2 = doc.CreateElement("date");
			
//			XmlElement xmlChild3 = doc.CreateElement("date");
//			xmlChild3.InnerText = m_lsStatisticDay[j].m_strDate;
//			xmlChild2.AppendChild(xmlChild3);
			
//			xmlChild3 = doc.CreateElement("therapyTimeMin");
//			xmlChild3.InnerText = m_lsStatisticDay[j].m_fTherapyTimeMin.ToString();
//			xmlChild2.AppendChild(xmlChild3);	
						
//			xmlChild0.AppendChild(xmlChild2);
//		}
//		doc.DocumentElement.AppendChild(xmlChild0);
		
//		// list of completed levels
//		XmlElement xmlChild = doc.CreateElement("levelCompleted");
//		for (int j = 0; j < m_lsCompletedLevel.Count; j++)
//		{
//			XmlElement xmlChild2 = doc.CreateElement("level");
			
//			XmlElement xmlChild3 = doc.CreateElement("level");
//			xmlChild3.InnerText = m_lsCompletedLevel[j].m_intLevel.ToString();
//			xmlChild2.AppendChild(xmlChild3);

//			xmlChild3 = doc.CreateElement("correctTrials");
//			xmlChild3.InnerText = m_lsCompletedLevel[j].m_intCorrectTrials.ToString();
//			xmlChild2.AppendChild(xmlChild3);

//			xmlChild3 = doc.CreateElement("incorrectTrials");
//			xmlChild3.InnerText = m_lsCompletedLevel[j].m_intIncorrectTrials.ToString();
//			xmlChild2.AppendChild(xmlChild3);

//			xmlChild3 = doc.CreateElement("totalTrials");
//			xmlChild3.InnerText = m_lsCompletedLevel[j].m_intTotalTrials.ToString();
//			xmlChild2.AppendChild(xmlChild3);

//			// add child accuracy
//			XmlElement xmlChild4 = doc.CreateElement("accuracy");
//			xmlChild4.InnerText = m_lsCompletedLevel[j].m_fAccuracy.ToString();
//			xmlChild2.AppendChild(xmlChild4);

//			xmlChild4 = doc.CreateElement("correctTrialsRTMeanSec");
//			xmlChild4.InnerText = m_lsCompletedLevel[j].m_fCorrectTrialsRTMeanSec.ToString();
//			xmlChild2.AppendChild(xmlChild4);

//			xmlChild4 = doc.CreateElement("correctTrialsRTStdDSec");
//			xmlChild4.InnerText = m_lsCompletedLevel[j].m_fCorrectTrialsRTStdDSec.ToString();
//			xmlChild2.AppendChild(xmlChild4);

//			xmlChild4 = doc.CreateElement("allTrialsRTMeanSec");
//			xmlChild4.InnerText = m_lsCompletedLevel[j].m_fAllTrialsRTMeanSec.ToString();
//			xmlChild2.AppendChild(xmlChild4);
			
//			xmlChild4 = doc.CreateElement("allTrialsRTStdDSec");
//			xmlChild4.InnerText = m_lsCompletedLevel[j].m_fAllTrialsRTStdDSec.ToString();
//			xmlChild2.AppendChild(xmlChild4);
			
//			// add child duration
//			XmlElement xmlChild5 = doc.CreateElement("durationMin");
//			xmlChild5.InnerText = m_lsCompletedLevel[j].m_fDurationMin.ToString();
//			xmlChild2.AppendChild(xmlChild5);
			
//			// add child xmlfile
//			XmlElement xmlChild6 = doc.CreateElement("xmlfile");
//			xmlChild6.InnerText = m_lsCompletedLevel[j].m_strXmlFile.ToString();
//			xmlChild2.AppendChild(xmlChild6);
			
//			xmlChild.AppendChild(xmlChild2);
//		}
//		doc.DocumentElement.AppendChild(xmlChild);
		
//		// list of completed levels
//		/*XmlElement xmlChild = doc.CreateElement("levelCompleted");
//		for (int j = 0; j < m_lsCompletedLevel.Count; j++)
//		{
//			XmlElement xmlChild2 = doc.CreateElement("level");
//			xmlChild2.InnerText = m_lsCompletedLevel[j].ToString();
//			xmlChild.AppendChild(xmlChild2);
			
//			// add child accuracy
//			XmlElement xmlChild3 = doc.CreateElement("accuracy");
//			xmlChild3.InnerText = m_lsCompletedLevelAccuracy[j].ToString();
//			xmlChild.AppendChild(xmlChild3);
//		}
//		doc.DocumentElement.AppendChild(xmlChild);*/
		
//		doc.PreserveWhitespace = true;
//		doc.Save(strFile);
//		//doc.Save(strPatientSt02Dir + "\\" + strPatientLabel + "_st02.xml");
		
//		Debug.Log ("SaveUserProfile - " + strFile);	
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// ConvertCsvToXml
//	//----------------------------------------------------------------------------------------------------
//	public void ConvertCsvToXml()
//	{
//		//string strWholeFile = System.IO.File.ReadAllText ();
		
//		TextAsset textAsset = Resources.Load("Doc/level12-V4-csv") as TextAsset;
//		string strWholeFile = textAsset.text;    

//		// split into lines
//		strWholeFile = strWholeFile.Replace ('\n', '\r');
//		string [] lines = strWholeFile.Split (new char[] {'\r'}, StringSplitOptions.RemoveEmptyEntries);
		
//		// see how many rows & columns there are
//		int intNumRows = lines.Length;
//		int intNumCols = lines [0].Split (',').Length;
		
//		List<CTrial> lsTrial = new List<CTrial>();
		
//		int i = 0;
//		while (i < intNumRows) 
//		{
//			if ((i % 7) == 0)
//			{
//				CTrial trial = new CTrial();
				
//				for (int j = 0; j < 7; j++)
//				{
//					CStimulus stim = new CStimulus();
//					string[] line_r = lines[i].Split(',');
//					if (line_r[0] == "Target")
//						line_r[0] = "target";
//					stim.m_strType = line_r[0];
//					stim.m_strName = line_r[1];
//					stim.m_strImage = line_r[2];
//					trial.m_lsStimulus.Add(stim);
//					if (j == 0)
//					{
//						// remove .wav
//						string[] strWav = line_r[3].Split('.');
//						trial.m_strTargetAudio = strWav[0];
//					}
//					i++;
//				}
//				trial.m_intTargetIdx = 0;
//				lsTrial.Add(trial);
//				/*for (int j = 0; j < intNumCols; j++)
//				{
//					stim.m_strType = line_r[j];
//				}*/
//			}
//		}	// end while
		
//		// save lsTrial to xml 
//		XmlDocument doc = new XmlDocument();
//		doc.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
//		            "<root>" +
//		            "</root>");
		
//		// Save the document to a file. White space is preserved (no white space).
//		string strFile = Application.persistentDataPath + "/level12-V4-xml.xml";
		
//		for (i = 0; i < lsTrial.Count; i++) 
//		{
//			XmlElement xmlNode = doc.CreateElement("node");
//			XmlAttribute attr = doc.CreateAttribute("idx");
//			attr.Value = i.ToString();
//			xmlNode.SetAttributeNode(attr); 
			
//			// add stimuli
//			for (var j = 0; j < lsTrial[i].m_lsStimulus.Count; j++)
//			{
//				XmlElement xmlStimulus = doc.CreateElement("stimulus");
//				XmlAttribute attr1 = doc.CreateAttribute("idx");
//				attr1.Value = j.ToString();
//				xmlStimulus.SetAttributeNode(attr1); 
				
//				XmlElement xmlChild = doc.CreateElement("name");
//				xmlChild.InnerText = lsTrial[i].m_lsStimulus[j].m_strName;
//				xmlStimulus.AppendChild(xmlChild);
				
//				xmlChild = doc.CreateElement("image");
//				xmlChild.InnerText = lsTrial[i].m_lsStimulus[j].m_strImage;
//				xmlStimulus.AppendChild(xmlChild);
				
//				xmlChild = doc.CreateElement("type");
//				xmlChild.InnerText = lsTrial[i].m_lsStimulus[j].m_strType;
//				xmlStimulus.AppendChild(xmlChild);
				
//				xmlNode.AppendChild(xmlStimulus);
//			}
			
//			XmlElement xmlChild2 = doc.CreateElement("targetAudio");
//			xmlChild2.InnerText = lsTrial[i].m_strTargetAudio;
//			xmlNode.AppendChild(xmlChild2);
			
//			xmlChild2 = doc.CreateElement("targetIdx");
//			xmlChild2.InnerText = lsTrial[i].m_intTargetIdx.ToString();
//			xmlNode.AppendChild(xmlChild2);			
			
//			doc.DocumentElement.AppendChild(xmlNode);
//		}
		
//		//doc.PreserveWhitespace = true;
//		doc.Save(strFile);
//	}
//}


////************************************************************************************************************************
//// CStimulus
////************************************************************************************************************************
//public class CStimulus 
//{
//    // each stimulus has a associated name and image file
//    public int intOriginalIdx = -1;  // the idx as in the corpus chellengeitem.xml, need to keep track this coz the foils will be randomised bef presenting
//	public string m_strName;
//	public string m_strImage;
//	public string m_strType;  // target / phonological / semantic / unrelated
//    public string m_strPType;

//    //----------------------------------------------------------------------------------------------------
//    // CStimulus
//    //----------------------------------------------------------------------------------------------------
//    public CStimulus()
//	{
//        intOriginalIdx = -1;
//        m_strName = "";
//		m_strImage = "";
//		m_strType = "";
//        m_strPType = "";
//    }
//}

////************************************************************************************************************************
//// CTrial 
////************************************************************************************************************************
//public class CTrial 
//{
//    public int m_intChallengeItemFeaturesIdx = -1;
//    public int m_intChallengeItemIdx = -1;
//    public List<CStimulus> m_lsStimulus;  // each trial will have a dynamic no of stimulus - 3 / 4 / 5 / 6
//	public string m_strTargetAudio;   // audio of the target stimulus
//	public int m_intTargetIdx;  // index of the target stimulus in lsStimulus
	
//	//----------------------------------------------------------------------------------------------------
//	// CTrial
//	//----------------------------------------------------------------------------------------------------
//	public CTrial () 
//	{
//        m_intChallengeItemFeaturesIdx = -1;
//        m_intChallengeItemIdx = -1;
//        m_lsStimulus = new List<CStimulus>();
//		m_strTargetAudio = "";   
//		m_intTargetIdx = -1;  
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// RandomizeStimuli
//	//----------------------------------------------------------------------------------------------------
//	public void RandomizeStimuli () 
//	{
//		for (int i = 0; i < m_lsStimulus.Count; i++) 
//		{
//			CStimulus temp = m_lsStimulus[i];
//			int intRandomIndex = UnityEngine.Random.Range(i, m_lsStimulus.Count);
//			m_lsStimulus[i] = m_lsStimulus[intRandomIndex];
//			m_lsStimulus[intRandomIndex] = temp;
//		}		
		
//		// find target in the randomized stimuli
//		for (var i = 0; i < m_lsStimulus.Count; i++) 
//		{
//			if (m_lsStimulus[i].m_strType.Equals("target"))
//			{
//				m_intTargetIdx = i;
//				break;
//			}
//		}
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// Update
//	//----------------------------------------------------------------------------------------------------
//	void Update () 
//	{
		
//	}
//}

////************************************************************************************************************************
//// CResponse 
////************************************************************************************************************************
//public class CResponse 
//{
//	public List<int> m_lsSelectedStimulusIdx;  // list of seelcted stimulus idx
//	public float m_fRTSec;
//	public int m_intReplayBtnCtr;  // how many times tthe replay button has been clicked
//	public int m_intScore;   // score earned
//	public int m_intCoinNum;  // no of coins earned - rf coin reward schedule
	
//	//----------------------------------------------------------------------------------------------------
//	// CResponse
//	//----------------------------------------------------------------------------------------------------
//	public CResponse () 
//	{
//        m_lsSelectedStimulusIdx = new List<int>();
//		m_fRTSec = 0f;
//		m_intReplayBtnCtr = 0;
//		m_intScore = 0;   
//		m_intCoinNum = 0;  
//	}
	
//	//----------------------------------------------------------------------------------------------------
//	// Reset
//	//----------------------------------------------------------------------------------------------------
//	public void Reset () 
//	{
//        m_lsSelectedStimulusIdx.Clear();
//		m_fRTSec = 0f;
//		m_intReplayBtnCtr = 0;
//		m_intScore = 0;   
//		m_intCoinNum = 0;  
//	}
//}

////************************************************************************************************************************
//// CCompletedLevel
////************************************************************************************************************************
//public class CCompletedLevel
//{
//	// each completed level is stored in separate xml file
//	public int m_intLevel;
//	public int m_intCorrectTrials;
//	public int m_intIncorrectTrials;
//	public int m_intTotalTrials;
//	public float m_fAccuracy;
//	public float m_fCorrectTrialsRTMeanSec;
//	public float m_fCorrectTrialsRTStdDSec;
//	public float m_fAllTrialsRTMeanSec;
//	public float m_fAllTrialsRTStdDSec;
//	public float m_fDurationMin;
//	public string m_strXmlFile;  
	
//	//----------------------------------------------------------------------------------------------------
//	// CCompletedLevel
//	//----------------------------------------------------------------------------------------------------
//	public CCompletedLevel()
//	{
//		m_intLevel = 0;
//		m_intCorrectTrials = 0;
//		m_intIncorrectTrials = 0;
//		m_intTotalTrials = 0;
//		m_fAccuracy = 0f;
//		m_fCorrectTrialsRTMeanSec = 0f;
//		m_fCorrectTrialsRTStdDSec = 0f;
//		m_fAllTrialsRTMeanSec = 0f;
//		m_fAllTrialsRTStdDSec = 0f;
//		m_fDurationMin = 0f;
//		m_strXmlFile = "";
//	}
//}

////************************************************************************************************************************
//// CStatisticDay
////************************************************************************************************************************
//public class CStatisticDay
//{
//	public string m_strDate;
//	public float m_fTherapyTimeMin;
		
//	//----------------------------------------------------------------------------------------------------
//	// CStatisticDay
//	//----------------------------------------------------------------------------------------------------
//	public CStatisticDay()
//	{
//		m_strDate = "";
//		m_fTherapyTimeMin = 0f;
//	}
//}


