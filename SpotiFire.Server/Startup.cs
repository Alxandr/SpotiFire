using Owin;
using Gate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder
                .Map("/signalr", SignalRConfiguration)
                .UseGate((Func<Request, Response, Task>)GetFile);
        }

        private async Task GetFile(Request request, Response response)
        {
            
        }

        private void SignalRConfiguration(IAppBuilder builder)
        {
            
        }
    }
}
