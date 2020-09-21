using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Character
{
    public enum CharacterState
    {
        Idle = 0,
        Walk,
        Jump,
        Attack,
        Max
    }

    [RequireComponent(typeof(Animator))]
    public class CharacterStatus : MonoBehaviour
    {
        private CharacterState m_characterState;
        /// <summary>
        ///状态
        /// </summary>
        public CharacterState CharacterState
        {
            get
            {
                return m_characterState;
            }
        }

        private delegate void StateFunction();
        private StateFunction[] m_stateEnterFunctions = new StateFunction[(int)CharacterState.Max]; 
        private StateFunction[] m_stateUpdateFunctions = new StateFunction[(int)CharacterState.Max]; 
        private StateFunction[] m_stateLeaveFunctions = new StateFunction[(int)CharacterState.Max]; 

        private Animator m_animator;

        public int hp;
        public int sp;

        private void Awake()
        {
            m_animator = this.GetComponent<Animator>();

            m_stateEnterFunctions[(int)CharacterState.Idle] = EnterIdleState;
        }

        #region 状态模式

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState"></param>
        public void SetState(CharacterState newState)
        {
            CharacterState oldState = m_characterState;

            if (m_stateLeaveFunctions[(int)oldState] != null)
            {
                m_stateLeaveFunctions[(int)oldState]();
            }

            if (m_stateEnterFunctions[(int)newState] != null)
            {
                m_stateEnterFunctions[(int)newState]();
            }

            m_characterState = newState;
        }

        void EnterIdleState()
        {

        }

        #endregion

    }
}

