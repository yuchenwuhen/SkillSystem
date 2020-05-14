using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GeneratorResConfig : Editor
{
    /// <summary>
    /// 将Resources里的Prefab名字和完整路径存储起来
    /// </summary>
    [MenuItem("Tools/Resources/GeneratorResConfig")]
    public static void GeneratorRes()
    {
        string[] assets = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });
        for(int i = 0; i < assets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[i]);

            string fileName = Path.GetFileNameWithoutExtension(path);
            string filePath = path.Replace("Assets/Resources/", string.Empty).Replace(".prefab", string.Empty);
            assets[i] = fileName + "=" + filePath;
        }

        //写入文件
        File.WriteAllLines("Assets/StreamingAssets/ResConfig.txt", assets);
        
        AssetDatabase.Refresh();
    }
}
