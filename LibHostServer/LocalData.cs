using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer
{
    public class LocalData
    {
        public LocalData()
        {
            Self = new ServerInfo();
            Self.ServerType = ServerRole.lonely;
            DatabaseManagers = new List<string>();
            ProcessingServers = new List<ServerInfo>();
            BackupServers = new List<ServerInfo>();
            InputServer = new ServerInfo();
        }

        public ServerInfo Self;

        public ServerInfo InputServer;
        public ServerInfo OutputServer;

        public List<string> DatabaseManagers { get; set; }
        public List<ServerInfo> ProcessingServers { get; set; }
        public List<ServerInfo> BackupServers { get; set; }
        public void AddHost(ServerInfo obj)
        {
            switch (Self.ServerType)
            {
                case ServerRole.standby:
                    break;
                case ServerRole.input:
                    Self.Request("/db/set/");
                    break;
                case ServerRole.lonely:
                    obj.ServerType = ServerRole.processing;
                    ProcessingServers.Add(obj);
                    Self.ServerType = ServerRole.inoutput;
                    break;
                case ServerRole.inoutput:
                    obj.ServerType = ServerRole.output;
                    OutputServer = obj;
                    Self.ServerType = ServerRole.input;
                    InputServer = Self;
                    break;
                default:
                    InputServer.Request($"/server/join?address={obj.Address}");
                    break;
            }
        }
        public void RemoveHost(ServerInfo obj)
        {
            ProcessingServers.Remove(obj); 
        }
        public ServerInfo[] GetHosts { 
            get
            {
                ServerInfo[] x = ProcessingServers.ToArray();
                return x;
            }
        }
    }
}
