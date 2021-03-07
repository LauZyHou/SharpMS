using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL process model in memory, which can be trans to <template/> within <nta/>.
    /// An UPPAAL process model have a local declaration and a finite PG (program graph) model.
    /// </summary>
    public class UpProcess
    {
        private readonly string name;
        private readonly UpDeclaration localDeclaration;
        private readonly UpPG progGraph;
        private readonly List<UpParam> parameters;

        public UpProcess(string name, UpDeclaration localDeclaration, UpPG progGraph)
        {
            this.name = name;
            this.localDeclaration = localDeclaration;
            this.progGraph = progGraph;
            this.parameters = new List<UpParam>();
        }

        public string Name => name;
        public List<UpParam> Params => parameters;
        public UpDeclaration LocalDeclaration => localDeclaration;
        public UpPG ProgGraph => progGraph;

        public override string ToString()
        {
            string res = $"<template>\n<name x=\"0\" y=\"0\">{name}</name>\n";
            res += $"<parameter>{(string.Join(", ", parameters))}</parameter>\n";
            res += localDeclaration;
            res += progGraph;
            res += "</template>\n";
            return res;
        }
    }
}
