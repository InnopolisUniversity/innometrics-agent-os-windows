using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsMetrics.Enums;

namespace WindowsMetrics.Structures
{
    public struct IP_ADAPTER_INFO
    {
        public IntPtr Next;
        public Int32 ComboIndex;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdaptersConsts.MAX_ADAPTER_NAME_LENGTH + 4)]
        public string AdapterName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdaptersConsts.MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
        public string AdapterDescription;
        public UInt32 AddressLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AdaptersConsts.MAX_ADAPTER_ADDRESS_LENGTH)]
        public byte[] Address;
        public Int32 Index;
        public UInt32 Type;
        public UInt32 DhcpEnabled;
        public IntPtr CurrentIpAddress;
        public IP_ADDR_STRING IpAddressList;
        public IP_ADDR_STRING GatewayList;
        public IP_ADDR_STRING DhcpServer;
        public bool HaveWins;
        public IP_ADDR_STRING PrimaryWinsServer;
        public IP_ADDR_STRING SecondaryWinsServer;
        public Int32 LeaseObtained;
        public Int32 LeaseExpires;
    }
}
