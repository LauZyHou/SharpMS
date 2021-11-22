using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL code statement, general abstract class.
    /// </summary>
    public abstract class UpStatement
    {
    }

    /// <summary>
    /// UPPAAL assign statement, i.e. left hand := right hand
    /// e.g. a[1].b.c[0] := myFun(e, f[0])
    /// [todo] class definition need to be improve.
    /// </summary>
    public class UpAssignment : UpStatement
    {
        private readonly string lh;
        private readonly string rh;

        public UpAssignment(string lh, string rh)
        {
            this.lh = lh;
            this.rh = rh;
        }

        public string LH => lh;
        public string RH => rh;

        public override string ToString()
        {
            return $"{lh} := {rh}";
        }
    }

    /// <summary>
    /// UPPAAL pass statement, just an empty code line.
    /// </summary>
    public class UpPass : UpStatement
    {
        private UpPass()
        {
        }

        public static UpPass Me = new UpPass();

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// UPPAAL process instance concurrency statement, which is used at the end of the model declaration.
    /// </summary>
    public class UpConcurrency : UpStatement
    {
        private List<string> processInstances;

        public UpConcurrency(params string[] insts)
        {
            this.processInstances = new List<string>();
            foreach (string inst in insts)
            {
                this.processInstances.Add(inst);
            }
        }

        public UpConcurrency()
        {
            this.processInstances = new List<string>();
        }

        public UpConcurrency(List<string> procInsts)
        {
            this.processInstances = procInsts;
        }

        public List<string> ProcessInstances { get => processInstances; set => processInstances = value; }

        public override string ToString()
        {
            return $"system {(string.Join(", ", processInstances))}";
        }
    }

    /// <summary>
    /// UPPAAL new variable declaration statement.
    /// </summary>
    public class UpNewVar : UpStatement
    {
        private readonly UpType type;
        private readonly string name;
        private readonly bool isMeta;

        public UpNewVar(UpType type, string name, bool isMeta = false)
        {
            this.type = type;
            this.name = name;
            this.isMeta = isMeta;
        }

        public UpType Type => type;
        public string Name => name;
        public bool IsMeta => isMeta;

        public override string ToString()
        {
            if (isMeta) return $"meta {type.Name} {name}";
            return $"{type.Name} {name}";
        }
    }

    /// <summary>
    /// UPPAAL type defination (one line def).
    /// </summary>
    public class UpTypeDef : UpStatement
    {
        private readonly UpType type;

        public UpTypeDef(UpType type)
        {
            this.type = type;
        }

        public UpType Type => type;

        public override string ToString()
        {
            string descPrefix = "";
            if (!string.IsNullOrEmpty(type.Description))
            {
                descPrefix = $"// {type.Description}\n";
            }
            if (type.FromType is null)
            {
                string res = descPrefix + "typedef struct\n{\n";
                foreach (UpParam prop in type.Props)
                {
                    res += $"\t{prop};\n";
                }
                res += "} " + type.Name;
                return res;
            }
            return $"{descPrefix}typedef {type.FromType.Name} {type.Name}";
        }
    }

    /// <summary>
    /// UPPAAL注释
    /// </summary>
    public class UpComment : UpStatement
    {
        private readonly string content;
        private readonly bool threeLine;

        public UpComment(string content, bool threeLine = false)
        {
            this.content = content;
            this.threeLine = threeLine;
        }

        public string Content => content;
        /// <summary>
        /// 三行大注释
        /// </summary>
        public bool ThreeLine => threeLine;

        public override string ToString()
        {
            if (!threeLine)
                return $"// {content}";
            return $"//\n// {content}\n//";
        }
    }

    /// <summary>
    /// 初始化语句
    /// </summary>
    public class UpInitVar : UpStatement
    {
        private readonly UpType type;
        private readonly string name;
        private readonly string value;

        public UpInitVar(UpType type, string name, string value)
        {
            this.type = type;
            this.name = name;
            this.value = value;
        }

        public UpType Type => type;
        public string Name => name;
        public string Value => value;

        public override string ToString()
        {
            return $"{type.Name} {name} = {value}";
        }
    }

    /// <summary>
    /// 进程实例化语句
    /// </summary>
    public class UpInstProc : UpStatement
    {
        private readonly string instName;
        private readonly UpProcess proc;
        private List<string> values;

        public UpInstProc(string instName, UpProcess proc)
        {
            this.instName = instName;
            this.proc = proc;
            this.values = new List<string>();
        }

        public string InstName => instName;
        public UpProcess Proc => proc;
        public List<string> Values { get => values; set => values = value; }

        public override string ToString()
        {
            return $"{instName} = {proc.Name}({string.Join(", ", values)})";
        }
    }
}
