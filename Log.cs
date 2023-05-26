using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VRP.Helpers
{
    public static class Log
    {
        private const string DefaultTag = "VRP";
        private const string MultipleLogsFileName = "DebugLogExport_{0}.txt";
        private const string OneLogFileName = "DebugLogExport.txt";
        private const string DateTimeFormat = "MM-dd-yy_hh-mm-ss";
        private const string LogFolder = "Logs";
        private const string StackTraceStartSeparator = "\r\n-----------------CODE PATH:\r\n";
        private const string EndLogMessageSeparator = "-----------------END LOG MESSAGE!\r\n\n\n";
        
        private const int MaxLogsFiles = 25;

        private static bool _isActive;
        private static string _filePath;
        private static string _folderPath;
        
        private static ILogger UnityLogger => Debug.unityLogger;
        
        public static bool IsFullUnityLogs { private get; set; }
        
        public static bool ToKeepAllLogFiles { private get; set; }
        
        private static string FolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_folderPath))
                {
                    _folderPath = Path.Combine(Application.persistentDataPath, LogFolder);
                }

                return _folderPath;
            }
        }


        static Log()
        {
            Application.logMessageReceived += HandleLog;
        }
        
        /// <summary>
        /// Called on all messages, including system
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!_isActive)
            {
                return;
            }

            DateTime time = DateTime.Now;
            string timeStamp = $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}.{time.Millisecond:D3}";

            string fullMessage = $"### {timeStamp}. LOG TYPE: {type}\r\n{logString}\r\n";
            
            if (IsFullUnityLogs)
            {
                fullMessage += StackTraceStartSeparator + stackTrace + EndLogMessageSeparator;
            }
            else
            {
                fullMessage += StackTraceStartSeparator + EndLogMessageSeparator;
            }

            WriteToFile(fullMessage);
        }

        public static void SetLoggerActive(bool isActive)
        {
            _isActive = isActive;

            if (_isActive)
            {
                CreateLogFile();
            }
        }

        public static void Info(string message)
        {
            if (_isActive)
            {
                UnityLogger.Log(message);
            }
        }

        public static void Info(string message, Color color)
        {
            if (_isActive)
            {
                UnityLogger.Log(message.GetColoredString(color));
            }
        }

        public static void Warning(string message)
        {
            if (_isActive)
            {
                UnityLogger.LogWarning(DefaultTag, message);
            }
        }

        public static void Warning(string message, Color color)
        {
            if (_isActive)
            {
                UnityLogger.LogWarning(DefaultTag, message.GetColoredString(color));
            }
        }

        public static void Error(string message)
        {
            if (_isActive)
            {
                UnityLogger.LogError(DefaultTag, message);
            }
        }

        public static void Error(string message, Color color)
        {
            if (_isActive)
            {
                UnityLogger.LogError(DefaultTag, message.GetColoredString(color));
            }
        }

        public static void Assert(bool condition, string message, bool justWarning = false)
        {
            if (!_isActive || !condition)
            {
                return;
            }

            if (justWarning)
            {
                Warning(message);
            }
            else
            {
                Error(message);
            }
        }

        private static void CreateLogFile()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }else
            {
                DeleteLogsIfMore();
            }

            _filePath = GetFilePath();

            Debug.Log($"Exporting Log File to {_filePath}");

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
        
        private static async void DeleteLogsIfMore()
        {
            if (Directory.GetFiles(FolderPath, "*.txt").Length < MaxLogsFiles)
            {
                return;
            }

            foreach (string file in Directory.GetFiles(FolderPath))
            {
                await Task.Run(() => File.Delete(file));
            }
        }
        
        private static string GetFilePath()
        {
            return Path.Combine(FolderPath, ToKeepAllLogFiles ?
                string.Format(MultipleLogsFileName, DateTime.Now.ToString(DateTimeFormat)) : OneLogFileName);
        }

        private static void WriteToFile(string message)
        {
            if (!_isActive)
            {
                return;
            }

            File.AppendAllText(_filePath, message);
        }
    }
}