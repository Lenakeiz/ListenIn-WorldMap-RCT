using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

namespace ListenIn
{
    public enum LoggerMessageType { Info, Warning, Error };

    public class MessageToLog {
        public string message; public LoggerMessageType messageType; public string logDate;
        public string stacktrace = String.Empty;
        private string format = "[{0}]{1}::{2}:::stack trace:::{3}";
        public override string ToString() { return String.Format(format, logDate, messageType.ToString(), message, stacktrace); }
    }

    public class ListenInLogger : Singleton<ListenInLogger>
    {
        private Text _screenLogger;
        private string _externalPath;
        public string GetLogPath
        {
            get { return _externalPath; }
            private set { _externalPath = value; }
        }
        
        private bool _logToExternalFile;
        private bool _isLoggerReady;
        private bool _logToServer = false;
        private bool _isNotSending = true;
        private DateTime _lastLogTime;
        private readonly IList<MessageToLog> LogBufferList = new List<MessageToLog>();
        private readonly string _logFormat = "[{0}]: {1}";

        private Color GetLoggerMessageColor(LoggerMessageType messageType)
        {
            Color col = Color.black;
            switch (messageType)
            {
                case LoggerMessageType.Info:
                    col = Color.white;
                    break;
                case LoggerMessageType.Warning:
                    col = Color.yellow;
                    break;
                case LoggerMessageType.Error:
                    col = Color.red;
                    break;
                default:
                    break;
            }
            return col;
        }

        public void SetLoggerUIFrame(Text text)
        {
            _screenLogger = text;
            _isLoggerReady = true;
            _lastLogTime = DateTime.UtcNow;
        }

        public void SetLoggerLogToExternal(bool logToExternal)
        {
            _logToExternalFile = logToExternal;

            //Setting external logger
            if (_logToExternalFile)
            {
                if (!Directory.Exists(_externalPath))
                {
                    Directory.CreateDirectory(_externalPath);
                    Debug.Log("LOGGER: created local directory for external logs");
                }
            }

            _isLoggerReady = true;
            _lastLogTime = DateTime.UtcNow;
        }

        //public void Log(string mes, LoggerMessageType mesType)
        //{
        //    if (!_isLoggerReady)
        //        return;

        //    string currLogTime = DateTime.Now.ToString("HH::mm:ss");
        //    MessageToLog mtl = new MessageToLog() { message = mes, messageType = mesType, logDate = currLogTime };

        //    try
        //    {
        //        WriteConsole(mtl);
        //        if (!_logToExternalFile)
        //            return;

        //        lock (LogBufferList)
        //        {
        //            LogBufferList.Add(mtl);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Debug.LogError(ex.Message);
        //        //Detaching logger to prevent an infinite recursive call
        //        Application.logMessageReceived -= HandleLog;
        //        _isLoggerReady = false;
        //    }


        //}

        public void WakeUp()
        {
            _isLoggerReady = true;
        }

        public void EmptyBuffer()
        {
            try
            {
                if (!_isLoggerReady)
                    return;

                _isNotSending = false;
                lock (LogBufferList)
                {
                    if (LogBufferList.Count == 0)
                        return;

                    StringBuilder sb = new StringBuilder();
                    foreach (var item in LogBufferList)
                    {
                        sb.AppendLine(item.ToString());
                    }

                    LogBufferList.Clear();

                    byte[] dataAsBytes = Encoding.ASCII.GetBytes(sb.ToString());

                    WWWForm form = new WWWForm();
                    form.AddField("id_user", NetworkManager.IdUser);
                    form.AddField("file_name", "Log");
                    form.AddField("file_size", dataAsBytes.Length);
                    form.AddField("folder_name", GlobalVars.LogFolderName);
                    form.AddBinaryData("file_data", dataAsBytes, "Log");
                    NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadLogFile);
                    _isNotSending = true;
                }
            }
            catch (Exception ex)
            {
                //Detaching logger to prevent an infinite recursive call
                Application.logMessageReceived -= HandleLog;
                _isLoggerReady = false;
            }
        }

        /// <summary>
        /// Writes on the screen (bottom left) the current message
        /// </summary>
        /// <param name="mtl"></param>
        private void WriteConsole(MessageToLog mtl)
        {
            if (_screenLogger != null)
            {
                _screenLogger.text = String.Format(_logFormat, mtl.messageType.ToString(), mtl.message);
                _screenLogger.color = GetLoggerMessageColor(mtl.messageType);
            }

            //Removing for avoiding loop within the editor
            //if (Application.isEditor)
            //    Debug.Log(mtl.message);

        }

        private string GetLoggerLine(MessageToLog mtl)
        {
            return string.Concat("[", mtl.logDate, "] : ", String.Format(_logFormat, mtl.messageType.ToString(), mtl.message));
        }

        #region Unity functions

        protected override void Start()
        {
            //_screenLogger = null;
            //_externalPath = Path.Combine(Application.persistentDataPath, "Logs");
            //_logToExternalFile = false;
            _logToServer = true;
            
            StartCoroutine(SendLogsToServer());
        }

        protected override void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }
        protected override void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        //Handling unity debug log messages
        private void HandleLog(string logString, string stackTrace, LogType type)
        {

            if (applicationQuitting)
                return;

            MessageToLog mtl = new MessageToLog() { message = logString, stacktrace = stackTrace};
            if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
            {
                mtl.messageType = LoggerMessageType.Error;
            }
            else if (type == LogType.Warning)
            {
                mtl.messageType = LoggerMessageType.Warning;
            }
            else if (type == LogType.Log)
            {
                mtl.messageType = LoggerMessageType.Info;
            }

            var culture = new CultureInfo("en-GB");
            string currLogTime = DateTime.Now.ToString(culture);
            mtl.logDate = currLogTime;

            //Andrea Uploading only raised errors and exception
            if (mtl.messageType == LoggerMessageType.Error)
            {
                lock (LogBufferList)
                {
                    LogBufferList.Add(mtl);
                }
            }
            
        }

        #endregion

        #region MainLoop

        System.Collections.IEnumerator SendLogsToServer()
        {
            while (_isLoggerReady && _isNotSending)
            {
                if (LogBufferList.Count != 0)
                {
                    EmptyBuffer();
                    yield return new WaitForSeconds(0.1f);
                }
                else
                    yield return new WaitForSeconds(1);

            }
        }

        #endregion

        void OnApplicationQuit()
        {
            StopAllCoroutines();
            if (_isNotSending)
            {
                if (LogBufferList.Count != 0)
                {
                    EmptyBuffer();
                }
            }
            _isLoggerReady = false;
        }


    }
}
