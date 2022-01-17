using Plat._C;
using Plat._M;
using Plat._VM;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plat._T
{
    /// <summary>
    /// SharpMS-UPPAAL语义对齐机
    /// </summary>
    public class UppaalAlignMachine
    {
        public static UpProject? Run()
        {
            #region 对齐数据类型

            #region 数据类型表

            // 【T-UT表】SharpMS -> UPPAAL 类型映射（原生
            Dictionary<Type, UpType> typeMap = new Dictionary<Type, UpType>()
            {
                { Type.TYPE_INT, UpType.INT },
                { Type.TYPE_BOOL, UpType.BOOL },
                { Type.TYPE_MSG, UpType.MSG },
                { Type.TYPE_KEY, UpType.KEY },
                { Type.TYPE_PUB_KEY, UpType.PUBKEY },
                { Type.TYPE_PVT_KEY, UpType.PVTKEY }
            };
            // 【UT表】UPPAAL模型中需要做typedef的类型表（int和bool不需要def
            List<UpType> upTypes = new List<UpType>()
            {
                UpType.MSG, UpType.KEY, UpType.PUBKEY, UpType.PVTKEY
            };
            // 【T-UT_S表】SharpMS -> UPPAAL 类型映射（对称加密
            Dictionary<Type, UpType> typeMap_S = new Dictionary<Type, UpType>();
            // 【T-UT_A表】SharpMS -> UPPAAL 类型映射（非对称加密
            Dictionary<Type, UpType> typeMap_A = new Dictionary<Type, UpType>();
            // 【UT_S-UT/UT_S/UT_A表】UPPAAL类型名 -> UPPAAL类型引用的映射，用于后面状态机迁移的解析
            Dictionary<string, UpType> strTypeMap = new Dictionary<string, UpType>()
            {
                { "int", UpType.INT },
                { "bool", UpType.BOOL }
            };

            #endregion

            #region 填充前述的关于数据类型的映射表

            foreach (Type type in ResourceManager.types)
            {
                // int / bool 不必处理
                if (type == Type.TYPE_INT || type == Type.TYPE_BOOL) continue;
                // 非基本类型的需要补充“原生”到T-UT表、UT表、UT_S-UT/UT_S/UT_A表中
                if (!type.IsBase)
                {
                    UpType upType = new UpType(type.Identifier) { Description = type.Description };
                    typeMap.Add(type, upType);
                    upTypes.Add(upType);
                    strTypeMap.Add(upType.Name, upType);
                }
                // 继承自Msg的即可“对称加密”和“非对称加密”，补充到T-UT_S和T-UT_A表中
                // 这个过程中因为原生类型已经在表中，所以可以直接补充Props
                if (type.InheritFrom(Type.TYPE_MSG))
                {
                    UpType upType_S = UpType.GenEncryptedType(typeMap[type], false);
                    typeMap_S.Add(type, upType_S);
                    upTypes.Add(upType_S);
                    strTypeMap.Add(upType_S.Name, upType_S);

                    UpType upType_A = UpType.GenEncryptedType(typeMap[type], true);
                    typeMap_A.Add(type, upType_A);
                    upTypes.Add(upType_A);
                    strTypeMap.Add(upType_A.Name, upType_A);
                }
            }

            #endregion

            #region 填表后，可以保证所有Type都能找到，再遍历一次，构造所有原生strcut的Props

            foreach (Type type in ResourceManager.types)
            {
                if (type.IsBase) continue;
                UpType upType = typeMap[type];
                // 构造从祖先到自己的例化关系表
                List<Type> parentChain = new List<Type>();
                Type? cur = type;
                do
                {
                    parentChain.Add(cur);
                    cur = cur.Parent;
                } while (cur is not null);
                parentChain.Reverse();
                // 从祖先到自己的所有Props
                foreach (Type t in parentChain)
                {
                    foreach (Attribute attr in t.Attributes)
                    {
                        // 构造UpParam
                        UpParam p;
                        if (attr.IsEncrypted)
                        {
                            if (attr.IsAsymmetric)
                            {
                                p = new UpParam(typeMap_A[attr.Type], attr.Identifier);
                            }
                            else
                            {
                                p = new UpParam(typeMap_S[attr.Type], attr.Identifier);
                            }
                        }
                        else
                        {
                            p = new UpParam(typeMap[attr.Type], attr.Identifier);
                        }
                        upType.Props.Add(p);
                    }
                }
            }

            #endregion

            #endregion

            #region 遍历UT表生成所有的typedef

            UpDeclaration globalDec = new UpDeclaration();
            globalDec.Statements.Add(UpPass.Me);
            globalDec.Statements.Add(new UpComment($"{splitLine} TypeDef {splitLine}"));
            foreach (UpType upType in upTypes)
            {
                globalDec.Statements.Add(UpPass.Me);
                globalDec.Statements.Add(new UpTypeDef(upType));
            }

            #endregion

            #region 遍历进程模板，生成对应的参数表List<UpParam>

            Dictionary<Proc, List<UpParam>> procToUpParamListMap = new Dictionary<Proc, List<UpParam>>();
            foreach (Proc proc in ResourceManager.procs)
            {
                List<UpParam> upParams = new List<UpParam>();
                // 获取祖先进程到当前proc的泛化表
                List<Proc> parentChain = new List<Proc>();
                Proc? cur = proc;
                do
                {
                    parentChain.Add(cur);
                    cur = cur.Parent;
                } while (cur is not null);
                parentChain.Reverse();
                // 从祖先进程开始所有参数都搞进来
                foreach (Proc p in parentChain)
                {
                    foreach (VisAttr visAttr in p.Attributes)
                    {
                        UpParam upParam;
                        if (visAttr.Type.IsBase)
                        {
                            upParam = new UpParam(typeMap[visAttr.Type], visAttr.Identifier, false);
                        }
                        else
                        {
                            upParam = new UpParam(typeMap[visAttr.Type], visAttr.Identifier, true);
                        }
                        upParams.Add(upParam);
                    }
                }
                procToUpParamListMap.Add(proc, upParams);
            }

            #endregion

            #region 遍历进程图状态机，生成对应的本地变量声明UpDeclaration

            //
            // 即寻找形如 var: Type = xxxx 的动作语句，构造本地变量声明Type var;
            //
            // 【P-LOCDEC表】从SharpMS进程模板映射到UPPAAL的本地变量表
            Dictionary<Proc, UpDeclaration> procToUpDecMap = new Dictionary<Proc, UpDeclaration>();
            // 【P-PGPVM表】从SharpMS进程模板映射到其进程图状态机的表
            Dictionary<Proc, ProcGraph_P_VM> procToPGPVMMap = new Dictionary<Proc, ProcGraph_P_VM>();
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.mainWindow_VM.ProcGraph_PG_VM.ProcGraph_P_VMs)
            {
                Proc proc = procGraph_P_VM.ProcGraph.Proc;
                // 加到P-PGPVM表中
                procToPGPVMMap.Add(proc, procGraph_P_VM);
                UpDeclaration localDec = new UpDeclaration(true);
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is TransNode_VM)
                    {
                        TransNode_VM transNode_VM = (TransNode_VM)dragDrop_VM;
                        foreach (Formula action in transNode_VM.LocTrans.Actions)
                        {
                            // 只处理赋值和从端口接收的语句
                            if (!action.Content.Contains("=") && !action.Content.Contains("<-")) continue;
                            string lh;
                            if (action.Content.Contains("="))
                            {
                                lh = action.Content.Split('=')[0].Trim();
                            }
                            else
                            {
                                lh = action.Content.Split("<-")[0].Trim();
                            }
                            if (!lh.Contains(":")) continue;
                            string[] nameAndType = lh.Split(':');
                            if (nameAndType.Length != 2)
                            {
                                ResourceManager.UpdateTip($"Syntax error at action [{action.Content}] in proc graph [{proc.Identifier}]!");
                                return null;
                            }
                            string varName = nameAndType[0].Trim();
                            string typeName = IdentifierMapper.MapToUpTypeName(nameAndType[1].Trim());
                            if (string.IsNullOrEmpty(varName))
                            {
                                ResourceManager.UpdateTip($"Empty var name at action [{action.Content}] in proc graph [{proc.Identifier}]!");
                                return null;
                            }
                            if (!strTypeMap.ContainsKey(typeName))
                            {
                                ResourceManager.UpdateTip($"Incorrect type name at action [{action.Content}] in proc graph [{proc.Identifier}]!");
                                return null;
                            }
                            UpType upType = strTypeMap[typeName];
                            localDec.Statements.Add(new UpNewVar(upType, varName));
                        }
                    }
                }
                procToUpDecMap.Add(proc, localDec);
            }

            #endregion

            #region 生成所有的信道同步定义

            //
            // 解析拓扑图中的Port-Chan
            // 获取每个拓扑结点的例化进程，每个例化边上的端口-信道实例映射
            // 后期将为每个拓扑结点生成单独的进程模板
            //
            globalDec.Statements.Add(UpPass.Me);
            globalDec.Statements.Add(new UpComment($"{splitLine} ChannelSync {splitLine}"));
            // 【PI-PORT-CI表】ProcInst -> Port -> ChanInst 的映射
            // 其中ChanInst 由 Channel.Identifier 组合 EnvInst的Id形成
            Dictionary<ProcInst, Dictionary<Port, string>> procInstToPortToChanInstMap = new Dictionary<ProcInst, Dictionary<Port, string>>();
            // 【PI表】所有需要转换的ProcInst（没连线也就没ProcEnvInst_CT_VM，也就不用考虑）
            HashSet<ProcInst> procInstList = new HashSet<ProcInst>();
            // 【P-PORT_S-PORT表】每个进程模板的端口名到端口引用的映射
            Dictionary<Proc, Dictionary<string, Port>> portNameToPortMap = new Dictionary<Proc, Dictionary<string, Port>>();
            // 信道生成去重表
            HashSet<string> chanPairSet = new HashSet<string>();
            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                if (dragDrop_VM is ProcEnvInst_CT_VM)
                {
                    ProcEnvInst_CT_VM procEnvInst_CT_VM = (ProcEnvInst_CT_VM)dragDrop_VM;
                    ProcEnvInst procEnvInst = procEnvInst_CT_VM.ProcEnvInst;
                    ProcInst procInst = procEnvInst.ProcInst;
                    EnvInst envInst = procEnvInst.EnvInst;
                    if (procInst.Proc is null || envInst.Env is null) continue;
                    // 加入到PI表
                    procInstList.Add(procInst);
                    foreach (PortChanInst portChanInst in procEnvInst.PortChanInsts)
                    {
                        if (portChanInst.Port is null || portChanInst.Chan is null) continue;
                        Port port = portChanInst.Port;
                        Channel chan = portChanInst.Chan;
                        // 加入P-PORT_S-PORT表中
                        if (!portNameToPortMap.ContainsKey(procInst.Proc))
                            portNameToPortMap.Add(procInst.Proc, new Dictionary<string, Port>());
                        portNameToPortMap[procInst.Proc][port.Identifier] = port;
                        // 加入PI-PORT-CI表中
                        string chanInst = $"{chan.Identifier}_E{envInst.Id}";
                        if (!procInstToPortToChanInstMap.ContainsKey(procInst))
                        {
                            procInstToPortToChanInstMap.Add(procInst, new Dictionary<Port, string>());
                        }
                        // 填充PI-PORT-CI表
                        procInstToPortToChanInstMap[procInst].Add(port, chanInst);

                        // 去重
                        if (chanPairSet.Contains(chanInst)) continue;
                        chanPairSet.Add(chanInst);
                        // 要生成信道同步定义，需要找到在这个信道实例上传递的参数类型
                        // 也就是要到EnvInst上找这个信道被例化成传递哪种类型的ChanInst
                        Type? chanType = null;
                        foreach (ChanInst ci in envInst.ChanInsts)
                        {
                            if (ci.Channel == chan)
                            {
                                chanType = ci.Type;
                                break;
                            }
                        }
                        // 如果没有类型就跳过
                        if (chanType is null) continue;
                        // 否则，就生成相应的UPPAAL chan用来同步，生成相应的meta swap变量
                        globalDec.Statements.Add(UpPass.Me);
                        globalDec.Statements.Add(new UpNewVar(UpType.CHAN, $"sync_{chanInst}"));
                        globalDec.Statements.Add(new UpNewVar(typeMap[chanType], $"swap_{chanInst}", true));
                    }
                }
                // dimiss ProcInst_VM / EnvInst_VM / Linker / ProcInst_NT_VM / EnvInst_NT_VM
            }

            #endregion

            #region 对每个需要转换的ProcInst生成对应的UPPAAL进程模板及其例化

            // 例化时的中间变量id以及例化容器
            int tmpVarId = 0;
            UpInstantiation upInstantiation = new UpInstantiation();
            // 【PI-UPPROC表】每个ProcInst到对应的UPPAAL进程模板的映射
            Dictionary<ProcInst, UpProcess> procInstToUpProcMap = new Dictionary<ProcInst, UpProcess>();
            // 【UPPROC表】所有的UPPAAL Process
            List<UpProcess> upProcList = new List<UpProcess>();
            // 【PI-STATE-UPLOC表】每个ProcInst状态机的状态VM到UpLocation的映射
            Dictionary<ProcInst, Dictionary<DragDrop_VM, UpLocation>> procInstToStateToUpLocMap = new Dictionary<ProcInst, Dictionary<DragDrop_VM, UpLocation>>();
            foreach (ProcInst procInst in procInstList)
            {
                // 取出其例化的SharpMS进程模板
                Proc? proc = procInst.Proc;
                Debug.Assert(proc is not null);
                // 转换后的UPPAAL进程模板的名称
                string upProcName = $"{proc.Identifier}_P{procInst.Id}";
                // PI-UPPROC表项容器构造
                UpPG upPG = new UpPG();
                UpProcess upProcess = new UpProcess(upProcName, procToUpDecMap[proc], upPG)
                {
                    Params = procToUpParamListMap[proc]
                };
                procInstToUpProcMap.Add(procInst, upProcess);
                // 加到UPPROC表中
                upProcList.Add(upProcess);
                // PI-STATE-UPLOC表项容器构造
                procInstToStateToUpLocMap.Add(procInst, new Dictionary<DragDrop_VM, UpLocation>());
                // 找到对应的进程图状态机
                ProcGraph_P_VM procGraph_P_VM = procToPGPVMMap[proc];

                #region 遍历状态机生成所有结点定义

                UpLocation? finalLoc = null; // 唯一的Final Location，用于去重
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is InitState_VM)
                    {
                        UpLocation upLocation = new UpLocation(null, true, true);
                        // State_VM -> UpLoc 映射关系
                        procInstToStateToUpLocMap[procInst].Add(dragDrop_VM, upLocation);
                        // UpPG中的Loc
                        upPG.Locations.Add(upLocation);
                    }
                    else if (dragDrop_VM is TinyState_VM)
                    {
                        TinyState_VM tinyState_VM = (TinyState_VM)dragDrop_VM;
                        UpLocation upLocation = new UpLocation($"S{tinyState_VM.State.Id}", true, false);
                        procInstToStateToUpLocMap[procInst].Add(dragDrop_VM, upLocation);
                        upPG.Locations.Add(upLocation);
                    }
                    else if (dragDrop_VM is FinalState_VM)
                    {
                        // 注意 Final Location 只创建一次
                        if (finalLoc is not null) continue;
                        finalLoc = new UpLocation("FIN", true, false);
                        procInstToStateToUpLocMap[procInst].Add(dragDrop_VM, finalLoc);
                        upPG.Locations.Add(finalLoc);
                    }
                }

                #endregion

                #region 遍历状态机生成的所有迁移边定义

                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is TransNode_VM)
                    {
                        TransNode_VM transNode_VM = (TransNode_VM)dragDrop_VM;
                        // 找到两边连接的State_VM
                        DragDrop_VM srcState_VM = transNode_VM.AttachedLinker.Source.HostVM;
                        DragDrop_VM destState_VM = transNode_VM.AttachedLinker.Dest.HostVM;
                        // 映射成对应的UpLoc
                        UpLocation srcLoc = procInstToStateToUpLocMap[procInst][srcState_VM];
                        UpLocation destLoc = procInstToStateToUpLocMap[procInst][destState_VM];
                        // 构造UpTrans
                        UpTransition upTransition = new UpTransition(srcLoc, destLoc);
                        upPG.Transitions.Add(upTransition);
                        // 迁移边信息
                        LocTrans locTrans = transNode_VM.LocTrans;
                        // 处理迁移边上的Guard
                        // todo
                        // 处理迁移边上的Action
                        foreach (Formula action in locTrans.Actions)
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
                                // 根据端口名可以映射到具体的chanInst名
                                Port port = portNameToPortMap[proc][portName];
                                string chanInst = procInstToPortToChanInstMap[procInst][port];
                                // 添加同步&传递语句
                                upTransition.UpSync = new UpSynchronisation(isSend, $"sync_{chanInst}");
                                if (isSend)
                                {
                                    upTransition.UpAssignments.Add(new UpAssignment($"swap_{chanInst}", varName));
                                }
                                else
                                {
                                    upTransition.UpAssignments.Add(new UpAssignment(varName, $"swap_{chanInst}"));
                                }
                            }
                            // 赋值语句
                            else if (action.Content.Contains("="))
                            {
                                string[] lhAndRhPair = action.Content.Split("=");
                                Debug.Assert(lhAndRhPair.Length == 2);
                                string lh = lhAndRhPair[0].Trim().Split(":")[0].Trim(); // var name
                                string rh = lhAndRhPair[1].Trim();
                                // 构造型操作步
                                if (rh.Contains("&"))
                                {
                                    // 取出类型名
                                    string upTypeName = rh.Split("&")[1].Trim().Split("{")[0].Trim();
                                    upTypeName = IdentifierMapper.MapToUpTypeName(upTypeName);
                                    // 找到对应的UPPAAL类型
                                    UpType upType = strTypeMap[upTypeName];
                                    // 取出实参列表
                                    string[] paramList = rh.Split("{")[1].Trim().Split("}")[0].Split(",");
                                    int len = paramList.Length;
                                    Debug.Assert(len == upType.Props.Count);
                                    for (int i = 0; i < len; i++)
                                    {
                                        paramList[i] = paramList[i].Trim();
                                    }
                                    // 构造赋值语句
                                    for (int i = 0; i < len; i++)
                                    {
                                        upTransition.UpAssignments.Add(
                                            new UpAssignment($"{lh}.{upType.Props[i].Name}", paramList[i])
                                        );
                                    }
                                }
                                // 对称加密
                                else if (rh.Contains("SymEnc") ||
                                    rh.Contains("AsymEnc") ||
                                    rh.Contains("SymDec") ||
                                    rh.Contains("AsymDec"))
                                {
                                    // 取出待加密的消息实参，和用来加密的对称密钥实参
                                    string[] paramList = rh.Split("(")[1].Trim().Split(")")[0].Split(",");
                                    int len = paramList.Length;
                                    Debug.Assert(len == 2);
                                    for (int i = 0; i < len; i++)
                                    {
                                        paramList[i] = paramList[i].Trim();
                                    }
                                    string msg = paramList[0], key = paramList[1];
                                    if (rh.Contains("SymEnc"))
                                    {
                                        // 对称加密即是构造一个_S对象写入msg和key
                                        upTransition.UpAssignments.Add(new UpAssignment($"{lh}.content", msg));
                                        upTransition.UpAssignments.Add(new UpAssignment($"{lh}.k", key));
                                    }
                                    else if (rh.Contains("AsymEnc"))
                                    {
                                        // 非对称加密即是构造一个_A对象写入msg和key
                                        upTransition.UpAssignments.Add(new UpAssignment($"{lh}.content", msg));
                                        upTransition.UpAssignments.Add(new UpAssignment($"{lh}.pk", key));
                                    }
                                    else if (rh.Contains("SymDec"))
                                    {
                                        // 对称解密即是判断密钥相等然后取出密文
                                        upTransition.UpGurad = new UpGuard(key, "==", $"{msg}.k");
                                        upTransition.UpAssignments.Add(new UpAssignment(lh, $"{msg}.content"));
                                    }
                                    else // "AsymDec"
                                    {
                                        // 非对称解密即是判断密钥匹配然后取出密文
                                        upTransition.UpGurad = new UpGuard(key, "==", $"{msg}.pk");
                                        upTransition.UpAssignments.Add(new UpAssignment(lh, $"{msg}.content"));
                                    }
                                }
                                // 普通赋值
                                else
                                {
                                    upTransition.UpAssignments.Add(new UpAssignment(lh, rh));
                                }
                            }
                        }
                    }
                }

                #endregion

                #region 解析ProcInst的例化生成相应的进程例化语句

                // 该进程例化语句容器
                UpInstProc upInstProc = new UpInstProc(upProcess.Name.ToLower(), upProcess);
                // 例化语句列表，最后要翻转
                List<UpStatement> instStmtList = new List<UpStatement> { upInstProc };
                // 引用型实例到临时变量名的映射
                Dictionary<ReferenceInstance, string> refInstToVarIdMap = new Dictionary<ReferenceInstance, string>();
                // 待解构的引用型实例队列，每次从队头取队尾插入
                Queue<ReferenceInstance> refInstQueue = new Queue<ReferenceInstance>();
                // 遍历这个ProcInst的实参列表，填充例化语句容器
                foreach (Instance inst in procInst.Properties)
                {
                    if (inst is ValueInstance)
                    {
                        ValueInstance valueInstance = (ValueInstance)inst;
                        upInstProc.Values.Add(valueInstance.Value); // 加到该进程的例化实参表
                    }
                    else if (inst is ReferenceInstance)
                    {
                        ReferenceInstance referenceInstance = (ReferenceInstance)inst;
                        string v = $"v{++tmpVarId}"; // 创建临时变量
                        refInstToVarIdMap.Add(referenceInstance, v); // 构建该引用实例到变量的映射
                        refInstQueue.Enqueue(referenceInstance); // 送入待解构队列
                        upInstProc.Values.Add(v); // 加到该进程的例化实参表
                    }
                    else // ArrayInstance
                    {
                        // todo
                    }
                }
                // 处理解构队列（拓扑序解构），直到队空表示全部解构完成
                while (refInstQueue.Count > 0)
                {
                    // 每次出队一个
                    ReferenceInstance referenceInstance = refInstQueue.Dequeue();
                    // 找到对应变量名，以及该引用的类型
                    string v = refInstToVarIdMap[referenceInstance];
                    UpType upType = typeMap[referenceInstance.Type];
                    // 遍历该引用实例的下级例化，形成其自身构造
                    UpInstType upInstType = new UpInstType(v, upType, true);
                    instStmtList.Add(upInstType); // 自身的构造语句加到语句表中
                    foreach (Instance inst in referenceInstance.Properties)
                    {
                        if (inst is ValueInstance)
                        {
                            ValueInstance valInst = (ValueInstance)inst;
                            upInstType.Values.Add(valInst.Value); // 加到该引用型变量的例化实参表
                        }
                        else if (inst is ReferenceInstance)
                        {
                            ReferenceInstance refInst = (ReferenceInstance)inst;
                            string _v = $"v{++tmpVarId}"; // 创建临时变量
                            refInstToVarIdMap.Add(refInst, _v); // 构建该引用实例到变量的映射
                            refInstQueue.Enqueue(refInst); // 送入待解构队列
                            upInstType.Values.Add(_v); // 加到该引用型变量的例化实参表
                        }
                        else // ArrayInstance
                        {
                            // todo
                        }
                    }
                }
                // 处理完，从后往前遍历所有语句加到总的例化容器中
                upInstantiation.Statements.Add(UpPass.Me);
                for (int i = instStmtList.Count - 1; i >= 0; i -- )
                {
                    upInstantiation.Statements.Add(instStmtList[i]);
                }

                #endregion
            }

            #endregion

            #region 总的例化语句

            upInstantiation.Statements.Add(UpPass.Me);
            UpConcurrency systemStmt = new UpConcurrency();
            foreach (UpProcess p in upProcList)
            {
                systemStmt.ProcessInstances.Add(p.Name.ToLower());
            }
            upInstantiation.Statements.Add(systemStmt);

            #endregion

            #region 性质的转换

            List<UpQuery> upQueryList = new List<UpQuery>();
            foreach (Property property in ResourceManager.props)
            {
                string pStr = property.Content; // 映射前
                string pTrans = ""; // 映射后
                switch (property.Prop)
                {
                    case Prop.INVAR:
                        break;
                    case Prop.CTL:
                        pTrans = MappingCTL(pStr);
                        break;
                    case Prop.SEC:
                        break;
                    case Prop.FSEC:
                        break;
                    case Prop.INTE:
                        break;
                    case Prop.IINTE:
                        break;
                    case Prop.AUTH:
                        break;
                    case Prop.IAUTH:
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(pTrans)) continue;
                upQueryList.Add(new UpQuery(pTrans, property.Description));
            }
            
            #endregion

            #region 填充并返回UPPAAL项目内存模型

            UpProject upProject = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = upProcList,
                Queries = upQueryList,
                UpInstantiation = upInstantiation
            };

            #endregion

            return upProject;
        }

        /// <summary>
        /// 分割线
        /// </summary>
        private static string splitLine = "=======================";

        /// <summary>
        /// 将CTL公式映射为相应的形式
        /// </summary>
        /// <param name="content">公式原型</param>
        /// <returns></returns>
        private static string MappingCTL(string content)
        {
            string res = "";
            string lh = content.Substring(0, 2); // CTL的路径时态算子
            char[] rh = content.Substring(3).ToCharArray(); // 除了路径时态算子，后面的部分
            if (lh == "AG" || lh == "EG")
            {
                lh = lh.Replace("G", "[]");
            }
            else // AF, EF
            {
                lh = lh.Replace("F", "&lt;&gt;");
            }
            res = lh + " ";
            int state = 0; // 0啥也没有，1敏感状态，2脱敏持续状态
            int rlen = rh.Length;
            string tmp = ""; // 存临时的敏感字符
            for (int i = 0; i < rlen; i++)
            {
                char c = rh[i];
                bool sens = (c == '.' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9');
                bool dot = c == '.';
                switch (state)
                {
                    case 0:
                        if (sens)
                        {
                            tmp += c;
                            state = 1;
                        }
                        else
                        {
                            res += c;
                        }
                        break;
                    case 1:
                        if (dot)
                        {
                            res += MappingProcInstName(tmp) + '.';
                            tmp = "";
                            state = 2;
                        }
                        else if (sens)
                        {
                            tmp += c;
                        }
                        else // not sens
                        {
                            res += tmp + c;
                            state = 0;
                        }
                        break;
                    case 2:
                        res += c;
                        if (!sens)
                        {
                            state = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrEmpty(tmp))
            {
                res += tmp;
            }
            res = res.Replace("->", "imply");
            return res;
        }

        /// <summary>
        /// 将ProcInst名映射为UPPAAL所需的
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string MappingProcInstName(string p)
        {
            char[] ch = p.ToCharArray();
            int len = ch.Length;
            int state = 0; // 0:纯数字区 1:另一区域
            string res = "";
            for (int i = len - 1; i >= 0; i -- )
            {
                char c = ch[i];
                bool numerical = c >= '0' && c <= '9';
                switch (state)
                {
                    case 0:
                        if (numerical)
                        {
                            res = c + res;
                        }
                        else
                        {
                            res = ToLow(c) + "_p" + res;
                            state = 1;
                        }
                        break;
                    case 1:
                        res = ToLow(c) + res;
                        break;
                    default:
                        break;
                }
            }
            return res;
        }

        /// <summary>
        /// 若输入为大写字母字符，转换为相应的小写字母字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static char ToLow(char c)
        {
            if (c >= 'A' && c <= 'Z')
            {
                c = (char)(c - 'A' + 'a');
            }
            return c;
        }
    }
}
