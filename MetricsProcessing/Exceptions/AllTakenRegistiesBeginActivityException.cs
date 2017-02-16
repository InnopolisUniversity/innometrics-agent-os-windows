using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsProcessing.Exceptions
{
    /// <summary>
    /// The exception means, that all the obtained registries are one activity (they have the same WindowId).
    /// There's a possibility that the first registries after that range can also be in the same activity.
    /// If this exception happen, more registries should be obtained in order to determine an activity and process it.
    /// </summary>
    class AllTakenRegistiesBeginActivityException : Exception
    {
        public RegistriesList TakenRegistries { get; }

        public AllTakenRegistiesBeginActivityException(RegistriesList takenRegistries)
        {
            TakenRegistries = takenRegistries;
        }
    }
}
