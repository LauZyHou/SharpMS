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
            inst.Statements.Add(new PvSendMsg(c, new List<string>{ rsa.Name }));
            inst.Statements.Add(new PvEndOp());

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
    }
}
