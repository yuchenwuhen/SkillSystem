using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VFramework.Tools
{
    public class FileTool
    {

        #region 文件与路径的增加删除创建

        public static void CreateFilePath(string filePath)
        {
            string newPathDir = Path.GetDirectoryName(filePath);

            CreatePath(newPathDir);
        }

        public static void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 删掉某个目录下的所有子目录和子文件，但是保留这个目录
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectory(string path)
        {
            string[] directorys = Directory.GetDirectories(path);

            //删掉所有子目录
            for (int i = 0; i < directorys.Length; i++)
            {
                string pathTmp = directorys[i];

                if (Directory.Exists(pathTmp))
                {
                    Directory.Delete(pathTmp, true);
                }
            }

            //删掉所有子文件
            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                string pathTmp = files[i];

                if (File.Exists(pathTmp))
                {
                    File.Delete(pathTmp);
                }
            }
        }

        #endregion

        #region 文件名

        //移除拓展名
        public static string RemoveExpandName(string name)
        {
            if (Path.HasExtension(name))
                name = Path.ChangeExtension(name, null);
            return name;
        }

        //取出一个路径下的文件名
        public static string GetFileNameByPath(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Name; // text.txt
        }

        #endregion

    }
}


