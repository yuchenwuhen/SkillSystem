using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VFramework.Common;

namespace VFramework.Tools
{
    /// <summary>
    /// 资源读取器，负责从不同路径读取资源
    /// </summary>
    public class ResourceIOTool : MonoSingleton<ResourceIOTool>
    {

        #region 读操作

        public static string ReadStringByFile(string path)
        {
            StringBuilder line = new StringBuilder();
            try
            {
                if (!File.Exists(path))
                {
                    Debug.Log("path don't exists:" + path);
                    return "";
                }

                using (StreamReader sr = File.OpenText(path))
                {
                    line.Append(sr.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log("Load text fail ! message:" + e.Message);
            }

            return line.ToString();
        }

        #endregion

        #region 写操作

#if !UNITY_WEBGL || UNITY_EDITOR
        //web Player 不支持写操作

        /// <summary>
        //  写文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteStringByFile(string path, string content)
        {
            byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);

            CreateFile(path, dataByte);
        }

        public static void CreateFile(string path, byte[] bytes)
        {
            try
            {
                FileTool.CreateFilePath(path);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError("File Create Fail! \n" + e.Message);
            }
        }
#endif

#endregion

    }
}


