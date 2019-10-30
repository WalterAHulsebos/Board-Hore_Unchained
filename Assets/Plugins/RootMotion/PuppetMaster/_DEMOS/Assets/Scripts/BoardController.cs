using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

namespace RootMotion.Demos
{

    // Simple snowboard controller template.
    public class BoardController : MonoBehaviour
    {

        public LayerMask groundLayer = 1 << 0;
        public Transform rotationTarget;
        public float torque = 1f;
        public float skidDrag = 0.5f;
        public float turnSensitivity = 15f;

        private Rigidbody _rigidbody;
        private bool _isGrounded;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Impulse);
            }
        
            float turn = Input.GetAxis("Horizontal");
            rotationTarget.rotation = Quaternion.AngleAxis(turn * turnSensitivity * Mathf.Min(_rigidbody.velocity.sqrMagnitude * 0.2f, 1f) * Time.deltaTime, Vector3.up) * rotationTarget.rotation;
        }

        private void FixedUpdate()
        {
            // Add torque to the Rigidbody to make it follow the rotation target
            Vector3 angularAcc = PhysXTools.GetAngularAcceleration(_rigidbody.rotation, rotationTarget.rotation);
            _rigidbody.AddTorque(angularAcc * torque);

            // Add snowboard-like skid drag
            if (_isGrounded)
            {
                Vector3 velocity = _rigidbody.velocity;
                Vector3 skid = V3Tools.ExtractHorizontal(velocity, _rigidbody.rotation * Vector3.up, 1f);
                skid = Vector3.Project(velocity, _rigidbody.rotation * Vector3.right);

                _rigidbody.velocity = velocity - Vector3.ClampMagnitude(skid * skidDrag * Time.deltaTime, skid.magnitude);
            }
        }

        // Check if the board is grounded
        void OnCollisionEnter(Collision c)
        {
            if (c.collider.gameObject.layer != groundLayer) return;
            _isGrounded = true;
        }

        void OnCollisionStay(Collision c)
        {
            if (c.collider.gameObject.layer != groundLayer) return;
            _isGrounded = true;
        }

        void OnCollisionExit(Collision c)
        {
            if (c.collider.gameObject.layer != groundLayer) return;
            _isGrounded = false;
        }
    }
}
