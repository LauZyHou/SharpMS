using Plat._C;
using Plat._M;
using System.Collections.Generic;

namespace Plat._T
{
    /// <summary>
    /// SharpMS-UPPAAL语义对齐机
    /// </summary>
    public class UppaalAlignMachine
    {
        public static UpProject Run()
        {
            //
            // 对齐数据类型
            //
            // 【M表】SharpMS -> UPPAAL 类型映射（原生
            Dictionary<Type, UpType> typeMap = new Dictionary<Type, UpType>()
            {
                { Type.TYPE_INT, UpType.INT },
                { Type.TYPE_BOOL, UpType.BOOL },
                { Type.TYPE_MSG, UpType.MSG },
                { Type.TYPE_KEY, UpType.KEY },
                { Type.TYPE_PUB_KEY, UpType.PUBKEY },
                { Type.TYPE_PVT_KEY, UpType.PVTKEY }
            };
            // 【L表】UPPAAL模型中需要做typedef的类型表（int和bool不需要def
            List<UpType> upTypes = new List<UpType>()
            {
                UpType.MSG, UpType.KEY, UpType.PUBKEY, UpType.PVTKEY
            };
            // 【MS表】SharpMS -> UPPAAL 类型映射（对称加密
            Dictionary<Type, UpType> typeMap_S = new Dictionary<Type, UpType>();
            // 【MA表】SharpMS -> UPPAAL 类型映射（非对称加密
            Dictionary<Type, UpType> typeMap_A = new Dictionary<Type, UpType>();
            // 填表
            foreach (Type type in ResourceManager.types)
            {
                // int / bool 不必处理
                if (type == Type.TYPE_INT || type == Type.TYPE_BOOL) continue;
                // 非基本类型的需要补充“原生”到M表和L表中
                if (!type.IsBase)
                {
                    UpType upType = new UpType(type.Identifier) { Description = type.Description };
                    typeMap.Add(type, upType);
                    upTypes.Add(upType);
                }
                // 继承自Msg的即可“对称加密”和“非对称加密”，补充到MS和MA表中
                // 这个过程中因为原生类型已经在表中，所以可以直接补充Props
                if (type.InheritFrom(Type.TYPE_MSG))
                {
                    UpType upType_S = UpType.GenEncryptedType(typeMap[type], false);
                    typeMap_S.Add(type, upType_S);
                    upTypes.Add(upType_S);

                    UpType upType_A = UpType.GenEncryptedType(typeMap[type], true);
                    typeMap_A.Add(type, upType_A);
                    upTypes.Add(upType_A);
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
            // 遍历L表生成所有的typedef
            UpDeclaration globalDec = new UpDeclaration();
            foreach (UpType upType in upTypes)
            {
                globalDec.Statements.Add(UpPass.Me);
                globalDec.Statements.Add(new UpTypeDef(upType));
            }

            // todo

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
                Processes = new List<UpProcess> { },
                Queries = new List<UpQuery> { },
                UpInstantiation = upInstantiation
            };

            return upProject;
        }
    }
}
