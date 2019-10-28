using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SocketIO;
using Project.Utility;
using Project.Player;

namespace Project.Networking {
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerGO;
        public static string ClientID { get; private set; }

        private Dictionary<string, NetworkIdentity> serverObjects;
                
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
        }

        private void Initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
        private void SetupEvents()
        {
            // Initial connection will connect just once, even if console displays TWO logs.
            On("open", (e) => {
                Debug.Log("connection made to server");
            });

            On("register", (e) => {
                ClientID = e.data["id"].ToString().RemoveQuotes();
                // Debug.LogFormat("Our Client's Id is ({0})", ClientID);
            });

            On("spawn", (e) => {
                // handling all spawned players
                string id = e.data["id"].ToString().RemoveQuotes();

                GameObject go = Instantiate(playerGO, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                serverObjects.Add(id, ni);
            });

            On("disconnected", (e) => {
                string id = e.data["id"].ToString().RemoveQuotes();
                Debug.Log("player disconnected");
                GameObject go = serverObjects[id].gameObject;
                Destroy(go); // remove GO from game
                serverObjects.Remove(id);
            });

            On("updateRotation", (e) => {
                string id = e.data["id"].ToString().RemoveQuotes();
                float weaponRot = e.data["weaponRotation"].f;
                bool flipped = e.data["playerFlipped"].b;
                NetworkIdentity ni = serverObjects[id];
                Debug.Log("updating other rotations");
                ni.GetComponent<PlayerManager>().SetWeaponRotation(weaponRot);
            });

            On("updatePosition", (e) => {
                string id = e.data["id"].ToString().RemoveQuotes();
                float x = e.data["position"]["x"].f;
                float y = e.data["position"]["y"].f;
                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y ,0);
            });
        }
    }
    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
        public Rotation rotation;
    }
    [Serializable]
    public class Position {
        public float x;
        public float y;
    }
    [Serializable]
    public class Rotation {
        public bool playerFlipped;
        public float weaponRotation;
    }
}
