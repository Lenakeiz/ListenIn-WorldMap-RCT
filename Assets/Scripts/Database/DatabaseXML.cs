using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text;

using MadLevelManager;

public class DatabaseXML : Singleton<DatabaseXML> {

    
    [SerializeField]
    public static int MaximumQueriesPerFile = 50;
    //Andrea is using ID 1 for internal testing
    public int PatientId = 1;
    public int DatasetId = 0;
    public TextAsset database_xml_file = null;

    public delegate void SwitchingPatient(string message, bool canContinue);
    public SwitchingPatient OnSwitchedPatient;

    //create an XML file to keep and read it
    //Andrea: making it a local variable
    public XmlDocument database_xml;
    //form to send thq ueries
    WWWForm xml_form;
    string xml_query_url;
    //xml file directory
    //for android Application.persistentDataPath
    string xml_file;
    string xml_location;
    //queue for the forms to insert
    Queue<DatabaseQuery> xml_forms_queue;
    bool insert_in_order;
    //urls
    //DEVELOPMENT

    //public string therapy_daily_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_daily_insert.php";
    //public string therapy_daily_update = "http://italk.ucl.ac.uk/listenin_dev/therapy_daily_update.php";
    //public string therapy_session_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_session_insert.php";
    //public string therapy_session_update = "http://italk.ucl.ac.uk/listenin_dev/therapy_session_update.php";
    //public string therapy_challenge_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_challenge_insert.php";
    //public string therapy_challenge_update = "http://italk.ucl.ac.uk/listenin_dev/therapy_challenge_update.php";
    //public string therapy_block_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_block_insert.php";
    //public string insert_patient_progress = "http://italk.ucl.ac.uk/listenin_dev/patient_game_update.php";
    //public string select_patient_progress = "http://italk.ucl.ac.uk/listenin_dev/patient_game_select.php";

    //public string therapy_time_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_time_insert.php";
    //public string game_time_insert = "http://italk.ucl.ac.uk/listenin_dev/game_time_insert.php";
    //public string select_patient_datasetid = "http://italk.ucl.ac.uk/listenin_dev/patient_datasetid_select.php";
    //public string therapy_history_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_history_insert.php";
    //public string therapy_history_insert_2 = "http://italk.ucl.ac.uk/listenin_dev/therapy_history_insert_2.php";
    //public string therapy_block_detail_insert = "http://italk.ucl.ac.uk/listenin_dev/therapy_block_detail_insert.php";
    //public string upload_files_server = "http://italk.ucl.ac.uk/listenin_dev/upload_file.php";

    //urls
    //REAL DATABASE
    public string therapy_daily_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_daily_insert.php";
    public string therapy_daily_update = "http://italk.ucl.ac.uk/listenin_rct/therapy_daily_update.php";
    public string therapy_session_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_session_insert.php";
    public string therapy_session_update = "http://italk.ucl.ac.uk/listenin_rct/therapy_session_update.php";
    public string therapy_challenge_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_challenge_insert.php";
    public string therapy_challenge_update = "http://italk.ucl.ac.uk/listenin_rct/therapy_challenge_update.php";
    public string therapy_block_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_block_insert.php";
    public string insert_patient_progress = "http://italk.ucl.ac.uk/listenin_rct/patient_game_update.php";
    public string select_patient_progress = "http://italk.ucl.ac.uk/listenin_rct/patient_game_select.php";

    public string therapy_time_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_time_insert.php";
    public string game_time_insert = "http://italk.ucl.ac.uk/listenin_rct/game_time_insert.php";
    public string select_patient_datasetid = "http://italk.ucl.ac.uk/listenin_rct/patient_datasetid_select.php";
    public string therapy_history_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_history_insert.php";
    public string therapy_history_insert_2 = "http://italk.ucl.ac.uk/listenin_rct/therapy_history_insert_2.php";
    public string therapy_block_detail_insert = "http://italk.ucl.ac.uk/listenin_rct/therapy_block_detail_insert.php";
    public string upload_files_server = "http://italk.ucl.ac.uk/listenin_rct/upload_file.php";

    //timers
    float therapy_time = 0;
    float therapy_pinball_time = 0;
    float therapy_worldmap_time = 0;
    bool count_therapy_time = false;
    bool count_pinball_time = false;
    bool count_worldmap_time = false;
    bool isMenuPaused = false;
    bool m_stop_forcetimer_routine = false;
    public bool SetIsMenu { get { return isMenuPaused; } set { isMenuPaused = value; } }
    private int reasonToExit = 0;
    public int ReasonToExit { get { return reasonToExit; } set { reasonToExit = value; } }
    //iddle time
    float idle_time = 0;
    bool count_iddle_time = false;
    //insert daily threapy once
    bool therapy_daily_inserted = false;
    
    float m_fTherapy_block_idle_time_sec = 0;   // to keep track the idle time within a therapy block
    float m_fTotal_therapy_time_sec = 0;   // to keep track the total therapy time

    public void InitializeDatabase()
    {

        Debug.Log(String.Format("Current Folder: {0}", Application.persistentDataPath));
        xml_forms_queue = new Queue<DatabaseQuery>();
        //current xml file to write on
        xml_location = Application.persistentDataPath + @"/ListenIn/Database/";

        //create the document
        database_xml = new XmlDocument();

        //check if directory doesn't exit -- FIRST INITIALIZATION
        if (!Directory.Exists(Application.persistentDataPath + @"/ListenIn/"))
        {
            //if it doesn't, create it
            Debug.Log("DatabaseXML: first initialization - creating directories");
            Directory.CreateDirectory(xml_location);
            Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/Database/backup");
            Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/Therapy/");
			Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/Therapy/all");   // 2016-12-06
            
            //create an xml from the local sample one
            if (database_xml_file == null)
            {
                //Getting the xml template from the resources
                database_xml_file = Resources.Load("database") as TextAsset;
            }
            database_xml.LoadXml(database_xml_file.text);
            //and save it only once, 1 is the default file name
            database_xml.Save(Application.persistentDataPath + @"/ListenIn/Database/1.xml");
        }

        string pathToXMLs = Path.Combine(Application.persistentDataPath, "ListenIn/Database");
        //number of .xml
        int currSplittedFiles = new DirectoryInfo(pathToXMLs).GetFiles().Length;

        ListenIn.Logger.Instance.Log(String.Format("Numbers of total databaseXML files: {0}", currSplittedFiles), ListenIn.LoggerMessageType.Info);
        if (currSplittedFiles == 0)
        {
            Debug.Log("DatabaseXML: No xml splitted files found!");
            currSplittedFiles++;
        }
        //create the file route by the current xml file
        xml_file = Path.Combine(pathToXMLs, String.Format("{0}.xml", currSplittedFiles.ToString()));// Application.persistentDataPath + @"/ListenIn/Database/" + currSplittedFiles + ".xml";

        // check database.xml file length - if the file corrupted with length 0kb, then recreate the using setting from PlayerPrefs
        {
            try
            {
                FileInfo info = new FileInfo(xml_file);
                if (info.Length == 0)
                {
                    Debug.Log("DatabaseXML: ********** database.xml is EMPTY!!!");

                    if (!LoadCurrentUserFromPlayerPrefs())
                        return;
                }
                else
                {
                    //load the xml from the path
                    database_xml.LoadXml(File.ReadAllText(xml_file));
                }
            }
            catch (Exception ex)
            {
                Debug.Log(String.Format("DatabaseXML: {0}", ex.Message));

                CleanXMLSplitting();                
            }

        }

        //load the xml from the path
        //database_xml.LoadXml(File.ReadAllText(xml_file));

        //Debug.Log(Application.persistentDataPath);

        if (!Directory.Exists(Application.persistentDataPath + @"/ListenIn/Therapy/"))
            Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/Therapy/");

        try
        {
            //get id patient from the xml
            PatientId = int.Parse(GetPatient());
            DatasetId = int.Parse(GetDatasetId());
        }
        catch (Exception ex)
        {
            Debug.Log(String.Format("DatabaseXML: {0}", ex.Message));
        }
        
        Debug.Log("DatabseXML: *** PatientId = " + PatientId + ", DatasetId = " + DatasetId);               

        // check if there is any therapy corrupted files
        fixTherapyCorruptedFiles();
    }

    private bool LoadCurrentUserFromPlayerPrefs()
    {
        string strPatientId = PlayerPrefs.GetString("patient_id", "");
        string strDatasetId = PlayerPrefs.GetString("dataset_id", "");
        database_xml.LoadXml(database_xml_file.text);
        XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
        if (strPatientId.Equals("") || strDatasetId.Equals(""))
        {
            Debug.Log("DatabaseXML: no previous patient or dataset ID saved in PlayerPref. Using default user #1...");
            patient_element.SetAttribute("id", "1");
            patient_element.SetAttribute("datasetid", "0");

            //save doc 
            database_xml.Save(xml_file);
            return false;
        }
        patient_element.SetAttribute("id", strPatientId);
        patient_element.SetAttribute("datasetid", strDatasetId);

        //save doc 
        database_xml.Save(xml_file);
        return true;
    }

    private void CleanXMLSplitting()
    {
        string pathToXMLs = Path.Combine(Application.persistentDataPath, "ListenIn/Database");
        //Found a number of splitted xmls which is not consistent (i.e. like 3 xmls but not 1,2,3 as standard)
        //Erasing files
        List<string> files = new DirectoryInfo(pathToXMLs).GetFiles().Select(X => X.FullName).ToList();
        if (files != null && files.Count != 0)
        {
            for (int i = 0; i < files.Count; i++)
            {
                File.Delete(files[i]);
            }
        }
        //Recreating original condition from Playerprefs
        xml_file = Application.persistentDataPath + @"/ListenIn/Database/1.xml";
        LoadCurrentUserFromPlayerPrefs();
    }

    private void fixTherapyCorruptedFiles()
    {
        string strXmlFile_UserProfile = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_profile.xml");
        string strCsvFile_TherapyBlocks = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks.csv");
        string strXmlFile_CifHistory = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_challengeitemfeatures_history.xml");
        string strCsvFile_LiHistory = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_lexicalitem_history_exposure.csv");
        string strXmlFile_TherapyBlocksAll = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_all.xml");
		Debug.Log(" ***** FIX CORRUPTED THERAPY FILES 1 ***** ");

        if ((!System.IO.File.Exists(strXmlFile_UserProfile)) || (!System.IO.File.Exists(strCsvFile_TherapyBlocks)) ||
                              (!System.IO.File.Exists(strXmlFile_CifHistory)) || (!System.IO.File.Exists(strCsvFile_LiHistory)) /*||
                              (!System.IO.File.Exists(strXmlFile_TherapyBlocksAll))*/)
            return;

        FileInfo info1 = new FileInfo(strXmlFile_UserProfile);
        FileInfo info2 = new FileInfo(strCsvFile_TherapyBlocks);
        FileInfo info3 = new FileInfo(strXmlFile_CifHistory);
        FileInfo info4 = new FileInfo(strCsvFile_LiHistory);
        //FileInfo info5 = new FileInfo(strXmlFile_TherapyBlocksAll);
        if ( (info1.Length == 0) || (info2.Length == 0) || (info3.Length == 0) || (info4.Length == 0) /*|| (info5.Length == 0)*/)
        {
			Debug.Log(" ***** FIX CORRUPTED THERAPY FILES 2 ***** ");
            // one of the files has corrupted, revert back to previous day
            if (Directory.Exists(Application.persistentDataPath + @"/ListenIn/Therapy/"))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.persistentDataPath + @"/ListenIn/Therapy/");
                int count = dir.GetFiles().Length;
                //we have already some backups
                if (count >= 4)
                {
                    // loop through all backup files till 
                    System.DateTime backup_date = System.DateTime.Now;
                    bool bFound = false;
                    int intCtr = 0;
                    string xml_backup_UserProfile = "";
                    string csv_backup_TherapyBlocks = "";
                    string xml_backup_CifHistory = "";
                    string csv_backup_LiHistory = "";
                    //string xml_backup_TherapyBlocksAll = "";

                    //Looping through backup code until 100 days before
                    while (!bFound && intCtr < 100)
                    {
                        //backup_date = backup_date.AddDays(-1);
                        string strDate = backup_date.ToString("yyyy-MM-dd");                        
                        xml_backup_UserProfile = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + PatientId + "_profile-" + strDate + ".xml";
                        csv_backup_TherapyBlocks = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + PatientId + "_therapyblocks-" + strDate + ".csv";
                        xml_backup_CifHistory = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + PatientId + "_challengeitemfeatures_history-" + strDate + ".xml";
                        csv_backup_LiHistory = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + PatientId + "_lexicalitem_history_exposure-" + strDate + ".csv";
                        //xml_backup_TherapyBlocksAll = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + PatientId + "_therapyblocks_all-" + strDate + ".xml";

                        if  ( (System.IO.File.Exists(xml_backup_UserProfile)) && (System.IO.File.Exists(csv_backup_TherapyBlocks)) &&
                              (System.IO.File.Exists(xml_backup_CifHistory)) && (System.IO.File.Exists(csv_backup_LiHistory)) /*&&
                              (System.IO.File.Exists(xml_backup_TherapyBlocksAll))*/  ) 
                        {
                            info1 = new FileInfo(xml_backup_UserProfile);
                            info2 = new FileInfo(csv_backup_TherapyBlocks);
                            info3 = new FileInfo(xml_backup_CifHistory);
                            info4 = new FileInfo(csv_backup_LiHistory);
                            //info5 = new FileInfo(xml_backup_TherapyBlocksAll);
                            if ((info1.Length > 0) && (info2.Length > 0) && (info3.Length > 0) && (info4.Length > 0) /*&& (info5.Length > 0)*/ )
                            {
                                bFound = true;
                                break;
                            }
                        }        
						backup_date = backup_date.AddDays(-1);
                        intCtr++;
                    }  // end while

                    if (bFound)
                    {
                        System.IO.File.Copy(xml_backup_UserProfile, strXmlFile_UserProfile, true);
                        System.IO.File.Copy(csv_backup_TherapyBlocks, strCsvFile_TherapyBlocks, true);
                        System.IO.File.Copy(xml_backup_CifHistory, strXmlFile_CifHistory, true);
                        System.IO.File.Copy(csv_backup_LiHistory, strCsvFile_LiHistory, true);
                        //System.IO.File.Copy(xml_backup_TherapyBlocksAll, strXmlFile_TherapyBlocksAll, true);

                        Debug.Log(" ***** FIX CORRUPTED THERAPY FILES ***** ");
                    }
                    else
                    {
                        Debug.Log("DatabaseXML: No full set of backup files was found");
                    }
                }
            }
        }
    }

    protected override void Awake()
    {
        //Getting the xml template from the resources
        Debug.Log("DatabaseXML: awake");
        database_xml_file = Resources.Load("database") as TextAsset;
    }

    protected void Update()
    {
        //get fingers on screen android only 
        int fingerCount = 0;

#if UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;

        }
#endif

#if UNITY_EDITOR
        fingerCount = Input.GetMouseButtonDown(0) ? 1 : 0;
#endif

        if (fingerCount > 0)
        {
            //if finger, reset the timer
            count_iddle_time = false;
            idle_time = 0;
        }
        else
        {
            //if no finger, then run the counter
            count_iddle_time = true;
        }

        //block timer
        if (count_therapy_time)
        {
            therapy_time += Time.deltaTime;
            m_fTotal_therapy_time_sec += Time.deltaTime;
        }

        if (count_pinball_time)
        {
            therapy_pinball_time += Time.deltaTime;
        }

        if (count_worldmap_time)
        {
            therapy_worldmap_time += Time.deltaTime;
        }

        //iddle timer
        if (count_iddle_time)
        {
            idle_time += Time.unscaledDeltaTime;
            //Debug.Log((int)therapy_block_time);
        }

        if (isMenuPaused)
            m_fTherapy_block_idle_time_sec += Time.unscaledDeltaTime;

        //TODO: unify the menu system
        #region TimeoutGame
        if (isMenuPaused && idle_time > 60 * 1)
        {
            Debug.Log("DatabaseXML: Update() Forcing ListenIn to quit due to timeout (99)");
            reasonToExit = 99;
            Application.Quit();

            //If we are running in the editor
#if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (!isMenuPaused && idle_time > 60 * 1 && !m_stop_forcetimer_routine)
        {
            if (OpenPauseMenu())
            {
                ResetTimer(TimerType.Idle);
            }
            else
            {
                Debug.LogWarning("DatabaseXML: preventing force idle menu. Could be setup screen, uploading screen or a transition");
                ResetTimer(TimerType.Idle);
            }
        }

        #endregion
    }

    private bool OpenPauseMenu()
    {
        Scene currScene = SceneManager.GetActiveScene();
        if (currScene.name == "SetupScreen")
        {
            //Debug.Log("DatabaseXML: OpenPauseMenu() preventing screen to pause in setupscreen");
            return false;
        }

        //Works only on the WorldMapScene
        if (currScene.name == "LevelSelectScene")
        {
            GameObject menuUI = GameObject.FindGameObjectWithTag("MenuUI");
            if (menuUI != null)
            {
                LevelSelectManager lsm = menuUI.GetComponent<LevelSelectManager>();
                if (lsm != null)
                {
                    lsm.OpenPauseMenu();

                    Debug.Log("DatabaseXML: OpenPauseMenu() Forcing menu after idle timeout - case WorldMap");
                    return true;
                }
            }
        }        

        //Andrea
        if (currScene.name == "GameLoop")
        {
            GameObject challengeTherapy = GameObject.FindGameObjectWithTag("Challenge");
            if (challengeTherapy != null)
            {
                MenuManager mm = challengeTherapy.GetComponentInChildren<MenuManager>();
                if (mm != null)
                {
                    mm.OpenMenu();
                    Debug.Log("DatabaseXML: OpenPauseMenu() Forcing menu after idle timeout - case Therapy Challenge");
                    return true;
                }
            }

            GameObject jigsawPuzzle = GameObject.FindGameObjectWithTag("PinballPrefab");
            if (jigsawPuzzle != null)
            {
                MenuManager csm = jigsawPuzzle.GetComponentInChildren<MenuManager>();
                if (csm != null)
                {
                    csm.OpenMenu();
                    Debug.Log("Forcing menu after idle timeout - case Pinball");
                    return true;
                }
            }
        }

        return false;

    }

    //return lenght
    public int QueriesOnTheXML()
    {
        XmlNodeList _list = database_xml.SelectNodes("/database/queries/query");
        //if there's none, do nothing
        return _list.Count;
    }

    public bool CheckUploadSafeCondition()
    {
        //current number of files
        string pathToXMLs = Path.Combine(Application.persistentDataPath, "ListenIn/Database");
        //number of .xml
        int number_of_xml = new DirectoryInfo(pathToXMLs).GetFiles().Length;
        //int number_of_xml = Directory.GetFiles(Application.persistentDataPath + @"/ListenIn/Database", "*.xml ", SearchOption.TopDirectoryOnly).Length;
        if (number_of_xml > 10 && UploadManager.Instance.currentBatteryLevel > 30.0f)
        {
            return true;
        }

        if (number_of_xml <= 10 && UploadManager.Instance.currentBatteryLevel > 15.0f)
        {
            return true;
        }

        ListenIn.Logger.Instance.Log("DatabaseXML: battery check failed", ListenIn.LoggerMessageType.Warning);
        return false;
    }

    //function which reads the xml in order
    public IEnumerator ReadDatabaseXML()
    {
        //current number of files
        string pathToXMLs = Path.Combine(Application.persistentDataPath, "ListenIn/Database");
        string pathToBkups = Path.Combine(Application.persistentDataPath, "ListenIn/Database/backup");
        int number_of_xml = new DirectoryInfo(pathToXMLs).GetFiles().Length;// Directory.GetFiles(pathToXMLs, "*.xml ", SearchOption.TopDirectoryOnly).Length;
        //current time
        ListenIn.Logger.Instance.Log(String.Format("DatabaseXML: ReadDatabaseXML() # xml found = {0}" ,number_of_xml), ListenIn.LoggerMessageType.Info);
        var current_date = System.DateTime.Now;// .ToString("yyyy.MM.dd-HH.mm.ss");
        string current_date_string = String.Concat(DateTime.Today.ToString("yyyy-MM-dd"), "-", current_date.Hour.ToString(), "_", current_date.Minute.ToString(),"_", current_date.Second.ToString());
        //Debug.Log("ReadDatabaseXML: current date string = " + current_date_string);
        //cycle for the xmls

        for (int i = 1; i <= number_of_xml; i++)
        {
            //Andrea 19/11: changing the order of execution in order to prevent DatabaseXML crashing
            if (QueriesOnTheXML() == 0)
            {
                Debug.Log("DatabaseXML: ReadDatabaseXML() - No queries to be read from file " + i);
                yield return null;
            }
            else
            {
                //Resetting forms enqueu
                //queue to insert the forms
                //Andrea: doing this in send query to database
                //xml_forms_queue.Clear();
                //create folder-backup
                //Directory.CreateDirectory(Application.persistentDataPath + @"/ListenIn/Database/backup/" + current_date + "/");
                //Create the backup with index plus date
                string bkupfilename = String.Concat(i.ToString(), "__", current_date_string, ".xml");
                string xml_backup = Path.Combine(pathToBkups, bkupfilename); //Application.persistentDataPath + @"/ListenIn/Database/backup/" + filename;
                Debug.Log("DatabaseXML: XML_BACKUP " + xml_backup);
                //current file
                xml_file = Path.Combine(pathToXMLs,String.Format("{0}.xml", i.ToString()));//Application.persistentDataPath + @"/ListenIn/Database/" + i.ToString() + ".xml";
                try
                {
                    File.Copy(xml_file, xml_backup);
                    Debug.Log("DatabaseXML: ReadDatabaseXML() - copied current version of DatabaseXML");
                    File.Delete(xml_file);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(String.Format("DatabaseXML: {0}", ex.Message));
                }
                                
                ////Reset the original xml_file to ampty state
                //try
                //{
                //    XmlElement elmRoot = (XmlElement)database_xml.SelectSingleNode("/database/queries");
                //    //remove all
                //    elmRoot.RemoveAll();
                //    //and save
                //    database_xml.Save(xml_file);
                //}
                //catch (System.Exception ex)
                //{
                //    Debug.LogError(ex.Message);
                //}
                //yield return null;

                //Read backup
                try
                {
                    Debug.Log("ReadDatabaseXML: reading database xml backup with index " + i);
                    XmlDocument backup_database_xml = new XmlDocument();
                    backup_database_xml.LoadXml(File.ReadAllText(xml_backup));

                    //select all the query tags
                    XmlNodeList _list = backup_database_xml.SelectNodes("/database/queries/query");

                    //go through each one of them
                    foreach (XmlNode _node in _list)
                    {
                        xml_form = new WWWForm();
                        //get the url of the query to be send
                        xml_query_url = _node.Attributes[0].Value;
                        Debug.Log(_node.Attributes[0].Value);
                        //go through all the XML variables nodes inside the current query
                        foreach (XmlNode _node_variables in _node.ChildNodes)
                        {
                            //create the fields for the WWWForm
                            xml_form.AddField(_node_variables.Attributes[0].Value, _node_variables.Attributes[1].Value);
                            //StartCoroutine(send_xml_query());
                            Debug.Log(_node_variables.Attributes[0].Value + " - " + _node_variables.Attributes[1].Value);
                        }
                        //queue the forms
                        xml_forms_queue.Enqueue(new DatabaseQuery(xml_query_url, xml_form));
                        Debug.Log(string.Format("DatabaseXML: ReadDatabaseXML() query prepared: {0}", xml_query_url));
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                //Send backup queries to DB
                //go through the queue and insert them in order
                ListenIn.Logger.Instance.Log(String.Format("DatabaseXML: ReadDatabaseXML() starting communication with DB for file #{0}", i), ListenIn.LoggerMessageType.Info);
                yield return StartCoroutine(send_xml_query());            

            }
        }

        try
        {
            //Recreating original condition
            //origin path
            xml_file = Application.persistentDataPath + @"/ListenIn/Database/1.xml";
            //create new original file
            //create the document
            database_xml = new XmlDocument();
            //create an xml from the local sample one
            database_xml.LoadXml(database_xml_file.text);
            //get/set the patient element
            XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
            patient_element.SetAttribute("id", PatientId.ToString());
            patient_element.SetAttribute("datasetid", DatasetId.ToString());
            //and save it
            database_xml.Save(xml_file);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        

        //Sending off queries
        //yield return StartCoroutine(send_xml_query());

        /////OLD VERSION
        ////current time
        //string current_date = System.DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");
        ////Debug.Log(current_date);
        ////select all the query tags
        //XmlNodeList _list = database_xml.SelectNodes("/database/queries/query");
        ////if there's none, do nothing
        //if (_list.Count != 0)
        //{
        //    //queue to insert the forms
        //    xml_forms_queue = new Queue<DatabaseQuery>();
        //    //go through each one of them
        //    foreach (XmlNode _node in _list)
        //    {
        //        xml_form = new WWWForm();
        //        //get the url of the query to be send
        //        xml_query_url = _node.Attributes[0].Value;
        //        Debug.Log(_node.Attributes[0].Value);
        //        //go through all the XML variables nodes inside the current query
        //        foreach (XmlNode _node_variables in _node.ChildNodes)
        //        {
        //            //create the fields for the WWWForm
        //            xml_form.AddField(_node_variables.Attributes[0].Value, _node_variables.Attributes[1].Value);
        //            //StartCoroutine(send_xml_query());
        //            Debug.Log(_node_variables.Attributes[0].Value + " - " + _node_variables.Attributes[1].Value);
        //        }
        //        //queue the forms
        //        xml_forms_queue.Enqueue(new DatabaseQuery(xml_query_url, xml_form));
        //        Debug.Log(string.Format("DatabaseXML: {0} query prepared", xml_query_url));
        //    }
        //    //go through the queue and insert them in order
        //    yield return StartCoroutine(send_xml_query());

        //    Debug.Log("DatabaseXML: backup and refreshing database xml");
        //    //when finishes, save it as the current date in another folder
        //    string xml_backup = Application.persistentDataPath + @"/ListenIn/Database/backup/" + current_date + ".xml";
        //    File.Copy(xml_file, xml_backup);
        //    //empty the inserter xml
        //    XmlElement elmRoot = (XmlElement)database_xml.SelectSingleNode("/database/queries");
        //    //remove all
        //    elmRoot.RemoveAll();
        //    //and save
        //    database_xml.Save(xml_file);

        //    //StartCoroutine("UploadHistory2");
        //}
    }

    IEnumerator send_xml_query()
    {
        //first in, first out logic
        foreach (DatabaseQuery form in xml_forms_queue)
        {
            insert_in_order = false;
            WWW xml_www = new WWW(form.query_url, form.query_form);
            while (!insert_in_order)
            {
                yield return xml_www;
                if (xml_www.isDone) insert_in_order = true;
            }
           
        }
        //Clear current content
        xml_forms_queue.Clear();
        Debug.Log("DatabaseXML: send_xml_query() end of send xml query (splitted version)");
    }

    public void resetTherapy_block_idle_time_sec()
    {
        m_fTherapy_block_idle_time_sec = 0;
    }

    public float getTherapy_block_idle_time_sec()
    {
        return m_fTherapy_block_idle_time_sec;
    }

    public void setTotal_therapy_time_sec(float fTotalTime)
    {
        m_fTotal_therapy_time_sec = fTotalTime;
    }

    public float getTotal_therapy_time_sec()
    {
        return m_fTotal_therapy_time_sec;
    }

    public void uploadHistoryXml()
    {
        StartCoroutine("UploadHistory2");
    }

    public IEnumerator UploadHistory2()
    {
        string strProfile = "";
        string strTherapyBlocksCsv = "";
        string strCifHistory = "";

        ListenIn.Logger.Instance.Log("DatabaseXML-UploadHistory2: reading user_profile.xml", ListenIn.LoggerMessageType.Info);

        byte[] patientUserProfileFile;
        byte[] patientUserTherapyBlocksFile;
        byte[] patientUserChallengeItemFeaturesFile;

        string filename = String.Concat("user_",PatientId,"_profile_");
        string fullfilename = String.Concat(filename,".xml");
        string strXmlFile1_ = System.IO.Path.Combine(Application.persistentDataPath, fullfilename);

        //Andrea: ANY ERROR HERE IS NOT CATCHED. How to deal with this?
        if(File.Exists(strXmlFile1_))
        {
            patientUserProfileFile = File.ReadAllBytes(strXmlFile1_);

            WWWForm form = new WWWForm();
            form.AddField("patient_id", DatabaseXML.Instance.PatientId.ToString());
            form.AddField("file_data", "file_data");
            form.AddBinaryData("file_data", patientUserProfileFile, fullfilename);

            //change the url to the url of the php file
            WWW w = new WWW(upload_files_server, form);

            yield return w;
            if (w.error != null)
            {
                print(w.error);
                Debug.Log(String.Format("DatabaseXML: UploadHistory2() {0}", w.error));
            }
            else
            {
                Debug.Log("DatabaseXML: UploadHistory2() done");
            }
        }            

        fullfilename = String.Concat("user_", PatientId, "_therapyblocks_.csv");
        string strCsvFile2_ = System.IO.Path.Combine(Application.persistentDataPath, fullfilename);

        if (File.Exists(strCsvFile2_))
        {
            patientUserProfileFile = File.ReadAllBytes(strCsvFile2_);

            WWWForm form = new WWWForm();
            form.AddField("patient_id", DatabaseXML.Instance.PatientId.ToString());
            form.AddField("file_data", "file_data");
            form.AddBinaryData("file_data", patientUserProfileFile, fullfilename);

            //change the url to the url of the php file
            WWW w = new WWW(upload_files_server, form);

            yield return w;
            if (w.error != null)
            {
                print(w.error);
                Debug.Log(String.Format("DatabaseXML: UploadHistory2() {0}", w.error));
            }
            else
            {
                Debug.Log("DatabaseXML: UploadHistory2() done");
            }
        }

        fullfilename = String.Concat("user_", PatientId, "_challengeitemfeatures_history_.xml");
        string strXmlFile8_ = System.IO.Path.Combine(Application.persistentDataPath, fullfilename);

        if (File.Exists(strXmlFile8_))
        {
            patientUserProfileFile = File.ReadAllBytes(strXmlFile8_);

            WWWForm form = new WWWForm();
            form.AddField("patient_id", DatabaseXML.Instance.PatientId.ToString());
            form.AddField("file_data", "file_data");
            form.AddBinaryData("file_data", patientUserProfileFile, fullfilename);

            //change the url to the url of the php file
            WWW w = new WWW(upload_files_server, form);

            yield return w;
            if (w.error != null)
            {
                print(w.error);
                Debug.Log(String.Format("DatabaseXML: UploadHistory2() {0}", w.error));
            }
            else
            {
                Debug.Log("DatabaseXML: UploadHistory2() done");
            }
        }

        //XmlDocument doc1 = new XmlDocument();

        //try
        //{
        //    if (System.IO.File.Exists(strXmlFile1_))
        //    {
        //        doc1.Load(strXmlFile1_);
        //        strProfile = doc1.OuterXml;
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
        //yield return null;

        //ListenIn.Logger.Instance.Log("DatabaseXML-UploadHistory2: reading user_therapyblocks.csv", ListenIn.LoggerMessageType.Info);
        ////Debug.Log("DatabaseXML-UploadHistory2: reading user_therapyblocks.csv");

        //string strCsvFile2_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_.csv");
        //try
        //{
        //    if (System.IO.File.Exists(strCsvFile2_))
        //    {
        //        strTherapyBlocksCsv = System.IO.File.ReadAllText(strCsvFile2_);
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
        //yield return null;

        //ListenIn.Logger.Instance.Log("DatabaseXML-UploadHistory2: reading user_challengeitemfeatures_history.xml", ListenIn.LoggerMessageType.Info);
        ////Debug.Log("DatabaseXML-UploadHistory2: reading user_challengeitemfeatures_history.xml");

        //XmlDocument doc8 = new XmlDocument();
        //string strXmlFile8_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_challengeitemfeatures_history_.xml");
        //try
        //{
        //    if (System.IO.File.Exists(strXmlFile8_))
        //    {
        //        doc8.Load(strXmlFile8_);
        //        strCifHistory = doc8.OuterXml;
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //}
        //yield return null;

        //// create WWWForm
        //WWWForm form = new WWWForm();
        //form.AddField("patientid", PatientId.ToString());
        //form.AddField("profile", strProfile);
        //form.AddField("tb", strTherapyBlocksCsv);
        //form.AddField("cifHistory", strCifHistory);

        //ListenIn.Logger.Instance.Log(String.Format("DatabaseXML: uploading {0}", therapy_history_insert_2), ListenIn.LoggerMessageType.Info);

        ////change the url to the url of the php file
        //WWW w = new WWW(therapy_history_insert_2, form);

        //yield return w;
        //if (w.error != null)
        //{
        //    print(w.error);
        //    Debug.Log(String.Format("DatabaseXML: UploadHistory2() {0}", w.error));
        //}
        //else
        //{
        //    Debug.Log("DatabaseXML: UploadHistory2() done");
        //}
    }

    public IEnumerator UploadHistory2_old()
    {
        /*map.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
                "<root>" +
                "</root>");*/

        string strProfile = "";
        string strTherapyBlocks = "";
        string strTherapyBlocksAll = "";
        string strLiExposure = "";
        //string strLiComplexity = "";
        //string strCiComplexity = "";
        //string strCifComplexity = "";
        string strCifHistory = "";

        Debug.Log("DatabaseXML-UploadHistory2: reading user_profile.xml");

        XmlDocument doc1 = new XmlDocument();
        //string strXmlFile1 = Application.persistentDataPath + "/" + "user_" + PatientId + "_profile.xml";
        //string strXmlFile1 = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_profile.xml");
        string strXmlFile1_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_profile_.xml");
        try
        {
            if (System.IO.File.Exists(strXmlFile1_))
            {
                //System.IO.File.Move(strXmlFile1, strXmlFile1_);
                //System.IO.File.Copy(strXmlFile1, strXmlFile1_, true);
                doc1.Load(strXmlFile1_);
                strProfile = doc1.OuterXml;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        Debug.Log("DatabaseXML-UploadHistory2: reading user_therapyblocks.xml");

        XmlDocument doc2 = new XmlDocument();
        //string strXmlFile2 = Application.persistentDataPath + "/" + "user_" + PatientId + "_therapyblocks.xml";
        //string strXmlFile2 = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks.xml");
        string strXmlFile2_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_.xml");
        try
        {
            if (System.IO.File.Exists(strXmlFile2_))
            {
                //System.IO.File.Move(strXmlFile2, strXmlFile2_);
                //System.IO.File.Copy(strXmlFile2, strXmlFile2_, true);
                doc2.Load(strXmlFile2_);
                strTherapyBlocks = doc2.OuterXml;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        Debug.Log("DatabaseXML-UploadHistory2: reading user_therapyblocks_rt.xml");

        XmlDocument doc3 = new XmlDocument();
        //string strXmlFile3 = Application.persistentDataPath + "/" + "user_" + PatientId + "_therapyblocks_all.xml";
        //string strXmlFile3 = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_all.xml");
        //string strXmlFile3_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_all_.xml");
        string strXmlFile3_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_therapyblocks_rt_.xml");
        try
        {
            if (System.IO.File.Exists(strXmlFile3_))
            {
                //System.IO.File.Move(strXmlFile3, strXmlFile3_);
                //System.IO.File.Copy(strXmlFile3, strXmlFile3_, true);
                doc3.Load(strXmlFile3_);
                strTherapyBlocksAll = doc3.OuterXml;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        Debug.Log("DatabaseXML-UploadHistory2: reading user_lexicalitem_history_exposure.csv");

        //string strCsvFile4 = Application.persistentDataPath + "/" + "user_" + PatientId + "_lexicalitem_history_exposure.csv";
        //string strCsvFile4 = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_lexicalitem_history_exposure.csv");
        string strCsvFile4_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_lexicalitem_history_exposure_.csv");
        try
        {
            if (System.IO.File.Exists(strCsvFile4_))
            {
                //System.IO.File.Move(strCsvFile4, strCsvFile4_);
                //System.IO.File.Copy(strCsvFile4, strCsvFile4_, true);
                strLiExposure = System.IO.File.ReadAllText(strCsvFile4_);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        /*string strCsvFile5 = Application.persistentDataPath + "/" + "user_" + PatientId + "_lexicalitem_history_complexity.csv";
        if (System.IO.File.Exists(strCsvFile5))
            strLiComplexity = System.IO.File.ReadAllText(strCsvFile5);
        
        string strCsvFile6 = Application.persistentDataPath + "/" + "user_" + PatientId + "_challengeitem_history_complexity.csv";
        if (System.IO.File.Exists(strCsvFile6))
            strCiComplexity = System.IO.File.ReadAllText(strCsvFile6);
        
        string strCsvFile7 = Application.persistentDataPath + "/" + "user_" + PatientId + "_challengeitemfeatures_history_complexity.csv";
        if (System.IO.File.Exists(strCsvFile7))
            strCifComplexity = System.IO.File.ReadAllText(strCsvFile7);*/

        Debug.Log("DatabaseXML-UploadHistory2: reading user_challengeitemfeatures_history.xml");

        XmlDocument doc8 = new XmlDocument();
        //Yean: I guess you have to change this line as well
        //string strXmlFile8 = Application.persistentDataPath + "/" + "user_" + PatientId + "_challengeitemfeatures_history.xml";
        //string strXmlFile8 = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_challengeitemfeatures_history.xml");
        string strXmlFile8_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + PatientId + "_challengeitemfeatures_history_.xml");
        try
        {
            if (System.IO.File.Exists(strXmlFile8_))
            {
                //System.IO.File.Move(strXmlFile8, strXmlFile8_);
                //System.IO.File.Copy(strXmlFile8, strXmlFile8_, true);
                doc8.Load(strXmlFile8_);
                strCifHistory = doc8.OuterXml;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        yield return null;

        // create WWWForm
        WWWForm form = new WWWForm();
        form.AddField("patientid", PatientId.ToString());
        form.AddField("profile", strProfile);
        form.AddField("tb", strTherapyBlocks);
        form.AddField("cifHistory", strCifHistory);
        form.AddField("tbAll", strTherapyBlocksAll);
        form.AddField("liExposure", strLiExposure);
        //form.AddField("liComplexity", strLiComplexity);
        //form.AddField("ciComplexity", strCiComplexity);
        //form.AddField("cifComplexity", strCifComplexity);        
        //form.AddBinaryData("file", levelData, fileName, "text/xml");

        Debug.Log("DatabaseXML: uploading " + therapy_history_insert);

        //change the url to the url of the php file
        WWW w = new WWW(therapy_history_insert, form);

        yield return w;
        if (w.error != null)
        {
            print(w.error);
            Debug.Log("UploadHistory - error - " + w.error);
        }
        else
        {
            Debug.Log("UploadHistory - done");
        }
    }

    //function which inserts queries to the xml file
    public void WriteDatabaseXML(Dictionary<string,string> www_form, string url_www_form)
    {
        //if more than 5 queries, create a new doc
        if (QueriesOnTheXML() > DatabaseXML.MaximumQueriesPerFile)
        {
            string pathToXMLs = Path.Combine(Application.persistentDataPath, "ListenIn/Database");
            //number of .xml
            int number_of_xml = new DirectoryInfo(pathToXMLs).GetFiles().Length;// Directory.GetFiles(pathToXMLs, "*.xml ", SearchOption.TopDirectoryOnly).Length;
            ListenIn.Logger.Instance.Log(String.Format("DatabaseXML: Created new database xml file with index: {0}", number_of_xml), ListenIn.LoggerMessageType.Info);
            //save
            //database_xml.Save(Application.persistentDataPath + @"/ListenIn/Database/" + number_of_xml + ".xml");
            //file name for the next one / if only 1 file then = number of files + 1
            xml_file = Path.Combine(pathToXMLs, String.Format("{0}.xml",(number_of_xml + 1).ToString()));
            //create the document
            database_xml = new XmlDocument();
            //create an xml from the local sample one
            database_xml.LoadXml(database_xml_file.text);
            //get the patient element
            XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
            patient_element.SetAttribute("id", PatientId.ToString());
            patient_element.SetAttribute("datasetid", DatasetId.ToString());
            //and save it
            database_xml.Save(xml_file);
        }

        //insert daily once
        if (url_www_form != therapy_daily_insert)
        {
            //variable node for the xml
            XmlElement variable_node;

            //get the root element
            XmlElement root_tag = (XmlElement)database_xml.SelectSingleNode("/database/queries");

            //create the query
            XmlElement query_node = database_xml.CreateElement("query");
            query_node.SetAttribute("url", url_www_form);

            //run through the dictornary values (the form to send variables names and values)
            foreach (KeyValuePair<string, string> www_form_values in www_form)
            {
                variable_node = database_xml.CreateElement("variable");
                variable_node.SetAttribute("name", www_form_values.Key);
                variable_node.SetAttribute("value", www_form_values.Value);
                //parenting
                query_node.AppendChild(variable_node);
            }

            //append to root node
            root_tag.AppendChild(query_node);
            //save doc 
            database_xml.Save(xml_file);
        }
        else if (!therapy_daily_inserted)
        {
            //one therapy only
            therapy_daily_inserted = true;
            //variable node for the xml
            XmlElement variable_node;

            //get the root element
            XmlElement root_tag = (XmlElement)database_xml.SelectSingleNode("/database/queries");

            //create the query
            XmlElement query_node = database_xml.CreateElement("query");
            query_node.SetAttribute("url", url_www_form);

            //run through the dictornary values (the form to send variables names and values)
            foreach (KeyValuePair<string, string> www_form_values in www_form)
            {
                variable_node = database_xml.CreateElement("variable");
                variable_node.SetAttribute("name", www_form_values.Key);
                variable_node.SetAttribute("value", www_form_values.Value);
                //parenting
                query_node.AppendChild(variable_node);
            }

            //append to root node
            root_tag.AppendChild(query_node);
            //save doc 
            database_xml.Save(xml_file);
        }    
    }

    void OnApplicationQuit()
    {
        try
        {
            Debug.Log("DatabaseXML: ListenIn starting quit routine");
            //if the patient played the game, then update the daily therapy - last level played, if not then don't update
            if (QueriesOnTheXML() != 0)
            {
                Dictionary<string, string> query_therapyDailyUpdate = new Dictionary<string, string>();
                query_therapyDailyUpdate.Add("patient", PatientId.ToString());
                query_therapyDailyUpdate.Add("level_end", StateJigsawPuzzle.Instance.currLevelPinball.ToString());
                //query_therapyDailyUpdate.Add("total_therapy_time", CUserTherapy.Instance.getTotalTherapyTimeMin().ToString());

                WriteDatabaseXML(query_therapyDailyUpdate, therapy_daily_update);

                //add patient progress to the xml when app close
                Dictionary<string, string> query_patient_game_progress = new Dictionary<string, string>();
                query_patient_game_progress.Add("patient", PatientId.ToString());
                query_patient_game_progress.Add("progress", MadLevelProfile.SaveProfileToString());

                WriteDatabaseXML(query_patient_game_progress, insert_patient_progress);
            }


            Dictionary<string, string> sessionUpdate = new Dictionary<string, string>();

            int patient = PatientId;

            sessionUpdate.Add("patient", patient.ToString());
            sessionUpdate.Add("therapy_time", therapy_time.ToString());
            sessionUpdate.Add("therapy_game_pinball", therapy_pinball_time.ToString());
            sessionUpdate.Add("therapy_game_world", therapy_worldmap_time.ToString());
            sessionUpdate.Add("exit_reason", reasonToExit.ToString());

            WriteDatabaseXML(sessionUpdate, therapy_session_update);
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }

        Debug.Log("DatabaseXML: Normally exiting ListenIn...");

        ListenIn.Logger.Instance.EmptyBuffer();

    }

    //return lenght
    public IEnumerator SetNewPatient(string patient_id, string dataset_text = "")
    {
        if (OnSwitchedPatient != null)
        {
            OnSwitchedPatient("Switching patient... please wait", false);
        }
        //Checking if current patient has still data to be loaded
        if (QueriesOnTheXML() != 0)
        {
            yield return StartCoroutine(ReadDatabaseXML());
        }

        Debug.Log("DatabaseXML: switching patient routine");
        //Save current game data
        yield return StartCoroutine(update_patient_progress(PatientId));

        //get the patient element
        XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
        patient_element.SetAttribute("id", patient_id);
        //set new patient
        PatientId = int.Parse(patient_id);

        //save doc 
        database_xml.Save(xml_file);

        // set dataset id
        if (dataset_text.Equals("Dataset A-2016-08"))
            DatasetId = 0;
        else if (dataset_text.Equals("Dataset B-2016-10"))
            DatasetId = 1;
        else if (dataset_text.Equals("Dataset A-2016-11"))
            DatasetId = 2;
        else if (dataset_text.Equals("Dataset B-2016-12"))
            DatasetId = 3;
        patient_element.SetAttribute("datasetid", DatasetId.ToString());

        //save doc 
        database_xml.Save(xml_file);

        // save patient id & dataset id in playerprefs
        PlayerPrefs.SetString("patient_id", PatientId.ToString());
        PlayerPrefs.SetString("dataset_id", DatasetId.ToString());
        //PlayerPrefs.Save();

        yield return StartCoroutine(get_patient_progress());

        //CUserTherapy.Instance.LoadUserProfile();
        CUserTherapy.Instance.LoadDataset_UserProfile();

        if (OnSwitchedPatient != null)
        {
            OnSwitchedPatient(String.Format("Switched to patient {0}.", PatientId.ToString()),true);
        }

        /*
        //get the patient element
        XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
        patient_element.SetAttribute("id", patient_id);
        //save doc 
        database_xml.Save(xml_file);
        //set new patient
        PatientId = int.Parse(patient_id);

        //set new dataset
        DatasetId = 0;
        // retrieve dataset id of this patient from database
        StartCoroutine(get_patient_datasetid());        

        StartCoroutine(get_patient_progress());

        //CUserTherapy.Instance.LoadUserProfile();
        CUserTherapy.Instance.LoadDataset_UserProfile();
        */
    }

    IEnumerator get_patient_datasetid()
    {
        //create the the form to send it 
        WWWForm patient_datasetid_form = new WWWForm();
        patient_datasetid_form.AddField("patient", PatientId);

        //sql_query
        WWW sql_patient_datasetid = new WWW(select_patient_datasetid, patient_datasetid_form);
        yield return sql_patient_datasetid;

        string strPatient_datasetid = sql_patient_datasetid.text;

        Debug.Log("Patient dataset id = " + strPatient_datasetid);

        if (strPatient_datasetid == null || strPatient_datasetid == string.Empty)
        {
            DatasetId = 0;
        }
        else
        {
            if (strPatient_datasetid.Equals("A"))
                DatasetId = 0;
            else if (strPatient_datasetid.Equals("B"))
                DatasetId = 1;

            XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
            patient_element.SetAttribute("datasetid", DatasetId.ToString());
            database_xml.Save(xml_file);
        }        
    }

    //return lenght
    public string GetPatient()
    {
        //get the patient element
        XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
        //Debug.Log("DatabaseXML: current patient " + patient_element.GetAttribute("id"));
        return patient_element.GetAttribute("id");
    }

    public string GetDatasetId()
    {
        //get the patient element
        XmlElement patient_element = (XmlElement)database_xml.SelectSingleNode("/database/patient");
        return patient_element.GetAttribute("datasetid");
    }

    //block timer
    public void SetTherapyBlockTimer()
    {
        //count_therapy_block_time = !count_therapy_block_time;
        //if (!count_therapy_block_time) therapy_block_time = 1;
    }

    IEnumerator get_patient_progress()
    {
        //create the the form to send it 
        WWWForm patient_progress_form = new WWWForm();
        patient_progress_form.AddField("patient", PatientId);

        //sql_query = new WWW(insert_patient_url, raw_data, headers);
        WWW sql_patient_progress_login = new WWW(select_patient_progress, patient_progress_form);
        yield return sql_patient_progress_login;

        string patient_game_progress = sql_patient_progress_login.text;

        Debug.Log(patient_game_progress);

        if (patient_game_progress == null || patient_game_progress == string.Empty)
        {
            MadLevelProfile.Reset();            
        }
        else
        {
            MadLevelProfile.LoadProfileFromString(patient_game_progress);
        }

        ResetTimer(TimerType.WorldMap);
        ResetTimer(TimerType.Therapy);
        ResetTimer(TimerType.Pinball);

        GameStateSaver.Instance.ResetListenIn();

        //Andrea: No need to reload current user as this will happen during setup screen
        //MadLevel.ReloadCurrent();

    }

    IEnumerator update_patient_progress(int currId)
    {
        //create the the form to send it
        Debug.Log("DatabaseXML: sending game state to the server...");
        WWWForm patient_progress_update_form = new WWWForm();
        patient_progress_update_form.AddField("patient", currId);

        string savedState = MadLevelProfile.SaveProfileToString();

        patient_progress_update_form.AddField("progress", savedState);

        Debug.Log(savedState);

        //sql_query = new WWW(insert_patient_url, raw_data, headers);
        WWW sql_patient_progress_update = new WWW(insert_patient_progress, patient_progress_update_form);
        yield return sql_patient_progress_update;
    }    

    #region TimerUpdates
    public enum TimerType { Idle, WorldMap, Therapy, Pinball }

#if UNITY_EDITOR
    void OnGUI()
    {
         //to hide timer
        float offset = 30;
        //YEAN: these are the timers
        string iddle_time_string = ((int)idle_time).ToString();
        iddle_time_string = GUI.TextField(new Rect(10, 10 + offset, 200, 20), "idle time: " + iddle_time_string + "(s)", 25);

        string block_time_string = ((int)therapy_worldmap_time).ToString();
        block_time_string = GUI.TextField(new Rect(10, 30 + offset, 200, 20), "WorldMap time: " + block_time_string + "(s)", 25);

        string block_therapy_string = ((int)therapy_time).ToString();
        block_therapy_string = GUI.TextField(new Rect(10, 50 + offset, 200, 20), "Therapy time: " + block_therapy_string + "(s)", 25);

        string total_therapy_string = ((int)m_fTotal_therapy_time_sec).ToString();
        total_therapy_string = GUI.TextField(new Rect(10, 70 + offset, 200, 20), "Total therapy time: " + total_therapy_string + "(s)", 25);

        string therapy_block_idle_string = ((int)m_fTherapy_block_idle_time_sec).ToString();
        therapy_block_idle_string = GUI.TextField(new Rect(10, 90 + offset, 200, 20), "Therapy block idle: " + therapy_block_idle_string + "(s)", 25);

        string block_game_string = ((int)therapy_pinball_time).ToString();
        block_game_string = GUI.TextField(new Rect(10, 110 + offset, 200, 20), "Pinball time: " + block_game_string + "(s)", 25);

        block_game_string = GUI.TextField(new Rect(10, 130 + offset, 200, 20), "Prevent Pause: " + m_stop_forcetimer_routine, 25);
        //#if UNITY_ANDROID
        //        GUI.TextField(new Rect(10, 100, 200, 20), "BATTERY: " + UploadManager.Instance.GetBatteryLevel() + "%");
        //#endif

    }
#endif
    public void SetTimerState(TimerType tymerType, bool state)
    {
        switch (tymerType)
        {
            case TimerType.Idle:
                count_iddle_time = state;
                break;
            case TimerType.WorldMap:
                count_worldmap_time = state;
                break;
            case TimerType.Therapy:
                count_therapy_time = state;                
                break;
            case TimerType.Pinball:
                count_pinball_time = state;
                break;
            default:
                break;
        }
    }

    public void ResetTimer(TimerType tymerType)
    {
        switch (tymerType)
        {
            case TimerType.Idle:
                ForcedTimerState = false;
                idle_time = 0.0f;
                break;
            case TimerType.WorldMap:
                therapy_worldmap_time = 0.0f;
                break;
            case TimerType.Therapy:                
                therapy_time = 0.0f;
                break;
            case TimerType.Pinball:
                therapy_pinball_time = 0.0f;
                break;
            default:
                break;
        }
    }

    //This bool is set to prevent that in certain part of the game the timer could cause the game to quit abruptly (i.e when finishing the pinball wait until the end of the uploading screen)
    public bool ForcedTimerState { set { m_stop_forcetimer_routine = value; } }
#endregion

}
