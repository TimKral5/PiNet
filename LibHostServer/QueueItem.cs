using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer
{
    public struct QueueItem
    {
        public DbOperation Operation { get; set; }
        public string Data { get; set; }
    }
}
