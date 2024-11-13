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

            if (headerID == MessageHelper.HEADER_LOGIN_REQ) {
                // LoginMessage
                LoginReqMessage message = JsonUtility.FromJson<LoginReqMessage>(str);
                OnPlayerLogin(ctx, connID, message);
            } else if (headerID == MessageHelper.HEADER_HELLO_REQ) {
                // HelloMessage
                // 1. string -> struct HelloMessage
                HelloReqMessage message = JsonUtility.FromJson<HelloReqMessage>(str);
                OnHello(ctx, connID, message);
            }
        }

        #region Messages
        static void OnPlayerLogin(ServerContext ctx, int connID, LoginReqMessage msg) {

            PlayerEntity player = new PlayerEntity();
            // player.onlyID = idService.GetPlayerID();
            player.connID = connID;
            player.username = msg.username;

            // Response
            LoginResMessage resMsg = new LoginResMessage();

            bool succ = ctx.playerRepository.AddPlayer(player);
            if (succ) {
                resMsg.status = 0;
                byte[] data = MessageHelper.BakeMessage(resMsg);
                ctx.server.Send(connID, data);
                Debug.Log($"Player Login Succ: " + msg.username);
            } else {
                resMsg.status = 1;
                byte[] data = MessageHelper.BakeMessage(resMsg);
                ctx.server.Send(connID, data);
                Debug.Log($"Player Login Failed Exist same user: " + msg.username);
            }
        }

        static void OnHello(ServerContext ctx, int connID, HelloReqMessage msg) {
            Debug.Log($"Received Hello: " + msg.myName + ", " + msg.myAge + ", " + msg.myData);
        }
        #endregion

    }
}