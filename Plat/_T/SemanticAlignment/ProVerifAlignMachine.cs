﻿

using Plat._C;
using Plat._M;
using Plat._VM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Plat._T
{
    public class ProVerifAlignMachine
    {
        /// <summary>
        /// ProVerif语义对齐机运行
        /// </summary>
        /// <returns>构造的ProVerif内存模型（多path）</returns>
        public static List<PvProject> Run()
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

            #region 全局常元映射
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
                // 私有版
                PvGlobalVar pvGlobalVar = new PvGlobalVar(pvType, s, true);
                globalDec.Statements.Add(new PvGlobalVarDeclaration(pvGlobalVar));
                // 公有版
                pvGlobalVar = new PvGlobalVar(pvType, $"{s}_pub", false);
                globalDec.Statements.Add(new PvGlobalVarDeclaration(pvGlobalVar));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            #endregion

            #region 进程模板+进程图状态机映射

            // 【Proc -> PvProcess】这里有进程声明，以及解构声明flatten出来的活动语句
            Dictionary<Proc, PvProcess> procMap = new Dictionary<Proc, PvProcess>();
            // 【Proc -> List<List<PvActiveStmt>>】这里是每个进程的所有执行Path
            Dictionary<Proc, List<List<PvActiveStmt>>> allPathMap = new Dictionary<Proc, List<List<PvActiveStmt>>>();
            // 遍历所有的进程模板及对应的进程图状态机生成声明
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.procGraph_P_VMs)
            {
                #region 进程模板映射

                // 对应的Proc
                Proc proc = procGraph_P_VM.ProcGraph.Proc;
                // 生成对应的PvProcess
                PvProcess pvProcess = new PvProcess(proc.Identifier);
                // 加到表里
                procMap.Add(proc, pvProcess);
                // 参数表 1 -- 属性参数
                foreach (VisAttr attr in proc.Attributes)
                {
                    // 递归展平属性，得到参数表、构成表
                    List<PvParam> paramList = new List<PvParam>();
                    List<PvLetStmt> constList = new List<PvLetStmt>();
                    FlattenAttribute(attr, paramList, typeMap, "", constList);
                    // 遍历参数表，加到当前的PvProcess的参数表中
                    foreach (PvParam pvParam in paramList)
                    {
                        pvProcess.Params.Add(pvParam);
                    }
                    // 反向遍历构成表，加到当前的PvProcess的语句中
                    for (int i = constList.Count - 1; i >= 0; i -- )
                    {
                        pvProcess.RootStmt.SubStmts?.Add(constList[i]);
                    }
                }
                // 参数表 2 -- 端口参数（建立相应的临时信道变量
                // todo 需要检查所有声明的端口都在Inst里映射过才行
                foreach (Port port in proc.Ports)
                {
                    pvProcess.Params.Add(new PvParam(PvType.CHANNEL, port.Identifier));
                }

                #endregion

                #region 进程图状态机映射

                // 先找到初始状态
                InitState_VM? initState_VM = null;
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is InitState_VM)
                    {
                        initState_VM = dragDrop_VM as InitState_VM;
                        break;
                    }
                }
                if (initState_VM is null) throw new System.NotImplementedException();
                // 接下来要从初始状态出发获取所有的DFS Path
                List<List<PvActiveStmt>> allPath = new List<List<PvActiveStmt>>();
                // 遍历初始状态的所有锚点，以构造所有的执行路径
                foreach (Anchor_VM startAnchor in initState_VM.Anchor_VMs)
                {
                    DfsFindActivePath(
                        startAnchor,
                        new HashSet<DragDrop_VM>(){ initState_VM },
                        new List<PvActiveStmt>(),
                        allPath
                    );
                }
                // 加入allPath表中
                allPathMap.Add(proc, allPath);

                #endregion
            }

            #endregion

            #region 例化生成

            PvInstantiation pvInstantiation = new PvInstantiation();

            #endregion

            #region 遍历所有的进程图的path组合，形成多个ProVerif项目

            // 待返回的模型列表
            List<PvProject> resList = new List<PvProject>();

            // 生成所有的下标表
            int processNum = ResourceManager.procs.Count;
            // 先构造n个0，拷贝加到下标总表里
            List<List<int>> indexList = new List<List<int>>(); // 下标总表
            List<int> index = new List<int>(); // 变换用的表
            for (int i = 0; i < processNum; i++) index.Add(0);
            indexList.Add(new List<int>(index));
            // 计算一下一共要变换多少次
            int count = 1;
            foreach (Proc proc in ResourceManager.procs)
            {
                count *= allPathMap[proc].Count;
            }
            // 减掉一次全0的初始态，剩下的就是变换次数
            count--;
            // 模拟每次变换
            while (count-- != 0)
            {
                int c = 1; // 来自上一位的进位值，这里只可能是1或者0
                // 遍历每个位置
                for (int i = 0; i < processNum; i ++ )
                {
                    Proc proc = ResourceManager.procs[i]; // 对应进程模板
                    int mod = allPathMap[proc].Count; // 模数（path数）
                    index[i] += c; // 先加上来自上一位的进位值
                    c = index[i] / mod; // 向下一位进位值
                    index[i] %= mod; // 当前位的变换值
                }
                // 变换完成后，拷贝加到下标总表中
                indexList.Add(index);
            }

            // 至此，下标总表构建完成，借其中的每个模式（下标List）构建所有的PvProject
            foreach (List<int> pattern in indexList)
            {
                PvProject pvProject = new PvProject()
                {
                    GlobalDeclaration = globalDec,
                    Processes = new List<PvProcess>(),
                    Queries = new List<PvQuery>(),
                    Instantiation = pvInstantiation
                };
                // 遍历模式中每一位取用的path号，模式一定是processNum这么长的int表
                for (int i = 0; i < processNum; i ++ )
                {
                    Proc proc = ResourceManager.procs[i]; // 对应进程模板
                    int pathId = pattern[i]; // path号
                    PvProcess protoPvProc = procMap[proc]; // 原型PvProcess，还没加path
                    List<PvActiveStmt> path = allPathMap[proc][pathId]; // path上的所有活动语句
                    PvProcess usePvProc = protoPvProc.SuitableCopy(); // 使用原型的拷贝版（防止互影响）
                    // 把这条path上所有活动语句加进来
                    foreach (PvActiveStmt pvActiveStmt in path)
                    {
                        usePvProc.RootStmt.SubStmts?.Add(pvActiveStmt);
                    }
                    // 构造好的PvProcess加到当前PvProject里
                    pvProject.Processes.Add(usePvProc);
                }
                // 构造好的PvProject加到总列表里
                resList.Add(pvProject);
            }

            #endregion

            return resList;
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
                string varName;
                if (valueInstance.Type == Type.TYPE_INT)
                    varName = $"i{valueInstance.Value}";
                else if (valueInstance.Type == Type.TYPE_BOOL)
                    varName = $"b{valueInstance.Value}";
                else if (valueInstance.Type == Type.TYPE_MSG) // to bitstring
                    varName = $"m{valueInstance.Value}";
                else if (valueInstance.Type == Type.TYPE_KEY)
                    varName = $"k{valueInstance.Value}";
                else if (valueInstance.Type == Type.TYPE_PUB_KEY)
                    varName = $"pk{valueInstance.Value}";
                else if (valueInstance.Type == Type.TYPE_PVT_KEY)
                    varName = $"sk{valueInstance.Value}";
                else
                    throw new System.NotImplementedException();
                table.Add(varName);
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

        /// <summary>
        /// 摊平Attribute，返回摊平后的ProVerif参数表
        /// </summary>
        /// <param name="attribute">要摊平的属性</param>
        /// <param name="returnList">要返回的参数表</param>
        /// <param name="typeMap">类型映射表</param>
        /// <param name="prefix">递归时的参数名前缀</param>
        /// <param name="consList">构成表</param>
        public static void FlattenAttribute(
            Attribute attribute,
            List<PvParam> returnList,
            Dictionary<Type, PvType> typeMap,
            string prefix,
            List<PvLetStmt> consList)
        {
            // 更新Prefix
            string newPrefix = attribute.Identifier;
            if (!string.IsNullOrEmpty(prefix))
                newPrefix = $"{prefix}_{newPrefix}";
            // 按属性类型区分
            if (attribute.IsArray) // 数组类型
            {
                // todo Array
            }
            else if (attribute.Type.IsBase) // 值类型
            {
                PvParam pvParam = new PvParam(typeMap[attribute.Type], newPrefix);
                returnList.Add(pvParam);
            }
            else // 引用类型
            {
                // 在进入递归前要先生成对应的Let语句构成
                List<string> subVarNameList = new List<string>();
                foreach (Attribute attr in attribute.Type.Attributes)
                {
                    subVarNameList.Add(newPrefix + "_" + attr.Identifier);
                }
                PvLetStmt pvLetStmt = new PvLetStmt($"{newPrefix}: bitstring", $"({string.Join(", ", subVarNameList)})");
                consList.Add(pvLetStmt);
                // 遍历参数表中的每个下级属性做递归处理
                foreach (Attribute attr in attribute.Type.Attributes)
                {
                    FlattenAttribute(attr, returnList, typeMap, newPrefix, consList);
                }
            }
        }

        /// <summary>
        /// 深搜状态机，获取所有的活动路径
        /// </summary>
        /// <param name="curAnchor">当前搜索的出发锚点（只考虑Source锚点）</param>
        /// <param name="histLoc">已经搜过的历史状态</param>
        /// <param name="curPath">当前正在构造的路径</param>
        /// <param name="allPath">所有路径，每条构造好后加入其中</param>

        public static void DfsFindActivePath(
            Anchor_VM curAnchor,
            HashSet<DragDrop_VM> histLoc,
            List<PvActiveStmt> curPath,
            List<List<PvActiveStmt>> allPath)
        {
            // 检查当前是Source锚点
            Linker_VM? linker_VM = curAnchor.LinkerVM;
            if (linker_VM is null || linker_VM.Dest == curAnchor) return;

            // 找对端锚点上的宿主状态
            DragDrop_VM hostStateVM = linker_VM.Dest.HostVM;
            // 如果是已经搜索过的就跳过
            if (histLoc.Contains(hostStateVM)) return;

            //
            // DFS状态记录
            //
            // 对端状态加入搜索过的集合
            histLoc.Add(hostStateVM);

            // 至此，是没有搜索过的，要对这条边上的Action做处理，构建相应的活动语句
            Debug.Assert(linker_VM.ExtMsg is not null);
            TransNode_VM transNode_VM = (TransNode_VM)linker_VM.ExtMsg;
            foreach (Formula action in transNode_VM.LocTrans.Actions)
            {
                // 信道同步
                if (action.Content.Contains("->") || action.Content.Contains("<-"))
                {
                    bool isSend = true;
                    if (action.Content.Contains("<-")) isSend = false;
                    string[] varAndPortPair;
                    if (isSend)
                        varAndPortPair = action.Content.Split("->");
                    else
                        varAndPortPair = action.Content.Split("<-");
                    Debug.Assert(varAndPortPair.Length == 2);
                    string varName = varAndPortPair[0].Split(":")[0].Trim();
                    string portName = varAndPortPair[1].Trim();
                    // 根据是发送还是接收生成相应的语句
                    if (isSend)
                    {
                        PvChannel sendChan = new PvChannel(portName);
                        List<string> varList = new List<string> { varName };
                        curPath.Add(new PvSendMsg(sendChan, varList));
                    }
                    else
                    {
                        PvChannel recvChan = new PvChannel(portName);
                        List<PvParam> paramList = new List<PvParam>() { new PvParam(PvType.BITSTRING, varName) };
                        curPath.Add(new PvRecvMsg(recvChan, paramList));
                    }
                }
                // 赋值语句
                else if (action.Content.Contains("="))
                {
                    // todo
                }
            }
            
            // 这个变量通过检查到底有没有成功的下一级递归来判断链路是否结束
            bool pathOver = true;
            // 遍历其上的所有锚点，向下级搜索
            foreach (Anchor_VM anchor_VM in hostStateVM.Anchor_VMs)
            {
                if (anchor_VM.LinkerVM is not null && 
                    anchor_VM.LinkerVM.Source == anchor_VM && 
                    !histLoc.Contains(anchor_VM.LinkerVM.Dest.HostVM))
                {
                    // 注意这里往下级传的是curPath的拷贝，就不用手动退栈了
                    DfsFindActivePath(anchor_VM, histLoc, new List<PvActiveStmt>(curPath.ToArray()), allPath);
                    pathOver = false;
                }
            }

            // 若链路已经结束，浅拷贝curPath加入到allPath
            if (pathOver)
            {
                allPath.Add(new List<PvActiveStmt>(curPath.ToArray()));
            }

            //
            // DFS状态恢复
            //
            // 对端状态移出搜索过的集合
            histLoc.Remove(hostStateVM);
        }
    }
}
