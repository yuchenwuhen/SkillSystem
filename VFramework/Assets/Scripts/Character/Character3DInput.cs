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

        private AutoCam m_autoCam;

        private void Awake()
        {
            m_characterMove = this.GetComponent<Character3DMove>();
            m_autoCam = Camera.main.GetComponent<AutoCam>();
        }

        private void Start()
        {
            playerActions = PlayerActions.CreateWithDefaultBindings();
            playerActions.Move3D.OnMoveHandler += OnMove;
        }

        private void OnMove()
        {
            
            
            
        }
    }
}

