using System;
using System.Collections.Generic;

namespace TixFactory.ApplicationAuthorization
{
    internal class ApplicationAuthorization
    {
        public ISet<string> OperationNames { get; set; }

        public DateTime LastRefresh { get; set; }

        public DateTime LastAccess { get; set; }
    }
}
