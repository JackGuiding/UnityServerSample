using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using UnityProtocol;

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

                // 3. 到底是什么类型的数据?
                int headerID = BitConverter.ToInt32(data.Array, 0);

                // 2. byte[] -> string(UTF8)
                string str = System.Text.Encoding.UTF8.GetString(data.Array, 4, data.Count - 4);

                if (headerID == 1) {
                    // LoginMessage
                    LoginMessage message = JsonUtility.FromJson<LoginMessage>(str);
                    Debug.Log($"Received {headerID} Login: " + message.username);
                } else if (headerID == 2) {
                    // HelloMessage
                    // 1. string -> struct HelloMessage
                    HelloMessage message = JsonUtility.FromJson<HelloMessage>(str);
                    Debug.Log($"Received {headerID} Hello: " + message.myName + ", " + message.myAge + ", " + message.myData);
                }

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
