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
            UpLocation startLoc = new UpLocation("start", true);
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
            
            UpLocation id0 = new UpLocation("idle", true);
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

            UpLocation id0 = new UpLocation("loop", true)
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

            UpLocation id1 = new UpLocation("idle", true);
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
    }
}
