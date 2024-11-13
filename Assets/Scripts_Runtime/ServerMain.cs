using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using UnityProtocol;

namespace UnityServer {

    public class ServerMain : MonoBehaviour {

        ServerContext ctx;

        Telepathy.Server server;

        bool isTearDown = false;

        void Start() {

            Application.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            // ==== Ctor ====
            ctx = new ServerContext();
            int messageSize = 1024;
            server = new Server(messageSize);

            // ==== Inject ====
            ctx.Inject(server);

            // ==== Init ====

            // - Binding Event
            server.OnConnected = (int clientID, string str) => {
                PlayerDomain.OnClientConnected(ctx, clientID, str);
            };
            server.OnData = (int clientID, ArraySegment<byte> data) => {
                PlayerDomain.OnData(ctx, clientID, data);
            };
            server.OnDisconnected = (int clientID) => {
                PlayerDomain.OnClientDisconnected(ctx, clientID);
            };

            // - Listen
            int port = 7777;
            server.Start(port);

            Debug.Log("Server started on port: " + port);

        }

        void OnGUI() {
            // 在线人数
            GUILayout.Label("Online Players: " + ctx.playerRepository.Count);
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
