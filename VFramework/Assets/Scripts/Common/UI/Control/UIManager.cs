using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VFramework.Common;

namespace VFramework.UI
{
    [RequireComponent(typeof(UIStackManager))]
    [RequireComponent(typeof(UILayerManager))]
    [RequireComponent(typeof(UIAnimManager))]
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : MonoBehaviour
    {

        private static GameObject s_uiManagerGo;
        
        /// <summary>
        /// UI层级管理器
        /// </summary>
        private static UILayerManager s_uiLayerManager;

        /// <summary>
        /// UI存储栈管理
        /// </summary>
        private static UIStackManager s_uiStackManager;

        /// <summary>
        /// UI动画管理
        /// </summary>
        private static UIAnimManager s_uiAnimManager;

        private static EventSystem s_eventSystem;

        /// <summary>
        /// 显示的UI列表
        /// </summary>
        public static Dictionary<string, List<UIWindowBase>> showUIList = new Dictionary<string, List<UIWindowBase>>();
        /// <summary>
        /// 隐藏的UI列表
        /// </summary>
        public static Dictionary<string, List<UIWindowBase>> hideUIList = new Dictionary<string, List<UIWindowBase>>();

        #region 初始化

        static bool m_isInit;

        public static void Init()
        {
            if (!m_isInit)
            {
                m_isInit = true;

                GameObject instance = GameObject.Find("UIManager");

                if (instance == null)
                {
                    instance = GameObjectPool.Instance.CreateGameObjectByPool("UIManager", Vector3.zero, Quaternion.identity);
                }

                s_uiManagerGo = instance;

                s_uiLayerManager = instance.GetComponent<UILayerManager>();
                s_uiAnimManager = instance.GetComponent<UIAnimManager>();
                s_uiStackManager = instance.GetComponent<UIStackManager>();
                s_eventSystem = instance.GetComponentInChildren<EventSystem>();

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance);
                }
            }
        }

        public static UILayerManager UILayerManager
        {
            get
            {
                if (s_uiLayerManager == null)
                {
                    Init();
                }
                return s_uiLayerManager;
            }

            set
            {
                s_uiLayerManager = value;
            }
        }

        public static UIAnimManager UIAnimManager
        {
            get
            {
                if (s_uiLayerManager == null)
                {
                    Init();
                }
                return s_uiAnimManager;
            }

            set
            {
                s_uiAnimManager = value;
            }
        }

        public static UIStackManager UIStackManager
        {
            get
            {
                if (s_uiStackManager == null)
                {
                    Init();
                }
                return s_uiStackManager;
            }

            set
            {

                s_uiStackManager = value;
            }
        }

        public static EventSystem EventSystem
        {
            get
            {
                if (s_eventSystem == null)
                {
                    Init();
                }
                return s_eventSystem;
            }

            set
            {
                s_eventSystem = value;
            }
        }

        public static GameObject UIManagerGo
        {
            get
            {
                if (s_uiManagerGo == null)
                {
                    Init();
                }
                return s_uiManagerGo;
            }

            set
            {
                s_uiManagerGo = value;
            }
        }

        #endregion

        #region EventSystem

        public static void SetEventSystemEnable(bool enable)
        {
            if (EventSystem != null)
            {
                EventSystem.enabled = enable;
            }
            else
            {
                Debug.LogError("EventSystem.current is null !");
            }
        }

        #endregion

        #region UI的打开与关闭方法

        /// <summary>
        /// 创建UI,如果不打开则存放在Hide列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateUIWindow<T>() where T : UIWindowBase
        {
            return (T)CreateUIWindow(typeof(T).Name);
        }

        public static UIWindowBase CreateUIWindow(string UIName)
        {
            Debug.Log("CreateUIWindow " + UIName);

            GameObject UItmp = GameObjectPool.Instance.CreateGameObjectByPool(UIName,Vector3.zero,Quaternion.identity, UIManagerGo);
            UIWindowBase UIWIndowBase = UItmp.GetComponent<UIWindowBase>();

            UIWIndowBase.windowStatus = UIWindowBase.WindowStatus.Create;

            try
            {
                UIWIndowBase.InitWindow(GetUIID(UIName));
            }
            catch (Exception e)
            {
                Debug.LogError(UIName + "OnInit Exception: " + e.ToString());
            }

            AddHideUI(UIWIndowBase);

            UILayerManager.SetLayer(UIWIndowBase);      //设置层级

            return UIWIndowBase;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI</returns>
        public static UIWindowBase OpenUIWindow(string UIName, UICallBack callback = null, params object[] objs)
        {
            UIWindowBase UIbase = GetHideUI(UIName);

            if (UIbase == null)
            {
                UIbase = CreateUIWindow(UIName);
            }

            RemoveHideUI(UIbase);
            AddUI(UIbase);

            UIStackManager.OnUIOpen(UIbase);
            UILayerManager.SetLayer(UIbase);      //设置层级

            UIbase.windowStatus = UIWindowBase.WindowStatus.OpenAnim;

            UISystemEvent.Dispatch(UIbase, UIEvent.OnOpen);  //派发OnOpen事件
            try
            {
                UIbase.OnOpen();
            }
            catch (Exception e)
            {
                Debug.LogError(UIName + " OnOpen Exception: " + e.ToString());
            }

            UISystemEvent.Dispatch(UIbase, UIEvent.OnOpened);  //派发OnOpened事件

            UIAnimManager.StartEnterAnim(UIbase, callback, objs); //播放动画
            return UIbase;
        }

        public static T OpenUIWindow<T>() where T : UIWindowBase
        {
            return (T)OpenUIWindow(typeof(T).Name);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UI">目标UI</param>
        /// <param name="isPlayAnim">是否播放关闭动画</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>
        public static void CloseUIWindow(UIWindowBase UI, bool isPlayAnim = false, UICallBack callback = null, params object[] objs)
        {
            RemoveUI(UI);        //移除UI引用
            UI.RemoveAllListener();

            if (isPlayAnim)
            {
                //动画播放完毕删除UI
                if (callback != null)
                {
                    callback += CloseUIWindowCallBack;
                }
                else
                {
                    callback = CloseUIWindowCallBack;
                }
                UI.windowStatus = UIWindowBase.WindowStatus.CloseAnim;
                //UIAnimManager.StartExitAnim(UI, callback, objs);
            }
            else
            {
                CloseUIWindowCallBack(UI, objs);
            }
        }
        static void CloseUIWindowCallBack(UIWindowBase UI, params object[] objs)
        {
            UI.windowStatus = UIWindowBase.WindowStatus.Close;
            UISystemEvent.Dispatch(UI, UIEvent.OnClose);  //派发OnClose事件
            try
            {
                UI.OnClose();
            }
            catch (Exception e)
            {
                Debug.LogError(UI.UIName + " OnClose Exception: " + e.ToString());
            }

            UIStackManager.OnUIClose(UI);
            AddHideUI(UI);

            UISystemEvent.Dispatch(UI, UIEvent.OnClosed);  //派发OnClosed事件
        }
        public static void CloseUIWindow(string UIname, bool isPlayAnim = false, UICallBack callback = null, params object[] objs)
        {
            UIWindowBase ui = GetUI(UIname);

            if (ui == null)
            {
                Debug.LogError("CloseUIWindow Error UI ->" + UIname + "<-  not Exist!");
            }
            else
            {
                CloseUIWindow(GetUI(UIname), isPlayAnim, callback, objs);
            }
        }

        public static void CloseUIWindow<T>(bool isPlayAnim = false, UICallBack callback = null, params object[] objs) where T : UIWindowBase
        {
            CloseUIWindow(typeof(T).Name, isPlayAnim, callback, objs);
        }

        public static UIWindowBase ShowUI(string UIname)
        {
            UIWindowBase ui = GetUI(UIname);
            return ShowUI(ui);
        }

        public static UIWindowBase ShowUI(UIWindowBase ui)
        {
            ui.windowStatus = UIWindowBase.WindowStatus.Open;
            UISystemEvent.Dispatch(ui, UIEvent.OnShow);  //派发OnShow事件

            try
            {
                ui.Show();
                ui.OnShow();
            }
            catch (Exception e)
            {
                Debug.LogError(ui.UIName + " OnShow Exception: " + e.ToString());
            }

            return ui;
        }

        public static UIWindowBase HideUI(string UIname)
        {
            UIWindowBase ui = GetUI(UIname);
            return HideUI(ui);
        }

        public static UIWindowBase HideUI(UIWindowBase ui)
        {
            ui.windowStatus = UIWindowBase.WindowStatus.Hide;
            UISystemEvent.Dispatch(ui, UIEvent.OnHide);  //派发OnHide事件

            try
            {
                ui.Hide();
                ui.OnHide();
            }
            catch (Exception e)
            {
                Debug.LogError(ui.UIName + " OnShow Exception: " + e.ToString());
            }

            return ui;
        }

        public static void HideOtherUI(string UIName)
        {
            List<string> keys = new List<string>(showUIList.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = showUIList[keys[i]];
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].UIName != UIName)
                    {
                        HideUI(list[j]);
                    }
                }
            }
        }

        public static void ShowOtherUI(string UIName)
        {
            List<string> keys = new List<string>(showUIList.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = showUIList[keys[i]];
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].UIName != UIName)
                    {
                        ShowUI(list[j]);
                    }
                }
            }
        }

        /// <summary>
        /// 移除全部UI
        /// </summary>
        public static void CloseAllUI(bool isPlayerAnim = false)
        {
            List<string> keys = new List<string>(showUIList.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = showUIList[keys[i]];
                for (int j = 0; j < list.Count; j++)
                {
                    CloseUIWindow(list[j], isPlayerAnim);
                }
            }
        }

        public static void CloseLastUI(UIType uiType = UIType.Normal)
        {
            UIStackManager.CloseLastUIWindow(uiType);
        }

        #endregion

        #region 打开UI列表的管理

        /// <summary>
        /// 删除所有打开的UI
        /// </summary>
        //public static void DestroyAllActiveUI()
        //{
        //    foreach (List<UIWindowBase> uis in showUIList.Values)
        //    {
        //        for (int i = 0; i < uis.Count; i++)
        //        {
        //            UISystemEvent.Dispatch(uis[i], UIEvent.OnDestroy);  //派发OnDestroy事件
        //            try
        //            {
        //                uis[i].Dispose();
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("OnDestroy :" + e.ToString());
        //            }
        //            GameObjectManager.DestroyGameObjectByPool(uis[i].gameObject);
        //        }
        //    }

        //    showUIList.Clear();
        //}

        public static T GetUI<T>() where T : UIWindowBase
        {
            return (T)GetUI(typeof(T).Name);
        }
        public static UIWindowBase GetUI(string UIname)
        {
            if (!showUIList.ContainsKey(UIname))
            {
                //Debug.Log("!ContainsKey " + UIname);
                return null;
            }
            else
            {
                if (showUIList[UIname].Count == 0)
                {
                    //Debug.Log("s_UIs[UIname].Count == 0");
                    return null;
                }
                else
                {
                    //默认返回最后创建的那一个
                    return showUIList[UIname][showUIList[UIname].Count - 1];
                }
            }
        }

        //public static UIBase GetUIBaseByEventKey(string eventKey)
        //{
        //    string UIkey = eventKey.Split('.')[0];
        //    string[] keyArray = UIkey.Split('_');

        //    string uiEventKey = "";

        //    UIBase uiTmp = null;
        //    for (int i = 0; i < keyArray.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            uiEventKey = keyArray[0];
        //            uiTmp = GetUIWindowByEventKey(uiEventKey);
        //        }
        //        else
        //        {
        //            uiEventKey += "_" + keyArray[i];
        //            uiTmp = uiTmp.GetItemByKey(uiEventKey);
        //        }

        //        Debug.Log("uiEventKey " + uiEventKey);
        //    }

        //    return uiTmp;
        //}

        //static Regex uiKey = new Regex(@"(\S+)\d+");

        //static UIWindowBase GetUIWindowByEventKey(string eventKey)
        //{
        //    string UIname = uiKey.Match(eventKey).Groups[1].Value;

        //    if (!s_UIs.ContainsKey(UIname))
        //    {
        //        throw new Exception("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
        //    }

        //    List<UIWindowBase> list = s_UIs[UIname];
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].UIEventKey == eventKey)
        //        {
        //            return list[i];
        //        }
        //    }

        //    throw new Exception("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
        //}

        static bool GetIsExits(UIWindowBase UI)
        {
            if (!showUIList.ContainsKey(UI.name))
            {
                return false;
            }
            else
            {
                return showUIList[UI.name].Contains(UI);
            }
        }

        static void AddUI(UIWindowBase UI)
        {
            if (!showUIList.ContainsKey(UI.name))
            {
                showUIList.Add(UI.name, new List<UIWindowBase>());
            }

            showUIList[UI.name].Add(UI);

            UI.Show();
        }

        static void RemoveUI(UIWindowBase UI)
        {
            if (UI == null)
            {
                throw new Exception("UIManager: RemoveUI error UI is null: !");
            }

            if (!showUIList.ContainsKey(UI.name))
            {
                throw new Exception("UIManager: RemoveUI error dont find UI name: ->" + UI.name + "<-  " + UI);
            }

            if (!showUIList[UI.name].Contains(UI))
            {
                throw new Exception("UIManager: RemoveUI error dont find UI: ->" + UI.name + "<-  " + UI);
            }
            else
            {
                showUIList[UI.name].Remove(UI);
            }
        }

        /// <summary>
        /// 分配ID，如果UIName已分配过，分配新的ID
        /// </summary>
        /// <param name="UIname"></param>
        /// <returns></returns>
        static int GetUIID(string UIname)
        {
            if (!showUIList.ContainsKey(UIname))
            {
                return 0;
            }
            else
            {
                int id = showUIList[UIname].Count;

                for (int i = 0; i < showUIList[UIname].Count; i++)
                {
                    if (showUIList[UIname][i].UIID == id)
                    {
                        id++;
                        i = 0;
                    }
                }

                return id;
            }
        }

        public static int GetNormalUICount()
        {
            return UIStackManager.m_normalStack.Count;
        }

        #endregion

        #region 隐藏UI列表的管理

        static void AddHideUI(UIWindowBase UI)
        {
            if (!hideUIList.ContainsKey(UI.name))
            {
                hideUIList.Add(UI.name, new List<UIWindowBase>());
            }

            hideUIList[UI.name].Add(UI);

            UI.Hide();
        }

        /// <summary>
        /// 获取一个隐藏的UI,如果有多个同名UI，则返回最后创建的那一个
        /// </summary>
        /// <param name="UIname">UI名</param>
        /// <returns></returns>
        public static UIWindowBase GetHideUI(string UIname)
        {
            if (!hideUIList.ContainsKey(UIname))
            {
                return null;
            }
            else
            {
                if (hideUIList[UIname].Count == 0)
                {
                    return null;
                }
                else
                {
                    UIWindowBase ui = hideUIList[UIname][hideUIList[UIname].Count - 1];
                    //默认返回最后创建的那一个
                    return ui;
                }
            }
        }

        static void RemoveHideUI(UIWindowBase UI)
        {
            if (UI == null)
            {
                throw new Exception("UIManager: RemoveUI error l_UI is null: !");
            }

            if (!hideUIList.ContainsKey(UI.name))
            {
                throw new Exception("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
            }

            if (!hideUIList[UI.name].Contains(UI))
            {
                throw new Exception("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
            }
            else
            {
                hideUIList[UI.name].Remove(UI);
            }
        }

        #endregion
    }

    #region UI事件 代理 枚举

    /// <summary>
    /// UI回调
    /// </summary>
    /// <param name="objs"></param>
    public delegate void UICallBack(UIWindowBase UI, params object[] objs);
    public delegate void UIAnimCallBack(UIWindowBase UIbase, UICallBack callBack, params object[] objs);

    public enum UIType
    {
        GameUI = 0,     //在UI分层中的最底层，一般用于游戏里面的血条，浮动UI等。

        Fixed = 1,      //在GameUI层之上，一般用于常驻的UI如主城UI。
        Normal = 2,     //在Fixed之上，一般用于普通UI，例如商店。
        TopBar = 3,     //在normal之上 ，一般用于常驻的置顶显示的UI,例如体力和金钱UI。 
        Upper = 4,      //在TopBar之上 ，一般用于更上一层，例如对话框UI。 
        PopUp = 5,      //在最上层，一般用来显示弹窗。
    }

    public enum UIEvent
    {
        OnOpen,
        OnOpened,

        OnClose,
        OnClosed,

        OnHide,
        OnShow,

        OnInit,
        OnDestroy,

        OnRefresh,

        OnStartEnterAnim,
        OnCompleteEnterAnim,

        OnStartExitAnim,
        OnCompleteExitAnim,
    }
    #endregion
}

