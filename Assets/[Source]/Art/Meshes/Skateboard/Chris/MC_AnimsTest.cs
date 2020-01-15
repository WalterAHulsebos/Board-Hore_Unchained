using CommonGames.Utilities.CGTK.Greasy;
using CommonGames.Utilities.Extensions;
using UnityEngine;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.Serialization;

#endif

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement.Effects
{
    [RequireComponent(typeof(Animator))]
    public class MC_AnimsTest : VehicleBehaviour
    {
        #region Variables

        [Required]
        [SerializeField] private Animator animator = null;

        [SerializeField] private AnimationCurve
            accelerationCurve =
                AnimationCurve.EaseInOut(timeStart: 0, timeEnd: 1, valueStart: 0, valueEnd: 1),

            decellerationCurve =
                AnimationCurve.EaseInOut(timeStart: 0, timeEnd: 1, valueStart: 1, valueEnd: 0),

            jumpCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(time: 0f, value: 0f), new Keyframe(time: .5f, value: 1f), new Keyframe(time: 1, value: 0)
            });

        [SerializeField] private float
            timeTillMaxSpeed = 10f,
            timeTillMinSpeed = 4f,
            timeTillJumpApex = 2f;

        [Range(0, 1)]
        [ReadOnly]
        [SerializeField]
        private float currentVelocity = 0f;

        [Range(0, 1)]
        [ReadOnly]
        [SerializeField]
        private float currentJump = 0f;

        private float _lastJump = 0f;

        private JumpDirection _jumpDirection = JumpDirection.None;

        private enum JumpDirection
        {
            None,
            Up,
            Down,
        }

        //[ReadOnly] [SerializeField]
        private float
            _timeAccelerated = 0f,
            _accelerateEvaluation = 0f,

            _timeDecelerated = 0f,
            _decelerateEvaluation = 0f,

            _timeJumping = 0f,
            _jumpingEvaluation = 0f;

        //[ReadOnly] [SerializeField]
        private bool
            _idle = false,
            _accelerate = false,
            _decelerate = false,
            _cruise = false;

        private bool
            _chargingJump = false,
            _jumping = false;

        private static readonly int A_ChargingJump = Animator.StringToHash("ChargingJump");
        private static readonly int A_Jump = Animator.StringToHash("Jump");

        private static readonly int A_Accelerate = Animator.StringToHash("Accelerate");
        private static readonly int A_Cruise = Animator.StringToHash("Cruise");
        private static readonly int A_Idle = Animator.StringToHash("Idle");
        private static readonly int A_GoingUp = Animator.StringToHash("GoingUp");
        private static readonly int A_Landing = Animator.StringToHash("Landing");

        #endregion

        #region Methods

        private void OnValidate()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        protected override void Start()
        {
            base.Start();
            
            _idle = true;
        }

        private void Update()
        {
            DoJumping();

            DoMovement();
        }

        private void DoMovement()
        {
            //No movement if not grounded.
            if(_jumping) return;

            if(Input.GetKey(KeyCode.W))
            {
                //Debug.Log("<color=red> Accelerating </color>");
                _timeDecelerated = default;
                _decelerateEvaluation = default;

                //=====

                currentVelocity = __CalcCurrentVelocity();

                __AnimationSwapping();

                float __CalcCurrentVelocity()
                {
                    _timeAccelerated += Time.deltaTime;

                    _accelerateEvaluation = (_timeAccelerated / timeTillMaxSpeed).Clamp01();

                    float __lerpValue = accelerationCurve.Evaluate(_accelerateEvaluation);
                    return Mathf.Lerp(0, 1, __lerpValue);
                }

                void __AnimationSwapping()
                {
                    //Movement speed can't increase when you're charging for a jump.
                    if(_chargingJump) return;

                    if(_idle && (currentVelocity > 0f && currentVelocity < 0.9f))
                    {
                        _idle = false;
                        _accelerate = true;

                        _decelerate = false;

                        SetAccelerate();
                    }

                    if(_accelerate && (currentVelocity >= 0.9f))
                    {
                        _accelerate = false;
                        _cruise = true;

                        _decelerate = false;

                        SetCruise();
                    }
                }
            }
            else
            {
                if(!_timeAccelerated.Approximately(default))
                {
                    _timeDecelerated =
                        decellerationCurve.TimeFromValue(
                            value: _accelerateEvaluation); //Reverse calculate starting point (closest to).
                }

                _timeAccelerated = default;
                _accelerateEvaluation = default;

                //=====

                if(_chargingJump) return;

                if(_cruise && Input.GetKey(KeyCode.S))
                {
                    SetIdle();

                    _cruise = false;
                    _accelerate = false;

                    _idle = true;
                }

                __CalcDeceleration();

                __AnimationSwapping();

                void __CalcDeceleration()
                {
                    _timeDecelerated += Time.deltaTime;

                    _decelerateEvaluation = (_timeDecelerated / timeTillMinSpeed).Clamp01();

                    float __lerpValue = decellerationCurve.Evaluate(_decelerateEvaluation);

                    currentVelocity = Mathf.Lerp(0, 1, __lerpValue);
                }

                void __AnimationSwapping()
                {
                    //Movement speed can't increase when you're charging for a jump.
                    if(_chargingJump) return;

                    if(_cruise && (currentVelocity <= 0.9f))
                    {
                        _cruise = false;
                        _decelerate = true;

                        _accelerate = true;

                        SetAccelerate();
                    }

                    if(_accelerate && (currentVelocity <= 0.5f))
                    {
                        _accelerate = false;
                        _decelerate = false;

                        _idle = true;

                        SetIdle();
                    }
                }
            }
        }

        private void DoJumping()
        {
            if(Input.GetKeyDown(KeyCode.Space) && _vehicle.Grounded)
            {
                SetChargingJump();

                _chargingJump = true;
            }

            if(Input.GetKeyUp(KeyCode.Space) && _chargingJump  && _vehicle.Grounded)
            {
                _idle = false;
                _accelerate = false;
                _cruise = false;

                _timeAccelerated = default;
                _accelerateEvaluation = default;

                _timeDecelerated = default;
                _decelerateEvaluation = default;

                currentVelocity = default;

                _jumping = true;

                SetJump();
            }

            if(!_vehicle.Grounded && !_jumping)
            {
                Debug.Log("In the air");
                
                ResetValues(_jumping: true);

                SetJump();
            }

            if(!_jumping) return;

            //currentJump = __CalcCurrentJump();

            currentJump = _vehicle.rigidbody.transform.position.y;

            _jumpDirection = (currentJump > _lastJump) ? JumpDirection.Up : JumpDirection.Down;
            
            if(_vehicle.Grounded && _jumpDirection == JumpDirection.Down)
            {
                SetLanding();

                ResetValues(_idle: true);

                return;
            }

            //if(__Landing()) return;

            _lastJump = currentJump;

            __SetJumpDirection();

            void __SetJumpDirection()
            {
                switch(_jumpDirection)
                {
                    case JumpDirection.Up:
                        animator.SetBool(A_GoingUp, true);
                        break;
                    case JumpDirection.Down:
                        animator.SetBool(A_GoingUp, false);
                        break;
                }
            }
        }

        private void ResetValues(
            float _timeAccelerated = default,
            float _accelerateEvaluation = default,

            float _timeDecelerated = default,
            float _decelerateEvaluation = default,

            float _timeJumping = default,
            float _jumpingEvaluation = default,

            bool _idle = default,
            bool _accelerate = default,
            bool _cruise = default,

            bool _chargingJump = default,
            bool _jumping = default
        )
        {
            this._timeAccelerated = _timeAccelerated;
            this._accelerateEvaluation = _accelerateEvaluation;

            this._timeDecelerated = _timeDecelerated;
            this._decelerateEvaluation = _decelerateEvaluation;

            this._timeJumping = _timeJumping;
            this._jumpingEvaluation = _jumpingEvaluation;

            this._idle = _idle;
            this._accelerate = _accelerate;
            this._cruise = _cruise;

            this._chargingJump = _chargingJump;
            this._jumping = _jumping;
        }

        #region Animation Adjusters

        private void SetChargingJump()
        {
            animator.SetBool(id: A_ChargingJump, true);
        }

        private void SetJump()
        {
            animator.SetBool(id: A_ChargingJump, false);

            animator.SetTrigger(id: A_Jump);
        }

        private void SetLanding() => animator.SetTrigger(A_Landing);

        private void SetIdle() => animator.SetTrigger(A_Idle);

        private void SetAccelerate() => animator.SetTrigger(A_Accelerate);

        private void SetCruise() => animator.SetTrigger(A_Cruise);

        #endregion

        #endregion

    }
}
