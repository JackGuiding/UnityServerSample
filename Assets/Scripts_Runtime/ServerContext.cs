using System;

namespace UnityServer {

    public class ServerContext {

        public Telepathy.Server server;

        public PlayerRepository playerRepository;

        public ServerContext() {
            playerRepository = new PlayerRepository();
        }

        public void Inject(Telepathy.Server server) {
            this.server = server;
        }
    }
}