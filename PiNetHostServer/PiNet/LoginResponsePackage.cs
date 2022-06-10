using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiNetHostServer.PiNet
{
    internal class LoginResponsePackage
    {
        public int Status { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
