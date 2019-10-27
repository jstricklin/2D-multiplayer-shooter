using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

namespace Project.Networking {
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerGO;

        private Dictionary<string, GameObject> serverObjects;
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
        }

        private void Initialize()
        {
            serverObjects = new Dictionary<string, GameObject>();
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
                string id = e.data["id"].ToString();
                Debug.LogFormat("Our Client's Id is ({0})", id);

            });

            On("spawn", (e) => {
                // handling all spawned players
                string id = e.data["id"].ToString();

                GameObject go = new GameObject("Server ID: " + id);
                go.transform.SetParent(networkContainer);
                serverObjects.Add(id, go);
                GameObject toSpawn = Instantiate(playerGO);
                toSpawn.name = "Player GO - ID: " + id;
            });

            On("disconnect", (e) => {
                string id = e.data["id"].ToString();

                GameObject go = serverObjects[id];
                Destroy(go); // remove GO from game
                serverObjects.Remove(id);

            });
        }
        void RemoveQuotes()
        {

        }
    }
}
