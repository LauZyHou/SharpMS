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
                ModelDeclaration = upInstantiation
            };

            // 打印
            Console.WriteLine(upProject);

            // dump磁盘
            UpDumpManager.OutUppalXml(upProject, "D:\\Code\\Mix\\CMSS-Case\\uppaal-gen\\simple.xml");
        }


        public static void BuildMemoryModel()
        {
            // Model Type define
            UpType ByteVec = new UpType("ByteVec");
            UpType SUCI = new UpType("SUCI");

            // Global Declaration
            UpDeclaration globalDeclaration = new UpDeclaration();
            globalDeclaration.Statements.Add(new UpAssignment("a", "1"));
            globalDeclaration.Statements.Add(new UpPass());
            globalDeclaration.Statements.Add(new UpAssignment("b[1].c", "2"));

            // Model Declaration
            UpInstantiation modelDeclaration = new UpInstantiation();
            modelDeclaration.Statements.Add(new UpConcurrency("p1", "p2", "p3"));

            // Process: UE
            UpDeclaration UEDeclaration = new UpDeclaration();
            UEDeclaration.Statements.Add(new UpNewVar(ByteVec, "__bytevec__encrypted"));
            UEDeclaration.Statements.Add(new UpNewVar(ByteVec, "bv"));
            UEDeclaration.Statements.Add(new UpNewVar(SUCI, "suci"));

            UpPG UEPG = new UpPG();
            UpLocation UE_init = new UpLocation("init", true);
            UEPG.Locations.Add(UE_init);
            UpLocation UE_Sended = new UpLocation("Sended");
            UEPG.Locations.Add(UE_Sended);
            UEPG.Transitions.Add(new UpTransition(UE_init, UE_Sended));

            UpProcess UEProc = new UpProcess("UE", UEDeclaration, UEPG);

            // Root Project
            List<UpProcess> processes = new List<UpProcess>();
            processes.Add(UEProc);
            UpProject upProject = new UpProject(globalDeclaration, processes, modelDeclaration, new List<UpQuery>());

            // Take a look
            Console.WriteLine(upProject);
        }
    }
}
