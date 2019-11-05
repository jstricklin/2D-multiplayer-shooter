using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utility;
using UnityEngine;

namespace Project.Player {
    public class PlayerManager : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField]
        private Transform weaponPivot;
        [SerializeField]
        private Transform projectileSpawn;
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
        // shooting
        private Cooldown shootingCooldown;
        private ProjectileData projectileData;

        void Start()
        {
            shootingCooldown = new Cooldown();
            projectileData = new ProjectileData();
            projectileData.position = new Position();
            projectileData.direction = new Position();
        }
        // Update is called once per frame
        void Update()
        {
            if (networkIdentity.IsControlling())           
            {
                CheckMovement();
                CheckAiming();
                CheckShooting();
            }
        }

        private void CheckShooting()
        {
            shootingCooldown.CooldownUpdate();
            if (Input.GetMouseButton(0) && !shootingCooldown.IsOnCooldown())
            {
                shootingCooldown.StartCooldown();

                // define bullet
                projectileData.activator = NetworkClient.ClientID;
                projectileData.position.x = projectileSpawn.position.x.TwoDecimals();
                projectileData.position.y = projectileSpawn.position.y.TwoDecimals();

                projectileData.direction.x = projectileSpawn.up.x;
                projectileData.direction.y = projectileSpawn.up.y;
                //send bullet
                networkIdentity.GetSocket().Emit("fireProjectile", JsonUtility.ToJson(projectileData));
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