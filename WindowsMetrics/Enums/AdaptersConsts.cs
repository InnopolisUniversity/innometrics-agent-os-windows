using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsMetrics.Enums
{
    public enum AdaptersConsts : uint
    {
        MAX_ADAPTER_DESCRIPTION_LENGTH = 128,
        ERROR_BUFFER_OVERFLOW = 111,
        MAX_ADAPTER_NAME_LENGTH = 256,
        MAX_ADAPTER_ADDRESS_LENGTH = 8,
        MIB_IF_TYPE_OTHER = 1,
        MIB_IF_TYPE_ETHERNET = 6,
        MIB_IF_TYPE_TOKENRING = 9,
        MIB_IF_TYPE_FDDI = 15,
        MIB_IF_TYPE_PPP = 23,
        MIB_IF_TYPE_LOOPBACK = 24,
        MIB_IF_TYPE_SLIP = 28
    }
}
