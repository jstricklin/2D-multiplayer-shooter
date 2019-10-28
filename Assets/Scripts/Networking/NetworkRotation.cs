using System.Collections;
using System.Collections.Generic;
using Project.Player;
using Project.Utility;
using Project.Utility.Attributes;
using UnityEngine;

namespace Project.Networking 
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkRotation : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        private float oldWeaponRotation;
        [SerializeField]
        [GreyOut]
        private bool oldFlipped;
        private NetworkIdentity networkIdentity;
        [Header("Class References")]
        [SerializeField]
        private PlayerManager playerManager;
        private Player player;

        private float stillCounter = 0;

        public void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            // playerManager = GetComponent<PlayerManager>();
            oldWeaponRotation = playerManager.GetWeaponLastRotation();
            player = new Player();
            player.rotation = new Rotation();
            player.rotation.weaponRotation = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        public void Update()
        {
            if (networkIdentity.IsControlling())
            {
                if (oldWeaponRotation != playerManager.GetWeaponLastRotation() || oldFlipped != playerManager.GetLastFlipped())
                {
                    oldWeaponRotation = playerManager.GetWeaponLastRotation();
                    oldFlipped = playerManager.GetLastFlipped();
                    stillCounter = 0;
                    SendData();
                } else {
                    stillCounter += Time.deltaTime;
                    if (stillCounter >= 1)
                    {
                        stillCounter = 0;
                        SendData();
                    }
                }
            }

        }

        private void SendData()
        {
            // update player information
            player.rotation.weaponRotation = playerManager.GetWeaponLastRotation().TwoDecimals();
            player.rotation.playerFlipped = playerManager.GetLastFlipped();

            // serialized player class makes it easy to convert to JSON
            networkIdentity.GetSocket().Emit("updateRotation", new JSONObject(JsonUtility.ToJson(player)));
        }
    }
}

