using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace VFramework.Common
{
    public class ResourcesMgr
    {
        private static Dictionary<string, string> m_resConfigDic;

        static ResourcesMgr()
        {
            //加载文件
            string fileContent = GetConfigFile();
            //解析文件
            BuildMap(fileContent);
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        private static string GetConfigFile()
        {
            string url;
#if UNITY_EDITOR || UNITY_STANDALONE
            url = "file://" + Application.dataPath + "/StreamingAssets/ResConfig.txt";
#elif UNITY_ANDROID
        url = "jar:file://" + Application.dataPath + "!/assets/ResConfig.txt";
#elif UNITY_IPHONE
        url = "file://" + Application.dataPath + "/Raw/ResConfig.txt";
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SendWebRequest();
            while (true)
            {
                if (webRequest.isDone)
                {
                    return webRequest.downloadHandler.text;
                }
            }
        }

        private static void BuildMap(string fileContent)
        {
            m_resConfigDic = new Dictionary<string, string>();

            //当程序退出using代码块，将自动调用reader.Dispose方法
            using (StringReader reader = new StringReader(fileContent))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] keyValue = line.Split('=');
                    if (m_resConfigDic.ContainsKey(keyValue[0]))
                    {
                        Debug.LogWarning("has same prefab name:" + keyValue[0]);
                        continue;
                    }
                    m_resConfigDic.Add(keyValue[0], keyValue[1]);
                }
            }
        }

        public static T Load<T>(string fileName) where T : Object
        {
            string prefabPath = m_resConfigDic[fileName];
            return Resources.Load<T>(prefabPath);
        }

    }
}


