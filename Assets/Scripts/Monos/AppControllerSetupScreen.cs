using UnityEngine;
using UnityEngine.UI;
using MadLevelManager;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;

public class AppControllerSetupScreen : MonoBehaviour
{

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
    void Start()
    {
        m_playButton.interactable = false;
        m_playButton.gameObject.SetActive(false);
        switchPatient.gameObject.SetActive(false);
        StartCoroutine(SetupInitialization());
    }

    private void UpdateFeedbackLog(String message, bool canContinue)
    {
        if (m_feedbackTextScreen != null)
            m_feedbackTextScreen.text = message;
        m_playButton.interactable = canContinue;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SetupInitialization()
    {
        Application.targetFrameRate = 60;
        //Preventing the screen to go off
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        int percentage = 1;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);

        //Setting the logger
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI_Canvas_Debug"));
        Text debug_text = go.GetComponentInChildren<Text>();
        ListenIn.Logger.Instance.SetLoggerUIFrame(debug_text);
        ListenIn.Logger.Instance.SetLoggerLogToExternal(true);
        ListenIn.Logger.Instance.Log("AppControllerSetup: Logger started", ListenIn.LoggerMessageType.Info);
        yield return new WaitForEndOfFrame();

        percentage = 3;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            DatabaseXML.Instance.InitializeDatabase();
            DatabaseXML.Instance.OnSwitchedPatient += UpdateFeedbackLog;
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 18;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            CUserTherapy.Instance.LoadDataset_UserProfile();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 33;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            StartCoroutine(UploadProfileHistory());
            //UploadManager.Instance.Initialize();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 48;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            StateJigsawPuzzle.Instance.OnGameLoadedInitialization();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 63;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            IMadLevelProfileBackend backend = MadLevelProfile.backend;
            String profile = backend.LoadProfile(MadLevelProfile.DefaultProfile);
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetupScreen: SetupInitialization() loaded pinball level profile: {0}", profile), ListenIn.LoggerMessageType.Info);
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetupScreen: SetupInitialization() {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 78;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);
        try
        {
            GameStateSaver.Instance.Load();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }
        yield return new WaitForEndOfFrame();

        percentage = 85;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);

        yield return SendLogs();
        yield return new WaitForEndOfFrame();

        percentage = 90;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);

        try
        {
            CleaningUpOlderLogs();
        }
        catch (System.Exception ex)
        {
            ListenIn.Logger.Instance.Log(String.Format("AppControllerSetup: {0}", ex.Message), ListenIn.LoggerMessageType.Error);
        }

        percentage = 100;
        m_textScreen.text = String.Format(m_textStringFormat, percentage);

        m_playButton.interactable = true;
        m_playButton.gameObject.SetActive(true);
        switchPatient.gameObject.SetActive(true);

    }

    private void CleaningUpOlderLogs()
    {
        //Andrea: need to implement this function
        String path = ListenIn.Logger.Instance.GetLogPath;
        if (!String.IsNullOrEmpty(path))
        {
            List<String> files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTime).Select(x => x.FullName).ToList();
            if (files != null)
            {
                int currCount = files.Count();
                Debug.Log(String.Format("SetupScreen: log count: {0}",currCount));
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

    private IEnumerator UploadProfileHistory()
    {
        yield return StartCoroutine(DatabaseXML.Instance.UploadHistory2());
    }

    public void GoToWorldMap()
    {
        //Debug.Log("PressedButton");
        DatabaseXML.Instance.OnSwitchedPatient -= UpdateFeedbackLog;
        MadLevel.LoadLevelByName("World Map Select");
    }

    private IEnumerator SendLogs()
    {
        if (!lockEmailSending)
        {
            yield return StartCoroutine(SendLatestLogs());
        }
        else
        {
            m_feedbackTextScreen.text = "Wait...";
        }
    }

    IEnumerator SendLatestLogs()
    {
        lockEmailSending = true;
        m_feedbackTextScreen.text = "Uploading latest logs...";

        yield return new WaitForEndOfFrame();

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            string path = ListenIn.Logger.Instance.GetLogPath;
            if (!string.IsNullOrEmpty(path))
            {
                var topFiles = new DirectoryInfo(path).GetFiles().OrderByDescending(f => f.LastWriteTime).Take(5).ToList();

                if (topFiles != null)
                {
                    m_feedbackTextScreen.text = "Uploading logs...";
                    byte[] logsFile;
                    foreach (var singleFile in topFiles)
                    {
                        logsFile = File.ReadAllBytes(singleFile.FullName);

                        WWWForm form = new WWWForm();
                        form.AddField("patient_id", DatabaseXML.Instance.PatientId.ToString());
                        form.AddField("file_log", "file_log");
                        form.AddBinaryData("file_log", logsFile, singleFile.Name);

                        //change the url to the url of the php file
                        WWW w = new WWW("http://italk.ucl.ac.uk/listenin_rct/upload_log.php", form);

                        yield return w;

                        if (w.error == null)
                        {
                            File.Delete(singleFile.FullName);
                        }
                    }

                    m_feedbackTextScreen.text = "Logs uploaded...";
                    yield return new WaitForEndOfFrame();

                }
            }
            else
            {
                m_feedbackTextScreen.text = "Uploading latest logs...";
                yield return new WaitForEndOfFrame();
            }
        }


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
