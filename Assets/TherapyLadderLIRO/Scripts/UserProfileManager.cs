using UnityEngine;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class TherapyLiroUserProfile
{
    public bool isBasketDone = false;
    public int m_currentBlock = -1;
    public int m_totalBlocks = 0;
    public int m_totalTherapyMinutes = 0;
    public int m_totalGameMinutes = 0;
    public int m_totalDayTherapyMinutes = 0;
}

public class BasketTracking
{
    public int[] m_basketTrackingCounters = { -1, -1, -1, -1, -1, -1, -1, -1 };
}

public class ACTLiroUserProfile
{
    public int m_previousScore = 0;
    public int m_currScore = 0;
    public int m_currentBlock = -1;
    public int m_totalBlocks = 0;
}

public class SARTLiroUserProfile
{
    public bool practiceCompleted;
    public bool testCompleted;
    public int attempts;
}

public class QuestionnaireUserProfile
{
    public int questionnairStage;
}

public class UserProfile
{
    #region User Properties
    public bool isFirstInit;
    public bool isTutorialDone;
    public int m_currIDUser;
    public int m_cycleNumber;
    public TherapyLadderStep m_LIROTherapyStep;
    public int m_TherapyLadderStepID;
    public TherapyLiroUserProfile m_TherapyLiroUserProfile = new TherapyLiroUserProfile();
    public ACTLiroUserProfile m_ACTLiroUserProfile = new ACTLiroUserProfile();
    public SARTLiroUserProfile m_SartLiroUserProfile = new SARTLiroUserProfile();
    public QuestionnaireUserProfile m_QuestionaireUserProfile = new QuestionnaireUserProfile();
    public BasketTracking m_BasketTracking = new BasketTracking();
    #endregion
}

public class UserProfileManager
{

    public UserProfile m_userProfile = new UserProfile();
    public TherapyLadderStep LIROStep
    {
        set
        {
            m_userProfile.m_LIROTherapyStep = value;
            m_userProfile.m_TherapyLadderStepID = (int)m_userProfile.m_LIROTherapyStep;
        }
        get { return m_userProfile.m_LIROTherapyStep; }
    }

    #region API
    public void SaveToDisk()
    {
        string currUser = string.Format(GlobalVars.LiroProfileTemplate, m_userProfile.m_currIDUser);
        string currFullPath = Path.Combine(GlobalVars.GetPathToLIROUserProfile(NetworkManager.IdUser), currUser);
        File.WriteAllText(currFullPath, JsonConvert.SerializeObject(m_userProfile));
    }
    public void LoadFromDisk(string fullpath)
    {
        m_userProfile = JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(fullpath));
    }
    /// <summary>
    /// PlayerPrefs are used as checkpoints, updated information are saved in the user_profile.xml
    /// </summary>
    public void SaveToPrefs()
    {
        //AndreaLIRO: need to check how to save in the playerprefs
        PlayerPrefs.SetInt("CurrentUser", m_userProfile.m_currIDUser);
        PlayerPrefs.SetString("TherapyLadderStep", m_userProfile.m_LIROTherapyStep.ToString());
        PlayerPrefs.SetInt("TherapyLadderStepID", (int)m_userProfile.m_LIROTherapyStep);
        //PlayerPrefs.SetInt("CurrentBlockNumber", m_currentBlock);
        //PlayerPrefs.SetInt("CurrentTotalBlockNumber", m_current_Total_Blocks);
    }
    public void GetFromPrefs()
    {
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            m_userProfile.m_currIDUser = PlayerPrefs.GetInt("CurrentUser");
        }

        if (PlayerPrefs.HasKey("TherapyLadderStepID"))
        {
            m_userProfile.m_TherapyLadderStepID = PlayerPrefs.GetInt("TherapyLadderStepID");
            m_userProfile.m_LIROTherapyStep = (TherapyLadderStep)m_userProfile.m_TherapyLadderStepID;
        }
    }

    public void SetToPrefs(string key, int number)
    {
        PlayerPrefs.SetInt(key, number);
    }
    public void GetFromPrefs(string key, string text)
    {

        PlayerPrefs.SetString(key, text);

    }
    public void GetFromPrefs(string key, ref string text)
    {
        if (PlayerPrefs.HasKey(key))
        {
            text = PlayerPrefs.GetString(key);
        }
    }
    #endregion

}
