using System;

namespace Simit.Data.Core
{
    public class FactoryCacheItem
    {
        public object Instance { get; set; }
        public Type Type { get; set; }
    }
}
