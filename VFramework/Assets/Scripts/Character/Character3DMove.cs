using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;
using VFramework.Tools;

namespace VFramework.Character
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Character3DInput))]
    [RequireComponent(typeof(GroundDetection))]
    [RequireComponent(typeof(CharacterMovement))]
    public class Character3DMove : CharacterMoveBase
    {
        [Header("Movement")]

        private PhotonView m_photonView;

        private Character3DInput m_character3DInput;

        private Transform m_camera;

        private Vector3 m_moveDirection;
        public Vector3 MoveDirection
        {
            get
            {
                return m_moveDirection;
            }
            set
            {
                m_moveDirection = Vector3.ClampMagnitude(value, 1.0f);
            }
        }

        [SerializeField]
        private float m_groundFriction = 8;
        private float GroundFriction
        {
            get
            {
                return m_groundFriction;
            }
            set
            {
                m_groundFriction = Mathf.Max(0.0f, value);
            }
        }

        /// <summary>
        /// Is this character on VALID 'ground'?
        /// </summary>
        public bool isValidGround
        {
            get { return m_groundDetection.isValidGround; }
        }

        /// <summary>
        /// Is this character standing on the 'solid' side of a ledge?
        /// </summary>
        public bool isOnLedgeSolidSide
        {
            get { return m_groundDetection.isOnLedgeSolidSide; }
        }

        /// <summary>
        /// Is the character sliding off a steep slope?
        /// </summary>

        public bool isSliding { get; private set; }

        /// <summary>
        /// Is the character standing on a platform? (eg: Kinematic Rigidbody)
        /// </summary>

        public bool isOnPlatform { get; private set; }

        /// <summary>
        /// The velocity of the platform the character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>

        public Vector3 platformVelocity { get; private set; }

        /// <summary>
        /// The angular velocity of the platform the character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>

        public Vector3 platformAngularVelocity { get; private set; }

        private bool _forceUnground;
        private float _forceUngroundTimer;
        private bool _performGroundDetection = true;

        private float m_airFriction = 0;
        private float AirFriction
        {
            get
            {
                return m_airFriction;
            }
            set
            {
                m_airFriction = Mathf.Max(0.0f, value);
            }
        }

        [SerializeField]
        private float m_airControl;
        public float AirControl
        {
            get { return m_airControl; }
            set { m_airControl = Mathf.Clamp01(value); }
        }

        [Header("Speed Limiters")]
        [SerializeField]
        private float m_maxLateralSpeed = 10f;
        [SerializeField]
        private float m_maxRiseSpeed = 15f;
        [SerializeField]
        private float m_maxFallSpeed = 10f;

        private float m_referenceCastDistance;

        /// <summary>
        /// Is the character sliding off a steep slope?
        /// </summary>
        public bool IsSliding { get; private set; }

        public float GroundAngle
        {
            get { return m_groundDetection.groundAngle; }
        }

        public bool WasGrounded
        {
            get
            {
                return m_groundDetection.prevGroundHit.isOnGround && m_groundDetection.prevGroundHit.isValidGround;
            }
        }

        public bool IsOnGround
        {
            get { return m_groundDetection.isOnGround; }
        }

        /// <summary>
        /// Is the character standing on a step?
        /// </summary>
        public bool IsOnStep
        {
            get { return m_groundDetection.isOnStep; }
        }

        public Vector3 GroundPoint
        {
            get { return m_groundDetection.groundPoint; }
        }

        private BaseGroundDetection m_groundDetection;

        [SerializeField]
        private float m_deceleration = 20.0f;
        public float Deceleration
        {
            get { return m_characterMovement.isGrounded ? m_deceleration : m_deceleration * AirControl; }
            set { m_deceleration = Mathf.Max(0.0f, value); }
        }

        [SerializeField]
        private float m_acceleration = 50f;
        public float Acceleration
        {
            get { return m_characterMovement.isGrounded ? m_acceleration : m_acceleration * AirControl; }
            set { m_acceleration = Mathf.Max(0.0f, value); }
        }

        private bool m_jump = false;
        public bool IsJump
        {
            get { return m_jump; }
            set
            {
                // If jump is released, allow to jump again

                if (m_jump && value == false)
                {
                    m_canJump = true;
                    m_jumpButtonHeldDownTimer = 0.0f;
                }

                // Update jump value; if pressed, update held down timer

                m_jump = value;
                if (m_jump)
                    m_jumpButtonHeldDownTimer += Time.deltaTime;
            }
        }

        private bool m_isPause = false;

        [Header("Jump Paramter")]

        private Vector3 m_normal;

        private CharacterMovement m_characterMovement;

        [SerializeField]
        private float m_jumpPreGroundedToleranceTime = 0.15f;

        [Tooltip("How long after leaving the ground you can press jump, and still perform the jump." +
                 "Typical values goes from 0.15f to 0.5f.")]
        [SerializeField]
        private float m_jumpPostGroundedToleranceTime = 0.15f;

        private bool m_isJumping;

        private float m_jumpUngroundedTimer = 0;
        private bool m_updateJumpTimer;
        protected float m_jumpButtonHeldDownTimer = 0;

        protected bool m_canJump = true;

        [SerializeField]
        private float _baseJumpHeight = 1.5f;

        /// <summary>
        /// The initial jump height (in meters).
        /// </summary>
        public float baseJumpHeight
        {
            get { return _baseJumpHeight; }
            set { _baseJumpHeight = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Computed jump impulse.
        /// </summary>

        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * baseJumpHeight * m_characterMovement.gravity.magnitude); }
        }

        protected bool m_allowVerticalMovement = false;

        public bool allowVerticalMovement
        {
            get { return m_allowVerticalMovement; }
            set
            {
                m_allowVerticalMovement = value;

                if (m_characterMovement)
                    m_characterMovement.useGravity = !m_allowVerticalMovement;
            }
        }

        protected override void Init()
        {
            base.Init();
            m_character3DInput = this.GetComponent<Character3DInput>();
            m_groundDetection = this.GetComponent<GroundDetection>();
            m_characterMovement = this.GetComponent<CharacterMovement>();
            m_photonView = this.GetComponent<PhotonView>();

            if (Camera.main != null)
            {
                m_camera = Camera.main.transform;
            }
        }

        protected override void FixedUpdate()
        {
            if (PhotonNetwork.IsConnected && !m_photonView.IsMine)
            {
                return;
            }

            base.FixedUpdate();

            UpdateMove();
        }

        protected override void Update()
        {
            if (PhotonNetwork.IsConnected && !m_photonView.IsMine)
            {
                return;
            }

            base.Update();

            HandleInput();

            if (m_isPause)
            {
                return;
            }

            UpdateRotation();
        }

        private Vector3 m_camForward;

        void HandleInput()
        {
            if (m_camera != null)
            {
                // calculate camera relative direction to move:
                m_camForward = Vector3.Scale(m_camera.forward, new Vector3(1, 0, 1)).normalized;
                MoveDirection = -m_character3DInput.PlayerActions.Move3D.Y * m_camForward + m_character3DInput.PlayerActions.Move3D.X * m_camera.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                MoveDirection = -m_character3DInput.PlayerActions.Move3D.Y * Vector3.forward + m_character3DInput.PlayerActions.Move3D.X * Vector3.right;
            }

            IsJump = m_character3DInput.PlayerActions.Jump.WasPressed;
        }

        #region 更新移动
        bool useBrakingFriction = false;
        float brakingFriction = 0;

        void UpdateMove()
        {
            var desiredVelocity = MoveDirection * m_moveSpeed;

            // Move with acceleration and friction
            var currentFriction = m_characterMovement.isGrounded ? GroundFriction : AirFriction;
            var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

            m_characterMovement.Move(desiredVelocity, m_moveSpeed, Acceleration, Deceleration, currentFriction,
                currentBrakingFriction,!m_allowVerticalMovement);

            // Jump logic
            Jump();
            MidAirJump();
            UpdateJumpTimer();
        }

        /// <summary>
        /// Perform jump logic.
        /// </summary>
        protected virtual void Jump()
        {
            // Update _isJumping flag state
            if (m_isJumping)
            {
                // On landing, reset _isJumping flag
                if (!m_characterMovement.wasGrounded && m_characterMovement.isGrounded)
                    m_isJumping = false;
            }

            // Update jump ungrounded timer (post jump tolerance time)
            if (m_characterMovement.isGrounded)
                m_jumpUngroundedTimer = 0.0f;
            else
                m_jumpUngroundedTimer += Time.deltaTime;

            // If jump button not pressed, or still not released, return

            if (!m_jump || !m_canJump)
                return;

            // Is jump button pressed within pre jump tolerance time?

            if (m_jumpButtonHeldDownTimer > m_jumpPreGroundedToleranceTime)
                return;

            // If not grounded or no post grounded tolerance time remains, return

            if (!m_characterMovement.isGrounded && m_jumpUngroundedTimer > m_jumpPostGroundedToleranceTime)
                return;

            m_canJump = false;           // Halt jump until jump button is released
            m_isJumping = true;          // Update isJumping flag
            m_updateJumpTimer = true;    // Allow mid-air jump to be variable height

            // Prevent _jumpPostGroundedToleranceTime condition to pass until character become grounded again (_jumpUngroundedTimer reseted).

            m_jumpUngroundedTimer = m_jumpPostGroundedToleranceTime;

            // Apply jump impulse
            m_characterMovement.ApplyVerticalImpulse(jumpImpulse);

            // 'Pause' grounding, allowing character to safely leave the 'ground'
            m_characterMovement.DisableGrounding();
        }

        int _midAirJumpCount = 0;
        float _maxMidAirJumps = 0;

        /// <summary>
        /// Mid-air jump logic.
        /// </summary>
        protected virtual void MidAirJump()
        {
            // Reset mid-air jumps counter
            if (_midAirJumpCount > 0 && m_characterMovement.isGrounded)
                _midAirJumpCount = 0;

            // If jump button not pressed, or still not released, return
            if (!m_jump || !m_canJump)
                return;

            // If grounded, return
            if (m_characterMovement.isGrounded)
                return;

            // Have mid-air jumps?
            if (_midAirJumpCount >= _maxMidAirJumps)
                return;

            _midAirJumpCount++;         // Increase mid-air jumps counter

            m_canJump = false;           // Halt jump until jump button is released
            m_isJumping = true;          // Update isJumping flag
            m_updateJumpTimer = true;    // Allow mid-air jump to be variable height

            // Apply jump impulse
            m_characterMovement.ApplyVerticalImpulse(jumpImpulse);

            // 'Pause' grounding, allowing character to safely leave the 'ground'
            m_characterMovement.DisableGrounding();
        }

        protected float m_jumpTimer;
        [SerializeField]
        private float m_extraJumpTime = 0.5f;

        [SerializeField]
        private float m_extraJumpPower = 25.0f;

        /// <summary>
        /// Perform variable jump height logic.
        /// </summary>
        protected virtual void UpdateJumpTimer()
        {
            if (!m_updateJumpTimer)
                return;

            // If jump button is held down and extra jump time is not exceeded...

            if (m_jump && m_jumpTimer < m_extraJumpTime)
            {
                // Calculate how far through the extra jump time we are (jumpProgress),

                var jumpProgress = m_jumpTimer / m_extraJumpTime;

                // Apply proportional extra jump power (acceleration) to simulate variable height jump,
                // this method offers better control and less 'floaty' feel.

                var proportionalJumpPower = Mathf.Lerp(m_extraJumpPower, 0f, jumpProgress);
                m_characterMovement.ApplyForce(transform.up * proportionalJumpPower, ForceMode.Acceleration);

                // Update jump timer

               m_jumpTimer = Mathf.Min(m_jumpTimer + Time.deltaTime, m_extraJumpTime);
            }
            else
            {
                // Button released or extra jump time ends, reset info

                m_jumpTimer = 0.0f;
                m_updateJumpTimer = false;
            }
        }

        #endregion

        void UpdateRotation()
        {
            m_characterMovement.Rotate(m_moveDirection.normalized, m_turnSpeed);
        }
    }
}

