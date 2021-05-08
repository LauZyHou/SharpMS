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
    }
}
