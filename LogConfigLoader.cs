using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VRP.Helpers
{
    public static class LogConfigLoader
    {
        private const string LogConfigName = "LogConfig.txt";
        private const string IsFullUnityLogsParameter = "IsFullUnityLogs";
        private const string ToKeepAllLogFilesParameter = "ToKeepAllLogFiles";
        private const string ActivateLoggerParameter = "ActivateLogger";
        private const char ParamsSeparator = '=';

        private static string PathToConfig => Path.Combine(Application.persistentDataPath, LogConfigName);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            Log.IsFullUnityLogs = ReadBool(IsFullUnityLogsParameter, defaultValue:true);
            Log.ToKeepAllLogFiles = ReadBool(ToKeepAllLogFilesParameter, defaultValue:true);
            Log.SetLoggerActive(ReadBool(ActivateLoggerParameter, defaultValue:true));
        }

        private static bool ReadBool(string paramName, bool defaultValue)
        {
            string value = ReadValue(paramName);

            if (!string.IsNullOrEmpty(value))
            {
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }

            WriteValue(paramName, defaultValue.ToString());
            return defaultValue;
        }
        
        private static string ReadValue(string paramName)
        {
            if (!File.Exists(PathToConfig))
                return null;

            string[] lines = File.ReadAllLines(PathToConfig);

            return (from line in lines select line.Split(ParamsSeparator)
                into parts 
                where parts.Length == 2 && parts[0].Trim() == paramName 
                select parts[1].Trim()).FirstOrDefault();
        }
        
        private static void WriteValue(string paramName, string value)
        {
            List<string> lines = new List<string>();

            if (File.Exists(PathToConfig))
            {
                string[] existingLines = File.ReadAllLines(PathToConfig);
                lines.AddRange(existingLines);
            }

            bool paramFound = false;
            
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(ParamsSeparator);

                if (parts.Length != 2 || parts[0].Trim() != paramName)
                {
                    continue;
                }

                lines[i] = paramName + ParamsSeparator + value;
                paramFound = true;
                break;
            }

            if (!paramFound)
                lines.Add(paramName + ParamsSeparator + value);

            File.WriteAllLines(PathToConfig, lines.ToArray());
        }
    }
}