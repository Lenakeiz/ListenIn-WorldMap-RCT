using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ListenIn
{

    public class Logger : Singleton<Logger>
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
                    Log("Created directory for external logger", LoggerMessageType.Info);
                }
            }

            _isLoggerReady = true;
            _lastLogTime = DateTime.UtcNow;
        }

        public void Log(string mes, LoggerMessageType mesType)
        {
            if (!_isLoggerReady)
                return;

            string currLogTime = DateTime.Now.ToString("HH::mm:ss");
            MessageToLog mtl = new MessageToLog() { message = mes, messageType = mesType, logDate = currLogTime };

            try
            {
                WriteConsole(mtl);
                if (!_logToExternalFile)
                    return;

                lock (LogBufferList)
                {
                    LogBufferList.Add(mtl);
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex.Message);
                //Detaching logger to prevent an infinite recursive call
                Application.logMessageReceived -= HandleLog;
                _isLoggerReady = false;
            }


        }

        public void EmptyBuffer()
        {
            try
            {
                if (!_logToExternalFile || !_isLoggerReady)
                    return;

                lock (LogBufferList)
                {
                    if (LogBufferList.Count == 0)
                        return;

                    string interpolated = String.Concat("LI-", DateTime.Today.ToString("yyyy-MM-dd"), "-", DateTime.Now.ToString("HH"), ".txt");
                    using
                        (
                            var log = File.AppendText(Path.Combine(_externalPath, interpolated))
                        )
                    {
                        foreach (var line in LogBufferList)
                        {
                            log.WriteLine(GetLoggerLine(line));
                        }

                        log.Flush();
                        LogBufferList.Clear();

                    }

                }
            }
            catch (Exception ex)
            {
                //This can cause infinite loop. Cannot do anything
                //throw ex;
                //Debug.LogError(ex.Message);
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

        protected override void Awake()
        {
            _screenLogger = null;
            _externalPath = Path.Combine(Application.persistentDataPath, "Logs");
            _logToExternalFile = false;
            _isLoggerReady = false;
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

            MessageToLog mtl = new MessageToLog() { message = logString };
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

            string currLogTime = DateTime.Now.ToString("HH::mm:ss");
            mtl.logDate = currLogTime;

            lock (LogBufferList)
            {
                LogBufferList.Add(mtl);
            }
        }

        public void Update()
        {
            try
            {
                if (_isLoggerReady && _lastLogTime.AddSeconds(60) < DateTime.UtcNow && !applicationQuitting)
                {
                    _lastLogTime = DateTime.UtcNow;
                    EmptyBuffer();  
                }
            }
            catch (Exception ex)
            {
            }
            
        }
        #endregion

    }
}
