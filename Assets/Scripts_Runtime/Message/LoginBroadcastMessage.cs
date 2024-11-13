using System;
using UnityEngine;

namespace UnityProtocol {

    // 谁上线了, 坐标
    public struct LoginBroadcastMessage {

        public string username;
        public Vector2 pos;

    }

}