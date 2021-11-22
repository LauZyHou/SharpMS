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
            //
            // 对齐数据类型
            //
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
            // 填表
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
            // 填表后，可以保证所有Type都能找到
            // 再遍历一次，构造所有原生strcut的Props
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

            //
            // 遍历UT表生成所有的typedef
            //
            UpDeclaration globalDec = new UpDeclaration();
            globalDec.Statements.Add(UpPass.Me);
            globalDec.Statements.Add(new UpComment($"{splitLine} TypeDef {splitLine}"));
            foreach (UpType upType in upTypes)
            {
                globalDec.Statements.Add(UpPass.Me);
                globalDec.Statements.Add(new UpTypeDef(upType));
            }

            //
            // 遍历进程模板，生成对应的参数表List<UpParam>
            //
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

            //
            // 遍历进程图状态机，生成对应的本地变量声明UpDeclaration
            // 寻找形如 var: Type = xxxx 的动作语句，构造本地变量声明Type var;
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

            //
            // 解析拓扑图中的Port-Chan
            // 获取每个拓扑结点的例化进程，每个例化边上的端口-信道实例映射
            // 后期将为每个拓扑结点生成单独的进程模板
            //
            // 【UP表】所有的UPPAAL进程模板
            List<UpProcess> upProcessList = new List<UpProcess>();
            // 【PI-PORT-CI表】ProcInst -> Port -> ChanInst 的映射
            // 其中ChanInst 由 Channel.Identifier 组合 EnvInst的Id形成
            Dictionary<ProcInst, Dictionary<Port, string>> procInstToPortToChanInstMap = new Dictionary<ProcInst, Dictionary<Port, string>>();
            // 【CI-SYNC表】从ChanInst字符串到同步信号chan的映射
            Dictionary<string, string> chanInstToSyncStrMap = new Dictionary<string, string>();
            // 【PI表】所有需要转换的ProcInst（没连线也就没ProcEnvInst_CT_VM，也就不用考虑）
            HashSet<ProcInst> procInstList = new HashSet<ProcInst>();
            // 【PORT_S-PORT表】端口名到端口引用的映射
            Dictionary<string, Port> portNameToPortMap = new Dictionary<string, Port>();
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
                        // 加入PORT_S-PORT表中
                        portNameToPortMap[port.Identifier] = port;
                        // 加入PI-PORT-CI表中
                        string chanInst = $"{chan.Identifier}_{envInst.Id}";
                        if (!procInstToPortToChanInstMap.ContainsKey(procInst))
                        {
                            procInstToPortToChanInstMap.Add(procInst, new Dictionary<Port, string>());
                        }
                        // 填充PI-PORT-CI表
                        procInstToPortToChanInstMap[procInst].Add(port, chanInst);
                        // 填充CI-SYNC表
                        string syncInst = $"sync_{chanInst}";
                        if (!chanInstToSyncStrMap.ContainsKey(chanInst))
                        {
                            chanInstToSyncStrMap.Add(chanInst, syncInst);
                        }
                    }
                }
                // dimiss ProcInst_VM / EnvInst_VM / Linker / ProcInst_NT_VM / EnvInst_NT_VM
            }

            //
            // 对每个需要转换的ProcInst生成对应的全局信道同步变量
            //
            foreach (ProcInst procInst in procInstList)
            {
                // 先找到对应的进程图状态机
                Debug.Assert(procInst.Proc is not null);
                Proc proc = procInst.Proc;
                ProcGraph_P_VM procGraph_P_VM = procToPGPVMMap[proc];
                // 遍历状态机迁移边上的端口收发操作
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is TransNode_VM)
                    {
                        TransNode_VM transNode_VM = (TransNode_VM)dragDrop_VM;
                        LocTrans locTrans = transNode_VM.LocTrans;
                        foreach (Formula action in locTrans.Actions)
                        {
                            if (action.Content.Contains("->") || action.Content.Contains("<-"))
                            {

                            }
                        }
                    }
                }
                // 遍历其中边上的Port交互
                /*
                // 在CI-SYNC表和全局声明中加入信道和交换临时变量的定义
                
                globalDec.Statements.Add(new UpNewVar(UpType.CHAN, syncInst));
                globalDec.Statements.Add(new UpNewVar(UpType.CHAN, syncInst));
                */
            }


            //
            // 进程图状态机解析
            //
            // 【S-L表】SharpMS的State_VM到UPPAAL的Location的映射
            // 注意，由于SharpMS允许多个操作步，有时需要被拆成多个Location，因此这个表里并没有全部Location
            Dictionary<DragDrop_VM, UpLocation> stateVMLocMap = new Dictionary<DragDrop_VM, UpLocation>();
            // 【I-L表】从UPPAAL Location Id到Location引用的映射
            Dictionary<int, UpLocation> idLocMap = new Dictionary<int, UpLocation>();
            // 遍历每个进程图
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.mainWindow_VM.ProcGraph_PG_VM.ProcGraph_P_VMs)
            {
                // 进程图所属的进程
                Proc proc = procGraph_P_VM.ProcGraph.Proc;
                // 遍历其中的每个元素
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is InitState_VM)
                    {
                        
                    }
                    else if (dragDrop_VM is TinyState_VM)
                    {

                    }
                    else if (dragDrop_VM is FinalState_VM)
                    {

                    }
                }
            }

            //
            // 信息同步和交换
            //

            // todo

            // test
            List<UpProcess> upProcesses = new List<UpProcess>();
            foreach (Proc proc in ResourceManager.procs)
            {
                UpProcess upProcess = new UpProcess(proc.Identifier, procToUpDecMap[proc], new UpPG());
                upProcess.Params = procToUpParamListMap[proc];
                upProcesses.Add(upProcess);
            }

            //
            // 进程例化
            //
            UpInstantiation upInstantiation = new UpInstantiation();

            //
            // 填充并返回UPPAAL项目内存模型
            //
            UpProject upProject = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = upProcesses,
                Queries = new List<UpQuery> { },
                UpInstantiation = upInstantiation
            };

            return upProject;
        }

        /// <summary>
        /// 分割线
        /// </summary>
        private static string splitLine = "=======================";
    }
}
