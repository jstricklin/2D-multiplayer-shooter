using System.Collections;
using System.Collections.Generic;
using Project.Utility.Attributes;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnityEngine;

namespace Project.Networking
{
    public class NetworkIdentity : SocketIOController
    {
        [Header("Helpful Values")]
        [SerializeField]
        [GreyOut]
        private string id;
        [SerializeField]
        [GreyOut]
        private bool isControlling;

        private BaseSocketIO socket;

        void Awake()
        {
            isControlling = false;
        }

        public void SetControllerID(string ID)
        {
            id = ID;
            isControlling = (NetworkClient.ClientID == ID) ? true : false;  // check id against ours saved from server
        }

        public void SetSocketReference(BaseSocketIO Socket)
        {
            socket = Socket;
        }
        public string GetID()
        {
            return id;
        }
        public bool IsControlling()
        {
            return isControlling;
        }

        public BaseSocketIO GetSocket()
        {
            return socket;
        }
    }

}