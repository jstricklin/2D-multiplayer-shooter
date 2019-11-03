﻿using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using UnityEngine;

namespace Project.Gameplay {
    public class Projectile : MonoBehaviour
    {
        Vector2 direction;
        float speed;

        public Vector2 Direction 
        {
            set {
                direction = value;
            }
        }

        public float Speed 
        {
            set {
                speed = value;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 pos = direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;   
            transform.position += new Vector3(pos.x, pos.y, 0);
        }
    }

}
