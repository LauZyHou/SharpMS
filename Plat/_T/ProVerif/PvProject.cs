﻿using System;
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
        private PvDeclaration? globalDeclaration;
        private List<PvProcess>? processes;
        private List<PvQuery>? queries;
        private PvInstantiation? instantiation;

        public PvProject()
        {
        }

        public PvProject(PvDeclaration globalDeclaration, List<PvProcess> processes, List<PvQuery> queries, PvInstantiation instantiation)
        {
            this.globalDeclaration = globalDeclaration;
            this.processes = processes;
            this.queries = queries;
            this.instantiation = instantiation;
        }

        public PvDeclaration? GlobalDeclaration { get => globalDeclaration; set => globalDeclaration = value; }
        public List<PvProcess>? Processes { get => processes; set => processes = value; }
        public List<PvQuery>? Queries { get => queries; set => queries = value; }
        public PvInstantiation? Instantiation { get => instantiation; set => instantiation = value; }

        public override string ToString()
        {
            string res = "";
            if (globalDeclaration is not null)
            {
                res = globalDeclaration + "\n";
            }
            if (processes is not null && processes.Count > 0)
            {
                foreach (PvProcess proc in processes)
                {
                    res += proc + "\n";
                }
            }
            res += "\n";
            if (queries is not null && queries.Count > 0)
            {
                foreach (PvQuery query in queries)
                {
                    res += query + "\n";
                }
            }
            res += "\n";
            return res + instantiation;
        }
    }
}
