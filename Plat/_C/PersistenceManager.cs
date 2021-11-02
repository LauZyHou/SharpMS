using Avalonia.Controls;
using Plat._M;
using Plat._VM;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace Plat._C
{
    /// <summary>
    /// 持久化管理器
    /// 负责内存模型与磁盘存储的项目模型的一致性交互
    /// </summary>
    public class PersistenceManager
    {
        private const string ModelFilePostfix = "sharpms";

        #region Dialog

        /// <summary>
        /// 打开对话框，由用户选择保存路径并返回
        /// 在保存文件前执行的操作
        /// </summary>
        /// <returns></returns>
        public static async Task<string> OpenDialogAndGetSavePathFromUser()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "SharpMS Model", Extensions = { ModelFilePostfix } });
            string result = await dialog.ShowAsync(ResourceManager.mainWindow_V);
            // Linux bugfix：某些平台输入文件名不会自动补全后缀名,这里判断一下手动补上
            if (string.IsNullOrEmpty(result) || result.EndsWith($".{ ModelFilePostfix }"))
                return result;
            return result + $".{ ModelFilePostfix }";
        }

        #endregion

        #region XML Generation

        /// <summary>
        /// 将项目模型保存为XML文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool SaveProjectModelAsXmlFile(string filePath)
        {
            // 传过来的Path一定非空，而且以指定的后缀名结尾
            Debug.Assert(filePath.EndsWith(ModelFilePostfix));

            // 以实际XML的格式写入文件
            XmlTextWriter xmlWriter = new XmlTextWriter(filePath, null);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartElement("SharpMS"); // Root开头

            //
            // 各类模型的静态全局标识（_id）
            //
            #region Static-Ids

            xmlWriter.WriteStartElement("Static-Ids");

            XmlWriteStaticId(xmlWriter, nameof(Axiom), Axiom._id);
            XmlWriteStaticId(xmlWriter, nameof(Channel), Channel._id);
            XmlWriteStaticId(xmlWriter, nameof(Env), Env._id);
            XmlWriteStaticId(xmlWriter, nameof(AttrPair), AttrPair._id);
            XmlWriteStaticId(xmlWriter, nameof(IK), IK._id);
            XmlWriteStaticId(xmlWriter, nameof(Port), Port._id);
            XmlWriteStaticId(xmlWriter, nameof(Proc), Proc._id);
            XmlWriteStaticId(xmlWriter, nameof(State), State._id);
            XmlWriteStaticId(xmlWriter, nameof(LocTrans), LocTrans._id);
            XmlWriteStaticId(xmlWriter, nameof(PortChanInst), PortChanInst._id);
            XmlWriteStaticId(xmlWriter, nameof(ProcEnvInst), ProcEnvInst._id);
            XmlWriteStaticId(xmlWriter, nameof(TopoInst), TopoInst._id); // = EnvInst._id = ProcInst._id
            XmlWriteStaticId(xmlWriter, nameof(Type), Type._id);
            XmlWriteStaticId(xmlWriter, nameof(Attribute), Attribute._id); // = ValAttr._id = VisAttr._id
            XmlWriteStaticId(xmlWriter, nameof(Caller), Caller._id);
            XmlWriteStaticId(xmlWriter, nameof(Formula), Formula._id);
            XmlWriteStaticId(xmlWriter, nameof(Anchor_VM), Anchor_VM._id);
            XmlWriteStaticId(xmlWriter, nameof(Linker_VM), Linker_VM._id);

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 数据类型
            //
            #region MetaInfo-Types

            xmlWriter.WriteStartElement($"MetaInfo-{nameof(Type)}s");

            foreach (Type type in ResourceManager.types)
            {
                xmlWriter.WriteStartElement(nameof(Type));

                xmlWriter.WriteAttributeString("id", type.Id.ToString());
                xmlWriter.WriteAttributeString("identifier", type.Identifier);
                xmlWriter.WriteAttributeString("parent-Ref", type.Parent?.Id.ToString());
                xmlWriter.WriteAttributeString("description", type.Description);
                xmlWriter.WriteAttributeString("isBase", type.IsBase.ToString());
                foreach (Attribute attribute in type.Attributes)
                {
                    XmlWriteAttribute(xmlWriter, attribute);
                }
                foreach (Caller caller in type.Methods)
                {
                    XmlWriteCaller(xmlWriter, caller, useLabel: "Method");
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 环境模板
            //
            #region MetaInfo-Envs

            xmlWriter.WriteStartElement($"MetaInfo-{nameof(Env)}s");

            foreach (Env env in ResourceManager.envs)
            {
                xmlWriter.WriteStartElement(nameof(Env));

                xmlWriter.WriteAttributeString("id", env.Id.ToString());
                xmlWriter.WriteAttributeString("identifier", env.Identifier);
                xmlWriter.WriteAttributeString("pub", env.Pub.ToString());
                xmlWriter.WriteAttributeString("parent-Ref", env.Parent?.Id.ToString());
                xmlWriter.WriteAttributeString("description", env.Description);
                foreach (VisAttr visAttr in env.Attributes)
                {
                    XmlWriteAttribute(xmlWriter, visAttr, useLabel: nameof(VisAttr));
                }
                foreach (Channel channel in env.Channels)
                {
                    XmlWriteChannel(xmlWriter, channel);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 进程模板
            //
            #region MetaInfo-Procs

            xmlWriter.WriteStartElement($"MetaInfo-{nameof(Proc)}s");

            foreach (Proc proc in ResourceManager.procs)
            {
                xmlWriter.WriteStartElement(nameof(Proc));

                xmlWriter.WriteAttributeString("id", proc.Id.ToString());
                xmlWriter.WriteAttributeString("identifier", proc.Identifier);
                xmlWriter.WriteAttributeString("parent-Ref", proc.Parent?.Id.ToString());
                xmlWriter.WriteAttributeString("description", proc.Description);
                foreach (VisAttr visAttr in proc.Attributes)
                {
                    XmlWriteAttribute(xmlWriter, visAttr, useLabel: nameof(VisAttr));
                }
                foreach (Caller caller in proc.Methods)
                {
                    XmlWriteCaller(xmlWriter, caller, useLabel: "Method");
                }
                foreach (Port port in proc.Ports)
                {
                    XmlWritePort(xmlWriter, port);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 初始知识
            //
            #region MetaInfo-IKs

            xmlWriter.WriteStartElement($"MetaInfo-{nameof(IK)}s");

            foreach (IK ik in ResourceManager.iks)
            {
                xmlWriter.WriteStartElement(nameof(IK));

                xmlWriter.WriteAttributeString("id", ik.Id.ToString());
                xmlWriter.WriteAttributeString("identifier", ik.Identifier);
                xmlWriter.WriteAttributeString("description", ik.Description);
                foreach (ValAttr valAttr in ik.Attributes)
                {
                    XmlWriteAttribute(xmlWriter, valAttr, useLabel: nameof(ValAttr));
                }
                foreach (AttrPair attrPair in ik.AttrPairs)
                {
                    XmlWriteAttrPair(xmlWriter, attrPair);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 公理
            //
            #region MetaInfo-Axioms

            xmlWriter.WriteStartElement($"MetaInfo-{nameof(Axiom)}s");

            foreach (Axiom axiom in ResourceManager.axioms)
            {
                xmlWriter.WriteStartElement(nameof(Axiom));
                xmlWriter.WriteAttributeString("id", axiom.Id.ToString());
                xmlWriter.WriteAttributeString("identifier", axiom.Identifier);
                xmlWriter.WriteAttributeString("description", axiom.Description);
                foreach (Formula formula in axiom.Formulas)
                {
                    XmlWriteFormula(xmlWriter, formula);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 类图元素排列
            //
            #region ClassDiagram

            xmlWriter.WriteStartElement($"ClassDiagram");

            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs)
            {
                XmlWriteClassDiagramItem(xmlWriter, dragDrop_VM);
            }

            xmlWriter.WriteEndElement();

            #endregion

            xmlWriter.WriteEndElement(); // Root结尾
            xmlWriter.Flush();
            xmlWriter.Close();

            return true;
        }

        /// <summary>
        /// XML持久化指定Model类的静态id
        /// </summary>
        /// <param name="xmlWriter">XML写入器</param>
        /// <param name="className">Model类名</param>
        /// <param name="idValue">其静态字段[_id]的取值</param>
        private static void XmlWriteStaticId(XmlTextWriter xmlWriter, string className, int idValue)
        {
            xmlWriter.WriteStartElement($"SID");

            xmlWriter.WriteAttributeString("className", className);
            xmlWriter.WriteAttributeString("_id", idValue.ToString());

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化指定的Attribute
        /// </summary>
        /// <param name="xmlWriter">XML写入器</param>
        /// <param name="attribute">待持久化Attribute</param>
        /// <param name="useLabel">持久化时使用的XML标签</param>
        private static void XmlWriteAttribute(XmlTextWriter xmlWriter, Attribute attribute, string useLabel = nameof(Attribute))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", attribute.Id.ToString());
            xmlWriter.WriteAttributeString("identifier", attribute.Identifier);
            xmlWriter.WriteAttributeString("type-Ref", attribute.Type.Id.ToString());
            xmlWriter.WriteAttributeString("isArray", attribute.IsArray.ToString());
            xmlWriter.WriteAttributeString("description", attribute.Description);
            if (attribute is VisAttr)
            {
                xmlWriter.WriteAttributeString("pub", ((VisAttr)attribute).Pub.ToString());
            }
            if (attribute is ValAttr)
            {
                xmlWriter.WriteAttributeString("value", ((ValAttr)attribute).Value);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化指定的Caller
        /// </summary>
        /// <param name="xmlWriter">XML写入器</param>
        /// <param name="caller">待持久化Caller</param>
        /// <param name="useLabel">持久化时使用的XML标签</param>
        private static void XmlWriteCaller(XmlTextWriter xmlWriter, Caller caller, string useLabel = nameof(Caller))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", caller.Id.ToString());
            xmlWriter.WriteAttributeString("identifier", caller.Identifier);
            xmlWriter.WriteAttributeString("returnType-Ref", caller.ReturnType.Id.ToString());
            xmlWriter.WriteAttributeString("description", caller.Description);

            foreach (Type type in caller.ParamTypes)
            {
                xmlWriter.WriteStartElement("ParamType");

                xmlWriter.WriteAttributeString("type-Ref", type.Id.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化Channel
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="channel"></param>
        /// <param name="useLabel"></param>
        private static void XmlWriteChannel(XmlTextWriter xmlWriter, Channel channel, string useLabel = nameof(Channel))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", channel.Id.ToString());
            xmlWriter.WriteAttributeString("identifier", channel.Identifier);
            xmlWriter.WriteAttributeString("pub", channel.Pub.ToString());
            xmlWriter.WriteAttributeString("capacity", channel.Capacity.ToString());
            xmlWriter.WriteAttributeString("description", channel.Description);

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化Port
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="port"></param>
        /// <param name="useLable"></param>
        private static void XmlWritePort(XmlTextWriter xmlWriter, Port port, string useLabel = nameof(Port))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", port.Id.ToString());
            xmlWriter.WriteAttributeString("identifier", port.Identifier);
            xmlWriter.WriteAttributeString("isOut", port.IsOut.ToString());
            xmlWriter.WriteAttributeString("description", port.Description);

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化IK的AttrPair
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="attrPair"></param>
        /// <param name="useLabel"></param>
        private static void XmlWriteAttrPair(XmlTextWriter xmlWriter, AttrPair attrPair, string useLabel = nameof(AttrPair))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", attrPair.Id.ToString());
            xmlWriter.WriteAttributeString("procA-Ref", attrPair.ProcA?.Id.ToString());
            xmlWriter.WriteAttributeString("procAttrA-Ref", attrPair.ProcAttrA?.Id.ToString());
            xmlWriter.WriteAttributeString("procB-Ref", attrPair.ProcB?.Id.ToString());
            xmlWriter.WriteAttributeString("procAttrB-Ref", attrPair.ProcAttrB?.Id.ToString());
            xmlWriter.WriteAttributeString("envA-Ref", attrPair.EnvA?.Id.ToString());
            xmlWriter.WriteAttributeString("envAttrA-Ref", attrPair.EnvAttrA?.Id.ToString());
            xmlWriter.WriteAttributeString("envB-Ref", attrPair.EnvB?.Id.ToString());
            xmlWriter.WriteAttributeString("envAttrB-Ref", attrPair.EnvAttrB?.Id.ToString());

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化Formula（用于Axiom等
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="formula"></param>
        /// <param name="useLabel"></param>
        private static void XmlWriteFormula(XmlTextWriter xmlWriter, Formula formula, string useLabel = nameof(Formula))
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", formula.Id.ToString());
            xmlWriter.WriteAttributeString("content", formula.Content);
            xmlWriter.WriteAttributeString("description", formula.Description);

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化类图中的DD VM
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="dragDrop_VM"></param>
        private static void XmlWriteClassDiagramItem(XmlTextWriter xmlWriter, DragDrop_VM dragDrop_VM)
        {
            // 每种DDVM特定的部分
            if (dragDrop_VM is Type_VM)
            {
                Type_VM type_VM = (Type_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(Type_VM));
                xmlWriter.WriteAttributeString("type-Ref", type_VM.Type.Id.ToString());
            }
            else if (dragDrop_VM is Env_VM)
            {
                Env_VM env_VM = (Env_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(Env_VM));
                xmlWriter.WriteAttributeString("env-Ref", env_VM.Env.Id.ToString());
            }
            else if (dragDrop_VM is Axiom_VM)
            {
                Axiom_VM axiom_VM = (Axiom_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(Axiom_VM));
                xmlWriter.WriteAttributeString("axiom-Ref", axiom_VM.Axiom.Id.ToString());
            }
            else if (dragDrop_VM is Proc_VM)
            {
                Proc_VM proc_VM = (Proc_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(Proc_VM));
                xmlWriter.WriteAttributeString("proc-Ref", proc_VM.Proc.Id.ToString());
            }
            else if (dragDrop_VM is IK_VM)
            {
                IK_VM ik_VM = (IK_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(IK_VM));
                xmlWriter.WriteAttributeString("ik-Ref", ik_VM.IK.Id.ToString());
            }
            else if (dragDrop_VM is Linker_VM)
            {
                Linker_VM linker_VM = (Linker_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(Linker_VM));
                xmlWriter.WriteAttributeString("id", linker_VM.Id.ToString());
                xmlWriter.WriteAttributeString("source-Ref", linker_VM.Source.Id.ToString());
                xmlWriter.WriteAttributeString("dest-Ref", linker_VM.Dest.Id.ToString());
            }
            else
            {
                throw new System.NotImplementedException();
            }

            // DDVM通用的部分（位置信息和周身锚点）
            XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
            foreach (Anchor_VM anchor_VM in dragDrop_VM.Anchor_VMs)
            {
                XmlWriteAnchor(xmlWriter, anchor_VM);
            }

            // 结尾符
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化拖拽VM的一些拖拽信息
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="dragDrop_VM"></param>
        private static void XmlWriteDragDropPos(XmlTextWriter xmlWriter, DragDrop_VM dragDrop_VM)
        {
            xmlWriter.WriteAttributeString("x", dragDrop_VM.Pos.X.ToString());
            xmlWriter.WriteAttributeString("y", dragDrop_VM.Pos.Y.ToString());
            //xmlWriter.WriteAttributeString("h", dragDrop_VM.H.ToString());
            //xmlWriter.WriteAttributeString("w", dragDrop_VM.W.ToString());
        }

        /// <summary>
        /// XML持久化锚点
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="anchor_VM"></param>
        private static void XmlWriteAnchor(XmlTextWriter xmlWriter, Anchor_VM anchor_VM)
        {
            string useLabel = nameof(Anchor_VM);
            if (anchor_VM is TopAnchor_VM)
            {
                useLabel = nameof(TopAnchor_VM);
            }
            else if (anchor_VM is BotAnchor_VM)
            {
                useLabel = nameof(BotAnchor_VM);
            }
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", anchor_VM.Id.ToString());
            XmlWriteDragDropPos(xmlWriter, anchor_VM);
            if (anchor_VM is BotAnchor_VM) // 多Linker
            {
                BotAnchor_VM botAnchor_VM = (BotAnchor_VM)anchor_VM;
                foreach (int id in botAnchor_VM.FetchLinkersIds())
                {
                    XmlWriteIdRef(xmlWriter, id, "Linker-Ref");
                }
            }
            else if (anchor_VM.LinkerVM is not null) // 单Linker
            {
                XmlWriteIdRef(xmlWriter, anchor_VM.LinkerVM.Id, "Linker-Ref");
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化仅Id的引用
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="id"></param>
        /// <param name="useLabel"></param>
        private static void XmlWriteIdRef(XmlTextWriter xmlWriter, int id, string useLabel)
        {
            xmlWriter.WriteStartElement(useLabel);

            xmlWriter.WriteAttributeString("id", id.ToString());

            xmlWriter.WriteEndElement();
        }

        #endregion
    }
}
