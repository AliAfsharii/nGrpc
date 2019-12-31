using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nGrpc.Worker
{
    public class ModulesData
    {
        public List<string> AllModulesNames { get; set; }
        public List<IModuleLoader> AllModuleLoaders { get; set; }
        public List<Type> AllDIServices { get; set; }
    }
}
