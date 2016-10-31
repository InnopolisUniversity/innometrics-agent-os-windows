using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsMetrics.Structures
{
    public struct IP_ADDR_STRING
    {
        public IntPtr Next;
        public IP_ADDRESS_STRING IpAddress;
        public IP_ADDRESS_STRING IpMask;
        public Int32 Context;
    }
}
