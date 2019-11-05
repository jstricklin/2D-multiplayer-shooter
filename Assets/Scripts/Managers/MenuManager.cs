using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utility;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Managers {
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Button queueButton;

        private BaseSocketIO socketReference;
        private BaseSocketIO SocketReference {
            get {
                return socketReference = (socketReference == null) ? FindObjectOfType<SocketIOController>().socketIO : socketReference;
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
            SocketReference.Emit("joinGame");
        }
    }

}
