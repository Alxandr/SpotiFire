using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace SpotiFire.Server
{
    public class Client
    {
        private static Dictionary<ISpotifireClient, Client> clients = new Dictionary<ISpotifireClient, Client>();
        private static Queue<Client> clientQueue = new Queue<Client>();
        private static readonly object clientQueueLock = new object();
        private static AutoResetEvent clientQueueResetEvent = new AutoResetEvent(false);
        private static Thread clientQueueThread;

        static Client()
        {
            clientQueueThread = new Thread(() =>
            {
                AutoResetEvent waitEvent = new AutoResetEvent(false);
                while (true)
                {
                    DateTime? next;
                    lock (clientQueueLock)
                    {
                        if (clientQueue.Count == 0)
                            next = null;
                        else
                            next = clientQueue.Peek().lastRequest;
                    }

                    if (next == null)
                    {
                        clientQueueResetEvent.WaitOne();
                        continue;
                    }

                    TimeSpan waitTime = next.Value.Add(TimeSpan.FromSeconds(31)).Subtract(DateTime.Now);
                    if (waitTime > TimeSpan.Zero)
                        waitEvent.WaitOne(waitTime);

                    Client client;
                    lock (clientQueueLock)
                        client = clientQueue.Dequeue();

                    if (DateTime.Now.Subtract(client.lastRequest) > TimeSpan.FromSeconds(30))
                        SpotifireServer.Instance.MessageClients(c => c.Ping(), client);
                }

            });
            clientQueueThread.IsBackground = true;
            clientQueueThread.Start();
        }

        internal static Client Current
        {
            get
            {
                ISpotifireClient client = OperationContext.Current.GetCallbackChannel<ISpotifireClient>();
                if (!clients.ContainsKey(client))
                    clients.Add(client, new Client(client));

                Client c = clients[client];
                c.Update();
                return c;
            }
        }
        internal static void UpdateCurrent()
        {
            var c = Client.Current; // Force update;
            c = null;
        }
        internal static Dictionary<ISpotifireClient, Client>.ValueCollection All
        {
            get
            {
                return clients.Values;
            }
        }

        private bool authenticated = false;
        private ISpotifireClient client = null;
        private DateTime lastRequest = DateTime.MinValue;
        private Client(ISpotifireClient client)
        {
            this.client = client;
        }

        private void Update()
        {
            this.lastRequest = DateTime.Now;
            lock (clientQueueLock)
                clientQueue.Enqueue(this);
            clientQueueResetEvent.Set();
        }

        public bool Authenticated
        {
            get
            {
                return authenticated;
            }
            internal set
            {
                authenticated = value;
            }
        }

        public ISpotifireClient Connection
        {
            get
            {
                if (authenticated)
                    return client;
                return null;
            }
        }
    }
}
