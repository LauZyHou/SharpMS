

using Plat._C;
using Plat._M;
using Plat._VM;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Plat._T
{
    public class ProVerifAlignMachine
    {
        /// <summary>
        /// ProVerif语义对齐机运行
        /// </summary>
        /// <returns>构造的ProVerif内存模型f</returns>
        public static PvProject? Run()
        {
            //
            // 全局声明
            //
            PvDeclaration globalDec = new PvDeclaration();

            #region 数据类型的映射

            // 【Type->PvType】
            Dictionary<Type, PvType> typeMap = new Dictionary<Type, PvType>()
            {
                { Type.TYPE_INT, PvType.INT },
                { Type.TYPE_BOOL, PvType.BOOL },
                { Type.TYPE_MSG, PvType.BITSTRING },
                { Type.TYPE_KEY, PvType.KEY },
                { Type.TYPE_PUB_KEY, PvType.PUBKEY },
                { Type.TYPE_PVT_KEY, PvType.PVTKEY }
            };
            // 【PvType List】
            List<PvType> pvTypeList = new List<PvType>()
            {
                PvType.INT, PvType.BOOL, PvType.KEY, PvType.PUBKEY, PvType.PVTKEY
            };
            // 数据类型声明生成
            globalDec.Statements.Add(new PvCommentForDec("Type Declaration"));
            foreach (PvType pvType in pvTypeList)
            {
                globalDec.Statements.Add(new PvTypeDeclaration(pvType));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 调用器（函数和方法）的映射

            // 【Caller->PvFun】
            Dictionary<Caller, PvFun> funMap = new Dictionary<Caller, PvFun>()
            {
                // todo
            };
            // 【PvFun List】
            List<PvFun> pvFunList = new List<PvFun>()
            {
                PvFun.PK, PvFun.ASYMENC, PvFun.ASYMDEC
            };
            // 函数声明生成
            globalDec.Statements.Add(new PvCommentForDec("Fun Declaration"));
            foreach (PvFun pvFun in pvFunList)
            {
                globalDec.Statements.Add(new PvFuncDeclaration(pvFun));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 解构器公式的映射

            // 【Axiom->PvEquation】
            Dictionary<Axiom, PvEquation> axiomMap = new Dictionary<Axiom, PvEquation>()
            {
                // todo
            };
            // 【PvEquation List】
            List<PvEquation> pvEquationList = new List<PvEquation>()
            {
                PvEquation.ASYM_ENC_DEC
            };
            // 解构器声明生成
            globalDec.Statements.Add(new PvCommentForDec("Equation Declaration"));
            foreach (PvEquation pvEquation in pvEquationList)
            {
                globalDec.Statements.Add(new PvEquationDeclaration(pvEquation));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 信道声明的映射
            // fixme

            // 【PvChannel List】
            List<PvChannel> pvChannelList = new List<PvChannel>();
            // 遍历拓扑图上的环境实例上的信道实例填表
            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                if (dragDrop_VM is EnvInst_VM)
                {
                    EnvInst_VM envInst_VM = (EnvInst_VM)dragDrop_VM;
                    foreach (ChanInst chanInst in envInst_VM.EnvInst.ChanInsts)
                    {
                        Channel? chan = chanInst.Channel;
                        if (chan is null) continue;
                        // 创建PvChannel
                        PvChannel pvChannel = new PvChannel($"E{envInst_VM.EnvInst.Id}_{chan.Identifier}", !chan.Pub);
                        pvChannelList.Add(pvChannel);
                    }
                }
            }
            // 生成信道声明
            globalDec.Statements.Add(new PvCommentForDec("Channel Declaration"));
            foreach (PvChannel pvChannel in pvChannelList)
            {
                globalDec.Statements.Add(new PvChannelDeclaration(pvChannel));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 全局常量声明
            // 为ProcInst和EnvInst中的所有（递归查找）ValueInst的值变成相应的全局变量声明
            // 比如 int值1变成i1，即
            // Int -> i
            // Bool -> b
            // Key -> k
            // PubKey -> pk
            // PvtKey -> sk
            HashSet<string> constNameSet = new HashSet<string>(); // 不重复的全局常量名表
            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                if (dragDrop_VM is EnvInst_VM || dragDrop_VM is ProcInst_VM)
                {
                    // 获取属性参数的实例列表
                    ObservableCollection<Instance> instList;
                    if (dragDrop_VM is EnvInst_VM)
                    {
                        EnvInst_VM envInst_VM = (EnvInst_VM)dragDrop_VM;
                        instList = envInst_VM.EnvInst.Properties;
                    }
                    else // dragDrop_VM is ProcInst_VM
                    {
                        ProcInst_VM procInst_VM = (ProcInst_VM)dragDrop_VM;
                        instList = procInst_VM.ProcInst.Properties;
                    }
                    // 递归遍历每个实例，加入到不重复的全局变量名表中
                    foreach (Instance instance in instList)
                    {
                        GenGlobalVarFromInstance(instance, constNameSet);
                    }
                }
            }
            // 遍历全局变量名变生成全局变量声明
            globalDec.Statements.Add(new PvCommentForDec("Channel Declaration"));
            foreach (string s in constNameSet)
            {
                PvType? pvType = null;
                if (s.StartsWith("i")) pvType = PvType.INT;
                else if (s.StartsWith("b")) pvType = PvType.BOOL;
                else if (s.StartsWith("m")) pvType = PvType.BITSTRING;
                else if (s.StartsWith("k")) pvType = PvType.KEY;
                else if (s.StartsWith("pk")) pvType = PvType.PUBKEY;
                else if (s.StartsWith("sk")) pvType = PvType.PVTKEY;
                else throw new System.NotImplementedException();
                PvGlobalVar pvGlobalVar = new PvGlobalVar(pvType, s, true);
                globalDec.Statements.Add(new PvGlobalVarDeclaration(pvGlobalVar));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 例化生成

            PvInstantiation pvInstantiation = new PvInstantiation();

            #endregion

            #region 填充并返回ProVerif项目模型

            PvProject pvProject = new PvProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<PvProcess>(),
                Queries = new List<PvQuery>(),
                Instantiation = pvInstantiation
            };

            #endregion

            return pvProject;
        }

        /// <summary>
        /// 从属性实例生成全局变量，并加入到表中
        /// </summary>
        /// <param name="instance">属性实例</param>
        /// <param name="table">变量名表</param>
        public static void GenGlobalVarFromInstance(Instance instance, HashSet<string> table)
        {
            if (instance is ValueInstance)
            {
                ValueInstance valueInstance = (ValueInstance)instance;
                if (valueInstance.Type == Type.TYPE_INT)
                    table.Add($"i{valueInstance.Value}");
                else if (valueInstance.Type == Type.TYPE_BOOL)
                    table.Add($"b{valueInstance.Value}");
                else if (valueInstance.Type == Type.TYPE_MSG) // to bitstring
                    table.Add($"m{valueInstance.Value}");
                else if (valueInstance.Type == Type.TYPE_KEY)
                    table.Add($"k{valueInstance.Value}");
                else if (valueInstance.Type == Type.TYPE_PUB_KEY)
                    table.Add($"pk{valueInstance.Value}");
                else if (valueInstance.Type == Type.TYPE_PVT_KEY)
                    table.Add($"sk{valueInstance.Value}");
                else
                    throw new System.NotImplementedException();
            }
            else if (instance is ArrayInstance)
            {
                ArrayInstance arrayInstance = (ArrayInstance)instance;
                foreach (Instance inst in arrayInstance.ArrayItems)
                {
                    GenGlobalVarFromInstance(inst, table);
                }
            }
            else // ReferenceInstance
            {
                ReferenceInstance referenceInstance = (ReferenceInstance)instance;
                foreach (Instance inst in referenceInstance.Properties)
                {
                    GenGlobalVarFromInstance(inst, table);
                }
            }
        }
    }
}
