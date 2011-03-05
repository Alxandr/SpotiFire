using System;

namespace SpotiFire.Server
{
    internal class ClientAction
    {
        private Action<Client> action;
        private Client client;

        internal ClientAction(Action<Client> action, Client client = null)
        {
            this.action = action;
            this.client = client;
        }

        internal void Execute()
        {
            if (client != null)
                try { action(client); }
                catch { /* TODO: Flag client for deletion */}
            else
                foreach (var c in Client.All)
                    try { action(c); }
                    catch { /* TODO: Flag client for deletion */ }
        }
    }
}
