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

        private JumpDirection _jumpDirection = JumpDirection.None;

        private enum JumpDirection
        {
            None,
            Up,
            Down,
        }


        private bool
            _isIdle = false,
            _isDecelerating = false,
            _isAccelerating = false,
            _isCruising = false,

            _isChargingJump = false,
            _isJumping = false;
        
        private static readonly int A_Accelerate = Animator.StringToHash("Accelerate");
        private static readonly int A_Cruise = Animator.StringToHash("Cruise");
        private static readonly int A_Idle = Animator.StringToHash("Idle");

        private static readonly int A_ChargingJump = Animator.StringToHash("ChargingJump");
        private static readonly int A_Jump = Animator.StringToHash("Jump");
        
        private static readonly int A_Landing = Animator.StringToHash("Landing");
        
        private static readonly int A_GoingUp = Animator.StringToHash("GoinUp");
        private static readonly int A_GoingDown = Animator.StringToHash("GoinDown");

        #endregion

        #region Methods

        private void OnValidate()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        /*
        private void OnEnable()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }
        */

        public void OnEnable()
        {
            //base.OnEnable();

            _vehicle.Idle_Event += IdleAnimations;
            
            _vehicle.Accelerating_Event += AccelerateAnimations;
            _vehicle.Decelerating_Event += DecelerateAnimations;

            _vehicle.Cruise_Event += CruiseAnimations;

            _vehicle.StartJumpCharge_Event += JumpChargeAnimations;
            _vehicle.Jump_Event += JumpAnimations;
            _vehicle.Landing_Event += LandingAnimations;
        }

        protected override void Start()
        {
            base.Start();
            
            //IdleAnimations();
        }

        private void OnDestroy()
        {
            _vehicle.Idle_Event -= IdleAnimations;
            
            _vehicle.Accelerating_Event -= AccelerateAnimations;
            _vehicle.Decelerating_Event -= DecelerateAnimations;
            
            _vehicle.Cruise_Event -= CruiseAnimations;

            _vehicle.StartJumpCharge_Event -= JumpChargeAnimations;
            _vehicle.Jump_Event -= JumpAnimations;
            _vehicle.Landing_Event -= LandingAnimations;
        }

        private void AccelerateAnimations()
        {
            if(!_vehicle.Grounded) return;
            
            if(!_isAccelerating)
            {
                Debug.Log("<color=cyan> Accelerating </color>");
                
                SetAccelerate();

                ResetValues(isAccelerating: true);
            }
        }
        
        private void DecelerateAnimations()
        {
            if(!_vehicle.Grounded) return;
            
            if(!_isDecelerating)
            {
                Debug.Log("<color=teal> Decelerating </color>");

                if(!_isCruising)
                {
                    SetCruise();   
                }

                ResetValues(isDecelerating: true);
            }
        }

        private void IdleAnimations()
        {
            if(!_isIdle)
            {
                //Debug.Log("<color=white> Idling </color>");

                SetIdle();

                ResetValues(isIdle: true);
            }
        }

        private void CruiseAnimations()
        {
            if(!_vehicle.Grounded) return;
            
            if(!_isCruising)
            {
                //Debug.Log("<color=green> Cruising </color>");

                if(!_isDecelerating)
                {
                    SetCruise();   
                }
                
                ResetValues(isCruising: true);
            }
        }
        
        private void JumpChargeAnimations()
        {
            if(!_isChargingJump)
            {
                Debug.Log("<color=pink> Charging Jump </color>");

                SetChargingJump(state: true);
                
                ResetValues(isChargingJump: true);
            }
        }

        private void JumpAnimations()
        {
            if(!_isJumping)
            {
                Debug.Log("<color=yellow> Jump! </color>");

                SetJump();
                
                ResetValues(isChargingJump: true);
            }
        }
        
        private void LandingAnimations()
        {
            Debug.Log("<color=purple> LANDING! </color>");

            SetLanding();
                
            ResetValues();
        }


        private void ResetValues(
            bool isIdle = default,
            bool isAccelerating = default,
            bool isDecelerating = default,
            bool isCruising = default,
            
            bool isChargingJump = default
        )
        {
            this._isIdle = (bool)isIdle;
            this._isAccelerating = (bool)isAccelerating;
            this._isDecelerating = (bool)isDecelerating;
            this._isCruising = (bool)isCruising;
            
            this._isChargingJump = (bool)isChargingJump;
        }

        /*
        private void DoMovement()
        {
            //No movement if not grounded.
            if(_jumping) return;
            if(!_vehicle.Grounded) return;

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
                    _timeDecelerated = decellerationCurve.TimeFromValue(value: _accelerateEvaluation); //Reverse calculate starting point (closest to).
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

            if((Input.GetKeyUp(KeyCode.Space) && _chargingJump)  && _vehicle.Grounded)
            {
                ResetValues(_jumping: true);

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
        */

        /*
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
        */

        #region Animation Adjusters

        private void SetChargingJump(bool state)
        {
            animator.SetBool(id: A_ChargingJump, state);
        }

        private void SetJump()
        {
            animator.SetBool(id: A_ChargingJump, false);

            animator.SetTrigger(id: A_Jump);
        }

        private void SetLanding()
        {
            animator.SetTrigger(A_Landing);   
        }

        private void SetIdle() => animator.SetTrigger(A_Idle);

        private void SetAccelerate() => animator.SetTrigger(A_Accelerate);

        private void SetCruise() => animator.SetTrigger(A_Cruise);

        #endregion

        #endregion

    }
}
