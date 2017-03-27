using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class OnlyOneNonProcessedRegistryTakenException : Exception
    {
        public Registry TheOnlyRemainingRegistry { get; }

        public OnlyOneNonProcessedRegistryTakenException(Registry remainingRegistry)
        {
            TheOnlyRemainingRegistry = remainingRegistry;
        }
    }
}
