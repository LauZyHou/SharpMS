using Plat._C;
using Plat._M;
using Plat._VM;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// 标识符映射器
    /// </summary>
    public class IdentifierMapper
    {
        /// <summary>
        /// 将进程图状态机中的类型名映射到UPPAAL类型名上
        /// </summary>
        /// <param name="sharpMsTypeInFSM"></param>
        /// <returns></returns>
        public static string MapToUpTypeName(string sharpMsTypeInFSM)
        {
            if (sharpMsTypeInFSM == "Int") return "int";
            if (sharpMsTypeInFSM == "Bool") return "bool";
            if (sharpMsTypeInFSM.EndsWith("?"))
            {
                return sharpMsTypeInFSM.Replace("?", "_A");
            }
            else if (sharpMsTypeInFSM.EndsWith("^"))
            {
                return sharpMsTypeInFSM.Replace("^", "_S");
            }
            return sharpMsTypeInFSM;
        }

        /// <summary>
        /// 将类型名映射到统一语义模型上的TypeWithCrypto
        /// </summary>
        /// <param name="sharpMsTypeInFSM">状态机中解析出来的类型名</param>
        /// <returns>构建的TypeWithCrypto，找不到类型时返回null</returns>
        public static TypeWithCrypto? MapToTypeWithCrypto(string sharpMsTypeInFSM)
        {
            int len = sharpMsTypeInFSM.Length;
            Crypto crypto = Crypto.None;
            if (sharpMsTypeInFSM.EndsWith("?"))
            {
                crypto = Crypto.Asym;
                sharpMsTypeInFSM = sharpMsTypeInFSM.Substring(0, len - 1);
            }
            else if (sharpMsTypeInFSM.EndsWith("^"))
            {
                crypto = Crypto.Sym;
                sharpMsTypeInFSM = sharpMsTypeInFSM.Substring(0, len - 1);
            }
            Type? type = null;
            foreach (Type t in ResourceManager.types)
            {
                if (t.Identifier == sharpMsTypeInFSM)
                {
                    type = t;
                    break;
                }
            }
            if (type == null) // 找不到此类型
            {
                return null;
            }
            return new TypeWithCrypto(type, crypto);
        }

        /// <summary>
        /// 将每个进程的进程图状态机中的变元映射至其数据类型上
        /// 【注意】该方法只能处理显式声明的部分，对于转换ProVerif时做了flatten的部分还没有记录
        /// 因此在后续的转换中也会更新该方法的返回map
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Proc, Dictionary<string, TypeWithCrypto>> MapPGVarToType()
        {
            Dictionary<Proc, Dictionary<string, TypeWithCrypto>> res = new Dictionary<Proc, Dictionary<string, TypeWithCrypto>>();
            // 遍历每个进程图状态机
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.mainWindow_VM.ProcGraph_PG_VM.ProcGraph_P_VMs)
            {
                Proc proc = procGraph_P_VM.ProcGraph.Proc;
                Dictionary<string, TypeWithCrypto> varToTypeMap = new Dictionary<string, TypeWithCrypto>();

                // 先把proc的所有属性参数都搞进来
                foreach (VisAttr attr in proc.Attributes)
                {
                    Crypto crypto = Crypto.None;
                    if (attr.IsEncrypted)
                    {
                        crypto = Crypto.Sym;
                        if (attr.IsAsymmetric)
                        {
                            crypto = Crypto.Asym;
                        }
                    }
                    varToTypeMap.Add(attr.Identifier, new TypeWithCrypto(attr.Type, crypto));
                }

                // 遍历其上的所有迁移边
                foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
                {
                    if (dragDrop_VM is TransNode_VM)
                    {
                        TransNode_VM transNode_VM = (TransNode_VM)dragDrop_VM;
                        // 遍历其上的所有操作步
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
                                throw new System.NotImplementedException();
                            }
                            string varName = nameAndType[0].Trim();
                            TypeWithCrypto? twc = MapToTypeWithCrypto(nameAndType[1].Trim());
                            if (twc is null)
                            {
                                ResourceManager.UpdateTip($"[ERROR] Syntax error for type of var {varName}.");
                                throw new System.NotImplementedException();
                            }
                            varToTypeMap.Add(varName, twc);
                        }
                    }
                }

                res.Add(proc, varToTypeMap);
            }
            return res;
        }
    }
}
