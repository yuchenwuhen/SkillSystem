using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VFramework.Common
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
            UISystemEvent.Dispatch(UIWIndowBase, UIEvent.OnInit);  //派发OnInit事件

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
        public static void CloseUIWindow(UIWindowBase UI, bool isPlayAnim = true, UICallBack callback = null, params object[] objs)
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
                UIAnimManager.StartExitAnim(UI, callback, objs);
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
        public static void CloseUIWindow(string UIname, bool isPlayAnim = true, UICallBack callback = null, params object[] objs)
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

        public static void CloseUIWindow<T>(bool isPlayAnim = true, UICallBack callback = null, params object[] objs) where T : UIWindowBase
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
            List<string> keys = new List<string>(s_UIs.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = s_UIs[keys[i]];
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
            List<string> keys = new List<string>(s_UIs.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = s_UIs[keys[i]];
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
            List<string> keys = new List<string>(s_UIs.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = s_UIs[keys[i]];
                for (int j = 0; j < list.Count; j++)
                {
                    CloseUIWindow(list[i], isPlayerAnim);
                }
            }
        }

        public static void CloseLastUI(UIType uiType = UIType.Normal)
        {
            UIStackManager.CloseLastUIWindow(uiType);
        }

        #endregion
    }
}

