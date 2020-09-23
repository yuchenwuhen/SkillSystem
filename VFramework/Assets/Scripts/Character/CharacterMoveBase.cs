using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;

namespace VFramework.Character
{
    public abstract class CharacterMoveBase : MonoBehaviour
    {

        #region 参数 

        private bool m_enable = true;
        /// <summary>
        /// 启用Move组件
        /// </summary>
        public bool Enable
        {
            get
            {
                return m_enable;
            }
            set
            {
                m_enable = value;
            }
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        [SerializeField]
        protected float m_moveSpeed;

        /// <summary>
        /// 旋转速度
        /// </summary>
        [SerializeField]
        protected float m_turnSpeed;

        protected CharacterInput m_characterInput;
        protected CharacterStatus m_characterStatus;

        #endregion

        #region 生命周期

        private void Awake()
        {
            m_characterInput = this.GetComponent<CharacterInput>();
            m_characterStatus = this.GetComponent<CharacterStatus>();
            Init();
        }

        protected virtual void Init()
        {

        }

        private void OnDestroy()
        {
            Clear();
        }

        protected virtual void Clear()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (!Enable)
            {
                return;
            }
        }

        protected virtual void Update()
        {
            if (!Enable)
            {
                return;
            }
        }

        #endregion

        #region 接口函数



        #endregion      
    }
}


