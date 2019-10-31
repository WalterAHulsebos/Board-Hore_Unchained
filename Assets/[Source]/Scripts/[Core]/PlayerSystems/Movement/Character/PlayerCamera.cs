using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using CommonGames.Utilities.Extensions;

using static Core.Utilities.Helpers;

#if Odin_Inspector
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

namespace Core.PlayerSystems.Movement
{
    public class PlayerCamera : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private List<Transform> _targetTransforms = new List<Transform>(0);

        [SerializeField] private Vector3 offset = Vector3.up * 2;
        
        public bool inputActive = true;
        public bool controlCursor = false;

        #region Clamping

        [FoldoutGroup("Clamping")]
        [SerializeField] private bool pitchClamp = true;
        
        [FoldoutGroup("Clamping")]
        [ShowIf("pitchClamp")]
        [Range(-90f, 90f)]
        [SerializeField] private float defaultVerticalAngle = 20f;
        
        [MinMaxSlider(-90, 90, true)]
        [ShowIf("pitchClamp")]
        [FoldoutGroup("Clamping")]
        [SerializeField] private Vector2 verticalClamps = new Vector2(-90, 90f);
        
        #endregion

        #region Smoothing
        
        [FoldoutGroup("Smoothing")]
        [SerializeField] private bool bypassSmoothing = false;
        [FoldoutGroup("Smoothing")]
        [SerializeField] private float mouseSmoothing = 20f;	//Lambda | higher = less latency but also less smoothing
        
        #endregion

        #region Sensitivity
        
        [FoldoutGroup("Sensitivity")] [HorizontalGroup("Sensitivity/Group", 0.5f, LabelWidth = 60)]
        
        [LabelText("Horizontal")] [SerializeField] private float horizontalSensitivity = 4f;
        [HorizontalGroup("Sensitivity/Group")]
        [LabelText("Vertical")] [SerializeField] private float verticalSensitivity = 4f;
        
        #endregion
        
        private BufferV2 mouseBuffer = new BufferV2();
        
        #region Accessors

        public List<Transform> TargetTransforms
        {
            get => _targetTransforms; 
            set => _targetTransforms = value;
        }
        
        #endregion
        
        #endregion
        
        #region Methods

        private void Awake()
        {
            //TargetTransforms.Add(this.transform);
        }

        /// <param name="rotationInput"></param>
        /// <param name="localTransform">The transform the camera's should act like they're parented under</param>
        public void UpdateWithInput(Vector3 rotationInput, Transform localTransform)
        {
            //if(Input.GetKeyDown(KeyCode.Space)){inputActive = !inputActive;}
            if(controlCursor)
            {	//Cursor Control
                if (inputActive && Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                if (!inputActive && Cursor.lockState != CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }		
            if(!inputActive){ return; }	//active?

            UpdateMouseBuffer(rotationInput);
            //TargetTransforms.For(targetTransform => targetTransform.position = (localTransform.position + offset));
            
            TargetTransforms.For(targetTransform => targetTransform.rotation = Quaternion.Euler(mouseBuffer.curAbs));
            
            TargetTransforms.For(targetTransform => targetTransform.position = (localTransform.position + offset));
        }

        //consider late Update for applying the rotation if your game needs it (e.g. if camera parents are rotated in Update for some reason)
        private void LateUpdate() {}

        private	void UpdateMouseBuffer(Vector3 rotationInput)
        {
            mouseBuffer.target += new Vector2( verticalSensitivity * rotationInput.y, horizontalSensitivity * rotationInput.x); //Mouse Input is inherently framerate independend!
            
            mouseBuffer.target.x = pitchClamp
                ? Mathf.Clamp(mouseBuffer.target.x, verticalClamps.x, verticalClamps.y) 
                : mouseBuffer.target.x;
            
            mouseBuffer.Update(mouseSmoothing, Time.deltaTime, bypassSmoothing);
        }
        
        #endregion
    }
    
}