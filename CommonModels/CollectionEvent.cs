using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels
{
    public enum CollectionEvent : ushort
    {
        WIN_CHANGE = 1,
        LEFT_CLICK = 2,
        STATE_SCAN = 3
    }
}
