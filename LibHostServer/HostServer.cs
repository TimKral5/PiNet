using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kingsoft.Utils.Http;
using Kingsoft.Utils.TypeExtensions;
using Kingsoft.Utils.TypeExtensions.Arrays;
using Kingsoft.Utils.TypeExtensions.Dictionaries;
using Newtonsoft.Json;
using JC = Newtonsoft.Json.JsonConvert;

namespace LibHostServer
{
    public class HostServer
    {
        public HttpServer Server { get; private set; }
        public TWebClient client;
        public LocalData data { get; set; }
        public Dictionary<string, string> dataBase { get; set; }
        public string[] keys = new string[] { "address" };

        public int ProcServerIndex = 0;

        public ServerInfo GetProcServer()
        {
            ServerInfo serverInfo = data.ProcessingServers[ProcServerIndex];
            ProcServerIndex++;
            if (ProcServerIndex > data.ProcessingServers.Count - 1) 
                ProcServerIndex = 0;
            return serverInfo;
        }

        public (bool, ServerInfo) GetHost(string key)
        {
            bool x = false;
            ServerInfo serverInfo = null;
            for (int i = 0; i < data.GetHosts.Length; i++)
            {
                if (data.GetHosts[i].Address == key)
                {
                    x = true;
                    serverInfo = data.GetHosts[i];
                }
            }
            return (x, serverInfo);
        }

        public void Init()
        {
            //if (!File.Exists("config.json"))

            client = new TWebClient();
            data = new LocalData();
            Server = new HttpServer();
            client.Timeout = 2000;
            client.ErrorMessage = new Dictionary<string, object> { ["status"] = "timeout" }.json();
            dataBase = new Dictionary<string, string>();

        }

        public void ConnectToNetwork(string address, string self)
        {
            TaskAwaiter<string> taskAwaiter = client.TDownloadStringTaskAsync(new Uri($"http://{address}/server/join?address={self}"))
                .GetAwaiter();
            taskAwaiter.OnCompleted(new Action(() =>
            {
                //Console.WriteLine(taskAwaiter.GetResult());
            }));
        }

        public void RunServer(int port) => Server.RunServer(port);
    }
}
