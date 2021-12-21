using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._T;
using Plat._C;


namespace Test
{
    public class TestProVerif
    {
        /// <summary>
        /// 构建一个简单的可达性质的模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/107927165 第4部分
        /// </summary>
        public static void BuildSimpleReachModel()
        {
            // 全局声明
            PvChannel c = new PvChannel("c");
            PvGlobalVar cocks = new PvGlobalVar(PvType.BITSTRING, "Cocks", true);
            PvGlobalVar rsa = new PvGlobalVar(PvType.BITSTRING, "RSA", true);

            PvDeclaration globalDec = new PvDeclaration();
            globalDec.Statements.Add(new PvChannelDeclaration(c));
            globalDec.Statements.Add(new PvGlobalVarDeclaration(cocks));
            globalDec.Statements.Add(new PvGlobalVarDeclaration(rsa));

            // 进程实例化
            PvInstantiation inst = new PvInstantiation();
            inst.RootStmt.SubStmts = new List<PvActiveStmt>
            {
                new PvSendMsg(c, new List<string> { rsa.Name }),
            };

            // 根Project构造
            PvProject pvProject = new PvProject()
            {
                GlobalDeclaration = globalDec,
                Processes = null,
                Queries = new List<PvQuery>
                {
                    new PvConfidentiality(cocks.Name),
                    new PvConfidentiality(rsa.Name)
                },
                Instantiation = inst
            };

            // 输出查看
            Console.WriteLine(pvProject);

            // dump到文件
            PvDumpManager.OutProVerifPv(pvProject, "D:\\Code\\Mix\\CMSS-Case\\proverif-gen\\simple-reach.pv");
        }

        /// <summary>
        /// 构建一个简单的Correspondence Assertion的模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/107927165 第5部分
        /// </summary>
        public static void BuildSimpleCorrespondenceAssertionModel()
        {
            // 全局声明
            PvChannel c = new PvChannel("c");
            PvGlobalVar cocks = new PvGlobalVar(PvType.BITSTRING, "Cocks", true);
            PvGlobalVar rsa = new PvGlobalVar(PvType.BITSTRING, "RSA", true);
            PvEvent evCocks = new PvEvent("evCocks", new List<PvType>());
            PvEvent evRSA = new PvEvent("evRSA", new List<PvType>());

            PvDeclaration globalDec = new PvDeclaration();
            globalDec.Statements.Add(new PvChannelDeclaration(c));
            globalDec.Statements.Add(new PvGlobalVarDeclaration(cocks));
            globalDec.Statements.Add(new PvGlobalVarDeclaration(rsa));
            globalDec.Statements.Add(new PvEventDeclaration(evCocks));
            globalDec.Statements.Add(new PvEventDeclaration(evRSA));

            // 进程实例化
            PvInstantiation inst = new PvInstantiation();
            string localVar = "x";
            inst.RootStmt.SubStmts = new List<PvActiveStmt>
            {
                new PvSendMsg(c, new List<string> { rsa.Name }), // out(c,RSA)
                new PvRecvMsg(c, new List<PvParam> {
                    new PvParam(PvType.BITSTRING, localVar)
                }), // in(c,x:bitstring)
                new PvBranchStmt()
                {
                    Guard = $"{localVar} = Cocks", // if x = Cocks then
                    IfStmt = new PvSeqStmt(
                        new List<PvActiveStmt>
                        {
                            new PvEventPoint(evCocks), // event evCocks
                            new PvEventPoint(evRSA) // event evRSA
                        }
                    ),
                    ElseStmt = new PvEventPoint(evRSA) // else \n event evRSA
                }
            };

            // 根Project构造
            PvProject pvProject = new PvProject()
            {
                GlobalDeclaration = globalDec,
                Processes = null,
                Queries = new List<PvQuery>
                {
                    new PvAuthentication(evCocks, evRSA, false),
                    new PvAuthentication(evCocks)
                },
                Instantiation = inst
            };

            // 打印输出
            Console.WriteLine(pvProject);

            // dump到磁盘文件
            PvDumpManager.OutProVerifPv(pvProject, "D:\\Code\\Mix\\CMSS-Case\\proverif-gen\\correspondence-assertion.pv");
        }


        /// <summary>
        /// 构建一个简单的握手协议模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/107937590 第3部分
        /// 以及 https://lauzyhou.blog.csdn.net/article/details/107952903 第3部分
        /// 两个部分拼接形成模型
        /// </summary>
        public static void BuildHandshakeProtocolModel()
        {
            // 全局声明
            PvDeclaration globalDec = new PvDeclaration();

            PvChannel c = new PvChannel("c");
            PvGlobalVar s = new PvGlobalVar(PvType.BITSTRING, "s", true);

            globalDec.Statements.Add(new PvChannelDeclaration(c));
            globalDec.Statements.Add(new PvGlobalVarDeclaration(s));

            // - 对称加密
            PvType key = new PvType("key");

            globalDec.Statements.Add(new PvTypeDeclaration(key));

            globalDec.Statements.Add(
                new PvFuncDeclaration(
                    new PvFun("senc", new List<PvType> { PvType.BITSTRING, key }, PvType.BITSTRING)
                    )
                );
            globalDec.Statements.Add(new PvReducDeclaration(
                new List<PvParam>
                {
                    new PvParam(PvType.BITSTRING, "m"),
                    new PvParam(key, "k")
                },
                "sdec(senc(m,k),k)=m"
            ));

            // - 非对称加密
            PvType skey = new PvType("skey");
            PvType pkey = new PvType("pkey");

            globalDec.Statements.Add(new PvTypeDeclaration(skey));
            globalDec.Statements.Add(new PvTypeDeclaration(pkey));

            globalDec.Statements.Add(
                new PvFuncDeclaration(
                    new PvFun("pk", new List<PvType> { skey }, pkey)
                    )
                );
            globalDec.Statements.Add(
                new PvFuncDeclaration(
                    new PvFun("aenc", new List<PvType> { PvType.BITSTRING, pkey }, PvType.BITSTRING)
                    )
                );
            globalDec.Statements.Add(new PvReducDeclaration(
                new List<PvParam>
                {
                    new PvParam(PvType.BITSTRING, "m"),
                    new PvParam(skey, "k")
                },
                "adec(aenc(m, pk(k)),k)=m"
            ));

            // - 数字签名
            PvType sskey = new PvType("sskey");
            PvType spkey = new PvType("spkey");

            globalDec.Statements.Add(new PvTypeDeclaration(sskey));
            globalDec.Statements.Add(new PvTypeDeclaration(spkey));

            globalDec.Statements.Add(
                new PvFuncDeclaration(
                    new PvFun("spk", new List<PvType> { sskey }, spkey)
                    )
                );
            globalDec.Statements.Add(
                new PvFuncDeclaration(
                    new PvFun("sign", new List<PvType> { PvType.BITSTRING, sskey }, PvType.BITSTRING)
                    )
                );
            globalDec.Statements.Add(new PvReducDeclaration(
                new List<PvParam>
                {
                    new PvParam(PvType.BITSTRING, "m"),
                    new PvParam(sskey, "k")
                },
                "getmess(sign(m,k))=m"
            ));
            globalDec.Statements.Add(new PvReducDeclaration(
                new List<PvParam>
                {
                    new PvParam(PvType.BITSTRING, "m"),
                    new PvParam(sskey, "k")
                },
                "checksign(sign(m,k),spk(k))=m"
            ));

            // 进程声明
            PvProcess clientA = new PvProcess("clientA");
            clientA.Params = new List<PvParam> { new PvParam(pkey, "pkA"), new PvParam(skey, "skA"), new PvParam(spkey, "pkB") };
            clientA.RootStmt.SubStmts = new List<PvActiveStmt>
            {
                new PvSendMsg(c, new List<string> { "pkA" }),
                new PvRecvMsg(c, new List<PvParam> { new PvParam(PvType.BITSTRING, "x") }),
                new PvLetStmt("y", "adec(x, skA)"),
                new PvLetStmt("(=pkB, k:key)", "checksign(y, pkB)"),
                new PvSendMsg(c, new List<string> { "senc(s, k)" })
            };

            PvProcess serverB = new PvProcess("serverB");
            serverB.Params = new List<PvParam> { new PvParam(spkey, "pkB"), new PvParam(sskey, "skB") };
            serverB.RootStmt.SubStmts = new List<PvActiveStmt>
            {
                new PvRecvMsg(c, new List<PvParam> { new PvParam(pkey, "pkX") }),
                new PvNewVar(key, "k"),
                new PvSendMsg(c, new List<string> { "aenc(sign((pkB,k),skB),pkX)" }),
                new PvRecvMsg(c, new List<PvParam> { new PvParam(PvType.BITSTRING, "x") }),
                new PvLetStmt("z", "sdec(x, k)"),
                new PvPass()
            };

            // 进程实例化
            PvInstantiation inst = new PvInstantiation();
            inst.RootStmt.SubStmts = new List<PvActiveStmt>
            {
                new PvNewVar(skey, "skA"),
                new PvNewVar(sskey, "skB"),
                new PvLetStmt("pkA", "pk(skA)"),
                new PvSendMsg(c, new List<string> { "pkA" }),
                new PvLetStmt("pkB", "spk(skB)"),
                new PvSendMsg(c, new List<string> { "pkB" }),
                new PvConcurrency(
                    new List<PvProcInst>()
                    {
                        new PvProcInst(clientA, true)
                        {
                            Params = new List<string> { "pkA", "skA", "pkB" }
                        },
                        new PvProcInst(serverB, true)
                        {
                            Params = new List<string> { "pkB", "skB" }
                        }
                    }
                )
            };

            // 根Project构造
            PvProject pvProject = new PvProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<PvProcess> { clientA, serverB },
                Queries = new List<PvQuery> { new PvConfidentiality("s") },
                Instantiation = inst
            };

            // 打印输出
            Console.WriteLine(pvProject);

            // dump到磁盘
            PvDumpManager.OutProVerifPv(pvProject, "D:\\Code\\Mix\\CMSS-Case\\proverif-gen\\handshake-protocol.pv");
        }

        /// <summary>
        /// 构建5G-AKA模型（测试Trans用
        /// </summary>
        public static void Build5GAKAModel()
        {
            //
            // Type
            //
            List<PvType> pvTypes = new List<PvType>();
            PvType pvInt = PvType.INT;
            pvTypes.Add(pvInt);
            PvType pvBool = PvType.BOOL;
            pvTypes.Add(pvBool);
            PvType pvMsg = PvType.MSG;
            pvTypes.Add(pvMsg);
            PvType pvKey = PvType.KEY;
            pvTypes.Add(pvKey);
            PvType pvPubKey = PvType.PUBKEY;
            pvTypes.Add(pvPubKey);
            PvType pvPvtKey = PvType.PVTKEY;
            pvTypes.Add(pvPvtKey);

            //
            // Global Declaration
            //
            PvDeclaration globalDec = new PvDeclaration();
            // For Type
            globalDec.Statements.Add(new PvCommentForDec("Type Declaration"));
            foreach (PvType pvType in pvTypes)
            {
                globalDec.Statements.Add(new PvTypeDeclaration(pvType));
            }
            globalDec.Statements.Add(new PvNewLineForDec());
            // For Function
            globalDec.Statements.Add(new PvCommentForDec("Fun Declaration"));
            globalDec.Statements.Add(new PvFuncDeclaration(PvFun.PK));
            globalDec.Statements.Add(new PvFuncDeclaration(PvFun.ASYMENC));
            globalDec.Statements.Add(new PvFuncDeclaration(PvFun.ASYMDEC));
            globalDec.Statements.Add(new PvNewLineForDec());
            // For Equation
            globalDec.Statements.Add(new PvCommentForDec("Equation Declaration"));
            globalDec.Statements.Add(new PvEquationDeclaration(PvEquation.ASYM_ENC_DEC));
            globalDec.Statements.Add(new PvNewLineForDec());
            // For Event
            globalDec.Statements.Add(new PvCommentForDec("Event Declaration"));
            PvEvent UE_Sended = new PvEvent(nameof(UE_Sended), new List<PvType> { PvType.INT });
            globalDec.Statements.Add(new PvEventDeclaration(UE_Sended));
            PvEvent HN_Recved = new PvEvent(nameof(HN_Recved), new List<PvType> { PvType.INT });
            globalDec.Statements.Add(new PvEventDeclaration(HN_Recved));
            globalDec.Statements.Add(new PvNewLineForDec());
            // For Channel
            globalDec.Statements.Add(new PvCommentForDec("Channel Declaration"));
            PvChannel uesn = new PvChannel(nameof(uesn));
            globalDec.Statements.Add(new PvChannelDeclaration(uesn));
            PvChannel snhn = new PvChannel(nameof(snhn));
            globalDec.Statements.Add(new PvChannelDeclaration(snhn));
            globalDec.Statements.Add(new PvNewLineForDec());
            // For Var
            globalDec.Statements.Add(new PvCommentForDec("Var Declaration"));
            List<PvGlobalVar> pvGlobalVars = new List<PvGlobalVar>()
            {
                new PvGlobalVar(PvType.INT, "i2", true),
                new PvGlobalVar(PvType.INT, "i3", true),
                new PvGlobalVar(PvType.INT, "i5", true),
                new PvGlobalVar(PvType.PUBKEY, "pk6", true),
                new PvGlobalVar(PvType.PVTKEY, "sk6", true)
            };
            foreach (PvGlobalVar pvGlobalVar in pvGlobalVars)
            {
                globalDec.Statements.Add(new PvGlobalVarDeclaration(pvGlobalVar));
            }
            globalDec.Statements.Add(new PvNewLineForDec());

            //
            // Process
            //
            PvProcess UE = new PvProcess(nameof(UE));
            UE.Params.Add(new PvParam(PvType.INT, "supi_idUE"));
            UE.Params.Add(new PvParam(PvType.INT, "supi_idHN"));
            UE.Params.Add(new PvParam(PvType.PUBKEY, "pkHN"));
            PvChannel t_SN = new PvChannel(nameof(t_SN));
            UE.Params.Add(new PvParam(PvType.CHANNEL, "t_SN"));
            UE.RootStmt.SubStmts.Add(new PvLetStmt("supi: bitstring", "(supi_idUE, supi_idHN)"));
            UE.RootStmt.SubStmts.Add(new PvLetStmt("t: bitstring", "(supi, 123)"));
            UE.RootStmt.SubStmts.Add(new PvLetStmt("bv: bitstring", "AsymEnc(t, pkHN)"));
            UE.RootStmt.SubStmts.Add(new PvLetStmt("suci: bitstring", "(bv, supi_idHN)"));
            UE.RootStmt.SubStmts.Add(new PvSendMsg(t_SN, new List<string> { "suci" }));
            UE.RootStmt.SubStmts.Add(new PvEventPoint(UE_Sended, new List<string> { "supi_idUE" }));

            PvProcess SN = new PvProcess(nameof(SN))
            {
                Params = new List<PvParam>()
                {
                    new PvParam(PvType.INT, "idSN"),
                    new PvParam(PvType.CHANNEL, "f_UE"),
                    new PvParam(PvType.CHANNEL, "t_HN")
                }
            };
            PvChannel f_UE = new PvChannel(nameof(f_UE));
            PvChannel t_HN = new PvChannel(nameof(t_HN));
            SN.RootStmt.SubStmts.Add(
                new PvRecvMsg(
                    f_UE, 
                    new List<PvParam> {
                        new PvParam(PvType.BITSTRING, "suci")
                    }
                )
            );
            SN.RootStmt.SubStmts.Add(new PvLetStmt("(bv: bitstring, idHN: Int)", "suci"));
            SN.RootStmt.SubStmts.Add(new PvLetStmt("suci", "(bv, idSN)"));
            SN.RootStmt.SubStmts.Add(new PvSendMsg(t_HN, new List<string>() { "suci" }));

            PvProcess HN = new PvProcess(nameof(HN))
            {
                Params = new List<PvParam>
                {
                    new PvParam(PvType.PVTKEY, "sk"),
                    new PvParam(PvType.CHANNEL, "f_SN")
                }
            };
            PvChannel f_SN = new PvChannel(nameof(f_SN));
            HN.RootStmt.SubStmts.Add(
                new PvRecvMsg(
                    f_SN,
                    new List<PvParam>
                    {
                        new PvParam(PvType.BITSTRING, "suci")
                    }
                )
            );
            HN.RootStmt.SubStmts.Add(new PvLetStmt("(bv: bitstring, idSN: Int)", "suci"));
            HN.RootStmt.SubStmts.Add(new PvLetStmt("t: bitstring", "AsymDec(bv, sk)"));
            HN.RootStmt.SubStmts.Add(new PvLetStmt("(supi: bitstring, rs: Int)", "t"));
            HN.RootStmt.SubStmts.Add(new PvLetStmt("(idUE: Int, idHN: Int)", "supi"));
            HN.RootStmt.SubStmts.Add(new PvEventPoint(HN_Recved, new List<string> { "idUE" }));

            //
            // Instantiation
            //
            PvInstantiation inst = new PvInstantiation();
            inst.RootStmt.SubStmts.Add(
                new PvConcurrency()
                {
                    ProcInsts = new List<PvProcInst>
                    {
                        new PvProcInst(UE)
                        {
                            Params = new List<string>
                            {
                                "i3", "i5", "pk6", "uesn"
                            }
                        },
                        new PvProcInst(SN)
                        {
                            Params = new List<string>
                            {
                                "i2", "uesn", "snhn"
                            }
                        },
                        new PvProcInst(HN)
                        {
                            Params = new List<string>
                            {
                                "sk6", "snhn"
                            }
                        }
                    }
                }
            );

            //
            // Root
            //
            PvProject pvProject = new PvProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<PvProcess>
                {
                    UE, SN, HN
                },
                Queries = new List<PvQuery>
                {
                    new PvAuthentication(HN_Recved, UE_Sended),
                    new PvConfidentiality("i2"),
                    new PvConfidentiality("i3"),
                    new PvConfidentiality("i5")
                },
                Instantiation = inst
            };

            PvDumpManager.OutProVerifPv(pvProject, "D:\\Code\\Mix\\CMSS-Case\\proverif-gen\\5g-aka-test.pv");
        }
    }
}
