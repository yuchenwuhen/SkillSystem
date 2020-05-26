using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using VFramework.Common;
using VFramework.Skill;

namespace VFramework.Character
{
    public class CharacterInput : MonoBehaviour
    {
        PlayerActions playerActions;
        public PlayerActions PlayerActions
        {
            get
            {
                return playerActions;
            }
        }

        private CharacterSkillSystem m_skillSystem;

        private CharacterMove m_characterMove;

        private AutoCam m_autoCam;

        private void Awake()
        {
            m_skillSystem = this.GetComponent<CharacterSkillSystem>();
            m_characterMove = this.GetComponent<CharacterMove>();
            m_autoCam = Camera.main.GetComponent<AutoCam>();
        }

        private void Start()
        {
            playerActions = PlayerActions.CreateWithDefaultBindings();
            playerActions.Fire.WasPressedHandler += OnFire;
            playerActions.Rush.WasPressedHandler += OnRush;
            playerActions.Move.OnMoveHandler += OnMove;

        }

        /// <summary>
        /// 普通攻击
        /// </summary>
        void OnFire()
        {
            //m_skillSystem.AttackUseSkill(1001);
        }

        /// <summary>
        /// 冲刺
        /// </summary>
        void OnRush()
        {
            Debug.Log(playerActions.ActiveDevice.DeviceClass);
            m_characterMove.RushTo(5, Vector3.up, 40, GlobalLayerDef.StaticSceneCheckLayer, 0.3f);
        }

        void OnMove()
        {

            Debug.Log(playerActions.LastDeviceClass);

            Vector3 vDir = Vector3.zero;

            vDir.x = playerActions.Move.X;
            vDir.y = playerActions.Move.Y;
            vDir.z = 0;

            if (!m_characterMove.IsRush)
            {
                m_characterMove.MoveFunc(vDir, 5, 1 << LayerMask.NameToLayer("StaticScene"), 0.3f,true);
            }
        }

        void Update()
        {
            ProcessDirectionAndCameraPos();
        }

        Vector3 beforePos = Vector3.zero;
        Vector3 faceDir = Vector3.zero;

        void ProcessDirectionAndCameraPos()
        {
            // 处理移动
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-2.5f));
            Vector3 forward = (mousePos - transform.position);

            if (forward.magnitude > 0.3f)
            {
                m_characterMove.InvokeRotate(forward.normalized);
            }
        }
    }
}


