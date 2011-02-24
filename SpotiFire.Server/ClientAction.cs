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
                action(client);
            else
                foreach (var c in Client.All)
                    action(c);
        }
    }
}
