using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingsoft.Utils.Http;
using Kingsoft.Utils.TypeExtensions;

namespace LibHostServer
{
    public class ServerInfo
    {
        public string Address { get; set; }
        public ServerRole ServerType { get; set; }
        public string[] Logs { 
            get {
                return new TWebClient().TDownloadStringTaskAsync(new Uri($"{Address}/getLogs"))
                    .Result.jobj<string[]>();
            } 
        }

        public List<QueueItem> Queue { get; set; }

        private static bool Compare(ServerInfo val1, ServerInfo val2)
        {
            return val1.Address == val2.Address;
        }

        public static bool operator ==(ServerInfo a, ServerInfo b) => Compare(a,b);
        public static bool operator !=(ServerInfo a, ServerInfo b) => !Compare(a, b);
        public string Request(string path) => new TWebClient().TDownloadStringTaskAsync(new Uri($"http://{Address}{path}")).GetAwaiter().GetResult();
        public override bool Equals(object obj)
        {
            return (ServerInfo)obj == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
