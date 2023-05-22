using System;
using System.IO;
using UnityEngine;

namespace VRP.Helpers
{
    public static class Log
    {
        private const string DefaultTag = "VRP";
        private const string FileName = "DebugLogExport_{0}.txt";
        private const string DateTimeFormat = "MM-dd-yy_hh-mm-ss";

        private static bool _isActive;
        private static string _folderName => Application.persistentDataPath;
        private static string _filePath;
        private static ILogger UnityLogger => Debug.unityLogger;


        static Log()
        {
            Application.logMessageReceived += HandleLog;
        }

        /// <summary>
        /// Called on all messages, including system
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stacktrace"></param>
        /// <param name="type"></param>
        private static void HandleLog(string condition, string stacktrace, LogType type)
        {
            if (!_isActive)
            {
                return;
            }

            DateTime time = DateTime.Now;
            string timeStamp = $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}.{time.Millisecond:D3}";

            string fullMessage = $"## {timeStamp} {type}\n{condition}\n\n";

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
            if (Application.isEditor && !Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }

            _filePath = Path.Combine(_folderName, string.Format(FileName, DateTime.Now.ToString(DateTimeFormat)));

            Debug.Log($"Exporting Log File to {_filePath}");

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
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