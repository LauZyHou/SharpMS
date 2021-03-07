using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL project model, which contains global declaration, processes and instantiation.
    /// A UPPAAL project is a top model in UPPAAL verification tool.
    /// </summary>
    public class UpProject
    {
        private readonly UpDeclaration globalDeclaration;
        private readonly List<UpProcess> processes;
        private readonly UpInstantiation modelDeclaration;
        private readonly List<UpQuery> queries;

        public UpProject(UpDeclaration globalDeclaration, UpInstantiation modelDeclaration)
        {
            this.globalDeclaration = globalDeclaration;
            this.processes = new List<UpProcess>();
            this.modelDeclaration = modelDeclaration;
            this.queries = new List<UpQuery>();
        }

        public UpDeclaration GlobalDeclaration => globalDeclaration;
        public List<UpProcess> Processes => processes;
        public UpInstantiation ModelDeclaration => modelDeclaration;
        public List<UpQuery> Queries => queries;

        public override string ToString()
        {
            string res = "<?xml version=\"1.0\" encoding=\"utf - 8\"?>\n";
            res += "<!DOCTYPE nta PUBLIC '-//Uppaal Team//DTD Flat System 1.1//EN' 'http://www.it.uu.se/research/group/darts/uppaal/flat-1_2.dtd'>\n";
            res += "<nta>\n";
            res += globalDeclaration;
            foreach (UpProcess process in processes)
            {
                res += process;
            }
            res += modelDeclaration;
            res += "<queries>\n";
            foreach (UpQuery query in queries)
            {
                res += query;
            }
            res += "</queries>\n</nta>\n";
            return res;
        }
    }
}
