using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kingsoft.Utils.Http;
using static Kingsoft.Utils.KSystem.Global;
using LibHostServer;
using LibHostServer.Modules;

namespace PiNetHostServer
{
    public class Program
    {
        static void Main()
        {
            wl("starting server...");
            HostServer hostServer = new HostServer();
            hostServer.Init();
            ServerModule.Init(hostServer);
            DbModule.Init(hostServer);
            if(Console.ReadKey().KeyChar == 'y')
            {
                wl("");
                hostServer.ConnectToNetwork(inp("Node-Address:"), inp("Host-Address:"));
            }
            hostServer.RunServer(int.Parse(inp("Port:")));
            
        }
    }
}
