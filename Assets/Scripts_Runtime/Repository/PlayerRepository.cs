using System;
using System.Collections.Generic;

namespace UnityServer {

    public class PlayerRepository {

        Dictionary<int, PlayerEntity> allByConnID;
        Dictionary<string, PlayerEntity> allByUsername;

        public int Count => allByConnID.Count;

        public PlayerRepository() {
            allByConnID = new Dictionary<int, PlayerEntity>();
            allByUsername = new Dictionary<string, PlayerEntity>();
        }

        public void Foreach(Action<PlayerEntity> action) {
            foreach (var player in allByConnID.Values) {
                action(player);
            }
        }

        public bool AddPlayer(PlayerEntity player) {
            bool succ = allByConnID.TryAdd(player.connID, player);
            succ &= allByUsername.TryAdd(player.username, player);
            return succ;
        }

        public void RemovePlayer(PlayerEntity player) {
            allByConnID.Remove(player.connID);
            allByUsername.Remove(player.username);
        }

        public PlayerEntity GetPlayerByConnID(int connID) {
            return allByConnID[connID];
        }

        public PlayerEntity GetPlayerByUsername(string username) {
            return allByUsername[username];
        }

    }
}