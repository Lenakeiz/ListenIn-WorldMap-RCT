using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using MadLevelManager;

//public enum TherapyLadderStep { ACT1 = 0, OUT1 = 1, CORE1 =  2, SETA = 3, ACT2 = 4, OUT2 = 5, CORE2 = 6, SETB = 7};


/// <summary>
/// Final version of listen in will be as follow
/// 1 REGISTRATION
/// 2 ACT
/// 3 SART
/// 4 BASKET + THERAPY
/// 5 ACT
/// 6 SART
/// 7 QUESTIONNAIRE PART 1 + 2 
/// repeat from 2
/// </summary>
//
public enum TherapyLadderStep { ACT_1_ = 0, SART_PRACTICE_1_ = 1, SART_TEST_1_ = 2, BASKET = 3, CORE = 4, ACT_2_ = 5, SART_PRACTICE_2_ = 6, SART_TEST_2_ = 7, QUESTIONAIRE_1 = 8, QUESTIONAIRE_2 = 9 };

//THESE ARE FOR DEBUGGING
//BASKET FIRST
//public enum TherapyLadderStep { ACT_1_ = 8, SART_PRACTICE_1_ = 9, SART_TEST_1_ = 2, BASKET = 0, CORE = 1, ACT_2_ = 5, SART_PRACTICE_2_ = 6, SART_TEST_2_ = 7,  QUESTIONAIRE_1 = 3, QUESTIONAIRE_2 = 4 };
//ACT FIRST
//public enum TherapyLadderStep { ACT_1_ = 0, SART_PRACTICE_1_ = 1, SART_TEST_1_ = 2, BASKET = 8, CORE = 9, ACT_2_ = 5, SART_PRACTICE_2_ = 6, SART_TEST_2_ = 7, QUESTIONAIRE_1 = 3, QUESTIONAIRE_2 = 4 };
//SART FIRST
//public enum TherapyLadderStep { ACT_1_ = 2, SART_PRACTICE_1_ = 0, SART_TEST_1_ = 1, BASKET = 8, CORE = 9, ACT_2_ = 5, SART_PRACTICE_2_ = 6, SART_TEST_2_ = 7, QUESTIONAIRE_1 = 3, QUESTIONAIRE_2 = 4 };


public class TherapyLIROManager : Singleton<TherapyLIROManager> {

    #region CORE

    [SerializeField]
    private UserProfileManager m_UserProfileManager = new UserProfileManager();
    public UserProfileManager GetUserProfile { get{ return m_UserProfileManager; } }


    [Header("Debug Settings")]
    public int m_maxACTChallenges = 1;
    [Tooltip("You can change this to skip to different LIRO sections")]
    public TherapyLadderStep m_DebugLiroStep = TherapyLadderStep.BASKET;

    private int m_currSectionCounter;
    public int SectionCounter { get { return m_currSectionCounter; } set { m_currSectionCounter = value; } }
    private static int m_numberSelectedPerLexicalItem = 15;
    private int currentBasketFile = 0;
    private int totalBasketFile = 0;

    #endregion

    #region Delegates
    public delegate void OnUpdateProgress(int progressAmount);
    public OnUpdateProgress m_onUpdateProgress;

    /// <summary>
    /// This delegate is used to detect that a new section is currently being created
    /// </summary>
    public delegate void OnSwitchingSection(UserProfileManager currUserSection, int amount);
    public OnSwitchingSection m_OnSwitchingSection;

    /// <summary>
    /// This delegate is used to update the current section, no chage detected
    /// </summary>
    public delegate void OnUpdatingCurrentSection(UserProfileManager currUserSection, int amount);
    public OnUpdatingCurrentSection m_OnUpdatingCurrentSection;

    /// <summary>
    /// This delegate is used to trigger special animation when ending a particular section
    /// </summary>
    public delegate void OnEndingSection(UserProfileManager currUserSection);
    public OnEndingSection m_OnEndingSection;

    #endregion

    #region Unity
    protected override void Awake()
    {
    }
#if DEBUG_LIRO
    public void ChangeLIROSectionButton(String currSection = null)
    {
        UserProfileDefault();
        if (currSection != null)
        {
            m_UserProfileManager.LIROStep = (TherapyLadderStep)Enum.Parse(typeof(TherapyLadderStep), currSection);
        }
        else
        {
            //Taking from inspector
            m_UserProfileManager.LIROStep = m_DebugLiroStep;
        }
        StartCoroutine(ChangeLIROSection());        
    }

    /// <summary>
    /// Function to Debug sections within the editor
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeLIROSection()
    {
        m_UserProfileManager.m_userProfile.isFirstInit = false;

        int previousTherapyTime = m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes;
        int previousGameTherapyTime = m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes;
        int previousDailyTherapyTime = m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes;

        yield return SaveCurrentUserProfile();

        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes = previousTherapyTime;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes    = previousGameTherapyTime;
        m_UserProfileManager.m_userProfile.m_cycleNumber                                  = 1;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes = previousDailyTherapyTime;
        //ReloadLevel
        MadLevel.LoadLevelByName("MainHUB");
    }
#endif
#endregion

    public void UserProfileDefault()
    {
        m_UserProfileManager.LIROStep = TherapyLadderStep.ACT_1_;
        m_UserProfileManager.m_userProfile.isFirstInit = true; //This is to make the first initialization 
        m_UserProfileManager.m_userProfile.isTutorialDone = false; //This is used to make sure the initial tutorial has been done
        //m_UserProfileManager.m_userProfile.m_currIDUser = int.Parse(profileData[3]);
        m_UserProfileManager.m_userProfile.m_cycleNumber = 0;

        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock = -1; //It is a shortcut for when initializing the game for the first time.
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks = 0;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes = 0;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes = 0;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes = 0;

        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock = -1; //It is a shortcut for when initializing the game for the first time.
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks = 0;

        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted = false;
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted = false;
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts = 0;

        m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = 0;
    }

    public bool SetUserProfile(string response)
    {
        string[] profileData = response.Split('+');

        m_UserProfileManager.LIROStep = (TherapyLadderStep)int.Parse(profileData[0]);
        m_UserProfileManager.m_userProfile.isFirstInit = profileData[1] == "1"; //This is to make the first initialization 
        m_UserProfileManager.m_userProfile.isTutorialDone = profileData[2] == "0"; //This is used to make sure the initial tutorial has been done
        //m_UserProfileManager.m_userProfile.m_currIDUser = int.Parse(profileData[3]);
        m_UserProfileManager.m_userProfile.m_cycleNumber = int.Parse(profileData[3]);

        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock = int.Parse(profileData[4]); //It is a shortcut for when initializing the game for the first time.
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks = int.Parse(profileData[5]);
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes = int.Parse(profileData[6]);
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes = int.Parse(profileData[7]);

        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes = int.Parse(profileData[8]);

        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock = int.Parse(profileData[9]); //It is a shortcut for when initializing the game for the first time.
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks = int.Parse(profileData[10]);

        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted = profileData[11] == "0";
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted = profileData[12] == "0";
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts = int.Parse(profileData[13]);

        m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = int.Parse(profileData[14]);
        return true;
    }

    public bool SetUserBasketTrackingProfile(string response)
    {
        string[] profileData = response.Split('+');
        for (int i = 0; i < 8; i++)
        {
            m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[i] = int.Parse(profileData[i]);
        }
        return true;
    }

    public IEnumerator SaveCurrentUserBasketTracking()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("Basket1", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[0]);
        form.AddField("Basket2", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[1]);
        form.AddField("Basket3", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[2]);
        form.AddField("Basket4", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[3]);
        form.AddField("Basket5", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[4]);
        form.AddField("Basket6", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[5]);
        form.AddField("Basket7", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[6]);
        form.AddField("Basket8", (int)m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[7]);
        NetworkManager.SendDataServer(form, NetworkUrl.SqlSetUserBasketTracking);
        yield return null;
    }

    /// <summary>
    /// This function is called at initialiation to load the last saved information for the current user on the therapy ladder
    /// AndreaLIRO: DEPRECATED???
    /// </summary>
    public IEnumerator LoadCurrentUserProfile()
    {
        string currUser = string.Format(GlobalVars.LiroProfileTemplate, NetworkManager.IdUser);
        string currFullPath = Path.Combine(GlobalVars.GetPathToLIROUserProfile(NetworkManager.IdUser), currUser);

        FileInfo info = new FileInfo(currFullPath);

        if (!info.Exists || info.Length == 0)
        {
            Debug.Log("LIRO User Profile not found, creating a new one");
            //Setting a new LIRO user profile
            m_UserProfileManager.LIROStep = (TherapyLadderStep)0;
            m_UserProfileManager.m_userProfile.isFirstInit = true; //This is to make the first initialization 
            m_UserProfileManager.m_userProfile.isTutorialDone = false; //This is used to make sure the initial tutorial has been done
            m_UserProfileManager.m_userProfile.m_currIDUser = 1;
            m_UserProfileManager.m_userProfile.m_cycleNumber = 0;

            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock = -1; //It is a shortcut for when initializing the game for the first time.
            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks = 0;
            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes = 0;
            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes = 0;
            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes = 0;

            m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock = -1; //It is a shortcut for when initializing the game for the first time.
            m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks = 8;

            m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted = false;
            m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted = false;
            m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts = 0;

            m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = 0;

            //Wait for the data to be saved
            yield return StartCoroutine(SaveCurrentUserProfile());
        }
        else
        {
            bool exceptionThrown = false;

            try
            {
                //Loading current profile
                m_UserProfileManager.LoadFromDisk(currFullPath);
                Debug.Log("Current player section: " + m_UserProfileManager.LIROStep.ToString());

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                exceptionThrown = true;
                //Getting information from the last checkpoint -> PlayerPrefs
                m_UserProfileManager.GetFromPrefs();
            }

            if (exceptionThrown)
                yield return StartCoroutine(SaveCurrentUserProfile());

        }

        yield return null;
    }
    public IEnumerator SaveCurrentUserProfile()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("liro_step", (int)m_UserProfileManager.LIROStep);
        form.AddField("is_first_init", m_UserProfileManager.m_userProfile.isFirstInit ? 1 : 0);
        form.AddField("is_tutorial_done", m_UserProfileManager.m_userProfile.isTutorialDone ? 1 : 0);
        form.AddField("cycle_number", m_UserProfileManager.m_userProfile.m_cycleNumber);
        form.AddField("therapy_current_block", m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock);
        form.AddField("therapy_total_blocks", m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks);
        form.AddField("total_game_time", m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes);
        form.AddField("total_therapy_time", m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes);
        form.AddField("daily_therapy_time", m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes);
        form.AddField("act_current_block", m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock);
        form.AddField("act_total_blocks", m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks);
        form.AddField("practice_completed", m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted ? 1 : 0);
        form.AddField("test_completed", m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted ? 1 : 0);
        form.AddField("attempts", m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts);
        form.AddField("questionaire_completed", m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage);
        NetworkManager.SendDataServer(form, NetworkUrl.SqlSetGameUserProfile);

        try
        {
            m_UserProfileManager.SaveToPrefs();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        try
        {
            //AndreaLIRO: switching to json
            m_UserProfileManager.SaveToDisk();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;


    }

#region API
    public IEnumerator LIROInitializationACTPairChoose()
    {
        //These two lists are used in the ACT only
        List<string> list_A = new List<string>();
        List<string> list_B = new List<string>();

        //Final List
        //AndreaLiro: to be sent the backend as well
        List<string> personalized_List = new List<string>();

        if (m_UserProfileManager.m_userProfile.isFirstInit)
        {
            //Loading list of ACT_A
            yield return StartCoroutine(LoadInitialACTFile(GlobalVars.LiroACT_A, value => list_A = value));
            //Loading list of ACT_B
            yield return StartCoroutine(LoadInitialACTFile(GlobalVars.LiroACT_B, value => list_B = value));

            //Load ACT Basket
            List<Challenge> basket_act_list = new List<Challenge>();
            List<Challenge> generated_basket_act = new List<Challenge>();
            List<string> currLines = new List<string>();

            //Add code
            CoreItemReader cir = new CoreItemReader();
            string actbasketName = "ACT_basket";
            string actbasketPath= Path.Combine(GlobalVars.GetPathToLIROBaskets(), actbasketName);

            //Loading all the basket challenges.
            //These are items paired to the lexical items in set A and B and will be the one inserted in the therapy cycle (excluding the untrained lexical items)
            basket_act_list = cir.ParseCsv(actbasketPath, true).ToList();

            List<string> lexItems = basket_act_list.Select(x => x.LexicalItem).Distinct().ToList();
            List<string> listAlexical = list_A.Select(x => x.Split(new char[] { ',' })[1]).ToList();
            List<string> listBlexical = list_B.Select(x => x.Split(new char[] { ',' })[1]).ToList();

            foreach (var item in listAlexical)
            {
                bool found = false;
                if (lexItems.Any(x => x == item))
                    found = true;
                if (!found)
                {
                    Debug.Log("< color = yellow > Warning act pair choose </ yellow >: no therapy cycle item for " + item);
                }
            }

            foreach (var item in listBlexical)
            {
                bool found = false;
                if (lexItems.Any(x => x == item))
                    found = true;
                if (!found)
                {
                    Debug.Log("< color = yellow > Warning act pair choose </ yellow >: no therapy cycle item for " + item);
                }
            }

            string currentSelectedChallenge = String.Empty;
            string currentSelectedLexicalItem = String.Empty;

            int randomNumber = 0;
            //List_A and List_B are matched by size
            for (int i = 0; i < list_A.Count; i++)
            {
                //Reset
                currentSelectedChallenge = String.Empty;
                currentSelectedLexicalItem = String.Empty;

                //Choose random between 0 and 1
                randomNumber = RandomGenerator.GetRandomInRange(0, 2);
                currentSelectedChallenge = randomNumber == 0 ? list_A[i].Replace("\r", "").Trim() : list_B[i].Replace("\r", "").Trim(); //select by removing trailing \r
                currentSelectedLexicalItem = currentSelectedChallenge.Split(new char[] { ',' })[1];
                //Adding all the trials for that lexical item to the basket that goes into the therapy
                generated_basket_act.AddRange(basket_act_list.Where(x => x.LexicalItem == currentSelectedLexicalItem));

                personalized_List.Add(currentSelectedChallenge);

            }

#if SAVE_LOCALLY
            //#ERASE
            //Saving to the local folder
            File.WriteAllLines(GlobalVars.GetPathToLIROACTGenerated(NetworkManager.UserId), personalized_List.ToArray());
            //#ERASE
#endif

            //Sending to server the generated paired list
            //AndreaLIRO_Q: what if this fail?
            byte[] dataAsBytes = personalized_List.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", GlobalVars.LiroGeneratedACTFileName);
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.ActFolderName);
            form.AddBinaryData("file_data", dataAsBytes, GlobalVars.LiroGeneratedACTFileName);
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

            GlobalVars.LiroGenActFile = String.Join(Environment.NewLine, personalized_List.ToArray()); //SAVE FILE

            //public static string LiroGeneratedACT = @"ACT/GEN_ACT";

            for (int j = 0; j < generated_basket_act.Count; j++)
            {
                currLines.Add(String.Join(",", new string[] { generated_basket_act[j].ChallengeID.ToString(),
                                                              generated_basket_act[j].Difficulty.ToString(),
                                                              generated_basket_act[j].LexicalItem.ToString(),
                                                              generated_basket_act[j].FileAudioIDs[0].ToString(),
                                                              generated_basket_act[j].FileAudioIDs[1].ToString(),
                                                              generated_basket_act[j].FileAudioIDs[2].ToString(),
                                                              generated_basket_act[j].FileAudioIDs[3].ToString(),
                                                              generated_basket_act[j].FileAudioIDs[4].ToString(),
                                                              generated_basket_act[j].CorrectImageID.ToString(),
                                                              generated_basket_act[j].Foils[1].ToString(),
                                                              generated_basket_act[j].Foils[2].ToString(),
                                                              generated_basket_act[j].Foils[3].ToString(),
                                                              generated_basket_act[j].Foils[4].ToString(),
                                                              generated_basket_act[j].Foils[5].ToString()
                }));

            }

#if SAVE_LOCALLY
            //#ERASE
            File.WriteAllLines(GlobalVars.GetPathToLIROBasketACTGenerated(NetworkManager.UserId), currLines.ToArray());
            //#ERASE
#endif

            //Sending to server the generated act basket file to be used in therapy
            dataAsBytes = currLines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
            form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", GlobalVars.LiroGeneratedACTBasketFileName);
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.ActFolderName);
            form.AddBinaryData("file_data", dataAsBytes, GlobalVars.LiroGeneratedACTBasketFileName);
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

            GlobalVars.LiroGenActBasketFile = String.Join(Environment.NewLine, currLines.ToArray()); //SAVE FILE

            //AndreaLIRO: add the file to be sent online
            m_UserProfileManager.m_userProfile.isFirstInit = false;
            //AndreaLIRO: cycle number is 0 only at the beginning to do this operation. After this we can safely start the cycle.
            m_UserProfileManager.m_userProfile.m_cycleNumber = 1;
            //Saving user profile info
            yield return StartCoroutine(SaveCurrentUserProfile());
        }
        else
        {
            GetActBasketFile();
            GetActFile();
            yield return null;
        }
    }

    private void GetActBasketFile()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", GlobalVars.LiroGeneratedACTBasketFileName);
        form.AddField("folder_name", GlobalVars.ActFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetFile, GetActBasketFileCallback);
    }

    private void GetActBasketFileCallback(string response)
    {
        if(string.IsNullOrEmpty(response))
        {
            //CRITICAL ERROR
            Debug.LogError("<color=red>SERVER ERROR; empty generated act file</color>");
        }
        GlobalVars.LiroGenActBasketFile = response;
    }

    private void GetActFile()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", GlobalVars.LiroGeneratedACTFileName);
        form.AddField("folder_name", GlobalVars.ActFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetFile, GetActFileCallback);
    }

    private void GetActFileCallback(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            //CRITICAL ERROR
            Debug.LogError("<color=red>SERVER ERROR; empty file</color>");
        }
        GlobalVars.LiroGenActFile = response;
    }

    public TherapyLadderStep GetCurrentLadderStep()
    {
        return m_UserProfileManager.LIROStep;
    }
    public int GetCurrentBlockNumber()
    {
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.CORE:
                return m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock;
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                return m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock;
                break;
            default:
                break;
        }
        return -1;
    }
    public int GetCurrentTherapyCycle() {

        //AndreaLIRO: therapy cycle ID will be held only in the userProfile, so switch is not necessary
        return m_UserProfileManager.m_userProfile.m_cycleNumber;

    }
    /// <summary>
    /// Checking the folder with the blocks if they match the current registered step of the algorithm.
    /// If not load from playerprefs and redo from the last saved checkpoint
    /// </summary>
    public void CheckCurrentSection()
    {
        //AndreaLIRO: simplify this version. We switch between the sections and call separate methods

        //AndreaLIRO: We check for first initialization only as this is treated specially , MUST BE NEGATED
        if (m_UserProfileManager.m_userProfile.isTutorialDone)
        {
            //AndreaLIRO:
            //First initalization, should start practice
            Debug.Log("Detected first initialization. Need to start tutorial section");
            //return;
        }

        //AdnreaLIRO: this will be removed, it s here just to check consistency with the game.
        //************************************************************************************************************************
        //************************************************************************************************************************

        //AndreaLIRO: Create an escape for when having finished the tutorial
        //AndreaLIRO: There is no tutorial anymore
        //************************************************************************************************************************
        if (  m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock == -1
                && m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock == -1
                && (m_UserProfileManager.m_userProfile.m_LIROTherapyStep == TherapyLadderStep.BASKET || m_UserProfileManager.m_userProfile.m_LIROTherapyStep == TherapyLadderStep.ACT_1_ || m_UserProfileManager.m_userProfile.m_LIROTherapyStep == TherapyLadderStep.ACT_2_))
        {
            StartCoroutine(CreateCurrentSection());
            return;
        }
        //************************************************************************************************************************

        bool advance = false;
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.BASKET:
                advance = CheckTherapyBasketEscapeSection();
                break;
            case TherapyLadderStep.CORE:
                advance = CheckTherapyCoreEscapeSection();
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                advance = CheckACTEscapeSection();
                break;
            case TherapyLadderStep.SART_PRACTICE_1_:
            case TherapyLadderStep.SART_PRACTICE_2_:
                advance = CheckSARTEscapeSection();
                break;
            case TherapyLadderStep.SART_TEST_1_:
            case TherapyLadderStep.SART_TEST_2_:
                advance = CheckSARTTESTEscapeSection();
                break;
            case TherapyLadderStep.QUESTIONAIRE_1:
            case TherapyLadderStep.QUESTIONAIRE_2:
                advance = CheckQuestionaireEscapeSection();
                break;
            default:
                break;
        }

        if (advance)
        {
            //We first do any rewarding animation
            if (m_OnEndingSection != null)
            {
                m_OnEndingSection(m_UserProfileManager);
            }
            return;
        }
        else
        {
            PrepareCurrentSection();
        }
    }
    /// <summary>
    /// Escape condition for the basket section (bool in user profile)
    /// </summary>
    /// <returns></returns>
    private bool CheckTherapyBasketEscapeSection()
    {
        return m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.isBasketDone;
    }
    /// <summary>
    /// Escaping condition for therapy core section
    /// </summary>
    /// <returns></returns>
    private bool CheckTherapyCoreEscapeSection()
    {
        return (m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock > m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks);        
    }
    /// <summary>
    /// Escaping condition for ACT section
    /// </summary>
    /// <returns></returns>
    private bool CheckACTEscapeSection()
    {
#if DEBUGGING
        return (m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock > m_maxACTChallenges);
#endif
        return (m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock > m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks);
    }
    /// <summary>
    /// Escaping condition for the SART PRACTICE
    /// </summary>
    /// <returns></returns>
    private bool CheckSARTEscapeSection()
    {
        return m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted || m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts > 1;
    }
    /// <summary>
    /// Escaping condition for the SART
    /// </summary>
    /// <returns></returns>
    private bool CheckSARTTESTEscapeSection()
    {
        return m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted;
    }
    /// <summary>
    /// Escaping condition for the questionaire
    /// </summary>
    /// <returns></returns>
    private bool CheckQuestionaireEscapeSection()
    {
        if (m_UserProfileManager.m_userProfile.m_LIROTherapyStep == TherapyLadderStep.QUESTIONAIRE_1)
            return m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage > 0;
        else if (m_UserProfileManager.m_userProfile.m_LIROTherapyStep == TherapyLadderStep.QUESTIONAIRE_2)
            return m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage == 2;
        else return false;
    }

    /// <summary>
    /// Tis function is called from the UI to notify the manager to change the section
    /// </summary>
    public void GoToNextSection()
    {
        AdvanceCurrentSection();
        StartCoroutine(CreateCurrentSection());
    }
    public void PrepareCurrentSection()
    {
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.BASKET:
            case TherapyLadderStep.CORE:
                PrepareTherapyScreen();
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                PrepareACTScreen();
                break;
            case TherapyLadderStep.SART_PRACTICE_1_:
            case TherapyLadderStep.SART_PRACTICE_2_:
            case TherapyLadderStep.SART_TEST_1_:
            case TherapyLadderStep.SART_TEST_2_:
                PrepareSARTScreen();
                break;
            case TherapyLadderStep.QUESTIONAIRE_1:
            case TherapyLadderStep.QUESTIONAIRE_2:
                PrepareQuestionaireScreen();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Called to increment the section before starting a new one
    /// </summary>
    public void AdvanceCurrentSection()
    {
        int currStep = (int)m_UserProfileManager.LIROStep;
        Array currValues = Enum.GetValues(typeof(TherapyLadderStep));
        int size = currValues.Length;

        currStep = (currStep + 1) % size;
        m_UserProfileManager.LIROStep = (TherapyLadderStep)currStep;

        //AndreaLIRO: increasing number of cycle
        if (m_UserProfileManager.LIROStep == TherapyLadderStep.ACT_1_)
        {
            m_UserProfileManager.m_userProfile.m_cycleNumber++;
        } 

    }
    public IEnumerator AddTherapyMinutes(int minutes)
    {
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalTherapyMinutes += minutes;
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalDayTherapyMinutes += minutes;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
    public IEnumerator AddGameMinutes(int minutes)
    {
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalGameMinutes += minutes;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
    public void StartCoreTherapy(List<BasketUI> selectedBasket)
    {
        StartCoroutine(LoadTherapyFromBasketFiles(selectedBasket));
    }
    //public void StartACT()
    //{
    //    //AndreaLIRO: need to check if this is created 
    //    StartCoroutine(LoadACTFile(GlobalVars.GetPathToLIROACTGenerated()));
    //}

    /// <summary>
    /// Called at the end of the section cycle (core and ACT)
    /// </summary>
    /// <returns></returns>
    public IEnumerator AdvanceCurrentBlockInSection(string fileToDelete = "")
    {
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.CORE:
                CleanCurrentBlock(fileToDelete);
                AdvanceBlock();
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                CleanCurrentBlock(fileToDelete);
                AdvanceBlock();
                break;
            default:
                break;
        }
        //Saving profile
        yield return StartCoroutine(SaveCurrentUserProfile());
    }

    /// <summary>
    /// This function is called at the end of the SART practice to update the current state before going back to the mainHub
    /// </summary>
    /// <param name="isCompleted"></param>
    /// <returns></returns>
    public IEnumerator SaveCurrentSARTPractice(bool isCompleted)
    {
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted = isCompleted;
        if (!isCompleted)
        {
            m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts++;
        }
        yield return StartCoroutine(SaveCurrentUserProfile());
    }

    public IEnumerator SaveHalfQuestionnaire(bool isCompleted)
    {
        m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = 1;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
    public IEnumerator SaveSecondHalfQuestionaire(bool isSkipped)
    {
        m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = 2;
        if (isSkipped)
            m_UserProfileManager.m_userProfile.m_LIROTherapyStep = TherapyLadderStep.QUESTIONAIRE_2;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }

    public IEnumerator SaveSARTFinished()
    {
        m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted = true;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
    public IEnumerator SaveBasketCompletedInfo()
    {
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.isBasketDone = true;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
    /// <summary>
    /// Called by the UploadManager to update the current score in the ACT
    /// </summary>
    /// <param name="currScore"></param>
    /// <returns></returns>
    public IEnumerator UpdateACTScore(int currScore)
    {
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currScore = m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currScore + currScore;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }

    public IEnumerator SaveACTPreviousScore()
    {
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_previousScore = m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currScore;
        yield return StartCoroutine(SaveCurrentUserProfile());
    }
#endregion

#region Internal Functions

    //UPDATING
    //*******************************************************
    internal void PrepareTherapyScreen()
    {
        if (m_OnUpdatingCurrentSection != null)
            m_OnUpdatingCurrentSection(m_UserProfileManager, 100);
    }
    internal void PrepareACTScreen()
    {
        //Do eventually additional things here!
        if (m_OnUpdatingCurrentSection != null)
            m_OnUpdatingCurrentSection(m_UserProfileManager, 100);
    }
    internal void PrepareSARTScreen()
    {
        if (m_OnUpdatingCurrentSection != null)
            m_OnUpdatingCurrentSection(m_UserProfileManager, 100);
    }
    internal void PrepareQuestionaireScreen()
    {
        if (m_OnUpdatingCurrentSection != null)
            m_OnUpdatingCurrentSection(m_UserProfileManager, 100);
    }
    //*******************************************************

    //SWITCHING
    //*******************************************************
    /// <summary>
    /// Called once after the UI notify that the ending animation for current section is being done
    /// </summary>
    /// <returns></returns>
    internal IEnumerator CreateCurrentSection()
    {
        //AndreaLIRO
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.BASKET:
                m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.isBasketDone = false;
                yield return StartCoroutine(SaveCurrentUserProfile());
                PrepareBasketSelectionScreen();
                break;
            case TherapyLadderStep.CORE:
                yield return StartCoroutine(SaveCurrentUserProfile());
                PrepareBasketSelectionScreen();
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currScore = 0;
                yield return StartCoroutine(SaveCurrentUserProfile());
                LoadingACTScreen(0);
                yield return StartCoroutine(LoadACTFile());
                break;
            case TherapyLadderStep.SART_PRACTICE_1_:
            case TherapyLadderStep.SART_PRACTICE_2_:
                //AndreaLIRO: resetting completed state
                m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.practiceCompleted = false;
                m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.testCompleted = false;
                m_UserProfileManager.m_userProfile.m_SartLiroUserProfile.attempts = 0;
                yield return StartCoroutine(SaveCurrentUserProfile());
                LoadingSARTSCreen();
                break;
            case TherapyLadderStep.SART_TEST_1_:
            case TherapyLadderStep.SART_TEST_2_:
                LoadingSARTSCreen();
                break;
            case TherapyLadderStep.QUESTIONAIRE_1:
                //For questionaire only, if cycle == 0 skip
                m_UserProfileManager.m_userProfile.m_QuestionaireUserProfile.questionnairStage = 0;
                yield return StartCoroutine(SaveCurrentUserProfile());
                LoadingQuestionaireScreen();
                break;
            case TherapyLadderStep.QUESTIONAIRE_2:
                //For questionaire only, if cycle == 0 skip
                LoadingQuestionaireScreen();
                break;
            default:
                Debug.LogError("TherapyLiroManager: Current step has not being created...");
                break;
        }

        VideoPlayerController.Instance.SetVideo(m_UserProfileManager.LIROStep);

        yield return null;
    }
    /// <summary>
    /// Called once when switching to the therapy
    /// </summary>
    internal void PrepareBasketSelectionScreen()
    {
        if (m_OnSwitchingSection != null)
            m_OnSwitchingSection(m_UserProfileManager,0);
    }
    /// <summary>
    /// Called many time when switching to the ACT
    /// </summary>
    /// <param name="amount"></param>
    internal void LoadingACTScreen(int amount)
    {
        if (m_OnSwitchingSection != null)
            m_OnSwitchingSection(m_UserProfileManager, amount);
    }
    internal IEnumerator LoadInitialACTFile(string fileToLoad, System.Action<List<string>> result)
    {
        //Loading asset
        TextAsset ta = Resources.Load<TextAsset>(fileToLoad);

        List<string> lines = ta.text.Split(new char[] { '\n' }).ToList();
        //Removing last line if empty
        if (lines[lines.Count - 1] == String.Empty)
        {
            lines.RemoveAt(lines.Count - 1);
        }
        result(lines);

        yield return null;
    }
    /// <summary>
    /// Loading ACT file and generate splitted files
    /// </summary>
    /// <returns></returns>
    internal IEnumerator LoadACTFile()
    {
        yield return new WaitForEndOfFrame();

        int currAmount = 3;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();

        List<string> list_A = new List<string>();
        List<string> list_B = new List<string>();

        //Loading assets list A and B
        //Loading list of ACT_A
        yield return StartCoroutine(LoadInitialACTFile(GlobalVars.LiroACT_A, value => list_A = value));
        currAmount = 9;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();
        //Loading list of ACT_B
        yield return StartCoroutine(LoadInitialACTFile(GlobalVars.LiroACT_B, value => list_B = value));
        currAmount = 17;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();

        List<string> list_AB = new List<string>();
        list_AB.AddRange(list_A);
        list_AB.AddRange(list_B);

        //External notify
        currAmount = 19;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();

        ShuffleLines(ref list_AB);

        currAmount = 30;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();

        //Reading list and writing to files
        int currCount = 0;
        int total_blocks = 1;
        List<string> currBlockLines = new List<string>();
        string currFilename = String.Empty;
        for (int i = 0; i < list_AB.Count; i++)
        {
            currBlockLines.Add(list_AB[i].Replace("\r","").Trim());
            currCount++;
            if (currCount == GlobalVars.ActChallengeLength)
            {
                currFilename = String.Format("{0}_{1}_Cycle_{2}", m_UserProfileManager.LIROStep, total_blocks, m_UserProfileManager.m_userProfile.m_cycleNumber);

#if SAVE_LOCALLY
                //#ERASE
                File.WriteAllLines(Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.UserId), currFilename), currBlockLines.ToArray());
                //#ERASE
#endif

                //SEND TO SERVER
                byte[] dataAsBytes = currBlockLines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                WWWForm form = new WWWForm();
                form.AddField("id_user", NetworkManager.IdUser);
                form.AddField("file_name", currFilename);
                form.AddField("file_size", dataAsBytes.Length);
                form.AddField("folder_name", GlobalVars.SectionFolderName);
                form.AddBinaryData("file_data", dataAsBytes, currFilename);
                NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);
                
                currBlockLines.Clear();
                currCount = 0;
                total_blocks++;
            }

            currAmount = 30 + 30 * (i / list_AB.Count);
            if (m_OnSwitchingSection != null)
            {
                m_OnSwitchingSection(m_UserProfileManager, currAmount);
            }
            yield return new WaitForEndOfFrame();
        }
        //Writing remaining lines
        if (currBlockLines.Count != 0)
        {
            currFilename = String.Format("{0}_{1}_Cycle_{2}", m_UserProfileManager.LIROStep, total_blocks, m_UserProfileManager.m_userProfile.m_cycleNumber);

#if SAVE_LOCALLY
            //#ERASE
            File.WriteAllLines(Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.UserId), currFilename), currBlockLines.ToArray());
            //#ERASE
#endif

            //SEND TO SERVER
            byte[] dataAsBytes = currBlockLines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", currFilename);
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.SectionFolderName);
            form.AddBinaryData("file_data", dataAsBytes, currFilename);
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

            currBlockLines.Clear();
        }

        //Updating UserProfile in the ACT section
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_totalBlocks = total_blocks; //Should be always 8
        m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock = 1;

        currAmount += 2;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(SaveCurrentUserProfile());

        currAmount = 100;
        if (m_OnSwitchingSection != null)
        {
            m_OnSwitchingSection(m_UserProfileManager, currAmount);
        }
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// Just loading the sart screen calling the event in the main HUB
    /// </summary>
    internal void LoadingSARTSCreen()
    {
        if (m_OnSwitchingSection != null)
            m_OnSwitchingSection(m_UserProfileManager, 0);
    }
    internal void LoadingQuestionaireScreen()
    {
        if (m_OnSwitchingSection != null)
            m_OnSwitchingSection(m_UserProfileManager, 0);
    }
    //*******************************************************
    /// <summary>
    /// Called every time a new preparation for the ladder step must be completed
    /// </summary>
    /// <returns></returns>
    internal void ShuffleLines(ref List<string> lines)
    {
        List<string> shuffledLines = lines.OrderBy(x => Guid.NewGuid()).ToList();
        lines.Clear();
        lines.AddRange(shuffledLines);
    }
    /// <summary>
    /// Create therapy files after basket select screen
    /// </summary>
    /// <param name="basketsInfos"> basket infos coming from basket selction</param>
    /// <returns></returns>
    internal IEnumerator LoadTherapyFromBasketFiles(List<BasketUI> basketsInfos)
    {

#if DEBUG_LIRO_THERAPY
        bool singleTherapyCicle = true;
        bool blockSent = false;
#endif

        basketsInfos.Shuffle();

        //Loading ACT GEN file
        //CoreItemReader cir = new CoreItemReader();
        //List<Challenge> listTrainedItems = cir.ParseCsv(GlobalVars.GetPathToLIROBasketACTGenerated(NetworkManager.UserId), false).ToList();

        if(string.IsNullOrEmpty(GlobalVars.LiroGenActBasketFile))
        {
            //CRITICAL ERROR
            Debug.LogError("<color=red>CRITICAL ERROR</color> generated basket with trained ACT items is not present");
            yield break;
        }

        //******************************* BASKET WITH ACT ITEMS ****************************************************
        CoreItemReader cir = new CoreItemReader();
        //This has been created at the first initialization of the user to create the list of trained lexical items
        List<Challenge> listTrainedItems = cir.ParseCsvFromContent(GlobalVars.LiroGenActBasketFile).ToList();
        listTrainedItems = listTrainedItems.Select(x => { x.BasketNumber = 99; return x; }).ToList<Challenge>();
        List<Challenge> curr_Selected_BasketACT = new List<Challenge>();
        //Extracting distinct lexical items
        List<string> lexicalItems = new List<string>();
        lexicalItems = listTrainedItems.Select(x => x.LexicalItem).Distinct().ToList();
        lexicalItems.Shuffle();
        int countLexicalItems = 0;

        //******************************* BASKET ****************************************************
        string currBasketsPath;
        //This is currently build for each of the basket
        //Used for reading the basket
        List<Challenge> curr_basket_list_read = new List<Challenge>();
        //Used to select items for each lexical item
        List<Challenge> curr_Selected_Challenges = new List<Challenge>();
        //Used to extract distinct lexical items
        List<string> curr_basket_lexical_item_list = new List<string>();
        //Used to write the list of challenges that create the therapy
        List<Challenge> curr_basket_list_write = new List<Challenge>();

        string coreFormat = String.Concat( "THERAPY_{0}_Cycle_{1}");
        string currFilename;
        int total_blocks = 1;

        //AndreaLIRO_TB: do not have to clear this at every basket as the shuffle will be done at the end to distribute evenly the trials
        curr_basket_list_write.Clear();
        int countChallengesLexicalItem = 0;

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < basketsInfos.Count; i++)
        {
            curr_basket_list_read.Clear();
            curr_basket_lexical_item_list.Clear();
                        
            yield return new WaitForEndOfFrame();

            cir = new CoreItemReader();
            string basketName = String.Format("Basket_{0}", basketsInfos[i].basketId);
            currBasketsPath = Path.Combine(GlobalVars.GetPathToLIROBaskets(), basketName);

            //Loading all the basket challenges excluding the untrained items
            curr_basket_list_read = cir.ParseCsv(currBasketsPath, true).ToList();
            curr_basket_list_read = curr_basket_list_read.Select(x => { x.BasketNumber = basketsInfos[i].basketId; return x; }).ToList<Challenge>();

            //Getting current difficulty
            //int currDifficulty = basketsInfos[i].hardMode ? 3 : 1;
            try
            {
                //select distinct lexical item for this batch
                curr_basket_lexical_item_list = curr_basket_list_read.Select(x => x.LexicalItem).Distinct().ToList();
                int currentStep = 0;
                //For each distinct lexical item choose a set number of random challenges
                foreach (string lexical_item in curr_basket_lexical_item_list)
                {
                    //Resetting challenges count
                    countChallengesLexicalItem = 0;
                    //Filling up with hard mode first if it s selected
                    if (basketsInfos[i].hardMode)
                    {
                        curr_Selected_Challenges.Clear();
                        //Selecting hard challenges from current basket 
                        curr_Selected_Challenges = curr_basket_list_read.Where(x => x.LexicalItem == lexical_item && x.Difficulty > 1).ToList();
                        if (curr_Selected_Challenges != null || curr_Selected_Challenges.Count != 0)
                        {
                            curr_Selected_Challenges.Shuffle();
                            currentStep = 0;

                            //Until reaching desired number for lexical item or finishing challenges for this hard difficulty
                            while (countChallengesLexicalItem < m_numberSelectedPerLexicalItem && currentStep < curr_Selected_Challenges.Count)
                            {
                                Challenge ch = new Challenge(curr_Selected_Challenges[currentStep]);
                                ch.BasketNumber = basketsInfos[i].basketId;
                                curr_basket_list_write.Add(ch);
                                //curr_challenges_lexical_item.RemoveAt(0);
                                countChallengesLexicalItem++;
                                currentStep++;
                            }
                        }
                        else 
                        {
                            Debug.LogError(String.Format("Could not find easy challenges for {0}", lexical_item));
                        }
                        curr_Selected_Challenges.Clear();
                    }

                    curr_Selected_Challenges.Clear();
                    //Selecting easy challenges from the current basket
                    curr_Selected_Challenges = curr_basket_list_read.Where(x => x.LexicalItem == lexical_item && x.Difficulty == 1).ToList();
                    curr_Selected_Challenges.Shuffle();

                    if (curr_Selected_Challenges != null && curr_Selected_Challenges.Count != 0)
                    {
                        curr_Selected_Challenges = curr_Selected_Challenges.OrderBy(y => RandomGenerator.GetRandom()).ToList();

                        //Resetting counter to cycle through challenges
                        currentStep = 0;

                        //populating until reaching the final number of items
                        while (countChallengesLexicalItem < m_numberSelectedPerLexicalItem)
                        {
                            Challenge ch = new Challenge(curr_Selected_Challenges[currentStep]);
                            ch.BasketNumber = basketsInfos[i].basketId;
                            curr_basket_list_write.Add(ch);
                            currentStep++;
                            //This should make sure that challenges are always cycled through the current list
                            currentStep = currentStep % curr_Selected_Challenges.Count;
                            countChallengesLexicalItem++;
                        }
                    }
                    else
                    { 
                        Debug.Log("<color=yellow>Therapy Warning</color> Could not find hard challenges for " + lexical_item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }          
           
        }

        //At this point we have all the baskets.
        //Repeating the same for the ACT trained items that are generated in a basket

        foreach (string lexical_item in lexicalItems)
        {
            //Resetting challenges count
            countChallengesLexicalItem = 0;
            curr_Selected_Challenges.Clear();
            curr_Selected_Challenges = listTrainedItems.Where(x => x.LexicalItem == lexical_item).ToList();
            curr_Selected_Challenges.Shuffle();
            int currentStep = 0;
            if (curr_Selected_Challenges != null && curr_Selected_Challenges.Count != 0)
            {
                curr_Selected_Challenges = curr_Selected_Challenges.OrderBy(y => RandomGenerator.GetRandom()).ToList();

                //Resetting counter to cycle through challenges
                currentStep = 0;

                //populating until reaching the final number of items
                while (countChallengesLexicalItem < m_numberSelectedPerLexicalItem)
                {
                    Challenge ch = new Challenge(curr_Selected_Challenges[currentStep]);
                    ch.BasketNumber = 99;
                    curr_basket_list_write.Add(ch);
                    currentStep++;
                    //This should make sure that challenges are always cycled through the current list
                    currentStep = currentStep % curr_Selected_Challenges.Count;
                    countChallengesLexicalItem++;
                }
            }
            else
            {
                Debug.Log("<color=yellow>Therapy Warning</color> Could not find act trained basket challenges for " + lexical_item);
            }

        }

        //---------- GENERATION COMPLETE

        //Shuffling challenges
        curr_basket_list_write.Shuffle();

        //Adding information about number presentation inside the current cycle
        for (int i = 0; i < curr_basket_list_write.Count; i++)
        {
            Challenge c = new Challenge(curr_basket_list_write.ElementAt(i));
            c.PresentationNumber = i+1;
            curr_basket_list_write[i] = c;
        }

        //I am trynig to cheat here.
        //curr_basket_list_write = curr_basket_list_write.OrderBy(x => x.PresentationNumber).ToList();

        //Getting all distinct lexical item
        curr_basket_lexical_item_list = curr_basket_list_write.Select(x => x.LexicalItem).Distinct().ToList();
        foreach (string lexical_item in curr_basket_lexical_item_list)
        {
            int count = 1;
            foreach (var item in curr_basket_list_write.Where(x => x.LexicalItem == lexical_item).OrderBy(x => x.PresentationNumber))
            {
                item.LexicalPresentationNumber = count;
                count++;
            }
        }

        totalBasketFile = curr_basket_list_write.Count / GlobalVars.ChallengeLength;

        List<string> currLines = new List<string>();
        int challengeCounter = 0;
        //Selected all the items for this basket, allocating them to blocks
        for (int j = 0; j < curr_basket_list_write.Count; j++)
        {
            currLines.Add(String.Join(",", new string[] { curr_basket_list_write[j].ChallengeID.ToString(),
                                                              curr_basket_list_write[j].Difficulty.ToString(),
                                                              curr_basket_list_write[j].LexicalItem.ToString(),
                                                              curr_basket_list_write[j].PresentationNumber.ToString(),
                                                              curr_basket_list_write[j].LexicalPresentationNumber.ToString(),
                                                              curr_basket_list_write[j].BasketNumber.ToString(),
                                                              curr_basket_list_write[j].FileAudioIDs[0].ToString(),
                                                              curr_basket_list_write[j].FileAudioIDs[1].ToString(),
                                                              curr_basket_list_write[j].FileAudioIDs[2].ToString(),
                                                              curr_basket_list_write[j].FileAudioIDs[3].ToString(),
                                                              curr_basket_list_write[j].FileAudioIDs[4].ToString(),
                                                              curr_basket_list_write[j].CorrectImageID.ToString(),
                                                              curr_basket_list_write[j].Foils[1].ToString(),
                                                              curr_basket_list_write[j].Foils[2].ToString(),
                                                              curr_basket_list_write[j].Foils[3].ToString(),
                                                              curr_basket_list_write[j].Foils[4].ToString(),
                                                              curr_basket_list_write[j].Foils[5].ToString()
                }));

            challengeCounter++;

            if (challengeCounter == GlobalVars.ChallengeLength)
            {
                currFilename = String.Format(coreFormat, total_blocks+GlobalVars.TherapyFilesOffset, m_UserProfileManager.m_userProfile.m_cycleNumber); //Starting from 100 for naming convention

#if SAVE_LOCALLY
                    //#ERASE
                    File.WriteAllLines(Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.UserId), currFilename), currLines.ToArray());
                    //#ERASE
#endif

                //Sending basket file to the server
                //SEND TO SERVER
                byte[] dataAsBytes = currLines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                yield return new WaitForEndOfFrame();

                WWWForm form = new WWWForm();
                form.AddField("id_user", NetworkManager.IdUser);
                form.AddField("file_name", currFilename);
                form.AddField("file_size", dataAsBytes.Length);
                form.AddField("folder_name", GlobalVars.SectionFolderName);
                form.AddBinaryData("file_data", dataAsBytes, "temp");
                
#if DEBUG_LIRO_THERAPY
                if (singleTherapyCicle && !blockSent)
                {
                    NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile, currFilename, currFilename, BasketUploadCallback);
                    blockSent = true;
                }
#else
                NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile, currFilename, currFilename, BasketUploadCallback);

#endif

                currLines.Clear();
                total_blocks++;
                challengeCounter = 0;

            }
        }
        //Save the remaining in the last file
        if (currLines.Count != 0)
        {
            currFilename = String.Format(coreFormat, total_blocks+ GlobalVars.TherapyFilesOffset, m_UserProfileManager.m_userProfile.m_cycleNumber);

#if SAVE_LOCALLY
                //#ERASE
                File.WriteAllLines(Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.UserId), currFilename), currLines.ToArray());
                //#ERASE
#endif

            //SEND TO SERVER
            byte[] dataAsBytes = currLines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", currFilename);
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.SectionFolderName);
            form.AddBinaryData("file_data", dataAsBytes, "temp");
            
#if DEBUG_LIRO_THERAPY
            if (singleTherapyCicle && !blockSent)
            {
                NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile, currFilename, currFilename, BasketUploadCallback);
                blockSent = true;
            }
#else
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile, currFilename, currFilename, BasketUploadCallback);

#endif
            currLines.Clear();
            total_blocks++;
            totalBasketFile++;
        }

        //Updating UserProfile in the therapy section
#if DEBUG_LIRO_THERAPY
        if (singleTherapyCicle)
        {
            m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks = 1;
        }
#else
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_totalBlocks = total_blocks - 1;

#endif
        m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock = 1;

        yield return StartCoroutine(SaveCurrentUserProfile());

        //Andrea_LIRO: adding the tracking for the baskets counter
        foreach (var bs in basketsInfos)
        {
            int currID = bs.basketId - 1;
            m_UserProfileManager.m_userProfile.m_BasketTracking.m_basketTrackingCounters[currID] += 1;
        }

        yield return StartCoroutine(SaveCurrentUserBasketTracking());

    }

    private void BasketUploadCallback(string response)
    {
        currentBasketFile++;
        if (m_onUpdateProgress != null)
        {
            m_onUpdateProgress((int)(100f * ((float)currentBasketFile / (float)totalBasketFile)));
        }
    }

    public void CleanCurrentBlock(string fileToDelete = "")
    {
        string fullPathFile = String.Empty;
        int currBlock = -1;
        string fileName = string.Empty;

        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.CORE:
                currBlock = m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock;
                fileName = String.Format("THERAPY_{0}_Cycle_{1}", currBlock, m_UserProfileManager.m_userProfile.m_cycleNumber);
                //ERASE
                fullPathFile = Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.IdUser), String.Format("THERAPY_{0}_Cycle_{1}", currBlock, m_UserProfileManager.m_userProfile.m_cycleNumber));
                //ERASE
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                currBlock = m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock;
                fileName = String.Format("{0}_{1}_Cycle_{2}", m_UserProfileManager.LIROStep.ToString(), currBlock, m_UserProfileManager.m_userProfile.m_cycleNumber);
                //ERASE
                fullPathFile = Path.Combine(GlobalVars.GetPathToLIROCurrentLadderSection(NetworkManager.IdUser), String.Format("{0}_{1}_Cycle_{2}", m_UserProfileManager.LIROStep.ToString(), currBlock, m_UserProfileManager.m_userProfile.m_cycleNumber));
                //ERASE
                break;
            default:
                break;
        }

        //create form
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("folder_name", GlobalVars.SectionFolderName);
        form.AddField("file_name", fileName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlDeleteFile);

        if (currBlock == -1)
        {
            Debug.Log("TLM: Wrong registered block to delete; section is " + m_UserProfileManager.LIROStep.ToString());
        }

        if (fileToDelete != String.Empty)
        {
            Debug.LogWarning("TLM: Deleting automatic captured therapy block");
            File.Delete(fileToDelete);
        }

        File.Delete(fullPathFile);
    }

    internal void AdvanceBlock()
    {
        switch (m_UserProfileManager.LIROStep)
        {
            case TherapyLadderStep.CORE:
                m_UserProfileManager.m_userProfile.m_TherapyLiroUserProfile.m_currentBlock++;
                break;
            case TherapyLadderStep.ACT_1_:
            case TherapyLadderStep.ACT_2_:
                m_UserProfileManager.m_userProfile.m_ACTLiroUserProfile.m_currentBlock++;
                break;
            default:
                break;
        }
    }
#endregion

}
