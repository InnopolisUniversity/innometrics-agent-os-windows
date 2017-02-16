using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing.Exceptions
{
    public class OnlyOneNonProcessedRegistryTakenException : Exception
    {
        public Registry TheOnlyRemainingRegistry { get; }

        public OnlyOneNonProcessedRegistryTakenException(Registry remainingRegistry)
        {
            TheOnlyRemainingRegistry = remainingRegistry;
        }
    }
}
