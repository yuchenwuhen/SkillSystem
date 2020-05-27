using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VFramework.Tools;

namespace VFramework.Common
{
    public static class ResourcesConfigManager
    {

        public const string ManifestFileName = "ResourcesManifest";
        public const string PathKey = "Path";

        static bool s_isInit = false;

        public static void ClearConfig()
        {
            s_isInit = false;
        }

#if UNITY_EDITOR
        public const string MainKey = "Res";
        static int s_direIndex = 0;
        public const string ResourceParentPath = "/Resources/";


        public static bool GetIsExistResources()
        {
            string resourcePath = Application.dataPath + ResourceParentPath;
            return Directory.Exists(resourcePath);
        }

        public static void CreateResourcesConfig()
        {
            string content = DataTable.Serialize(GenerateResourcesConfig());
            string path = PathTool.GetAbsolutePath(ResLoadLocation.Resource, ManifestFileName + "." + DataManager.ExpandName);

            ResourceIOTool.WriteStringByFile(path, content);
        }

        /// <summary>
        /// 生成资源清单路径，仅在Editor下可以调用
        /// </summary>
        /// <returns></returns>
        public static DataTable GenerateResourcesConfig()
        {
            DataTable data = new DataTable();

            data.TableKeys.Add(MainKey);

            data.TableKeys.Add(PathKey);
            data.SetDefault(PathKey, "资源相对路径");
            data.SetFieldType(PathKey, FieldType.String, null);

            string resourcePath = Application.dataPath + ResourceParentPath;
            s_direIndex = resourcePath.LastIndexOf(ResourceParentPath);
            s_direIndex += ResourceParentPath.Length;

            RecursionAddResouces(data, resourcePath);

            return data;
        }

        static void RecursionAddResouces(DataTable data, string path)
        {
            if (!File.Exists(path))
            {
                FileTool.CreatePath(path);
            }

            string[] dires = Directory.GetDirectories(path);

            for (int i = 0; i < dires.Length; i++)
            {
                RecursionAddResouces(data, dires[i]);
            }

            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(files[i]));
                string relativePath = files[i].Substring(s_direIndex);
                if (relativePath.EndsWith(".meta") || relativePath.EndsWith(".DS_Store"))
                    continue;
                else
                {
                    relativePath = FileTool.RemoveExpandName(relativePath).Replace("\\", "/");

                    SingleData sd = new SingleData();
                    sd.Add(MainKey, fileName.ToLower());
                    sd.Add(PathKey, relativePath.ToLower());

                    if (fileName.EndsWith(" "))
                    {
                        Debug.LogError("文件名尾部中有空格！ ->" + fileName + "<-");
                    }
                    else
                    {
                        if (!data.ContainsKey(fileName.ToLower()))
                        {
                            data.AddData(sd);
                        }
                        else
                        {
                            Debug.LogError("GenerateResourcesConfig error 存在重名文件！" + relativePath);
                        }
                    }
                }
            }
        }
#endif
    }
}


