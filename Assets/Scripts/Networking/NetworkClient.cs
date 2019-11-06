using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnitySocketIO;
using UnitySocketIO.Events;
using Project.Utility;
using Project.Player;
using Project.Scriptable;
using Project.Gameplay;
using Project.Managers;

namespace Project.Networking {
    public class NetworkClient : MonoBehaviour
    {
        // this value should be server interval update time divided by 10
        public const float SERVER_UPDATE_TIME = 10;
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerGO;
        [SerializeField]
        private ServerObjects serverSpawnables;
        public static string ClientID { get; private set; }
        SocketIOController io;

        private Dictionary<string, NetworkIdentity> serverObjects;
                
        // Start is called before the first frame update
        public void Start()
        {
            io = FindObjectOfType<SocketIOController>();
            io.Connect();
            Initialize();
            SetupEvents();
        }

        private void Initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        // Update is called once per frame
        // public override void Update()
        // {
        //     base.Update();
        // }
        private void SetupEvents()
        {
            // Initial connection will connect just once, even if console displays TWO logs.
            io.On("open", (e) => {
                Debug.Log("connection made to server");
            });

            io.On("register", (e) => {
                ClientID = new JSONObject(e.data)["id"].str;
                Debug.LogFormat("Our Client's Id is ({0})", ClientID);
            });

            io.On("spawn", (e) => {
                // handling all spawned players
                string id = new JSONObject(e.data)["id"].str;

                GameObject go = Instantiate(playerGO, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(io.socketIO);
                serverObjects.Add(id, ni);
            });

            io.On("disconnected", (e) => {
                string id = new JSONObject(e.data)["id"].str;
                Debug.Log("player disconnected");
                GameObject go = serverObjects[id].gameObject;
                Destroy(go); // remove GO from game
                serverObjects.Remove(id);
            });

            io.On("updateRotation", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                float weaponRot = data["weaponRotation"].f;
                bool flipped = data["playerFlipped"].b;
                NetworkIdentity ni = serverObjects[id];
                // Debug.Log("updating other rotations");
                ni.GetComponent<PlayerManager>().SetWeaponRotation(weaponRot);
            });

            io.On("updatePosition", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, 0);
            });

            io.On("serverSpawn", (e) => {
                JSONObject data = new JSONObject(e.data);
                string name = data["name"].str;
                string id = data["id"].ToString().RemoveQuotes();
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                if (!serverObjects.ContainsKey(id))
                {
                    // Debug.LogFormat("Server wants to spawn '{0}'", name);
                    ServerObjectData sod = serverSpawnables.GetObjectByName(name);
                    var spawnObject = Instantiate(sod.Prefab, networkContainer);
                    spawnObject.transform.position = new Vector3(x, y, 0);
                    NetworkIdentity ni = spawnObject.GetComponent<NetworkIdentity>();
                    ni.SetControllerID(id);
                    ni.SetSocketReference(io.socketIO);

                    // if projectile apply direction as well
                    if (name == "Arrow_Regular") 
                    {
                        float directionX = data["direction"]["x"].f;
                        float directionY = data["direction"]["y"].f;
                        string activator = data["activator"].ToString().RemoveQuotes();
                        float speed = data["speed"].f;

                        float rot = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;
                        Vector3 currentRotation = new Vector3(0, 0, rot + 180);
                        spawnObject.transform.rotation = Quaternion.Euler(currentRotation);

                        WhoActivatedMe whoActivatedMe = spawnObject.GetComponent<WhoActivatedMe>();
                        whoActivatedMe.SetActivator(activator);

                        Projectile projectile = spawnObject.GetComponent<Projectile>();
                        projectile.Direction = new Vector2(directionX, directionY);
                        projectile.Speed = speed;
                    }
                    serverObjects.Add(id, ni);
                }
            });
            io.On("serverDespawn", (e) => {
                string id = new JSONObject(e.data)["id"].str;
                NetworkIdentity ni = serverObjects[id];
                serverObjects.Remove(id);
                Destroy(ni.gameObject);
            });
            io.On("playerDied", (e) => {
                string id = new JSONObject(e.data)["id"].str;
                NetworkIdentity ni = serverObjects[id];
                ni.gameObject.SetActive(false);
            });
            io.On("playerRespawn", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, 0);
                ni.gameObject.SetActive(true);
            });
            io.On("loadGame", (e) => {
                SceneManagementManager.Instance.LoadLevel(levelName: SceneList.LEVEL, onLevelLoaded: (levelName) => {
                    SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
                });
            });
        }

        public void AttemptToJoinLobby() {
            io.Emit("joinGame");
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
    [Serializable]
    public class ProjectileData
    {
        public string id;
        public string activator;
        public Position position;
        public Position direction;
    }
    [Serializable]
    public class IDData {
        public string id;
    }
}
