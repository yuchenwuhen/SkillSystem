using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.UI
{
    public class UISystemEvent
    {
        public static Dictionary<UIEvent, UICallBack> allUIEvents = new Dictionary<UIEvent, UICallBack>();
        public static Dictionary<string, Dictionary<UIEvent, UICallBack>> singleUIEvents = new Dictionary<string, Dictionary<UIEvent, UICallBack>>();

        /// <summary>
        /// 每个UIEvent都会派发的事件
        /// </summary>
        /// <param name="Event">事件类型</param>
        /// <param name="callback">回调函数</param>
        public static void RegisterAllUIEvent(UIEvent UIEvent, UICallBack CallBack)
        {
            if (allUIEvents.ContainsKey(UIEvent))
            {
                allUIEvents[UIEvent] += CallBack;
            }
            else
            {
                allUIEvents.Add(UIEvent, CallBack);
            }
        }

        public static void RemoveAllUIEvent(UIEvent UIEvent, UICallBack l_CallBack)
        {
            if (allUIEvents.ContainsKey(UIEvent))
            {
                allUIEvents[UIEvent] -= l_CallBack;
            }
            else
            {
                Debug.LogError("RemoveAllUIEvent don't exits: " + UIEvent);
            }
        }

        /// <summary>
        /// 注册单个UI派发的事件
        /// </summary>
        /// <param name="Event">事件类型</param>
        /// <param name="callback"回调函数></param>
        public static void RegisterEvent(string UIName, UIEvent UIEvent, UICallBack CallBack)
        {
            if (singleUIEvents.ContainsKey(UIName))
            {
                if (singleUIEvents[UIName].ContainsKey(UIEvent))
                {
                    singleUIEvents[UIName][UIEvent] += CallBack;
                }
                else
                {
                    singleUIEvents[UIName].Add(UIEvent, CallBack);
                }
            }
            else
            {
                singleUIEvents.Add(UIName, new Dictionary<UIEvent, UICallBack>());
                singleUIEvents[UIName].Add(UIEvent, CallBack);
            }
        }

        public static void RemoveEvent(string UIName, UIEvent UIEvent, UICallBack CallBack)
        {
            if (singleUIEvents.ContainsKey(UIName))
            {
                if (singleUIEvents[UIName].ContainsKey(UIEvent))
                {
                    singleUIEvents[UIName][UIEvent] -= CallBack;
                }
                else
                {
                    Debug.LogError("RemoveEvent 不存在的事件！ UIName " + UIName + " UIEvent " + UIEvent);
                }

            }
            else
            {
                Debug.LogError("RemoveEvent 不存在的事件！ UIName " + UIName + " UIEvent " + UIEvent);
            }
        }

        /// <summary>
        /// 派发UI事件
        /// </summary>
        /// <param name="UI"></param>
        /// <param name="UIEvent"></param>
        /// <param name="objs"></param>
        public static void Dispatch(UIWindowBase UI, UIEvent UIEvent, params object[] objs)
        {
            if (UI == null)
            {
                Debug.LogError("Dispatch UI is null!");

                return;
            }

            if (allUIEvents.ContainsKey(UIEvent))
            {
                try
                {
                    if (allUIEvents[UIEvent] != null)
                        allUIEvents[UIEvent](UI, objs);
                }
                catch (Exception e)
                {
                    Debug.LogError("UISystemEvent Dispatch error:" + e.ToString());
                }
            }

            if (singleUIEvents.ContainsKey(UI.name))
            {
                if (singleUIEvents[UI.name].ContainsKey(UIEvent))
                {
                    try
                    {
                        if (singleUIEvents[UI.name][UIEvent] != null)
                            singleUIEvents[UI.name][UIEvent](UI, objs);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("UISystemEvent Dispatch error:" + e.ToString());
                    }
                }
            }
        }
    }
}

