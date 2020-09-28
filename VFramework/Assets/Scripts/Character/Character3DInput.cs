using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;

namespace VFramework.Character
{
    public class Character3DInput : MonoBehaviour
    {
        PlayerActions playerActions;
        public PlayerActions PlayerActions
        {
            get
            {
                return playerActions;
            }
        }

        private Character3DMove m_characterMove;

        private AnimatorController m_animatorController;

        private AutoCam m_autoCam;

        private void Awake()
        {
            m_characterMove = this.GetComponent<Character3DMove>();
            m_autoCam = Camera.main.GetComponent<AutoCam>();
            m_animatorController = this.GetComponent<AnimatorController>();
        }

        private void Start()
        {
            playerActions = PlayerActions.CreateWithDefaultBindings();
            playerActions.Attack.WasPressedHandler += OnPrepareAttack;
            playerActions.Attack.WasReleasedHandler += OnAttack;
        }

        private void OnDestroy()
        {
            playerActions.Attack.WasPressedHandler -= OnPrepareAttack;
            playerActions.Attack.WasReleasedHandler -= OnAttack;
        }

        void OnAttack()
        {
            m_animatorController.SetVariable("Attack", false);
        }

        void OnPrepareAttack()
        {
            m_animatorController.SetVariable("Attack", true);
        }
    }
}

