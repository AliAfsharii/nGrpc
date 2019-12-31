using System;
using System.Collections.Generic;

namespace nGrpc.ServerCommon
{
    public class ModulesData
    {
        public List<string> AllModulesNames { get; set; }
        public List<IModuleLoader> AllModuleLoaders { get; set; }
        public List<Type> AllDIServices { get; set; }
    }
}
