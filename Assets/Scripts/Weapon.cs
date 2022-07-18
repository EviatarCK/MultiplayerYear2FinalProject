using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MultiPlayerProject
{
    public class Weapon : MonoBehaviour
    {
        #region Variables
        public Gun[] loadOut;
        public Transform weaponParent;
        private int currentIndex;
        private GameObject currentWeapon;
        #endregion

        #region MonoBehaviorCallbacks

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Equip(0);
            }

            if (currentWeapon != null)
            {
                Aim(Input.GetMouseButton(1));
            }
        }
        #endregion

        #region Private Methods
        void Equip(int p_ind)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            currentIndex = p_ind;

            GameObject t_newWeapon = Instantiate(loadOut[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newWeapon.transform.localPosition = Vector3.zero;
            t_newWeapon.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newWeapon;
        }

        void Aim(bool isAiming)
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_states_ads = currentWeapon.transform.Find("States/ADS");
            Transform t_states_hip = currentWeapon.transform.Find("States/Hip");

            if (isAiming)
            {
                //aim
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_states_ads.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
            }
            else
            {
                //hip
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_states_hip.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);

            }
        }


    #endregion
    }
}
