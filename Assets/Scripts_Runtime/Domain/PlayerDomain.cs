using System;
using UnityEngine;
using UnityProtocol;

namespace UnityServer {

    public static class PlayerDomain {

        public static void OnClientConnected(ServerContext ctx, int connID, string str) {
            Debug.Log("Connected: " + connID + ", " + str);
        }

        public static void OnClientDisconnected(ServerContext ctx, int connID) {
            ctx.playerRepository.RemovePlayerByConnID(connID);
        }

        public static void OnData(ServerContext ctx, int connID, ArraySegment<byte> data) {
            // 3. 到底是什么类型的数据?
            int headerID = BitConverter.ToInt32(data.Array, 0);

            // 2. byte[] -> string(UTF8)
            string str = System.Text.Encoding.UTF8.GetString(data.Array, 4, data.Count - 4);

            if (headerID == MessageHelper.LOGIN_REQ) {
                // LoginMessage
                LoginReqMessage message = JsonUtility.FromJson<LoginReqMessage>(str);
                OnPlayerLogin(ctx, connID, message);
            } else if (headerID == MessageHelper.HELLO_REQ) {
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
            player.pos = UnityEngine.Random.Range(0, 5f) * Vector2.one;

            // Response
            LoginResMessage resMsg = new LoginResMessage();

            bool succ = ctx.playerRepository.AddPlayer(player);
            if (succ) {
                // - Response: 给本人
                resMsg.status = 0;
                byte[] data = MessageHelper.BakeMessage(resMsg);
                ctx.server.Send(connID, data);
                Debug.Log($"Player Login Succ: " + msg.username);

                // - Broadcast
                ctx.playerRepository.Foreach(value => {
                    // 告诉所有人: 该新玩家的坐标
                    LoginBroadcastMessage broadcastMessage = new LoginBroadcastMessage();
                    broadcastMessage.username = value.username;
                    broadcastMessage.pos = player.pos;
                    byte[] data2 = MessageHelper.BakeMessage(broadcastMessage);
                    ctx.server.Send(value.connID, data2);
                });

                // - Boradcast: 告诉新人, 其他人的坐标
                ctx.playerRepository.Foreach(other => {
                    if (other.connID != connID) {
                        LoginBroadcastMessage broadcastMessage = new LoginBroadcastMessage();
                        broadcastMessage.username = other.username;
                        broadcastMessage.pos = other.pos;
                        byte[] data2 = MessageHelper.BakeMessage(broadcastMessage);
                        ctx.server.Send(connID, data2);
                    }
                });
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