using Plat._T;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._C;

namespace Test
{
    class TestUPPAAL
    {
        /// <summary>
        /// 构建一个简单转移的模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/108569153 第2部分
        /// </summary>
        public static void BuildSimpleTransModel()
        {
            // 全局声明
            UpDeclaration globalDec = new UpDeclaration();

            // 进程模板：Template
            UpPG upPG = new UpPG();
            UpLocation startLoc = new UpLocation("start", false, true);
            upPG.Locations.Add(startLoc);
            UpLocation endLoc = new UpLocation("end");
            upPG.Locations.Add(endLoc);
            upPG.Transitions.Add(new UpTransition(startLoc, endLoc));

            UpProcess upProcess = new UpProcess("Template", new UpDeclaration(), upPG);

            // 进程例化
            UpInstantiation upInstantiation = new UpInstantiation();
            upInstantiation.Statements.Add(new UpAssignment("Process", "Template()"));
            upInstantiation.Statements.Add(new UpConcurrency("Process"));

            // 根Project构造
            UpProject upProject = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<UpProcess>() { upProcess },
                Queries = new List<UpQuery>()
                {
                    new UpQuery("A&lt;&gt; Process.end", ""),
                    new UpQuery("E&lt;&gt; Process.end", ""),
                },
                UpInstantiation = upInstantiation
            };

            // 打印
            Console.WriteLine(upProject);

            // dump磁盘
            UpDumpManager.OutUppalXml(upProject, "D:\\Code\\Mix\\CMSS-Case\\uppaal-gen\\simple.xml");
        }

        /// <summary>
        /// 构建一个简单的互斥进入临界区的模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/108569153 第3部分
        /// </summary>
        public static void BuildSimpleMutuallyExclusiveEntryIntoTheCriticalSectionModel()
        {
            // 全局声明
            UpDeclaration globalDec = new UpDeclaration();
            globalDec.Statements.Add(new UpNewVar(UpType.INT, "req1"));
            globalDec.Statements.Add(new UpNewVar(UpType.INT, "req2"));
            globalDec.Statements.Add(new UpNewVar(UpType.INT, "turn"));

            // 进程模板：mutex
            UpPG mutexPG = new UpPG();
            
            UpLocation id0 = new UpLocation("idle", false, true);
            mutexPG.Locations.Add(id0);

            UpLocation id1 = new UpLocation("want");
            mutexPG.Locations.Add(id1);

            UpLocation id3 = new UpLocation("wait");
            mutexPG.Locations.Add(id3);

            UpLocation id2 = new UpLocation("CS");
            mutexPG.Locations.Add(id2);

            mutexPG.Transitions.Add(
                new UpTransition(id2, id0)
                {
                    UpAssign = new UpAssignment("req_self", "0")
                }
            );

            mutexPG.Transitions.Add(
                new UpTransition(id3, id2)
                {
                    UpGurad = new UpGuard("req_other", "==", "0")
                }
            );

            mutexPG.Transitions.Add(
                new UpTransition(id3, id2)
                {
                    UpGurad = new UpGuard("turn", "==", "me")
                }
            );

            mutexPG.Transitions.Add(
                new UpTransition(id1, id3)
                {
                    UpAssign = new UpAssignment("turn", "(me==1?2:1)")
                }
            );

            mutexPG.Transitions.Add(
                new UpTransition(id0, id1)
                {
                    UpAssign = new UpAssignment("req_self", "1")
                }
            );

            UpProcess mutexProc = new UpProcess("mutex", new UpDeclaration(), mutexPG);
            mutexProc.Params.Add(new UpParam(UpType.INT, "me"));
            mutexProc.Params.Add(new UpParam(UpType.INT, "req_self", true));
            mutexProc.Params.Add(new UpParam(UpType.INT, "req_other", true));

            // 进程例化
            UpInstantiation system = new UpInstantiation();
            system.Statements.Add(new UpAssignment("P1", "mutex(1, req1, req2)"));
            system.Statements.Add(new UpAssignment("P2", "mutex(2, req2, req1)"));
            system.Statements.Add(new UpConcurrency("P1", "P2"));

            // 根Project构造
            UpProject upProject = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<UpProcess> { mutexProc },
                Queries = new List<UpQuery> {
                    new UpQuery("E&lt;&gt; P1.CS"),
                    new UpQuery("A[] not(P1.CS and P2.CS)")
                },
                UpInstantiation = system
            };

            // 控制台输出
            Console.WriteLine(upProject);

            // dump到磁盘
            UpDumpManager.OutUppalXml(upProject, "D:\\Code\\Mix\\CMSS-Case\\uppaal-gen\\mutually-exclusive.xml");
        }

        /// <summary>
        /// 构建一个简单的通道同步和时钟控制的模型
        /// case见 https://lauzyhou.blog.csdn.net/article/details/108569153 第4部分
        /// </summary>
        public static void BuildSimpleChannelSyncAndClockControlModel()
        {
            // 全局声明
            UpDeclaration globalDec = new UpDeclaration();
            globalDec.Statements.Add(new UpNewVar(UpType.CLOCK, "x"));
            globalDec.Statements.Add(new UpNewVar(UpType.CHAN, "reset"));

            // 进程模板：P1
            UpPG p1PG = new UpPG();

            UpLocation id0 = new UpLocation("loop", false, true)
            {
                Invariant = "x&lt;=3"
            };
            p1PG.Locations.Add(id0);

            UpTransition id0_id0 = new UpTransition(id0, id0)
            {
                UpGurad = new UpGuard("x", "&gt;", "2"),
                UpSync = new UpSynchronisation(true, "reset")
            };
            p1PG.Transitions.Add(id0_id0);

            UpProcess p1Proc = new UpProcess("P1", new UpDeclaration(), p1PG);

            // 进程模板：Obs
            UpPG obsPG = new UpPG();

            UpLocation id1 = new UpLocation("idle", false, true);
            obsPG.Locations.Add(id1);

            UpLocation id2 = new UpLocation("taken");
            obsPG.Locations.Add(id2);

            UpTransition id2_id1 = new UpTransition(id2, id1)
            {
                UpAssign = new UpAssignment("x", "0")
            };
            obsPG.Transitions.Add(id2_id1);

            UpTransition id1_id2 = new UpTransition(id1, id2)
            {
                UpSync = new UpSynchronisation(false, "reset")
            };
            obsPG.Transitions.Add(id1_id2);

            UpProcess obsProc = new UpProcess("Obs", new UpDeclaration(), obsPG);

            // 进程例化
            UpInstantiation system = new UpInstantiation();
            system.Statements.Add(new UpConcurrency("P1", "Obs"));

            // 根Project构造
            UpProject upProject = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<UpProcess> { p1Proc, obsProc },
                Queries = new List<UpQuery> { 
                    new UpQuery("E&lt;&gt; Obs.idle and x&gt;2"),
                    new UpQuery("A[] Obs.taken imply (x&gt;=2 and x&lt;=3)")
                },
                UpInstantiation = system
            };

            // 控制台打印
            Console.WriteLine(upProject);

            // dump到磁盘文件
            UpDumpManager.OutUppalXml(upProject, "D:\\Code\\Mix\\CMSS-Case\\uppaal-gen\\channel-clock.xml");
        }
    
        /// <summary>
        /// 构建5G-AKA模型（测试Trans用
        /// </summary>
        public static void Build5GAKAModel()
        {
            //
            // Type
            //
            List<UpType> upTypes = new List<UpType>();
            UpType msg = UpType.MSG;
            upTypes.Add(msg);
            UpType key = UpType.KEY;
            upTypes.Add(key);
            UpType pubKey = UpType.PUBKEY;
            upTypes.Add(pubKey);
            UpType pvtKey = UpType.PVTKEY;
            upTypes.Add(pvtKey);
            UpType supi = new UpType("SUPI");
            supi.Props.Add(new UpParam(UpType.INT, "idUE"));
            supi.Props.Add(new UpParam(UpType.INT, "idHN"));
            upTypes.Add(supi);
            UpType compSUPI = new UpType("CompSUPI");
            compSUPI.Props.Add(new UpParam(supi, "supi"));
            compSUPI.Props.Add(new UpParam(UpType.INT, "nonce"));
            upTypes.Add(compSUPI);
            UpType compSUPI_A = UpType.GenEncryptedType(compSUPI, true);
            upTypes.Add(compSUPI_A);
            UpType suci = new UpType("SUCI");
            suci.Props.Add(new UpParam(compSUPI_A, "content"));
            suci.Props.Add(new UpParam(UpType.INT, "id"));
            upTypes.Add(suci);
            
            //
            // Global Declaration
            //
            UpDeclaration globalDec = new UpDeclaration();
            foreach (UpType type in upTypes)
            {
                globalDec.Statements.Add(new UpTypeDef(type));
                globalDec.Statements.Add(UpPass.Me);
            }
            globalDec.Statements.Add(new UpNewVar(UpType.CHAN, "sync_1"));
            globalDec.Statements.Add(new UpNewVar(suci, "chanswap_1"));
            globalDec.Statements.Add(new UpNewVar(UpType.CHAN, "sync_2"));
            globalDec.Statements.Add(new UpNewVar(suci, "chanswap_2"));

            //
            // Process: UE
            //
            UpPG uePG = new UpPG();
            UpLocation id0 = new UpLocation(null, true, true);
            uePG.Locations.Add(id0);
            UpLocation id1 = new UpLocation(null, true, false);
            uePG.Locations.Add(id1);
            UpLocation id2 = new UpLocation(null, true, false);
            uePG.Locations.Add(id2);
            UpLocation id3 = new UpLocation(null, true, false);
            uePG.Locations.Add(id3);
            UpLocation id4 = new UpLocation("Sended", true, false);
            uePG.Locations.Add(id4);
            UpTransition t_3_4 = new UpTransition(id3, id4)
            {
                UpSync = new UpSynchronisation(true, "sync_1"),
                UpAssign = new UpAssignment("chanswap_1", "suci")
            };
            uePG.Transitions.Add(t_3_4);
            UpTransition t_2_3 = new UpTransition(id2, id3)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("suci.content", "bv"),
                    new UpAssignment("suci.id", "supi.idHN")
                }
            };
            uePG.Transitions.Add(t_2_3);
            UpTransition t_1_2 = new UpTransition(id1, id2)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("bv.content", "t"),
                    new UpAssignment("bv.pk", "pkHN")
                }
            };
            uePG.Transitions.Add(t_1_2);
            UpTransition t_0_1 = new UpTransition(id0, id1)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("t.supi", "supi"),
                    new UpAssignment("t.nonce", "rs")
                }
            };
            uePG.Transitions.Add(t_0_1);
            UpDeclaration ueDec = new UpDeclaration(true);
            ueDec.Statements.Add(new UpNewVar(UpType.INT, "rs"));
            ueDec.Statements.Add(new UpNewVar(compSUPI, "t"));
            ueDec.Statements.Add(new UpNewVar(compSUPI_A, "bv"));
            ueDec.Statements.Add(new UpNewVar(suci, "suci"));
            UpProcess ueProcess = new UpProcess("UE", ueDec, uePG);
            ueProcess.Params.Add(new UpParam(supi, "supi", true));
            ueProcess.Params.Add(new UpParam(pubKey, "pkHN"));

            //
            // Process: SN
            //
            UpPG snPG = new UpPG();
            UpLocation id5 = new UpLocation(null, true, true);
            snPG.Locations.Add(id5);
            UpLocation id6 = new UpLocation(null, true, false);
            snPG.Locations.Add(id6);
            UpLocation id7 = new UpLocation(null, true, false);
            snPG.Locations.Add(id7);
            UpLocation id8 = new UpLocation(null, true, false);
            snPG.Locations.Add(id8);
            UpLocation id9 = new UpLocation(null, true, false);
            snPG.Locations.Add(id9);
            UpTransition t_8_9 = new UpTransition(id8, id9)
            {
                UpSync = new UpSynchronisation(true, "sync_2"),
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("chanswap_2", "suci")
                }
            };
            snPG.Transitions.Add(t_8_9);
            UpTransition t_7_8 = new UpTransition(id7, id8)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("suci.content", "bv"),
                    new UpAssignment("suci.id", "idSN")
                }
            };
            snPG.Transitions.Add(t_7_8);
            UpTransition t_6_7 = new UpTransition(id6, id7)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("bv", "suci.content"),
                    new UpAssignment("idHN", "suci.id")
                }
            };
            snPG.Transitions.Add(t_6_7);
            UpTransition t_5_6 = new UpTransition(id5, id6)
            {
                UpSync = new UpSynchronisation(false, "sync_1"),
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("suci", "chanswap_1")
                }
            };
            snPG.Transitions.Add(t_5_6);
            UpDeclaration snDec = new UpDeclaration(true);
            snDec.Statements.Add(new UpNewVar(suci, "suci"));
            snDec.Statements.Add(new UpNewVar(compSUPI_A, "bv"));
            snDec.Statements.Add(new UpNewVar(UpType.INT, "idHN"));
            UpProcess snProcess = new UpProcess("SN", snDec, snPG);
            snProcess.Params.Add(new UpParam(UpType.INT, "idSN"));

            //
            // Process: HN
            //
            UpPG hnPG = new UpPG();
            UpLocation id10 = new UpLocation(null, true, true);
            hnPG.Locations.Add(id10);
            UpLocation id11 = new UpLocation(null, true, false);
            hnPG.Locations.Add(id11);
            UpLocation id12 = new UpLocation(null, true, false);
            hnPG.Locations.Add(id12);
            UpLocation id13 = new UpLocation(null, true, false);
            hnPG.Locations.Add(id13);
            UpLocation id14 = new UpLocation("Unpacked", true, false);
            hnPG.Locations.Add(id14);
            UpTransition t_13_14 = new UpTransition(id13, id14)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("supi", "t.supi"),
                    new UpAssignment("rs", "t.nonce")
                }
            };
            hnPG.Transitions.Add(t_13_14);
            UpTransition t_12_13 = new UpTransition(id12, id13)
            {
                UpGurad = new UpGuard("sk", "==", "bv.pk"),
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("t", "bv.content")
                }
            };
            hnPG.Transitions.Add(t_12_13);
            UpTransition t_11_12 = new UpTransition(id11, id12)
            {
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("bv", "suci.content"),
                    new UpAssignment("idSN", "suci.id")
                }
            };
            hnPG.Transitions.Add(t_11_12);
            UpTransition t_10_11 = new UpTransition(id10, id11)
            {
                UpSync = new UpSynchronisation(false, "sync_2"),
                UpAssignments = new List<UpAssignment>()
                {
                    new UpAssignment("suci", "chanswap_2")
                }
            };
            hnPG.Transitions.Add(t_10_11);
            UpDeclaration hnDec = new UpDeclaration(true);
            hnDec.Statements.Add(new UpNewVar(suci, "suci"));
            hnDec.Statements.Add(new UpNewVar(compSUPI_A, "bv"));
            hnDec.Statements.Add(new UpNewVar(UpType.INT, "idSN"));
            hnDec.Statements.Add(new UpNewVar(compSUPI, "t"));
            hnDec.Statements.Add(new UpNewVar(supi, "supi"));
            hnDec.Statements.Add(new UpNewVar(UpType.INT, "rs"));
            UpProcess hnProcess = new UpProcess("HN", hnDec, hnPG);
            hnProcess.Params.Add(new UpParam(pvtKey, "sk"));

            //
            // Instantiation
            //
            UpInstantiation system = new UpInstantiation();
            system.Statements.Add(new UpInitVar(supi, "supi", "{ 3, 5 }"));
            system.Statements.Add(new UpInstProc("ue", ueProcess)
            { 
                Values = new List<string>()
                {
                    "supi", "6"
                }
            });
            system.Statements.Add(new UpInstProc("sn", snProcess)
            {
                Values = new List<string>()
                {
                    "2"
                }
            });
            system.Statements.Add(new UpInstProc("hn", hnProcess)
            {
                Values = new List<string>()
                {
                    "6"
                }
            });
            system.Statements.Add(new UpConcurrency(
                new List<string>
                {
                    "ue", "sn", "hn"
                }
            ));

            //
            // Project Root
            //
            UpProject project = new UpProject()
            {
                GlobalDeclaration = globalDec,
                Processes = new List<UpProcess>()
                {
                    ueProcess, snProcess, hnProcess
                },
                Queries = new List<UpQuery>()
                {
                    new UpQuery("A&lt;&gt; hn.Unpacked"),
                    new UpQuery("E&lt;&gt; ue.Sended"),
                    new UpQuery("A[] hn.Unpacked imply ue.Sended")
                },
                UpInstantiation = system
            };

            Console.WriteLine(project);

            //
            // Dump to Disk
            //
            UpDumpManager.OutUppalXml(project, "D:\\Code\\Mix\\CMSS-Case\\uppaal-gen\\5g-aka-test.xml");
        }
    }
}
