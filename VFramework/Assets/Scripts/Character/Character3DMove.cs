using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class Character3DMove : CharacterMoveBase
    {
        [SerializeField]
        private float m_jumpForce;

        private Rigidbody m_rigidbody;

        protected override void Init()
        {
            base.Init();
            m_rigidbody = this.GetComponent<Rigidbody>();
        }

        protected override void Update()
        {
            base.Update();

            
        }

    }
}

