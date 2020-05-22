using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using BindingsExample;

namespace VFramework.Skill
{
    public class CharacterInput : MonoBehaviour
    {
        PlayerActions playerActions;

        private CharacterSkillSystem m_skillSystem;

        private void Awake()
        {
            m_skillSystem = this.GetComponent<CharacterSkillSystem>();
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

        private void Update()
        {
            //Vector3 vMove = Vector3.zero;
            //Vector3 vDir = Vector3.zero;

            //vDir.x = playerActions.Move.X;
            //vDir.y = playerActions.Move.Y;

            //Vector3 oldLocalPos = transform.localPosition;

            //vMove = vDir.normalized * (5 * Time.deltaTime);
            //vMove.z = 0;
            //vDir.z = 0;
            //transform.localPosition = oldLocalPos + vMove;
        }

        void OnMove()
        {
            Vector3 vMove = Vector3.zero;
            Vector3 vDir = Vector3.zero;

            vDir.x = playerActions.Move.X;
            vDir.y = playerActions.Move.Y;

            Vector3 oldLocalPos = transform.localPosition;

            vMove = vDir.normalized * (5 * Time.deltaTime);
            vMove.z = 0;
            vDir.z = 0;
            transform.localPosition = oldLocalPos + vMove;
        }
    }
}


