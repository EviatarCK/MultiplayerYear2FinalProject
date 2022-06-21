using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MultiPlayerProject
{
    public class Motion : MonoBehaviour
    {
        #region Variables
        public float speed;
        public float sprintModifier;
        private Rigidbody rig;
        public Camera normalCam;
        private float basefov;
        private float sprintFovModifier = 1.25f;
        public float jumpForce;
        public Transform GroundDetector;
        public LayerMask Ground;
        #endregion

        #region MonoBehaviorCallbacks
        void Start()
        {
            basefov = normalCam.fieldOfView;
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");
      
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKey(KeyCode.Space);
       
            bool IsGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, Ground);
            bool IsJumping = jump && IsGrounded;
            bool IsSprinting = sprint && t_vmove > 0 && !IsJumping && IsGrounded;

            if (IsJumping)
            {
                rig.AddForce(Vector3.up * jumpForce);
            }
        }

        private void FixedUpdate()
        {
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKey(KeyCode.Space);
            bool IsGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, Ground);
            bool IsJumping = jump && IsGrounded;
        
            bool IsSprinting = sprint && t_vmove > 0 && !IsJumping && IsGrounded;
        
            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();

            float t_adjustedSpeed = speed;

            if (IsSprinting)
            {
                t_adjustedSpeed *= sprintModifier;
            }



            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;

            if (IsSprinting)
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, basefov * sprintFovModifier,Time.deltaTime * 8f);
            }
            else
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, basefov, Time.deltaTime * 8);
            }
        }

    }
    #endregion
    }