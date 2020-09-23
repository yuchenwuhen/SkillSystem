using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.Common;

namespace VFramework.UI
{
    /// <summary>
    /// UI类基类,包括窗口类，子Item类
    /// </summary>
    public class UIBase : MonoBehaviour,UILifeCycleInterface
    {
        #region 重载方法

        /// <summary>
        /// 当UI第一次打开调用OnInit方法，调用时机在OnOpen之前
        /// </summary>
        public virtual void OnInit()
        {

        }

        public virtual void OnUIDestroy()
        {

        }

        #endregion

        #region 继承方法

        //一个UI窗口需要两个字段，name和ID，在同一场景中可能出现多个相同UIWindow
        private int m_UIID = -1;
        public int UIID
        {
            get { return m_UIID; }
        }

        private string m_UIName = string.Empty;
        public string UIName
        {
            get
            {
                if (m_UIName == string.Empty)
                {
                    m_UIName = name;
                }

                return m_UIName;
            }
            set
            {
                m_UIName = value;
            }
        }

        public string UIEventKey
        {
            get { return UIName + "@" + UIID; }
        }

        /// <summary>
        /// 创建窗口时调用
        /// </summary>
        /// <param name="UIEventKey"></param>
        /// <param name="id"></param>
        public void Init(string UIEventKey, int id)
        {
            if (UIEventKey != null)
            {
                //初始化加上UIEventKey可以追溯父级窗口
                //例如当前window UIEventKey是"ShopWindow@0",变成"ShopWindow@0_ShopWindow_Item"
                UIName = UIEventKey + "_" + UIName;
            }

            m_UIID = id;

            //优化方法
            CreateObjectTable();

            OnInit();
        }

        public void Dispose()
        {

        }

        #endregion

        #region 获取对象

        [HideInInspector]
        public List<GameObject> m_childUIList = new List<GameObject>();

        private Dictionary<string, GameObject> m_interactObjList = new Dictionary<string, GameObject>();
        Dictionary<string, Image> m_images = new Dictionary<string, Image>();
        Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();
        Dictionary<string, Text> m_texts = new Dictionary<string, Text>();
        Dictionary<string, TextMesh> m_textmeshs = new Dictionary<string, TextMesh>();
        Dictionary<string, Button> m_buttons = new Dictionary<string, Button>();
        Dictionary<string, ScrollRect> m_scrollRects = new Dictionary<string, ScrollRect>();
        Dictionary<string, RawImage> m_rawImages = new Dictionary<string, RawImage>();
        Dictionary<string, RectTransform> m_rectTransforms = new Dictionary<string, RectTransform>();
        Dictionary<string, InputField> m_inputFields = new Dictionary<string, InputField>();
        Dictionary<string, Slider> m_Sliders = new Dictionary<string, Slider>();
        Dictionary<string, Toggle> m_Toggle = new Dictionary<string, Toggle>();
        Dictionary<string, ReusingScrollRect> m_reusingScrollRects = new Dictionary<string, ReusingScrollRect>();

        //Dictionary<string, UGUIJoyStick> m_joySticks = new Dictionary<string, UGUIJoyStick>();
        //Dictionary<string, UGUIJoyStickBase> m_joySticks_ro = new Dictionary<string, UGUIJoyStickBase>();
        Dictionary<string, LongPressAcceptor> m_longPressList = new Dictionary<string, LongPressAcceptor>();
        Dictionary<string, DragAcceptor> m_dragList = new Dictionary<string, DragAcceptor>();
        Dictionary<string, UGUIJoyStick> m_joySticks = new Dictionary<string, UGUIJoyStick>();
        Dictionary<string, UGUIJoyStickBase> m_joySticks_ro = new Dictionary<string, UGUIJoyStickBase>();

        /// <summary>
        /// 生成对象表，便于快速获取对象，并忽略层级
        /// </summary>
        private void CreateObjectTable()
        {
            if (m_interactObjList == null)
            {
                m_interactObjList = new Dictionary<string, GameObject>();
            }
            m_interactObjList.Clear();

            m_images.Clear();
            m_Sprites.Clear();
            m_texts.Clear();
            m_textmeshs.Clear();
            m_buttons.Clear();
            m_scrollRects.Clear();
            m_reusingScrollRects.Clear();
            m_rawImages.Clear();
            m_rectTransforms.Clear();
            m_inputFields.Clear();
            m_Sliders.Clear();
            m_joySticks.Clear();
            m_joySticks_ro.Clear();

            m_childUIList.Clear();

            m_childUIList.AddRange(transform.FindChildByComponent<UILifeCycleInterface>());
        }

        /// <summary>
        /// 获取子物体对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetGameObject(string name)
        {
            if (m_interactObjList == null)
            {
                CreateObjectTable();
            }

            if (m_interactObjList.ContainsKey(name))
            {
                GameObject go = m_interactObjList[name];

                if (go == null)
                {
                    throw new Exception("UIWindowBase GetGameObject error: " + UIName + " m_objects[" + name + "] is null !!");
                }

                return go;
            }
            else
            {
                GameObject go = gameObject.FindChildByName(name);

                if (go == null)
                {
                    Debug.LogError(GetType() + "/GetGameObject()/can't find gameObject by " + name);
                }

                return go;
            }
        }

        public Sprite GetSprite(string name)
        {
            if (m_Sprites.ContainsKey(name))
            {
                return m_Sprites[name];
            }

            Sprite tmp = GetGameObject(name).GetComponent<Sprite>();

            if (tmp == null)
            {
                throw new Exception(" GetImage ->" + name + "<- is Null !");
            }

            m_Sprites.Add(name, tmp);
            return tmp;
        }
        public Image GetImage(string name)
        {
            if (m_images.ContainsKey(name))
            {
                return m_images[name];
            }

            Image tmp = GetGameObject(name).GetComponent<Image>();

            if (tmp == null)
            {
                throw new Exception(" GetImage ->" + name + "<- is Null !");
            }

            m_images.Add(name, tmp);
            return tmp;
        }
        public TextMesh GetTextMesh(string name)
        {
            if (m_textmeshs.ContainsKey(name))
            {
                return m_textmeshs[name];
            }

            TextMesh tmp = GetGameObject(name).GetComponent<TextMesh>();



            if (tmp == null)
            {
                throw new Exception(" GetText ->" + name + "<- is Null !");
            }

            m_textmeshs.Add(name, tmp);
            return tmp;
        }
        public Text GetText(string name)
        {
            if (m_texts.ContainsKey(name))
            {
                return m_texts[name];
            }

            Text tmp = GetGameObject(name).GetComponent<Text>();

            if (tmp == null)
            {
                throw new Exception(" GetText ->" + name + "<- is Null !");
            }

            m_texts.Add(name, tmp);
            return tmp;
        }
        public Toggle GetToggle(string name)
        {
            if (m_Toggle.ContainsKey(name))
            {
                return m_Toggle[name];
            }

            Toggle tmp = GetGameObject(name).GetComponent<Toggle>();

            if (tmp == null)
            {
                throw new Exception(" GetText ->" + name + "<- is Null !");
            }

            m_Toggle.Add(name, tmp);
            return tmp;
        }

        public Button GetButton(string name)
        {
            if (m_buttons.ContainsKey(name))
            {
                return m_buttons[name];
            }

            Button tmp = GetGameObject(name).GetComponent<Button>();

            if (tmp == null)
            {
                throw new Exception(" GetButton ->" + name + "<- is Null !");
            }

            m_buttons.Add(name, tmp);
            return tmp;
        }

        public InputField GetInputField(string name)
        {
            if (m_inputFields.ContainsKey(name))
            {
                return m_inputFields[name];
            }

            InputField tmp = GetGameObject(name).GetComponent<InputField>();

            if (tmp == null)
            {
                throw new Exception(" GetInputField ->" + name + "<- is Null !");
            }

            m_inputFields.Add(name, tmp);
            return tmp;
        }

        public ScrollRect GetScrollRect(string name)
        {
            if (m_scrollRects.ContainsKey(name))
            {
                return m_scrollRects[name];
            }

            ScrollRect tmp = GetGameObject(name).GetComponent<ScrollRect>();

            if (tmp == null)
            {
                throw new Exception(" GetScrollRect ->" + name + "<- is Null !");
            }

            m_scrollRects.Add(name, tmp);
            return tmp;
        }

        public RawImage GetRawImage(string name)
        {
            if (m_rawImages.ContainsKey(name))
            {
                return m_rawImages[name];
            }

            RawImage tmp = GetGameObject(name).GetComponent<RawImage>();

            if (tmp == null)
            {
                throw new Exception(" GetRawImage ->" + name + "<- is Null !");
            }

            m_rawImages.Add(name, tmp);
            return tmp;
        }

        public Slider GetSlider(string name)
        {
            if (m_Sliders.ContainsKey(name))
            {
                return m_Sliders[name];
            }

            Slider tmp = GetGameObject(name).GetComponent<Slider>();

            if (tmp == null)
            {
                throw new Exception(" GetSlider ->" + name + "<- is Null !");
            }

            m_Sliders.Add(name, tmp);
            return tmp;
        }

        private RectTransform m_rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null)
                {
                    m_rectTransform = GetComponent<RectTransform>();
                }

                return m_rectTransform;
            }
            set { m_rectTransform = value; }
        }

        public RectTransform GetRectTransform(string name)
        {
            if (m_rectTransforms.ContainsKey(name))
            {
                return m_rectTransforms[name];
            }

            RectTransform tmp = GetGameObject(name).GetComponent<RectTransform>();


            if (tmp == null)
            {
                throw new Exception(" GetRectTransform ->" + name + "<- is Null !");
            }

            m_rectTransforms.Add(name, tmp);
            return tmp;
        }

        #endregion

        #region 生命周期管理 

        protected List<UILifeCycleInterface> m_lifeComponent = new List<UILifeCycleInterface>();

        public void AddLifeCycleComponent(UILifeCycleInterface comp)
        {
            comp.Init(UIEventKey, m_lifeComponent.Count);
            m_lifeComponent.Add(comp);
        }

        void DisposeLifeComponent()
        {
            for (int i = 0; i < m_lifeComponent.Count; i++)
            {
                try
                {
                    m_lifeComponent[i].Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogError("UIBase DisposeLifeComponent Exception -> UIEventKey: " + UIEventKey + " Exception: " + e.ToString());
                }

            }

            m_lifeComponent.Clear();
        }

        public virtual void OnDestroy()
        {
            OnUIDestroy();
            EventMgr.Instance.RemoveListener(this);
        }

        #endregion

        #region 自定义组件

        public ReusingScrollRect GetReusingScrollRect(string name)
        {
            if (m_reusingScrollRects.ContainsKey(name))
            {
                return m_reusingScrollRects[name];
            }

            ReusingScrollRect tmp = GetGameObject(name).GetComponent<ReusingScrollRect>();

            if (tmp == null)
            {
                throw new Exception(m_EventNames + " GetReusingScrollRect ->" + name + "<- is Null !");
            }

            m_reusingScrollRects.Add(name, tmp);
            return tmp;
        }

        public LongPressAcceptor GetLongPressComp(string name)
        {
            if (m_longPressList.ContainsKey(name))
            {
                return m_longPressList[name];
            }

            LongPressAcceptor tmp = GetGameObject(name).GetComponent<LongPressAcceptor>();

            if (tmp == null)
            {
                throw new Exception(m_EventNames + " GetLongPressComp ->" + name + "<- is Null !");
            }

            m_longPressList.Add(name, tmp);
            return tmp;
        }

        public DragAcceptor GetDragComp(string name)
        {
            if (m_dragList.ContainsKey(name))
            {
                return m_dragList[name];
            }

            DragAcceptor tmp = GetGameObject(name).GetComponent<DragAcceptor>();

            if (tmp == null)
            {
                throw new Exception(m_EventNames + " GetDragComp ->" + name + "<- is Null !");
            }

            m_dragList.Add(name, tmp);
            return tmp;
        }


        public UGUIJoyStick GetJoyStick(string name)
        {
            if (m_joySticks.ContainsKey(name))
            {
                return m_joySticks[name];
            }

            UGUIJoyStick tmp = GetGameObject(name).GetComponent<UGUIJoyStick>();

            if (tmp == null)
            {
                throw new Exception(m_EventNames + " GetJoyStick ->" + name + "<- is Null !");
            }

            m_joySticks.Add(name, tmp);
            return tmp;
        }

        public UGUIJoyStickBase GetJoyStick_ro(string name)
        {
            if (m_joySticks_ro.ContainsKey(name))
            {
                return m_joySticks_ro[name];
            }

            UGUIJoyStickBase tmp = GetGameObject(name).GetComponent<UGUIJoyStickBase>();

            if (tmp == null)
            {
                throw new Exception(m_EventNames + " GetJoyStick_ro ->" + name + "<- is Null !");
            }

            m_joySticks_ro.Add(name, tmp);
            return tmp;
        }

        #endregion

        #region 注册监听

        protected List<Enum> m_EventNames = new List<Enum>();
        //protected List<EventHandRegisterInfo> m_EventListeners = new List<EventHandRegisterInfo>();

        protected List<InputEventRegisterInfo> m_OnClickEvents = new List<InputEventRegisterInfo>();
        protected List<InputEventRegisterInfo> m_LongPressEvents = new List<InputEventRegisterInfo>();
        protected List<InputEventRegisterInfo> m_DragEvents = new List<InputEventRegisterInfo>();
        protected List<InputEventRegisterInfo> m_BeginDragEvents = new List<InputEventRegisterInfo>();
        protected List<InputEventRegisterInfo> m_EndDragEvents = new List<InputEventRegisterInfo>();

        public virtual void RemoveAllListener()
        {
            //for (int i = 0; i < m_EventListeners.Count; i++)
            //{
            //    m_EventListeners[i].RemoveListener();
            //}
            //m_EventListeners.Clear();
            //CoreRoot.eventMgr.RemoveListener(this);

            for (int i = 0; i < m_OnClickEvents.Count; i++)
            {
                m_OnClickEvents[i].RemoveListener();
            }
            m_OnClickEvents.Clear();

            for (int i = 0; i < m_LongPressEvents.Count; i++)
            {
                m_LongPressEvents[i].RemoveListener();
            }
            m_LongPressEvents.Clear();

            #region 拖动事件
            for (int i = 0; i < m_DragEvents.Count; i++)
            {
                m_DragEvents[i].RemoveListener();
            }
            m_DragEvents.Clear();

            for (int i = 0; i < m_BeginDragEvents.Count; i++)
            {
                m_BeginDragEvents[i].RemoveListener();
            }
            m_BeginDragEvents.Clear();

            for (int i = 0; i < m_EndDragEvents.Count; i++)
            {
                m_EndDragEvents[i].RemoveListener();
            }
            m_EndDragEvents.Clear();
            #endregion
        }

        #region 添加监听

        bool GetRegister(List<InputEventRegisterInfo> list, string eventKey)
        {
            int registerCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].eventKey == eventKey)
                {
                    registerCount++;
                }
            }

            return registerCount == 0;
        }

        public void AddOnClickListener(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
        {
            InputButtonClickRegisterInfo info = InputUIEventProxy.GetOnClickListener(GetButton(buttonName), UIEventKey, buttonName, parm, callback);
            info.AddListener();
            m_OnClickEvents.Add(info);
        }

        public void AddOnClickListenerByCreate(Button button, string compName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
        {
            InputButtonClickRegisterInfo info = InputUIEventProxy.GetOnClickListener(button, UIEventKey, compName, parm, callback);
            info.AddListener();
            m_OnClickEvents.Add(info);
        }

        public void AddLongPressListener(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm = null)
        {
            InputEventRegisterInfo<InputUILongPressEvent> info = InputUIEventProxy.GetLongPressListener(GetLongPressComp(compName), UIEventKey, compName, parm, callback);
            info.AddListener();
            m_LongPressEvents.Add(info);
        }

        public void AddBeginDragListener(string compName, InputEventHandle<InputUIOnBeginDragEvent> callback, string parm = null)
        {
            InputEventRegisterInfo<InputUIOnBeginDragEvent> info = InputUIEventProxy.GetOnBeginDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
            info.AddListener();
            m_BeginDragEvents.Add(info);
        }

        public void AddEndDragListener(string compName, InputEventHandle<InputUIOnEndDragEvent> callback, string parm = null)
        {
            InputEventRegisterInfo<InputUIOnEndDragEvent> info = InputUIEventProxy.GetOnEndDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
            info.AddListener();
            m_EndDragEvents.Add(info);
        }

        public void AddOnDragListener(string compName, InputEventHandle<InputUIOnDragEvent> callback, string parm = null)
        {
            InputEventRegisterInfo<InputUIOnDragEvent> info = InputUIEventProxy.GetOnDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
            info.AddListener();
            m_DragEvents.Add(info);
        }

        //public void AddEventListener(Enum EventEnum, EventHandle handle)
        //{
        //    EventHandRegisterInfo info = new EventHandRegisterInfo();
        //    info.m_EventKey = EventEnum;
        //    info.m_hande = handle;

        //    GlobalEvent.AddEvent(EventEnum, handle);

        //    m_EventListeners.Add(info);
        //}

        #endregion

        #region 移除监听

        //TODO 逐步添加所有的移除监听方法

        public InputButtonClickRegisterInfo GetClickRegisterInfo(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm)
        {
            string eventKey = InputUIOnClickEvent.GetEventKey(UIEventKey, buttonName, parm);
            for (int i = 0; i < m_OnClickEvents.Count; i++)
            {
                InputButtonClickRegisterInfo info = (InputButtonClickRegisterInfo)m_OnClickEvents[i];
                if (info.eventKey == eventKey
                    && info.callBack == callback)
                {
                    return info;
                }
            }

            throw new Exception("GetClickRegisterInfo Exception not find RegisterInfo by " + buttonName + " parm " + parm);
        }

        public void RemoveOnClickListener(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
        {
            InputButtonClickRegisterInfo info = GetClickRegisterInfo(buttonName, callback, parm);
            m_OnClickEvents.Remove(info);
            info.RemoveListener();
        }

        public void RemoveLongPressListener(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm = null)
        {
            InputEventRegisterInfo<InputUILongPressEvent> info = GetLongPressRegisterInfo(compName, callback, parm);
            m_LongPressEvents.Remove(info);
            info.RemoveListener();
        }

        public InputEventRegisterInfo<InputUILongPressEvent> GetLongPressRegisterInfo(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm)
        {
            string eventKey = InputUILongPressEvent.GetEventKey(UIName, compName, parm);
            for (int i = 0; i < m_LongPressEvents.Count; i++)
            {
                InputEventRegisterInfo<InputUILongPressEvent> info = (InputEventRegisterInfo<InputUILongPressEvent>)m_LongPressEvents[i];
                if (info.eventKey == eventKey
                    && info.callBack == callback)
                {
                    return info;
                }
            }

            throw new Exception("GetLongPressRegisterInfo Exception not find RegisterInfo by " + compName + " parm " + parm);
        }

        #endregion



        #endregion
    }
}


