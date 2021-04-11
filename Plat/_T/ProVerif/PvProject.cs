using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif project model.
    /// A ProVerif project is a top model in ProVerif verification tool.
    /// </summary>
    public class PvProject
    {
        private readonly PvDeclaration globalDeclaration;
        private readonly List<PvProcess> processes;
        private readonly List<PvQuery> queries;
        private readonly PvInstantiation instantiation;

        public PvProject(PvDeclaration globalDeclaration, List<PvProcess> processes, List<PvQuery> queries, PvInstantiation instantiation)
        {
            this.globalDeclaration = globalDeclaration;
            this.processes = processes;
            this.queries = queries;
            this.instantiation = instantiation;
        }

        public PvDeclaration GlobalDeclaration => globalDeclaration;
        public List<PvProcess> Processes => processes;
        public List<PvQuery> Queries => queries;
        public PvInstantiation Instantiation => instantiation;
    }
}
