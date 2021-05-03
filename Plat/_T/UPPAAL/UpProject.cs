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
        private UpDeclaration? globalDeclaration;
        private List<UpProcess>? processes;
        private UpInstantiation? modelDeclaration;
        private List<UpQuery>? queries;

        public UpProject()
        {
        }

        public UpProject(UpDeclaration globalDeclaration, List<UpProcess> processes, UpInstantiation modelDeclaration, List<UpQuery> queries)
        {
            this.globalDeclaration = globalDeclaration;
            this.processes = processes;
            this.modelDeclaration = modelDeclaration;
            this.queries = queries;
        }

        public UpDeclaration? GlobalDeclaration { get => globalDeclaration; set => globalDeclaration = value; }
        public List<UpProcess>? Processes { get => processes; set => processes = value; }
        public UpInstantiation? ModelDeclaration { get => modelDeclaration; set => modelDeclaration = value; }
        public List<UpQuery>? Queries { get => queries; set => queries = value; }

        public override string ToString()
        {
            if (globalDeclaration is null || processes is null || modelDeclaration is null || queries is null)
            {
                return "[ERROR: null reference in UpProject]";
            }

            string res = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
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
