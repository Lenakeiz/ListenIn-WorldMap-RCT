//using UnityEngine;
//using System.Collections;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class PatientDB : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//{

//    bool mouse_down;
//    float time_mouse_down;
//    public GameObject switch_user_panel;
//    public GameObject add_user_panel;
//    public GameObject add_patient_loading;
//    //inputs to grab
//    //DoB
//    public InputField patient_id;
//    //dropdown switch users
//    public Dropdown patient_switch;
//    //dropdown switch patient dataaset
//    public Dropdown patient_dataset_switch;
//    //position switch
//    CanvasGroup switch_position;
//    //patient added animator
//    public Animator patient_added;
//    //patient switch animator
//    public GameObject patient_switch_message;
//    //patient already rexist
//    public Animator PatientExists;

//    //db vriables
//    WWWForm insert_patient;
//    //Dictionary<string, string> insert_patient;
//    WWWForm get_patient;
//    //security
//    //byte[] raw_data;
//    //Dictionary<string, string> headers = new Dictionary<string, string>();
//    WWW sql_query;
//    WWW sql_query_get_patient;
//    WWW sql_insert_patient;
//    //offline query
//    WWW offline_sql_query;
//    //URL

//    //string insert_patient_url = "http://italk.ucl.ac.uk/listenin_dev/patient_insert.php";
//    //string get_patient_url = "http://italk.ucl.ac.uk/listenin_dev/get_patients.php";
//    //string get_stroke_site_url = "http://italk.ucl.ac.uk/listenin_dev/get_stroke_site.php";
//    //string login_url = "http://italk.ucl.ac.uk/listenin_dev/doctor_login.php";


//    string insert_patient_url = "http://italk.ucl.ac.uk/listenin_rct/patient_insert.php";
//    string get_patient_url = "http://italk.ucl.ac.uk/listenin_rct/get_patients.php";
//    string get_stroke_site_url = "http://italk.ucl.ac.uk/listenin_rct/get_stroke_site.php";
//    string login_url = "http://italk.ucl.ac.uk/listenin_rct/doctor_login.php";

//    //login dctor
//    public InputField username;
//    public InputField password;
//    WWW sql_login;
//    public GameObject loading_panel;
//    public GameObject login_panel;
//    public Animator login_failed;
//    //offline
//   // List<DatabaseQuery> database_offline_queries = new List<DatabaseQuery>();
//    //List<DatabaseQuery> database_offline_queries_success = new List<DatabaseQuery>();
//    public Animator internet_fail;
//    bool no_initial_internet;
//    string offline_url;
//    WWWForm offline_form;
//    //DatabaseXML databaseXML;


//    // Use this for initialization
//    void Start () {
//        //headers.Add("Content-Type", "application/x-www-form-urlencoded");
//        //headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("angryalgorithmdata:Angry20aLgo15*")));
//        switch_position = switch_user_panel.GetComponent<CanvasGroup>();
//        //databaseXML = FindObjectOfType<DatabaseXML>();
//    }

//    void Awake()
//    {
//        //checking internet
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            //no internet
//            no_initial_internet = true;
//        }
//        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
//        {
//            LoadPatientSwitch();
//            //LoadStrokeSite();
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //there is no initial internet, therefore no loading patients
//        if(no_initial_internet)
//        {
//            //ask for the internet to come back
//            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
//            {
//                LoadPatientSwitch();
//                //LoadStrokeSite();
//            }
//        }

//        if (mouse_down)
//        {
//            time_mouse_down += Time.deltaTime;
//        }
//        //show the menu
//        if(time_mouse_down > 5.0f)
//        {
//            ToggleLogin(true);
//            mouse_down = false;
//            time_mouse_down = 0;
//        }  
//    }
    
//    public void OnPointerDown(PointerEventData eventData)
//    {
//        mouse_down = true;
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//        mouse_down = false;
//        time_mouse_down = 0;
//    }

//    public void ToggleLogin(bool value)
//    {
//        login_panel.SetActive(value);
//        GetComponent<Image>().enabled = !value;
//    }

//    public void CloseAddPatientPanel()
//    {
//        add_user_panel.SetActive(false);
//        GetComponent<Image>().enabled = true;
//        patient_id.text = "";

//    }

//    public void ToggleSwitchPatient()
//    {
//        switch_position.alpha = 1;
//        //GetComponent<Image>().enabled = false;
//    }

//    public void LoadPatientSwitch()
//    {
//        //clear the dropwdown
//        patient_switch.ClearOptions();
//        //query to add the patients to the dropdown
//        //courtine to make sure it is send
//        StartCoroutine(get_patient_coru());
//    }

//    /*public void LoadStrokeSite()
//    {
//        //courtine to make sure it is send
//        StartCoroutine(get_stroke_site_coru());
//    }*/

//    //login doctor
//    public void LoginDoctor()
//    {
//        //loadong
//        loading_panel.SetActive(true);
//        //courtine to make sure it is send
//        StartCoroutine(login_doctor_coru());
//    }

//    public void HideSwitchMenu()
//    {
//        switch_position.alpha = 0;
//        GetComponent<Image>().enabled = true;
//        patient_switch_message.GetComponentInChildren<Text>().text = "Current patient ID: " + patient_switch.captionText.text;
//        patient_switch_message.GetComponent<Animator>().SetBool("Start", true);
//        //set the new patient in the xml
//        Debug.Log("Patient Dataset: " + patient_dataset_switch.captionText.text);
//        StartCoroutine(DatabaseXML.Instance.SetNewPatient(patient_switch.captionText.text, patient_dataset_switch.captionText.text));
//    }

//    public void AddNewPatientUI()
//    {
//        switch_position.alpha = 0;
//        //switch_user_panel.SetActive(false);
//        add_user_panel.SetActive(true);
//    }

//    public void AddNewPatientDB()
//    {
//        add_patient_loading.SetActive(true);
        
//        insert_patient = new WWWForm();
//        insert_patient.AddField("patient", patient_id.text);

//        StartCoroutine(insert_patient_coru());

//    }

//    IEnumerator insert_patient_coru()
//    {
//        //sql_query = new WWW(insert_patient_url, raw_data, headers);
//        sql_insert_patient = new WWW(insert_patient_url, insert_patient);
//        yield return sql_insert_patient;
//        add_patient_loading.SetActive(false);
//        //patient id exists
//        if (sql_insert_patient.text == "0")
//        {
//            PatientExists.SetBool("Start", true);
//        }
//        else
//        {
//            add_user_panel.SetActive(false);
//            GetComponent<Image>().enabled = true;
//            patient_added.SetBool("Start", true);
//            //set the new patient in the xml
//            DatabaseXML.Instance.SetNewPatient(patient_id.text);
//            LoadPatientSwitch();
//        }
//        patient_id.text = "";
//    }

//    IEnumerator get_patient_coru()
//    {
//        //sql_query = new WWW(insert_patient_url, raw_data, headers);
//        sql_query_get_patient = new WWW(get_patient_url);
//        yield return sql_query_get_patient;
//        //put the dropdown users
//        string[] patient = sql_query_get_patient.text.Split(',');
//        foreach(string patient_id in patient)
//        {
//            if(!string.IsNullOrEmpty(patient_id))
//            {
//                patient_switch.options.Add(new Dropdown.OptionData() { text = patient_id });
//            }
//        }

//        //select the current patient in the dropdown
//        int count_options = 0;
//        foreach (Dropdown.OptionData patient_switch_string in patient_switch.options)
//        {
//            if(int.Parse(patient_switch_string.text) == DatabaseXML.Instance.PatientId)
//            {
//                patient_switch.value = count_options;
//                break;
//            }
//            count_options++;
//        }        
//        patient_switch.RefreshShownValue();

//        //select the dataset of the current patient in the dropdown
//        string strDataset = "Dataset A-2016-08";
//        if (DatabaseXML.Instance.DatasetId == 1)
//            strDataset = "Dataset B-2016-10";
//        else if (DatabaseXML.Instance.DatasetId == 2)
//            strDataset = "Dataset A-2016-11";
//        else if (DatabaseXML.Instance.DatasetId == 3)
//            strDataset = "Dataset B-2016-12";
//        count_options = 0;
//        foreach (Dropdown.OptionData patient_ds_switch_string in patient_dataset_switch.options)
//        {
//            if (strDataset.Equals(patient_ds_switch_string.text))
//            {
//                patient_dataset_switch.value = count_options;
//                break;
//            }
//            count_options++;
//        }
//        patient_dataset_switch.RefreshShownValue();

//        //patient_dataset_switch.AddOptions(new List<Dropdown.OptionData>() { new Dropdown.OptionData() { text = "DatasetA" }, new Dropdown.OptionData() { text = "DatasetB" }, new Dropdown.OptionData() { text = "DatasetC" } });
//        patient_dataset_switch.RefreshShownValue();
//    }

//    IEnumerator login_doctor_coru()
//    {
//        string login_username = username.text;
//        string login_password = password.text;

//        //create the the form to send it 
//        WWWForm login_doctor_form = new WWWForm();
//        login_doctor_form.AddField("username", login_username);
//        login_doctor_form.AddField("password", login_password);

//        //reset values
//        username.text = password.text = "";

//        //sql_query = new WWW(insert_patient_url, raw_data, headers);
//        sql_login = new WWW(login_url, login_doctor_form);
//        yield return sql_login;
//        //if no internet
//        if(sql_login.error != null)
//        {
//            //loadong
//            loading_panel.SetActive(false);
//            internet_fail.SetBool("Start", true);

//        }
//        else
//        {
//            //put the dropdown users
//            if (sql_login.text == "1")
//            {
//                //loadong
//                loading_panel.SetActive(false);
//                login_panel.SetActive(false);
//                ToggleSwitchPatient();
//            }
//            else
//            {
//                //loadong
//                loading_panel.SetActive(false);
//                login_failed.SetBool("Start", true);
//            }
//        }
//    }


//    /*IEnumerator get_stroke_site_coru()
//     {
//         //sql_query = new WWW(insert_patient_url, raw_data, headers);
//         WWW sql_query_get_stroke_site = new WWW(get_stroke_site_url);
//         yield return sql_query_get_stroke_site;
//         //put the dropdown users
//         string[] temp = sql_query_get_stroke_site.text.Split(';');
//         string[] sites = temp[0].Split(',');
//         string[] stroke = temp[1].Split(',');

//         foreach (string stroke_type in stroke)
//         {
//             if (!string.IsNullOrEmpty(stroke_type))
//             {
//                 type_stroke.options.Add(new Dropdown.OptionData() { text = stroke_type });
//             }
//         }
//         type_stroke.RefreshShownValue();

//         foreach (string site_type in sites)
//         {
//             if (!string.IsNullOrEmpty(site_type))
//             {
//                 study_site.options.Add(new Dropdown.OptionData() { text = site_type });
//             }
//         }
//         study_site.RefreshShownValue();
//     }*/

//    public void buttontest()
//    {
//        //DatabaseXML.Instance.SetTherapyBlockTimer();
//    }
//}
