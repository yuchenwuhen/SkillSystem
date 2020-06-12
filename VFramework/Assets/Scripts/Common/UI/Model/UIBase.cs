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
    public class UIBase : MonoBehaviour
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

        #endregion

        #region 获取对象

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
            m_rawImages.Clear();
            m_rectTransforms.Clear();
            m_inputFields.Clear();
            m_Sliders.Clear();
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

    }
}


