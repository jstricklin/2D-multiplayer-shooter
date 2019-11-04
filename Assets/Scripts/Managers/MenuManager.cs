using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utility;
using SocketIO;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Managers {
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Button queueButton;

        private SocketIOComponent socketReference;
        private SocketIOComponent SocketReference {
            get {
                return socketReference = (socketReference == null) ? FindObjectOfType<NetworkClient>() : socketReference;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            queueButton.interactable = false;            

            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.ONLINE, onLevelLoaded: (levelName) => {
                queueButton.interactable = true;
            });
        } 
        public void OnQueue()
        {
            // lazy loading
            // Debug.Log("socket reference: " + SocketReference.GetInstanceID());
            SocketReference.Emit(ev: "joinGame");
        }
    }

}
