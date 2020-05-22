using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VFramework.Common
{
    public class FilePathHelp 
    {
        static public void SaveStringToFile(string strPath, string strInfo)
        {
            Debug.Log("存储文件" + strPath);

            FileStream fs = new FileStream(strPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(strInfo);

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        static public string LoadFileToString(string strPath)
        {
            //Debug.Log("加载文件" + strPath);
            StreamReader sr = new StreamReader(strPath, Encoding.UTF8);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
    }
}


