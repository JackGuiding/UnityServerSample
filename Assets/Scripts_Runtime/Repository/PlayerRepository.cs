using System;
using System.Collections.Generic;

namespace UnityServer {

    public class PlayerRepository {

        Dictionary<int, PlayerEntity> allByOnlyID;
        Dictionary<int, PlayerEntity> allByConnID;
        Dictionary<string, PlayerEntity> allByUsername;

        public PlayerRepository() {
            allByOnlyID = new Dictionary<int, PlayerEntity>();
            allByConnID = new Dictionary<int, PlayerEntity>();
            allByUsername = new Dictionary<string, PlayerEntity>();
        }

        public void Foreach(Action<PlayerEntity> action) {
            foreach (var player in allByOnlyID.Values) {
                action(player);
            }
        }

        public void AddPlayer(PlayerEntity player) {
            allByOnlyID.Add(player.onlyID, player);
            allByConnID.Add(player.clientID, player);
            allByUsername.Add(player.username, player);
        }

        public void RemovePlayer(PlayerEntity player) {
            allByOnlyID.Remove(player.onlyID);
            allByConnID.Remove(player.clientID);
            allByUsername.Remove(player.username);
        }

        public PlayerEntity GetPlayerByOnlyID(int onlyID) {
            return allByOnlyID[onlyID];
        }

        public PlayerEntity GetPlayerByConnID(int connID) {
            return allByConnID[connID];
        }

        public PlayerEntity GetPlayerByUsername(string username) {
            return allByUsername[username];
        }

    }
}