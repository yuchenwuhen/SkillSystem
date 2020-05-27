using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VFramework.Tools;

namespace VFramework.Common
{
    public class RecordManager
    {
        public const string DirectoryName = "Record";
        public const string ExpandName = "json";

        /// <summary>
        /// 记录缓存
        /// </summary>
        static Dictionary<string, RecordTable> s_recordCache = new Dictionary<string, RecordTable>();

        /// <summary>
        /// 根据RecordName获取数据,初次从文件中找Json数据
        /// </summary>
        /// <param name="recordName"></param>
        /// <returns></returns>
        public static RecordTable GetData(string recordName)
        {
            if (s_recordCache.ContainsKey(recordName))
            {
                return s_recordCache[recordName];
            }

            RecordTable record = null;

            string dataJson = "";

            string fullPath = PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(DirectoryName, recordName, ExpandName));

            if (File.Exists(fullPath))
            {
                dataJson = ResourceIOTool.ReadStringByFile(fullPath);
            }

            if (dataJson == "")
            {
                record = new RecordTable();
            }
            else
            {
                record = RecordTable.Analysis(dataJson);
            }

            s_recordCache.Add(recordName, record);

            return record;
        }

        public static void SaveData(string recordName, RecordTable table)
        {
#if !UNITY_WEBGL

            ResourceIOTool.WriteStringByFile(
                PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(DirectoryName, recordName, ExpandName)),
                RecordTable.Serialize(table));

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.AssetDatabase.Refresh();
            }
    #endif
#endif
        }

        public static void CleanRecord(string recordName)
        {
            RecordTable table = GetData(recordName);
            table.Clear();
            SaveData(recordName, table);
        }

        public static void CleanAllRecord()
        {
            FileTool.DeleteDirectory(Application.persistentDataPath + "/" + RecordManager.DirectoryName);
            CleanCache();
        }

        public static void CleanCache()
        {
            s_recordCache.Clear();
        }

        #region 取值封装

        /// <summary>
        /// 获取浮点记录值
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="key"></param>
        /// <param name="defaultKey"></param>
        /// <returns></returns>
        public static float GetFloatRecord(string recordName, string key, float defaultKey)
        {
            RecordTable table = GetData(recordName);

            return table.GetRecord(key, defaultKey);
        }

        public static int GetIntRecord(string RecordName, string key, int defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        public static string GetStringRecord(string RecordName, string key, string defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        public static bool GetBoolRecord(string RecordName, string key, bool defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        public static Vector2 GetVector2Record(string RecordName, string key, Vector2 defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        public static Vector3 GetVector3Record(string RecordName, string key, Vector3 defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        public static Color GetColorRecord(string RecordName, string key, Color defaultValue)
        {
            RecordTable table = GetData(RecordName);

            return table.GetRecord(key, defaultValue);
        }

        static Deserializer des = new Deserializer();

        public static T GetTRecord<T>(string RecordName, string key, T defaultValue)
        {
            string content = GetStringRecord(RecordName, key, null);

            if (content == null)
            {
                return defaultValue;
            }
            else
            {
                return des.Deserialize<T>(content);
            }
        }

        #endregion

        #region 保存封装

        public static void SaveRecord(string RecordName, string key, string value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord(string RecordName, string key, int value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord(string RecordName, string key, bool value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord(string recordName, string key, float value)
        {
            RecordTable table = GetData(recordName);
            table.SetRecord(key, value);
            SaveData(recordName,table);
        }

        public static void SaveRecord(string RecordName, string key, Vector2 value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord(string RecordName, string key, Vector3 value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord(string RecordName, string key, Color value)
        {
            RecordTable table = GetData(RecordName);
            table.SetRecord(key, value);
            SaveData(RecordName, table);
        }

        public static void SaveRecord<T>(string RecordName, string key, T value)
        {
            string content = Serializer.Serialize(value);
            SaveRecord(RecordName, key, content);
        }

        #endregion
    }

}
