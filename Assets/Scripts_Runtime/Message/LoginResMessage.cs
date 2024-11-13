using System;

namespace UnityProtocol {

    public struct LoginResMessage {

        public sbyte status; // 0: success, 1: 用户名已存在, 2: 密码错误, 3: 用户名长度不规范......

    }
}