using System;
using UnityEngine;
using UnityProtocol;

namespace UnityServer {

    public static class PlayerDomain {

        public static void OnClientConnected(ServerContext ctx, int connID, string str) {
            Debug.Log("Connected: " + connID + ", " + str);
        }

        public static void OnData(ServerContext ctx, int connID, ArraySegment<byte> data) {
            // 3. 到底是什么类型的数据?
            int headerID = BitConverter.ToInt32(data.Array, 0);

            // 2. byte[] -> string(UTF8)
            string str = System.Text.Encoding.UTF8.GetString(data.Array, 4, data.Count - 4);

            if (headerID == 1) {
                // LoginMessage
                LoginMessage message = JsonUtility.FromJson<LoginMessage>(str);
                OnPlayerLogin(ctx, connID, message);
            } else if (headerID == 2) {
                // HelloMessage
                // 1. string -> struct HelloMessage
                HelloMessage message = JsonUtility.FromJson<HelloMessage>(str);
                OnHello(ctx, connID, message);
            }
        }

        #region Messages
        static void OnPlayerLogin(ServerContext ctx, int connID, LoginMessage msg) {
            Debug.Log($"Received Login: " + msg.username);
        }

        static void OnHello(ServerContext ctx, int connID, HelloMessage msg) {
            Debug.Log($"Received Hello: " + msg.myName + ", " + msg.myAge + ", " + msg.myData);
        }
        #endregion

    }
}