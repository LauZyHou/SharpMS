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
            globalDec.Statements.Add(new PvCommentForDec("Function Declaration"));
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
            globalDec.Statements.Add(new PvCommentForDec("Global Var Declaration"));
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
            // 每个进程模板对应的状态机中变量名到类型的映射
            Dictionary<Proc, Dictionary<string, TypeWithCrypto>> varTypeMap = IdentifierMapper.MapPGVarToType();
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
                    // 递归展平属性，得到参数表、构成表，同时更新varTypeMap中此proc的部分
                    List<PvParam> paramList = new List<PvParam>();
                    List<PvLetStmt> constList = new List<PvLetStmt>();
                    FlattenAttribute(attr, paramList, typeMap, "", constList, varTypeMap[proc]);
                    // 遍历参数表，加到当前的PvProcess的参数表中
                    foreach (PvParam pvParam in paramList)
                    {
                        pvProcess.Params.Add(pvParam);
                    }
                    // 只要构成表非空，就说明有对参数的flatten操作
                    if (constList.Count != 0)
                    {
                        pvProcess.RootStmt.SubStmts?.Add(new PvCommentForAct("Param Flatten"));
                        // 反向遍历构成表，加到当前的PvProcess的语句中
                        for (int i = constList.Count - 1; i >= 0; i--)
                        {
                            pvProcess.RootStmt.SubStmts?.Add(constList[i]);
                        }
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
                        allPath,
                        varTypeMap[proc],
                        typeMap
                    );
                }
                // 加入allPath表中
                allPathMap.Add(proc, allPath);

                #endregion
            }

            #endregion

            #region 例化生成

            // 【ProcInst -> Port -> ChanInst】
            Dictionary<ProcInst, Dictionary<Port, string>> portChanMap = new Dictionary<ProcInst, Dictionary<Port, string>>();
            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                if (dragDrop_VM is ProcEnvInst_CT_VM)
                {
                    ProcEnvInst_CT_VM procEnvInst_CT_VM = (ProcEnvInst_CT_VM)dragDrop_VM;
                    ProcInst procInst = procEnvInst_CT_VM.ProcEnvInst.ProcInst;
                    if (!portChanMap.ContainsKey(procInst))
                    {
                        portChanMap.Add(procInst, new Dictionary<Port, string>());
                    }
                    EnvInst envInst = procEnvInst_CT_VM.ProcEnvInst.EnvInst;
                    foreach (PortChanInst portChanInst in procEnvInst_CT_VM.ProcEnvInst.PortChanInsts)
                    {
                        Port? port = portChanInst.Port;
                        Debug.Assert(port is not null);
                        Channel? chan = portChanInst.Chan;
                        Debug.Assert(chan is not null);
                        portChanMap[procInst].Add(port, $"E{envInst.Id}_{chan.Identifier}");
                    }
                }
            }
            // 并行语句容器
            PvConcurrency pvConcurrency = new PvConcurrency();
            // 【ProcInst -生成-> 实参表 + 实信道表】
            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                if (dragDrop_VM is ProcInst_VM)
                {
                    ProcInst_VM procInst_VM = (ProcInst_VM)dragDrop_VM;
                    ProcInst procInst = procInst_VM.ProcInst;
                    Debug.Assert(procInst.Proc is not null);
                    Proc proc = procInst.Proc;
                    // 找到相应的Pv进程模板并生成相应的Pv进程实例
                    PvProcess pvProcess = procMap[proc];
                    PvProcInst pvProcInst = new PvProcInst(pvProcess);
                    // Proc的每个属性参数是否是公有的
                    List<bool> propPubList = new List<bool>();
                    foreach (VisAttr visAttr in proc.Attributes)
                    {
                        propPubList.Add(visAttr.Pub);
                    }
                    // 例化的最外层实例数量一定和相应模板的属性参数的数量一致
                    int len = propPubList.Count;
                    Debug.Assert(len == procInst.Properties.Count);
                    // 遍历当前原生进程实例的实参表，填充Pv进程的实参
                    for (int i = 0; i < len; i ++ )
                    {
                        Instance inst = procInst.Properties[i];
                        List<string> glbVarUsed = new List<string>();
                        FlattenInstanceValue(inst, glbVarUsed, propPubList[i]);
                        foreach (string varName in glbVarUsed)
                        {
                            pvProcInst.Params.Add(varName);
                        }
                    }
                    // 遍历模板中的端口声明，借助端口-信道实例映射构建相应的信道实参
                    Dictionary<Port, string> curPortChan = portChanMap[procInst];
                    foreach (Port port in proc.Ports)
                    {
                        string chanInst = curPortChan[port];
                        pvProcInst.Params.Add(chanInst);
                    }
                    // 将构建好的进程实例加入到例化容器中
                    pvConcurrency.ProcInsts.Add(pvProcInst);
                }
            }
            // 例化容器
            PvInstantiation pvInstantiation = new PvInstantiation();
            pvInstantiation.RootStmt.SubStmts?.Add(pvConcurrency);

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
                    // 注意最后总要加个PvPass，否则若是空的，或是let结尾的这种就有语法错误
                    usePvProc.RootStmt.SubStmts?.Add(PvPass.Me);
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
        /// <param name="varTypeMap">变量名到类型的映射表</param>
        public static void FlattenAttribute(
            Attribute attribute,
            List<PvParam> returnList,
            Dictionary<Type, PvType> typeMap,
            string prefix,
            List<PvLetStmt> consList,
            Dictionary<string, TypeWithCrypto> varTypeMap)
        {
            // 更新Prefix（到叶子时即为变量名）
            string newPrefix = attribute.Identifier;
            if (!string.IsNullOrEmpty(prefix))
                newPrefix = $"{prefix}_{newPrefix}";
            // 更新varTypeMap（放在递归结束时也可以，不用Add是因为有一些可能已经加到这个表里过）
            varTypeMap[newPrefix] = BuildTypeWithCryptoFromAttr(attribute);
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
                    FlattenAttribute(attr, returnList, typeMap, newPrefix, consList, varTypeMap);
                }
            }
        }

        /// <summary>
        /// 展平属性实例中的数值为Pv全局常元
        /// </summary>
        /// <param name="instance">属性实例</param>
        /// <param name="returnList">返回的全局常元名称表</param>
        /// <param name="isPub">是否在模板中定义为公开的参数</param>
        public static void FlattenInstanceValue(
            Instance instance,
            List<string> returnList,
            bool isPub)
        {
            if (instance is ValueInstance) // 值类型
            {
                ValueInstance valueInstance = (ValueInstance)instance;
                string prefix = "m";
                Type type = valueInstance.Type;
                if (type.IsBase)
                {
                    if (type == Type.TYPE_INT)
                    {
                        prefix = "i";
                    }
                    else if (type == Type.TYPE_BOOL)
                    {
                        prefix = "b";
                    }
                    else if (type == Type.TYPE_KEY)
                    {
                        prefix = "k";
                    }
                    else if (type == Type.TYPE_PUB_KEY)
                    {
                        prefix = "pk";
                    }
                    else if (type == Type.TYPE_PVT_KEY)
                    {
                        prefix = "sk";
                    }
                }
                returnList.Add($"{prefix}{valueInstance.Value}{(isPub ? "_pub" : "")}");
            }
            else if (instance is ArrayInstance) // 数组类型
            {
                // todo fixme
            }
            else // 引用类型
            {
                ReferenceInstance referenceInstance = (ReferenceInstance)instance;
                foreach (Instance subInst in referenceInstance.Properties)
                {
                    FlattenInstanceValue(subInst, returnList, isPub);
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
        /// <param name="varTypeMap">变量到类型的映射表</param>
        /// <param name="typeMap">原生类型到Pv类型的映射表</param>
        public static void DfsFindActivePath(
            Anchor_VM curAnchor,
            HashSet<DragDrop_VM> histLoc,
            List<PvActiveStmt> curPath,
            List<List<PvActiveStmt>> allPath,
            Dictionary<string, TypeWithCrypto> varTypeMap,
            Dictionary<Type, PvType> typeMap)
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
                        curPath.Add(new PvCommentForAct($"Send to [{sendChan.Name}]"));
                        curPath.Add(new PvSendMsg(sendChan, varList));
                    }
                    else
                    {
                        PvChannel recvChan = new PvChannel(portName);
                        List<PvParam> paramList = new List<PvParam>() { new PvParam(PvType.BITSTRING, varName) };
                        curPath.Add(new PvCommentForAct($"Receive from [{recvChan.Name}]"));
                        curPath.Add(new PvRecvMsg(recvChan, paramList));
                    }
                }
                // 赋值语句
                else if (action.Content.Contains("="))
                {
                    string[] lhAndRhPair = action.Content.Split("=");
                    Debug.Assert(lhAndRhPair.Length == 2);
                    // 左值变量名
                    string lh = lhAndRhPair[0].Trim().Split(":")[0].Trim();
                    // 左值如果有":"，需要取一下类型名
                    string? lhTypeName = null;
                    if (lhAndRhPair[0].Trim().Contains(":"))
                        lhTypeName = lhAndRhPair[0].Trim().Split(":")[1].Trim();
                    // 右值表达式
                    string rh = lhAndRhPair[1].Trim();
                    // 构造型操作步
                    if (rh.Contains("&"))
                    {
                        // 取出&后的类型名
                        string typeName = rh.Split("&")[1].Trim().Split("{")[0].Trim();
                        // 该变量的原生类型
                        TypeWithCrypto twc = varTypeMap[lh];
                        // 取出实参列表，每个flatten一下，然后把'.'换成'_'
                        string[] paramList = rh.Split("{")[1].Trim().Split("}")[0].Split(",");
                        int len = paramList.Length;
                        Debug.Assert(len == twc.Type.Attributes.Count);
                        for (int i = 0; i < len; i++)
                        {
                            paramList[i] = paramList[i].Trim();
                            FlattenDotVar_OneShot(paramList[i], typeMap, varTypeMap, curPath);
                            paramList[i] = paramList[i].Replace(".", "_");
                        }
                        // 生成Let语句
                        PvType pvType = PvType.BITSTRING;
                        if (typeMap.ContainsKey(twc.Type)) // 构造型无法构造加密类型，所以不用判断是不是加密类型
                        {
                            pvType = typeMap[twc.Type];
                        }
                        string letLH = lh;
                        if (lhTypeName is not null) // 首次声明
                        {
                            letLH = $"{lh}: {pvType.Name}";
                            curPath.Add(new PvCommentForAct($"Construct [{lh}]"));
                        }
                        else
                        {
                            curPath.Add(new PvCommentForAct($"Reconstruct [{lh}]"));
                        }
                        string letRH = $"({string.Join(", ", paramList)})";
                        curPath.Add(new PvLetStmt(letLH, letRH));
                    }
                    // 对称加密
                    else if (rh.Contains("SymEnc") ||
                        rh.Contains("AsymEnc") ||
                        rh.Contains("SymDec") ||
                        rh.Contains("AsymDec"))
                    {
                        // 生成Let语句
                        // 左值就是这个变量
                        string letLH = lh;
                        // 该变量的原生类型
                        TypeWithCrypto twc = varTypeMap[lh];
                        if (lhTypeName is not null) // 首次声明
                        {
                            letLH = $"{lh}: bitstring";
                            if (twc.Crypto == Crypto.None && typeMap.ContainsKey(twc.Type)) // 原生且有映射的类型
                            {
                                letLH = $"{lh}: {typeMap[twc.Type].Name}";
                            }
                            curPath.Add(new PvCommentForAct($"New var [{lh}]"));
                        }
                        else
                        {
                            curPath.Add(new PvCommentForAct($"Set var [{lh}]"));
                        }
                        // 取出实参列表，每个flatten一下
                        string[] paramList = rh.Split("(")[1].Trim().Split(")")[0].Split(",");
                        int len = paramList.Length;
                        Debug.Assert(len == twc.Type.Attributes.Count);
                        for (int i = 0; i < len; i++)
                        {
                            paramList[i] = paramList[i].Trim();
                            FlattenDotVar_OneShot(paramList[i], typeMap, varTypeMap, curPath);
                        }
                        // 然后整个右值'.'换成'_'即可
                        string letRH = rh.Replace(".", "_");
                        curPath.Add(new PvLetStmt(letLH, letRH));
                    }
                    // 普通赋值
                    else
                    {
                        // 要生成Let语句

                        // 右值先flatten一下
                        FlattenDotVar_OneShot(rh, typeMap, varTypeMap, curPath);
                        // '.'换成'_'做下Let赋值
                        string letRH = rh.Replace(".", "_");

                        // 左值
                        string letLH = lh;
                        // 该变量的原生类型
                        TypeWithCrypto twc = varTypeMap[lh];
                        if (lhTypeName is not null) // 首次声明
                        {
                            letLH = $"{lh}: bitstring";
                            if (twc.Crypto == Crypto.None && typeMap.ContainsKey(twc.Type)) // 原生且有映射的类型
                            {
                                letLH = $"{lh}: {typeMap[twc.Type].Name}";
                            }
                            curPath.Add(new PvCommentForAct($"New var [{lh}]"));
                        }
                        else
                        {
                            curPath.Add(new PvCommentForAct($"Set var [{lh}]"));
                        }

                        curPath.Add(new PvLetStmt(letLH, letRH));
                    }
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
                    // varTypeMap同理，由于不能让递归后的变量定义状态影响到当前层，所以也要加拷贝
                    DfsFindActivePath(
                        anchor_VM,
                        histLoc,
                        new List<PvActiveStmt>(curPath),
                        allPath,
                        new Dictionary<string, TypeWithCrypto>(varTypeMap),
                        typeMap
                    );
                    pathOver = false;
                }
            }

            // 若链路已经结束，浅拷贝curPath加入到allPath
            if (pathOver)
            {
                allPath.Add(new List<PvActiveStmt>(curPath));
            }

            //
            // DFS状态恢复
            //
            // 对端状态移出搜索过的集合
            histLoc.Remove(hostStateVM);
        }

        /// <summary>
        /// 从Attribute构建相应的TypeWithCrypto
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static TypeWithCrypto BuildTypeWithCryptoFromAttr(Attribute attribute)
        {
            Crypto crypto = Crypto.None;
            if (attribute.IsEncrypted)
            {
                crypto = Crypto.Sym;
                if (attribute.IsAsymmetric)
                {
                    crypto = Crypto.Asym;
                }
            }
            return new TypeWithCrypto(attribute.Type, crypto);
        }

        /// <summary>
        /// 对x.y式的变量，展平一级
        /// </summary>
        /// <param name="dotVar">x.y式变量</param>
        /// <param name="typeMap">类型映射表</param>
        /// <param name="varTypeMap">已定义的临时变量表</param>
        /// <param name="curPath">当前活动语句表</param>
        public static void FlattenDotVar_OneShot(
            string dotVar,
            Dictionary<Type, PvType> typeMap,
            Dictionary<string, TypeWithCrypto> varTypeMap,
            List<PvActiveStmt> curPath)
        {
            // 不是dot var直接结束
            if (!dotVar.Contains(".")) return;
            // 当前dot var变成pv识别的形式
            string pvDotVar = dotVar.Replace(".", "_");
            // 检查一下如果有过就不用做flatten了
            if (varTypeMap.ContainsKey(pvDotVar)) return;
            // 目前只允许索引一级dot，所以直接取第一块就是root var
            // fixme 后续做多级的flatten工作，需要改一下
            string rootVar = dotVar.Split(".")[0];
            // 如果root var都没def过，直接报错
            if (!varTypeMap.ContainsKey(rootVar)) throw new System.NotImplementedException();
            // 接下来要对root var做一下flatten（仅flatten一级）
            // 对应原生数据类型
            Type type = varTypeMap[rootVar].Type;
            // 左值参数容器（Let解构）
            List<PvParam> lhList = new List<PvParam>();
            // 遍历下一级的每个属性得到左值表
            foreach (Attribute attr in type.Attributes)
            {
                // 加密的一定当bitstring处理
                PvType pvType = PvType.BITSTRING;
                // 没加密的可以映射一下
                if (!attr.IsEncrypted)
                    pvType = ToPvType(attr.Type, typeMap);
                // 构造左值参数
                PvParam pvParam = new PvParam(pvType, $"{rootVar}_{attr.Identifier}");
                // 加入左值参数表里
                lhList.Add(pvParam);
                // 同时还要补充一下varTypeMap
                varTypeMap.Add(pvParam.Name, BuildTypeWithCryptoFromAttr(attr));
            }
            // 构造相应的语句加到path里
            curPath.Add(new PvCommentForAct($"Flatten [{rootVar}]"));
            curPath.Add(new PvLetStmt($"({string.Join(", ", lhList)})", rootVar));
        }

        /// <summary>
        /// 原生类型转ProVerif类型
        /// </summary>
        /// <param name="type">原生类型</param>
        /// <param name="typeMap">映射表</param>
        /// <returns></returns>
        public static PvType ToPvType(Type type, Dictionary<Type, PvType> typeMap)
        {
            if (typeMap.ContainsKey(type)) return typeMap[type];
            return PvType.BITSTRING;
        }
    }
}
