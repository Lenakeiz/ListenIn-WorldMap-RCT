﻿using UnityEngine;
using UnityEngine.UI;
using MadLevelManager;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class AppControllerSetupScreen : MonoBehaviour {

    [SerializeField]
    private Text m_textScreen;
    private string m_textStringFormat = "Setting up... {0}%";

    [SerializeField]
    private Text m_feedbackTextScreen;
    private string m_resultOnScreen;

    [SerializeField]
    private Button m_playButton;

    [SerializeField]
    private GameObject switchPatient;

    private bool lockEmailSending = false;
    // Use this for initialization
    void Start () {
        m_playButton.interactable = false;
        m_playButton.gameObject.SetActive(false);
        switchPatient.gameObject.SetActive(false);
        StartCoroutine(SetupInitialization());        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator SetupInitialization()
    {
        Application.targetFrameRate = 60;
        int percentage = 1;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);

        //Setting the logger
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI_Canvas_Debug"));
        Text debug_text = go.GetComponentInChildren<Text>();
        ListenIn.Logger.Instance.SetLoggerUIFrame(debug_text);
        ListenIn.Logger.Instance.SetLoggerLogToExternal(true);
        ListenIn.Logger.Instance.Log("Log started", ListenIn.LoggerMessageType.Info);
        yield return new WaitForEndOfFrame();
        
        percentage = 3;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            DatabaseXML.Instance.InitializeDatabase();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 18;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            UploadManager.Instance.Initialize();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 33;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            CUserTherapy.Instance.LoadDataset_UserProfile();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 48;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            StateJigsawPuzzle.Instance.OnGameLoadedInitialization();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 63;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            IMadLevelProfileBackend backend = MadLevelProfile.backend;
            string profile = backend.LoadProfile(MadLevelProfile.DefaultProfile);
            ListenIn.Logger.Instance.Log(string.Format("Loaded profile: {0}", profile), ListenIn.LoggerMessageType.Info);
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 78;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);
        try
        {
            GameStateSaver.Instance.Load();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 85;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);

        try
        {
            CleaningUpOlderLogs();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }

        percentage = 100;
        m_textScreen.text = string.Format(m_textStringFormat, percentage);

        m_playButton.interactable = true;
        m_playButton.gameObject.SetActive(true);

        switchPatient.gameObject.SetActive(true);

    }

    private void CleaningUpOlderLogs()
    {
        //Andrea: need to implement this function
        string path = ListenIn.Logger.Instance.GetLogPath;
        if (!string.IsNullOrEmpty(path))
        {
            var files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTime).Select(x => x.FullName).ToList();
            if (files != null)
            {
                int currCount = files.Count();
                if (currCount > 50)
                {
                    Debug.Log("SetupScreen: removing oldest logs");
                    //Removing oldest one, leaving 50 total logs
                    for (int i = 0; i < currCount - 50; i++)
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            
        }
    }

    public void GoToWorldMap()
    {
        //Debug.Log("PressedButton");
        MadLevel.LoadLevelByName("World Map Select");
    }

    private IEnumerator SendLogToEmail()
    {

        lockEmailSending = true;
        //http://answers.unity3d.com/questions/473469/email-a-file-from-editor-script.html
        //For setting up accounts this must be turned on: https://www.google.com/settings/security/lesssecureapps
        m_feedbackTextScreen.text = "Praparing email...";

        yield return new WaitForEndOfFrame();

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            string path = ListenIn.Logger.Instance.GetLogPath;
            if (!string.IsNullOrEmpty(path))
            {
                var topFiles = new DirectoryInfo(path).GetFiles().OrderByDescending(f => f.LastWriteTime).Take(3).Select(x => x.FullName).ToList();

                if (topFiles != null)
                {
                    string fromEmail = "listeninlog@gmail.com";
                    string subject = "Patient id " + DatabaseXML.Instance.PatientId.ToString();

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromEmail);
                        mailMessage.To.Add("listeninlog@gmail.com");
                        mailMessage.Subject = subject;// subject;
                        mailMessage.Body = "Log created from application version " + Application.version;

                        for (int i = 0; i < topFiles.Count; i++)
                        {
                            //Adding attachments
                            Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(topFiles[i]);
                            mailMessage.Attachments.Add(attachment);
                        }                       

                        {
                            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                            smtpServer.Port = 587;
                            smtpServer.Credentials = new NetworkCredential("listeninlog@gmail.com", "listeninlogger");
                            smtpServer.EnableSsl = true;
                            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                            {
                                return true;
                            };

                            yield return new WaitForEndOfFrame();

                            try
                            {
                                smtpServer.Send(mailMessage);
                                m_feedbackTextScreen.text = "Thanks for feedback!";
                            }
                            catch (System.Exception ex)
                            {
                                m_feedbackTextScreen.text = "Log not uploaded...";
                                ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
                            }
                            finally
                            {
                                lockEmailSending = false;
                            }
                        }
                    }
                }

                
                
                yield return new WaitForEndOfFrame();
                
            }
            else
            {
                lockEmailSending = false;
                m_feedbackTextScreen.text = "No internet detected...";
            }
            yield return null;
        }
    }

    public void SendEmailButton()
    {
        if (!lockEmailSending)
        {
            StartCoroutine(SendLogToEmail());
        }
        else
        {
            m_feedbackTextScreen.text = "Wait...";
        }
        
        
    }
}