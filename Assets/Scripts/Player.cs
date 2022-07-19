using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MultiPlayerProject
{
    public class Player : MonoBehaviourPunCallbacks
    {
        #region Variables
        public float speed;
        public float sprintModifier;
        private Rigidbody rig;
        public Camera normalCam;
        public GameObject cameraParent;
        public Transform weaponParent;
        private float basefov;
        private float sprintFovModifier = 1.25f;
        public float jumpForce;
        public int max_Health;
        public Transform GroundDetector;
        public LayerMask Ground;
        private Vector3 weaponParentOrigin;
        private Vector3 targetWeaponBobPosition;
        private float movmentCounter;
        private float idleCounter;
        private int current_Health;
        private Manager manager;
        public Weapon weapon;

        private Transform ui_healthbar;


        #endregion

        #region MonoBehaviorCallbacks
        void Start()
        {
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            weapon = GetComponent<Weapon>();
            current_Health = max_Health;
            cameraParent.SetActive(photonView.IsMine);

            if (!photonView.IsMine)
            {
                gameObject.layer = 10;
            }
            
            basefov = normalCam.fieldOfView;
            if (Camera.main)
            {
                Camera.main.enabled = false;
            }
            rig = GetComponent<Rigidbody>();
            weaponParentOrigin = weaponParent.localPosition;

            if (photonView.IsMine)
            {
                ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
                RefreshHealthBar();
            }

        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

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

            if (Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage(100);
            }


            if (t_hmove == 0 && t_vmove == 0) // headbob
            {
                HeadBob(idleCounter, 0.025f, 0.025f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!IsSprinting)
            {
                HeadBob(movmentCounter, 0.035f, 0.035f);
                movmentCounter += Time.deltaTime * 2.7f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(movmentCounter, 0.15f, 0.075f);
                movmentCounter += Time.deltaTime * 7f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }

            //UI Refreshes
            RefreshHealthBar();

        }

        private void FixedUpdate()
        {

            if (!photonView.IsMine)
            {
                return;
            }

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

    #endregion


        #region Private Methods

        void HeadBob (float p_z, float p_x_intensity, float p_y_intensity)
        {
            float t_aim_adjust = 1f;
            if (weapon.isAiming)
            {
                t_aim_adjust = 0.1f;
            }
            targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity * t_aim_adjust, Mathf.Sin(p_z * 2) * p_y_intensity * t_aim_adjust, 0);
        }

        void RefreshHealthBar()
        {
            float t_health_ratio = (float)current_Health / (float)max_Health;
            ui_healthbar.localScale = Vector3.Lerp(ui_healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8);
        }


        #endregion


        #region Public Methods

        
        public void TakeDamage(int p_damage)
        {
            if (photonView.IsMine)
            {
                current_Health -= p_damage;
                RefreshHealthBar();

                if (current_Health <= 0)
                {
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        #endregion

    }

}