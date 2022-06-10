using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Kingsoft.Utils.Http;

namespace LibHostServer
{
    public class ManagerNodeInterface
    {
        public ManagerNodeInterface(string address)
        {
            NodeAddress = address;
        }
        public string NodeAddress { get; set; }
        public string DatabaseName { get; private set; }

        public void UpdateInfo()
        {
            TWebClient wc = new TWebClient();
            wc.Timeout = 1000;
            wc.TUploadStringTaskAsync(new Uri($"{NodeAddress}/get/node/info"), $"token={0}");

            wc.DownloadStringCompleted += (s, e) =>
            {
                Console.WriteLine(e.Error == null);
                Console.WriteLine(e.Result);
            };

            wc.UploadStringCompleted += (s, e) =>
            {
                Console.WriteLine(e.Error == null);
                Console.WriteLine(e.Result);
            };
        }
    }
}
