using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using BindingsExample;
using VFramework.Skill;

namespace VFramework.Character
{
    public class CharacterInput : MonoBehaviour
    {
        PlayerActions playerActions;

        private CharacterSkillSystem m_skillSystem;

        private CharacterMove m_characterMove;

        private void Awake()
        {
            m_skillSystem = this.GetComponent<CharacterSkillSystem>();
            m_characterMove = this.GetComponent<CharacterMove>();
        }

        private void Start()
        {
            playerActions = PlayerActions.CreateWithDefaultBindings();
            playerActions.Fire.WasPressedHandler += OnFire;
            playerActions.Move.OnMoveHandler += OnMove;
        }

        /// <summary>
        /// 普通攻击
        /// </summary>
        void OnFire()
        {
            Debug.Log("....开始普通攻击....");
            m_skillSystem.AttackUseSkill(1001);
        }

        void OnMove()
        {
            Vector3 vDir = Vector3.zero;

            vDir.x = playerActions.Move.X;
            vDir.y = playerActions.Move.Y;
            vDir.z = 0;

            if (!m_characterMove.IsRush)
            {
                m_characterMove.MoveFunc(vDir, 5, 1 << LayerMask.NameToLayer("StaticScene"), 0.3f);
            }
        }
    }
}


