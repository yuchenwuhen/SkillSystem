using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;

namespace VFramework.UI
{
    public class UIWindowBase : UIBase
    {
        [HideInInspector]
        public string cameraKey;

        [HideInInspector]
        public WindowStatus windowStatus;

        public UIType m_UIType;

        public float m_PosZ; //Z轴偏移

        #region 初始化

        public void InitWindow(int id)
        {
            List<UILifeCycleInterface> list = new List<UILifeCycleInterface>();
            Init(null, id);
            InitChildUI(null, this, id, list);
        }

        public void InitChildUI(UIBase parentUI,UIBase uiBase,int id, List<UILifeCycleInterface> UIList)
        {
            int childIndex = 0;
            for (int i = 0; i < uiBase.m_childUIList.Count; i++)
            {
                GameObject go = uiBase.m_childUIList[i];

                if (go != null)
                {
                    UILifeCycleInterface tmp = go.GetComponent<UILifeCycleInterface>();

                    if (tmp != null)
                    {
                        if (!UIList.Contains(tmp))
                        {
                            uiBase.AddLifeCycleComponent(tmp);

                            UIList.Add(tmp);

                            UIBase subUI = uiBase.m_childUIList[i].GetComponent<UIBase>();
                            if (subUI != null)
                            {
                                InitChildUI(uiBase, subUI, childIndex++, UIList);
                            }
                        }
                        else
                        {
                            Debug.LogError("InitWindow 重复的引用 " + uiBase.UIEventKey + " " + uiBase.m_childUIList[i].name);
                        }

                    }
                }
                else
                {
                    Debug.LogWarning("InitWindow objectList[" + i + "] is null !: " + uiBase.UIEventKey);
                }
            }
        }

        #endregion

        #region 重载方法

        /// <summary>
        /// 打开UI
        /// </summary>
        public virtual void OnOpen(){}

        /// <summary>
        /// 关闭UI
        /// </summary>
        public virtual void OnClose(){}

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public virtual void OnHide(){}

        /// <summary>
        /// 显示UI
        /// </summary>
        public virtual void OnShow(){}

        /// <summary>
        /// 刷新UI，需注册消息
        /// </summary>
        public virtual void OnRefresh(){}

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual IEnumerator EnterAnim(UIAnimCallBack animComplete, UICallBack callBack, params object[] objs)
        {
            //默认无动画
            animComplete(this, callBack, objs);

            yield break;
        }

        public virtual void OnCompleteEnterAnim()
        {
        }

        public virtual IEnumerator ExitAnim(UIAnimCallBack animComplete, UICallBack callBack, params object[] objs)
        {
            //默认无动画
            animComplete(this, callBack, objs);

            yield break;
        }

        public virtual void OnCompleteExitAnim()
        {
        }

        #endregion

        #region 继承方法

        public override void RemoveAllListener()
        {
            EventMgr.Instance.RemoveListener(this);
        }

        #endregion

        public enum WindowStatus
        {
            Create,
            Open,
            Close,
            OpenAnim,
            CloseAnim,
            Hide,
        }

    }

    
}


