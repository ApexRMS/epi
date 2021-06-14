// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2021 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Runtime.Serialization;

namespace SyncroSim.Epi
{
    [Serializable()]
    public sealed class EpidemicException : Exception
    {
        public EpidemicException() : base("Epidemic Exception")
        {
        }

        public EpidemicException(string message) : base(message)
        {
        }

        public EpidemicException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private EpidemicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
