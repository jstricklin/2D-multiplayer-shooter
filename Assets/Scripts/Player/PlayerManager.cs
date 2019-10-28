using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using UnityEngine;

namespace Project.Player {
    public class PlayerManager : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField]
        private Transform weaponPivot;
        [Header("Data")]
        [SerializeField]
        private float speed = 4;

        const float WEAPON_ROTATION_OFFSET = 180f;

        [SerializeField]
        private bool flipped = false;
        private bool lastFlipped;
        private float lastWeaponRotation;

        [Header("Class References")]
        [SerializeField]
        private NetworkIdentity networkIdentity;

        // Update is called once per frame
        void Update()
        {
            if (networkIdentity.IsControlling())           
            {
                CheckMovement();
                CheckAiming();
            }
        }
        public bool GetLastFlipped()
        {
            return lastFlipped;
        }

        public float GetWeaponLastRotation()
        {
            return lastWeaponRotation;
        }

        public void SetWeaponRotation(float value)
        {
            Debug.Log("setting weapon rotation");
            weaponPivot.rotation = Quaternion.Euler(0, 0, value + WEAPON_ROTATION_OFFSET);
            // weaponPivot.rotation = Quaternion.Euler(0, 0, value);
        }
        private void CheckMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.position += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
        }

        private void CheckAiming()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 diff = mousePosition - transform.position;
            diff.Normalize();
            float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            lastWeaponRotation = rot;
            weaponPivot.rotation = Quaternion.Euler(0, 0, rot + WEAPON_ROTATION_OFFSET);
            // weaponPivot.rotation = Quaternion.Euler(0, 0, rot);
        }
    }
}