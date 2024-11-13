using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityProtocol {

    public static class MessageHelper {

        public const int LOGIN_REQ = 10;
        public const int LOGIN_RES = 11;
        public const int LOGIN_BROADCAST = 12;
        public const int HELLO_REQ = 20;

        static Dictionary<Type, int> typeToHeaderID = new Dictionary<Type, int>() {
            {typeof(LoginReqMessage), LOGIN_REQ},
            {typeof(LoginResMessage), LOGIN_RES},
            {typeof(LoginBroadcastMessage), LOGIN_BROADCAST},
            {typeof(HelloReqMessage), HELLO_REQ},
        };

        public static byte[] BakeMessage<T>(T message) where T : struct {
            // 1. struct Message -> string
            string jsonStr = JsonUtility.ToJson(message);
            // 2. string -> byte[]
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonStr);

            // 3. byte[] -> byte[] (add header)
            Type type = typeof(T);
            int headerID = typeToHeaderID[type];
            byte[] final = new byte[data.Length + 4];
            final[0] = (byte)headerID;
            final[1] = (byte)(headerID >> 8);
            final[2] = (byte)(headerID >> 16);
            final[3] = (byte)(headerID >> 24);

            Array.Copy(data, 0, final, 4, data.Length);
            return final;
        }

    }

}