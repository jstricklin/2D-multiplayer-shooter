using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scriptable
{
    [CreateAssetMenu(fileName = "Server_Objects", menuName = "ScriptableObjects/ServerObjects", order = 3)]
    public class ServerObjects : ScriptableObject
    {
        public List<ServerObjectData> serverObjects;
        public ServerObjectData GetObjectByName(string Name)
        {
            // fairly optimal Linq search
            return serverObjects.SingleOrDefault(x => x.Name == Name);
        }
    }
    // custom object with name and prefab because Dictionaries are not serializable
    [System.Serializable]
    public class ServerObjectData
    {
        public string Name = "New Object";
        public GameObject Prefab;
    }
}
