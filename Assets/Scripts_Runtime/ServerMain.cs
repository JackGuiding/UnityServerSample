using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;

namespace UnityServer {

    public class ServerMain : MonoBehaviour {

        Telepathy.Server server;

        bool isTearDown = false;

        void Start() {

            Application.runInBackground = true;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            int messageSize = 1024;
            server = new Server(messageSize);

            // - Binding Event
            server.OnConnected = (int clientID, string str) => {
                Debug.Log("Connected: " + clientID + ", " + str);
            };
            server.OnData = (int clientID, ArraySegment<byte> data) => {
                Debug.Log("Data: " + clientID + ", " + data.Count);
                server.Send(clientID, data);
            };
            server.OnDisconnected = (int clientID) => {
                Debug.Log("Disconnected: " + clientID);
            };

            // - Listen
            int port = 7777;
            server.Start(port);

            Debug.Log("Server started on port: " + port);

        }

        void Update() {
            server.Tick(100);
        }

        void OnDestroy() {
            OnTearDown();
        }

        void OnApplicationQuit() {
            OnTearDown();
        }

        void OnTearDown() {
            if (isTearDown) {
                return;
            }
            isTearDown = true;

            // 因为网络会启动一个线程
            server.Stop();
        }
    }
}
