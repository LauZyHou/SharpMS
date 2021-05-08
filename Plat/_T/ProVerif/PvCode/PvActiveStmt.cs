using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif active statement, used to build process body and instantiation.
    /// </summary>
    public abstract class PvActiveStmt
    {
        internal int tabNum = 1;
        private string tabSuffix = "";

        /// <summary>
        /// 生成ToString时，前置的Tab数量
        /// 在最顶层（根部）这个Tab数量为1，嵌套向下的每一层都+1
        /// </summary>
        public int TabNum
        {
            get => tabNum;
            // 当对TabNum做修改时，需要对其Sub的语句做级联处理
            set
            {
                tabNum = value;
                if (this is PvSeqStmt) // 顺序语句
                {
                    PvSeqStmt that = (PvSeqStmt)this;
                    if (that.SubStmts is null)
                    {
                        return;
                    }
                    foreach (PvActiveStmt stmt in that.SubStmts)
                    {
                        stmt.TabNum = value;
                    }
                }
                else if (this is PvBranchStmt) // 分支语句
                {
                    PvBranchStmt that = (PvBranchStmt)this;
                    if (that.IfStmt is null)
                    {
                        return;
                    }
                    that.IfStmt.TabNum = value + 1;
                    if (that.ElseStmt is null)
                    {
                        return;
                    }
                    that.ElseStmt.TabNum = value + 1;
                }
            }
        }

        /// <summary>
        /// 根据TabNum生成的前置Tab数量，lazy compute
        /// </summary>
        public string TabSuffix
        {
            get
            {
                if (!string.IsNullOrEmpty(tabSuffix))
                {
                    return tabSuffix;
                }
                tabSuffix = "";
                for (int i = 0; i < tabNum; i++)
                {
                    tabSuffix += "\t";
                }
                return tabSuffix;
            }
        }
    }

    /// <summary>
    /// ProVerif sequencial statement，用于顺序地组合多条子PvActiveStmt
    /// </summary>
    public class PvSeqStmt : PvActiveStmt
    {
        private List<PvActiveStmt>? subStmts;

        public PvSeqStmt()
        {
        }

        public PvSeqStmt(List<PvActiveStmt> subStmts)
        {
            this.subStmts = subStmts;
            // 内部子语句的Tab数也和这一级相等
            foreach (PvActiveStmt stmt in subStmts)
            {
                stmt.TabNum = this.tabNum;
            }
        }

        /// <summary>
        /// 子PvActiveStmt
        /// </summary>
        public List<PvActiveStmt>? SubStmts
        {
            get => subStmts;
            set
            {
                if (value is null)
                {
                    return;
                }
                subStmts = value;
                // 内部子语句的Tab数也和这一级相等
                foreach (PvActiveStmt stmt in subStmts)
                {
                    stmt.TabNum = this.tabNum;
                }
            }
        }

        public override string ToString()
        {
            if (subStmts is null)
            {
                return "[ERROR in PvSeqStmt]";
            }
            string res = "";
            int len = subStmts.Count;
            if (len == 0)
            {
                return res;
            }
            for (int i = 0; i < len - 1; i++) // 前n-1个带顺序连接符';'
            {
                res += $"{subStmts[i]};\n";
            }
            res += $"{subStmts[len - 1]}"; // 最后一个不带
            return res;
        }
    }

    /// <summary>
    /// ProVerif branch statement，用于构造if-else分支语句
    /// </summary>
    public class PvBranchStmt : PvActiveStmt
    {
        private string? guard;
        private PvActiveStmt? ifStmt;
        private PvActiveStmt? elseStmt;

        public PvBranchStmt()
        {
        }

        public PvBranchStmt(string guard, PvActiveStmt ifStmt, PvActiveStmt elseStmt)
        {
            this.guard = guard;
            this.ifStmt = ifStmt;
            this.elseStmt = elseStmt;
            this.ifStmt.TabNum = this.tabNum + 1;
            this.elseStmt.TabNum = this.tabNum + 1;
        }

        /// <summary>
        /// 进入If语句块的条件
        /// </summary>
        public string? Guard { get => guard; set => guard = value; }
        /// <summary>
        /// If语句块
        /// </summary>
        public PvActiveStmt? IfStmt
        {
            get => ifStmt;
            set
            {
                if (value is null)
                {
                    return;
                }
                ifStmt = value;
                ifStmt.TabNum = this.tabNum + 1;
            }
        }
        /// <summary>
        /// Else语句块，当Guard条件不成立的时候进入
        /// </summary>
        public PvActiveStmt? ElseStmt
        {
            get => elseStmt; set
            {
                if (value is null)
                {
                    return;
                }
                elseStmt = value;
                elseStmt.TabNum = this.tabNum + 1;
            }
        }

        public override string ToString()
        {
            if (guard is null || ifStmt is null)
            {
                return "[Error in PvBranchStmt]";
            }
            string res = $"{TabSuffix}if {guard} then\n";
            res += ifStmt + "\n";
            if (elseStmt is null)
            {
                return res;
            }
            res += $"{TabSuffix}else\n";
            res += elseStmt + "\n";
            return res;
        }
    }

    /// <summary>
    /// ProVerif let语句，用于
    /// （1）声明一个local的变量接收函数结果，并且后续的所有语句都在此变量的声明周期内
    /// （2）模式匹配，如等值和类型的匹配 let (=pkB,k:key) = checksign(y,pkB) in
    /// 其类型可以视使用方式自动推断或匹配
    /// </summary>
    public class PvLetStmt : PvActiveStmt
    {
        private readonly string lh;
        private readonly string rh;

        public PvLetStmt(string lh, string rh)
        {
            this.lh = lh;
            this.rh = rh;
        }

        public string LH => lh;
        public string RH => rh;

        public override string ToString()
        {
            return $"{TabSuffix}let {lh} = {rh} in";
        }
    }

    /// <summary>
    /// ProVerif new local variable语句，用于
    /// 声明一个local的变量，指定其类型
    /// e.g. new a: Msg;
    /// </summary>
    public class PvNewVar : PvActiveStmt
    {
        private PvType type;
        private string name;

        public PvNewVar(PvType type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public PvType Type { get => type; set => type = value; }
        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            return $"{TabSuffix}new {type}:{name}";
        }
    }

    /// <summary>
    /// ProVerif send message操作语句，可以发多个变量的打包结果
    /// e.g. out(SN2HN, (suci, idHN));
    /// </summary>
    public class PvSendMsg : PvActiveStmt
    {
        private readonly PvChannel channel;
        private readonly List<string> vars;

        public PvSendMsg(PvChannel channel, List<string> vars)
        {
            this.channel = channel;
            this.vars = vars;
        }

        public PvChannel Channel => channel;
        public List<string> Vars => vars;

        public override string ToString()
        {
            if (vars.Count == 1)
            {
                return $"{TabSuffix}out({channel.Name}, {vars[0]})";
            }
            return $"{TabSuffix}out({channel.Name}, ({(string.Join(", ", vars))}))";
        }
    }

    /// <summary>
    /// ProVerif receive message操作语句，可以接收多个结果并自动解包和匹配模式
    /// e.g. in(SN2HN, (suci: SUCI, idHN: int));
    /// </summary>
    public class PvRecvMsg : PvActiveStmt
    {
        private readonly PvChannel channel;
        private readonly List<PvParam> pattern;

        public PvRecvMsg(PvChannel channel, List<PvParam> pattern)
        {
            this.channel = channel;
            this.pattern = pattern;
        }

        public PvChannel Channel => channel;
        public List<PvParam> Pattern => pattern;

        public override string ToString()
        {
            if (pattern.Count == 1)
            {
                return $"{TabSuffix}in({channel.Name}, {pattern[0]})";
            }
            return $"{TabSuffix}in({channel.Name}, ({string.Join(", ", pattern)}))";
        }
    }

    /// <summary>
    /// ProVerif process instantiation操作语句，可以例化进程的并发执行
    /// e.g. Proc__UE(id_UE_1, supi, rs, pk_HN_2) | Proc__SN(id_SN_1) | Proc__HN(id_HN_1, sk_HN_1) | Proc__HN(id_HN_2, sk_HN_2)
    /// </summary>
    public class PvConcurrency : PvActiveStmt
    {
        /// <summary>
        /// e.g. {Proc__UE, Proc__SN, Proc__HN, Proc__HN}
        /// </summary>
        private readonly List<PvProcess> processes;
        /// <summary>
        /// e.g. {{id_UE_1, supi, rs, pk_HN_2}, {id_SN_1}, {id_HN_1, sk_HN_1}, {id_HN_2, sk_HN_2}}
        /// </summary>
        private readonly List<List<string>> procVars;

        public PvConcurrency(List<PvProcess> processes, List<List<string>> procParams)
        {
            Debug.Assert(processes.Count == procParams.Count);
            int procCnt = processes.Count;
            for (int i = 0; i < procCnt; i++)
            {
                Debug.Assert(processes[i].Parameters.Count == procParams[i].Count);
                // todo: type check
            }
            this.processes = processes;
            this.procVars = procParams;
        }

        public List<PvProcess> Processes => processes;
        public List<List<string>> ProcParams => procVars;

        public override string ToString()
        {
            List<string> resList = new List<string>();
            int procCnt = processes.Count;
            for (int i = 0; i < procCnt; i++)
            {
                resList.Add($"{processes[i].Name}({string.Join(", ", procVars[i])})");
            }
            return TabSuffix + string.Join($" |\n{TabSuffix}", resList);
        }
    }

    /// <summary>
    /// Event在ProVerif的code中的打点语句
    /// </summary>
    public class PvEventPoint : PvActiveStmt
    {
        private PvEvent pvEvent;

        public PvEventPoint(PvEvent pvEvent)
        {
            this.pvEvent = pvEvent;
        }

        public PvEvent PvEvent { get => pvEvent; set => pvEvent = value; }

        public override string ToString()
        {
            return $"{TabSuffix}event {pvEvent.Name}";
        }
    }
}
