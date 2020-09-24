using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VFramework.Common;

/// <summary>
/// 资源变化事件
/// </summary>
public class ProjectAssetsChangeEvent
{
    [InitializeOnLoadMethod]
    static void EventFileChange()
    {
        PeojectAssetWillModificationEvent.OnCreateAssetCallBack += OnCreateAssetCallBack;
        PeojectAssetWillModificationEvent.OnDeleteAssetCallBack += OnDeleteAssetCallBack;
        PeojectAssetWillModificationEvent.OnMoveAssetCallBack += OnMoveAssetCallBack;
        PeojectAssetWillModificationEvent.OnSaveAssetsCallBack += OnSaveAssetsCallBack;
        EditorApplication.projectWindowChanged += OnProjectWindowChanged;
    }

    private static void OnProjectWindowChanged()
    {
        //Debug.Log("OnProjectWindowChanged");
        //UpdateAsset(null);
    }

    private static void OnCreateAssetCallBack(string t)
    {
        List<string> paths = new List<string>();
        paths.Add(t);
        //UpdateAsset(paths);
        //Debug.Log("OnCreateAssetCallBack");
    }

    private static void OnSaveAssetsCallBack(string[] t)
    {
        List<string> paths = new List<string>();
        paths.AddRange(t);
        //UpdateAsset(paths);
        //Debug.Log("OnSaveAssetsCallBack");
    }

    private static void OnMoveAssetCallBack(AssetMoveResult t, string t1, string t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        paths.Add(t2);
        //UpdateAsset(paths);
        //Debug.Log("OnMoveAssetCallBack");
    }

    private static void OnDeleteAssetCallBack(AssetDeleteResult t, string t1, RemoveAssetOptions t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        //UpdateAsset(paths);
        //Debug.Log("OnDeleteAssetCallBack");
    }

    private static void UpdateAsset(List<string> paths)
    {
        bool isUpdate = false;
        //if (paths == null)
        //    isUpdate = true;
        //else
        //{
        //    foreach (var item in paths)
        //    {
        //        if (item.Contains("Assets/Resources"))
        //        {
        //            isUpdate = true;
        //            break;
        //        }
        //    }
        //}
        if (isUpdate)
        {
            if (ResourcesConfigManager.GetIsExistResources())
            {
                ResourcesConfigManager.CreateResourcesConfig();
                ResourcesConfigManager.ClearConfig();
                AssetDatabase.Refresh();
                Debug.Log("创建资源路径文件");
            }
        }
    }
}
