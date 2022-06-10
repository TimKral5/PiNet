using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer
{
    public enum ServerRole
    {
        standby=-1,
        input = 0,
        processing = 1,
        backup = 2,
        output = 3,
        lonely = 4,
        inoutput = 5,
    }
}
