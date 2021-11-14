using Avalonia.Controls;
using Plat._M;
using Plat._VM;
using System.Collections.Generic;
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
            // Linux bugfix: 某些平台输入文件名不会自动补全后缀名,这里判断一下手动补上
            if (string.IsNullOrEmpty(result) || result.EndsWith($".{ ModelFilePostfix }"))
                return result;
            return result + $".{ ModelFilePostfix }";
        }

        /// <summary>
        /// 打开对话框，由用户选择要载入的模型文件并返回路径
        /// 在读取模型文件前执行的操作
        /// </summary>
        /// <returns></returns>
        public static async Task<string> OpenDialogAndGetLoadPathFromUser()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "SharpMS Model", Extensions = { ModelFilePostfix } });
            string[] result = await dialog.ShowAsync(ResourceManager.mainWindow_V);
            return result is null ? "" : string.Join(" ", result); // Linux bugfix: 直接关闭时不能返回null
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

            //
            // 进程图
            //
            #region ProcGraphs

            xmlWriter.WriteStartElement($"ProcGraphs");

            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.mainWindow_VM.ProcGraph_PG_VM.ProcGraph_P_VMs)
            {
                XmlWriteProcGraph(xmlWriter, procGraph_P_VM);
            }

            xmlWriter.WriteEndElement();

            #endregion

            //
            // 拓扑图
            //
            #region TopoGraphs

            xmlWriter.WriteStartElement($"TopoGraph");

            foreach (DragDrop_VM dragDrop_VM in ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs)
            {
                XmlWriteTopoGraphItem(xmlWriter, dragDrop_VM);
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
                XmlWriteLinker(xmlWriter, (Linker_VM)dragDrop_VM);
                return;
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

        /// <summary>
        /// XML持久化Linker及其子类
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="linker_VM"></param>
        /// <param name="useLable"></param>
        private static void XmlWriteLinker(XmlTextWriter xmlWriter, Linker_VM linker_VM, int? extId = null, string? extType = null)
        {
            if (linker_VM is Arrow_VM)
            {
                xmlWriter.WriteStartElement(nameof(Arrow_VM));
            }
            else
            {
                xmlWriter.WriteStartElement(nameof(Linker_VM));
            }

            xmlWriter.WriteAttributeString("id", linker_VM.Id.ToString());
            xmlWriter.WriteAttributeString("source-Ref", linker_VM.Source.Id.ToString());
            xmlWriter.WriteAttributeString("dest-Ref", linker_VM.Dest.Id.ToString());
            if (extId is not null)
            {
                xmlWriter.WriteAttributeString("extMsg-Ref", extId.ToString());
            }
            if (extType is not null)
            {
                xmlWriter.WriteAttributeString("extType", extType);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化进程图
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="procGraph_P_VM"></param>
        private static void XmlWriteProcGraph(XmlTextWriter xmlWriter, ProcGraph_P_VM procGraph_P_VM)
        {
            xmlWriter.WriteStartElement($"ProcGraph");

            xmlWriter.WriteAttributeString("proc-Ref", procGraph_P_VM.ProcGraph.Proc.Id.ToString());
            foreach (DragDrop_VM dragDrop_VM in procGraph_P_VM.DragDrop_VMs)
            {
                XmlWriteProcGraphItem(xmlWriter, dragDrop_VM);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Debug用
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="log"></param>
        private static void XmlWriteDebug(XmlTextWriter xmlWriter, string log = "None")
        {
            xmlWriter.WriteStartElement("Debug");
            xmlWriter.WriteAttributeString("log", log);
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// XML持久化进程图中的DD VM
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="dragDrop_VM"></param>
        private static void XmlWriteProcGraphItem(XmlTextWriter xmlWriter, DragDrop_VM dragDrop_VM)
        {
            // 每种DDVM特定的部分
            if (dragDrop_VM is TinyState_VM)
            {
                TinyState_VM tinyState_VM = (TinyState_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(TinyState_VM));
                xmlWriter.WriteAttributeString("id", tinyState_VM.State.Id.ToString());
                xmlWriter.WriteAttributeString("name", tinyState_VM.State.Name);
            }
            else if (dragDrop_VM is InitState_VM)
            {
                xmlWriter.WriteStartElement(nameof(InitState_VM));
            }
            else if (dragDrop_VM is FinalState_VM)
            {
                xmlWriter.WriteStartElement(nameof(FinalState_VM));
            }
            else if (dragDrop_VM is Arrow_VM)
            {
                Arrow_VM arrow_VM = (Arrow_VM)dragDrop_VM;
                TransNode_VM? extMsg = (TransNode_VM?)arrow_VM.ExtMsg;
                XmlWriteLinker(xmlWriter, arrow_VM, extId: extMsg?.LocTrans.Id, extType: nameof(TransNode_VM));
                return;
            }
            else if (dragDrop_VM is TransNode_VM)
            {
                TransNode_VM transNode_VM = (TransNode_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(TransNode_VM));
                xmlWriter.WriteAttributeString("id", transNode_VM.LocTrans.Id.ToString());
                xmlWriter.WriteAttributeString("attachedLinker-Ref", transNode_VM.AttachedLinker.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM); // bugfix: XML的Attr不能在子内容后面写
                XmlWriteFormula(xmlWriter, transNode_VM.LocTrans.Guard, useLabel: "Guard");
                foreach (Formula formula in transNode_VM.LocTrans.Actions)
                {
                    XmlWriteFormula(xmlWriter, formula, useLabel: "Action");
                }
                xmlWriter.WriteEndElement();
                return;
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
        /// XML持久化拓扑图中的DD VM
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="dragDrop_VM"></param>
        private static void XmlWriteTopoGraphItem(XmlTextWriter xmlWriter, DragDrop_VM dragDrop_VM)
        {
            if (dragDrop_VM is ProcInst_VM) // Proc Inst
            {
                ProcInst_VM procInst_VM = (ProcInst_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(ProcInst_VM));
                xmlWriter.WriteAttributeString("id", procInst_VM.ProcInst.Id.ToString());
                xmlWriter.WriteAttributeString("proc-Ref", procInst_VM.ProcInst.Proc?.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
                foreach (Instance instance in procInst_VM.ProcInst.Properties)
                {
                    XmlWriteInstance(xmlWriter, instance);
                }
                foreach (Anchor_VM anchor_VM in dragDrop_VM.Anchor_VMs)
                {
                    XmlWriteAnchor(xmlWriter, anchor_VM);
                }
                xmlWriter.WriteEndElement();
                return;
            }
            else if (dragDrop_VM is EnvInst_VM) // Env Inst
            {
                EnvInst_VM envInst_VM = (EnvInst_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(EnvInst_VM));
                xmlWriter.WriteAttributeString("id", envInst_VM.EnvInst.Id.ToString());
                xmlWriter.WriteAttributeString("env-Ref", envInst_VM.EnvInst.Env?.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
                foreach (Instance instance in envInst_VM.EnvInst.Properties)
                {
                    XmlWriteInstance(xmlWriter, instance);
                }
                foreach (Anchor_VM anchor_VM in dragDrop_VM.Anchor_VMs)
                {
                    XmlWriteAnchor(xmlWriter, anchor_VM);
                }
                xmlWriter.WriteEndElement();
                return;
            }
            else if (dragDrop_VM is ProcEnvInst_CT_VM) // Proc Env Inst
            {
                ProcEnvInst_CT_VM procEnvInst_CT_VM = (ProcEnvInst_CT_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(ProcEnvInst_CT_VM));
                xmlWriter.WriteAttributeString("id", procEnvInst_CT_VM.ProcEnvInst.Id.ToString());
                //xmlWriter.WriteAttributeString("procInst-Ref", procEnvInst_CT_VM.ProcEnvInst.ProcInst.Id.ToString());
                //xmlWriter.WriteAttributeString("envInst-Ref", procEnvInst_CT_VM.ProcEnvInst.EnvInst.Id.ToString());
                xmlWriter.WriteAttributeString("attachedLinker-Ref", procEnvInst_CT_VM.AttachedLinker.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
                foreach (PortChanInst portChanInst in procEnvInst_CT_VM.ProcEnvInst.PortChanInsts)
                {
                    xmlWriter.WriteStartElement(nameof(PortChanInst));
                    xmlWriter.WriteAttributeString("id", portChanInst.Id.ToString());
                    xmlWriter.WriteAttributeString("port-Ref", portChanInst.Port?.Id.ToString());
                    xmlWriter.WriteAttributeString("chan-Ref", portChanInst.Chan?.Id.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                return;
            }
            else if (dragDrop_VM is Linker_VM) // Linker
            {
                Linker_VM linker_VM = (Linker_VM)dragDrop_VM;
                ProcEnvInst_CT_VM? extMsg = (ProcEnvInst_CT_VM?)linker_VM.ExtMsg;
                XmlWriteLinker(xmlWriter, linker_VM, extId: extMsg?.ProcEnvInst.Id, extType: nameof(ProcEnvInst_CT_VM));
                return;
            }
            else if (dragDrop_VM is ProcInst_NT_VM) // Proc Node Tag
            {
                ProcInst_NT_VM procInst_NT_VM = (ProcInst_NT_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(ProcInst_NT_VM));
                xmlWriter.WriteAttributeString("procInst-Ref", procInst_NT_VM.ProcInst.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
                xmlWriter.WriteEndElement();
                return;
            }
            else if (dragDrop_VM is EnvInst_NT_VM) // Env Node Tag
            {
                EnvInst_NT_VM envInst_NT_VM = (EnvInst_NT_VM)dragDrop_VM;
                xmlWriter.WriteStartElement(nameof(EnvInst_NT_VM));
                xmlWriter.WriteAttributeString("envInst-Ref", envInst_NT_VM.EnvInst.Id.ToString());
                XmlWriteDragDropPos(xmlWriter, dragDrop_VM);
                xmlWriter.WriteEndElement();
                return;
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// XML持久化实例数据
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="instance"></param>
        private static void XmlWriteInstance(XmlTextWriter xmlWriter, Instance instance)
        {
            if (instance is ValueInstance)
            {
                xmlWriter.WriteStartElement(nameof(ValueInstance));
                XmlWriteInstanceCommonInfo(xmlWriter, instance);
                ValueInstance valueInstance = (ValueInstance)instance;
                xmlWriter.WriteAttributeString("value", valueInstance.Value);
            }
            else if (instance is ReferenceInstance)
            {
                xmlWriter.WriteStartElement(nameof(ReferenceInstance));
                XmlWriteInstanceCommonInfo(xmlWriter, instance);
                ReferenceInstance referenceInstance = (ReferenceInstance)instance;
                foreach (Instance subInst in referenceInstance.Properties)
                {
                    XmlWriteInstance(xmlWriter, subInst);
                }
            }
            else if (instance is ArrayInstance)
            {
                xmlWriter.WriteStartElement(nameof(ArrayInstance));
                XmlWriteInstanceCommonInfo(xmlWriter, instance);
                ArrayInstance arrayInstance = (ArrayInstance)instance;
                foreach (Instance itemInst in arrayInstance.ArrayItems)
                {
                    XmlWriteInstance(xmlWriter, itemInst);
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }
            xmlWriter.WriteEndElement();
        }
        
        /// <summary>
        /// XML持久化Instance这个父类的共用信息
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="instance"></param>
        private static void XmlWriteInstanceCommonInfo(XmlTextWriter xmlWriter, Instance instance)
        {
            xmlWriter.WriteAttributeString("type-Ref", instance.Type.Id.ToString());
            xmlWriter.WriteAttributeString("identifier", instance.Identifier);
            xmlWriter.WriteAttributeString("isArray", instance.IsArray.ToString());
        }

        #endregion

        #region XML Parsing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool LoadProjectModelFromXmlFile(string filePath)
        {
            // 传过来的Path一定非空，而且以指定的后缀名结尾
            Debug.Assert(filePath.EndsWith(ModelFilePostfix));

            // 生成XML读取器
            XmlDocument doc = new XmlDocument();
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(filePath);
                doc.Load(reader);
            }
            catch (System.Exception)
            {
                return false;
            }

            // 清除当前内存模型
            ResourceManager.ClearAllResource();

            // id -> 模型 映射表
            Dictionary<int, Axiom> axiomMap = new Dictionary<int, Axiom>();
            Dictionary<int, Channel> channelMap = new Dictionary<int, Channel>();
            Dictionary<int, Env> envMap = new Dictionary<int, Env>();
            Dictionary<int, AttrPair> attrPairMap = new Dictionary<int, AttrPair>();
            Dictionary<int, IK> ikMap = new Dictionary<int, IK>();
            Dictionary<int, Port> portMap = new Dictionary<int, Port>();
            Dictionary<int, Proc> procMap = new Dictionary<int, Proc>();
            Dictionary<int, State> stateMap = new Dictionary<int, State>();
            //Dictionary<int, LocTrans> locTransMap = new Dictionary<int, LocTrans>();
            Dictionary<int, TransNode_VM> transNode_VMMap = new Dictionary<int, TransNode_VM>();
            Dictionary<int, PortChanInst> portChanInstMap = new Dictionary<int, PortChanInst>();
            Dictionary<int, ProcEnvInst> procEnvInstMap = new Dictionary<int, ProcEnvInst>();
            Dictionary<int, TopoInst> topoInstMap = new Dictionary<int, TopoInst>();
            Dictionary<int, Type> typeMap = new Dictionary<int, Type>();
            Dictionary<int, Attribute> attrMap = new Dictionary<int, Attribute>();
            Dictionary<int, Caller> callerMap = new Dictionary<int, Caller>();
            // Dictionary<int, Formula> formulaMap = new Dictionary<int, Formula>();
            Dictionary<int, Anchor_VM> anchorMap = new Dictionary<int, Anchor_VM>();
            Dictionary<int, Linker_VM> linkerMap = new Dictionary<int, Linker_VM>();
            Dictionary<int, ProcInst_VM> procInst_VMMap = new Dictionary<int, ProcInst_VM>();
            Dictionary<int, EnvInst_VM> envInst_VMMap = new Dictionary<int, EnvInst_VM>();

            //
            // 数据类型
            //
            #region MetaInfo-Types

            XmlNode? typesRoot = doc.SelectSingleNode($"SharpMS/MetaInfo-{nameof(Type)}s");
            if (typesRoot is null)
            {
                return false;
            }
            foreach (XmlNode typeNode in typesRoot.ChildNodes)
            {
                XmlElement typeElement = (XmlElement)typeNode;
                ParseTypeObj(typeElement, typeMap);
            }
            foreach (XmlNode typeNode in typesRoot.ChildNodes)
            {
                XmlElement typeElement = (XmlElement)typeNode;
                ParseTypeInfo(typeElement, typeMap, attrMap);
            }

            #endregion

            //
            // 环境模板
            //
            #region MetaInfo-Envs

            XmlNode? envsRoot = doc.SelectSingleNode($"SharpMS/MetaInfo-{nameof(Env)}s");
            if (envsRoot is null)
            {
                return false;
            }
            foreach (XmlNode envNode in envsRoot.ChildNodes)
            {
                XmlElement envElement = (XmlElement)envNode;
                ParseEnvObj(envElement, envMap);
            }
            foreach (XmlNode envNode in envsRoot.ChildNodes)
            {
                XmlElement envElement = (XmlElement)envNode;
                ParseEnvInfo(envElement, typeMap, envMap, channelMap, attrMap);
            }

            #endregion

            //
            // 进程模板
            //
            #region MetaInfo-Procs

            XmlNode? procsRoot = doc.SelectSingleNode($"SharpMS/MetaInfo-{nameof(Proc)}s");
            if (procsRoot is null)
            {
                return false;
            }
            foreach (XmlNode procNode in procsRoot.ChildNodes)
            {
                XmlElement procElement = (XmlElement)procNode;
                ParseProcObj(procElement, procMap);
            }
            foreach (XmlNode procNode in procsRoot.ChildNodes)
            {
                XmlElement procElement = (XmlElement)procNode;
                ParseProcInfo(procElement, typeMap, portMap, procMap, attrMap);
            }

            #endregion

            //
            // 初始知识
            //
            #region MetaInfo-IKs

            XmlNode? iksRoot = doc.SelectSingleNode($"SharpMS/MetaInfo-{nameof(IK)}s");
            if (iksRoot is null)
            {
                return false;
            }
            foreach (XmlNode ikNode in iksRoot.ChildNodes)
            {
                XmlElement ikElement = (XmlElement)ikNode;
                ParseIK(ikElement, typeMap, procMap, envMap, ikMap, attrPairMap, attrMap);
            }

            #endregion

            //
            // 公理
            //
            #region MetaInfo-Axioms

            XmlNode? axiomsRoot = doc.SelectSingleNode($"SharpMS/MetaInfo-{nameof(Axiom)}s");
            if (axiomsRoot is null)
            {
                return false;
            }
            foreach (XmlNode axiomNode in axiomsRoot.ChildNodes)
            {
                XmlElement axiomElement = (XmlElement)axiomNode;
                ParseAxiom(axiomElement, axiomMap);
            }

            #endregion

            //
            // 类图
            //
            #region Class Diagram

            XmlNode? classDiagRoot = doc.SelectSingleNode($"SharpMS/ClassDiagram");
            if (classDiagRoot is null)
            {
                return false;
            }
            foreach (XmlNode vmNode in classDiagRoot.ChildNodes)
            {
                XmlElement vmElement = (XmlElement)vmNode;
                switch (vmElement.Name)
                {
                    case nameof(Type_VM):
                        ParseType_VM(vmElement, typeMap, anchorMap);
                        break;
                    case nameof(Axiom_VM):
                        ParseAxiom_VM(vmElement, axiomMap);
                        break;
                    case nameof(Env_VM):
                        ParseEnv_VM(vmElement, envMap, anchorMap);
                        break;
                    case nameof(Proc_VM):
                        ParseProc_VM(vmElement, procMap, anchorMap);
                        break;
                    case nameof(IK_VM):
                        ParseIK_VM(vmElement, ikMap);
                        break;
                    case nameof(Linker_VM):
                        ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add
                            (
                            ParseLinkerObj(vmElement, anchorMap, linkerMap, ResourceManager.mainWindow_VM.ClassDiagram_P_VM)
                            );
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            #endregion

            //
            // 进程图状态机
            //
            #region Process Graph

            XmlNode? procGraphsRoot = doc.SelectSingleNode($"SharpMS/ProcGraphs");
            if (procGraphsRoot is null)
            {
                return false;
            }
            foreach (XmlElement procGraphElement in procGraphsRoot.ChildNodes)
            {
                Debug.Assert(procGraphElement.Name == nameof(ProcGraph));
                int procId = int.Parse(procGraphElement.GetAttribute("proc-Ref"));
                ProcGraph procGraph = new ProcGraph(procMap[procId]);
                ProcGraph_P_VM procGraph_P_VM = new ProcGraph_P_VM(procGraph);
                foreach (XmlElement subElement in procGraphElement.ChildNodes)
                {
                    switch (subElement.Name)
                    {
                        case nameof(InitState_VM):
                            ParseInitState_VM(subElement, procGraph_P_VM, anchorMap);
                            break;
                        case nameof(TinyState_VM):
                            ParseTinyState_VM(subElement, procGraph_P_VM, anchorMap);
                            break;
                        case nameof(FinalState_VM):
                            ParseFinalState_VM(subElement, procGraph_P_VM, anchorMap);
                            break;
                        case nameof(Arrow_VM):
                            procGraph_P_VM.DragDrop_VMs.Add(
                                ParseLinkerObj(subElement, anchorMap, linkerMap, procGraph_P_VM)
                                );
                            break;
                        case nameof(TransNode_VM):
                            ParseTransNode_VM(subElement, procGraph_P_VM, linkerMap, transNode_VMMap);
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
                foreach (XmlElement subElement in procGraphElement.ChildNodes) // 单独处理extMsg-Ref
                {
                    if (subElement.Name == nameof(Arrow_VM))
                    {
                        int id = int.Parse(subElement.GetAttribute(nameof(id)));
                        int extMsgId = int.Parse(subElement.GetAttribute("extMsg-Ref"));
                        Arrow_VM arrow_VM = (Arrow_VM)linkerMap[id];
                        arrow_VM.ExtMsg = transNode_VMMap[extMsgId];
                    }
                }
                ResourceManager.mainWindow_VM.ProcGraph_PG_VM.ProcGraph_P_VMs.Add(procGraph_P_VM);
            }

            #endregion

            //
            // 拓扑图
            //
            #region Topology Graph

            XmlNode? topoGraphNode = doc.SelectSingleNode($"SharpMS/TopoGraph");
            if (topoGraphNode is null)
            {
                return false;
            }
            foreach (XmlElement subElement in topoGraphNode.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case nameof(ProcInst_VM):
                        ParseProcInst_VM(subElement, procInst_VMMap, procMap, anchorMap, typeMap);
                        break;
                    case nameof(ProcInst_NT_VM):
                        ParseProcInst_NT_VM(subElement, procInst_VMMap);
                        break;
                    case nameof(EnvInst_VM):
                        ParseEnvInst_VM(subElement, envInst_VMMap, envMap, anchorMap, typeMap);
                        break;
                    case nameof(EnvInst_NT_VM):
                        ParseEnvInst_NT_VM(subElement, envInst_VMMap);
                        break;
                    case nameof(Linker_VM):
                        ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(
                            ParseLinkerObj(subElement, anchorMap, linkerMap, ResourceManager.mainWindow_VM.TopoGraph_P_VM)
                            );
                        break;
                    case nameof(ProcEnvInst_CT_VM):
                        ParseProcEnvInst_CT_VM(subElement, linkerMap, portMap, channelMap);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            #endregion

            //
            // 全局_id
            //
            #region Global Id

            XmlNode? sidsNode = doc.SelectSingleNode($"SharpMS/Static-Ids");
            if (sidsNode is null)
            {
                return false;
            }
            foreach (XmlElement sidElement in sidsNode.ChildNodes)
            {
                ParseSID(sidElement);
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 解析Type（仅对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        private static void ParseTypeObj(XmlElement element, Dictionary<int, Type> typeMap)
        {
            Debug.Assert(element.Name == nameof(Type));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            string description = element.GetAttribute(nameof(description));
            bool isBase = bool.Parse(element.GetAttribute(nameof(isBase)));
            Type type = new Type(identifier, description, isBase) { Id = id };

            typeMap[id] = type;
            ResourceManager.types.Add(type);
        }

        /// <summary>
        /// 解析Type（完整信息
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        private static void ParseTypeInfo(XmlElement element,
            Dictionary<int, Type> typeMap,
            Dictionary<int, Attribute> attrMap)
        {
            Debug.Assert(element.Name == nameof(Type));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            Type curType = typeMap[id];
            // Parent
            int? parentId = ParseIntNullAttr(element, "parent-Ref");
            if (parentId is not null)
            {
                curType.Parent = typeMap[(int)parentId];
            }
            // Attribute & Method
            foreach (XmlNode attrNode in element.ChildNodes)
            {
                XmlElement subElement = (XmlElement)attrNode;
                switch (subElement.Name)
                {
                    case nameof(Attribute):
                        curType.Attributes.Add(ParseAttribute(subElement, typeMap, attrMap));
                        break;
                    case "Method":
                        curType.Methods.Add(ParseCaller(subElement, typeMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 解析一个可能为空的数值属性
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private static int? ParseIntNullAttr(XmlElement element, string attr)
        {
            string valStr = element.GetAttribute(attr);
            if (string.IsNullOrEmpty(valStr))
            {
                return null;
            }
            return int.Parse(valStr);
        }

        /// <summary>
        /// 解析Attribute
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <returns></returns>
        private static Attribute ParseAttribute(XmlElement element,
            Dictionary<int, Type> typeMap,
            Dictionary<int, Attribute> attrMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            int typeId = int.Parse(element.GetAttribute("type-Ref"));
            bool isArray = bool.Parse(element.GetAttribute(nameof(isArray)));
            string description = element.GetAttribute(nameof(description));
            switch (element.Name)
            {
                case nameof(Attribute):
                    Attribute attribute = new Attribute(identifier, typeMap[typeId], isArray, description) { Id = id };
                    attrMap[id] = attribute;
                    return attribute;
                case nameof(VisAttr):
                    bool pub = bool.Parse(element.GetAttribute(nameof(pub)));
                    VisAttr visAttr = new VisAttr(identifier, typeMap[typeId], isArray, pub, description) { Id = id };
                    attrMap[id] = visAttr;
                    return visAttr;
                case nameof(ValAttr):
                    string value = element.GetAttribute(nameof(value));
                    ValAttr valAttr = new ValAttr(identifier, typeMap[typeId], isArray, value, description) { Id = id };
                    attrMap[id] = valAttr;
                    return valAttr;
                default:
                    throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// 解析Caller（Method或者Function
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <returns></returns>
        private static Caller ParseCaller(XmlElement element, Dictionary<int, Type> typeMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            int typeId = int.Parse(element.GetAttribute("returnType-Ref"));
            string description = element.GetAttribute(nameof(description));
            Caller caller = new Caller(identifier, typeMap[typeId], description) { Id = id };
            foreach (XmlNode paramNode in element.ChildNodes) // ParamType
            {
                XmlElement paramElement = (XmlElement)paramNode;
                int paramTypeId = int.Parse(paramElement.GetAttribute("type-Ref"));
                caller.ParamTypes.Add(typeMap[paramTypeId]);
            }
            return caller;
        }

        /// <summary>
        /// 解析Env（仅对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="envMap"></param>
        private static void ParseEnvObj(XmlElement element, Dictionary<int, Env> envMap)
        {
            Debug.Assert(element.Name == nameof(Env));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            string description = element.GetAttribute(nameof(description));
            bool pub = bool.Parse(element.GetAttribute(nameof(pub)));
            Env env = new Env(identifier, pub, description) { Id = id };

            envMap[id] = env;
            ResourceManager.envs.Add(env);
        }

        /// <summary>
        /// 解析Env（完整信息
        /// </summary>
        /// <param name="element"></param>
        /// <param name="envMap"></param>
        private static void ParseEnvInfo(XmlElement element, 
            Dictionary<int, Type> typeMap, 
            Dictionary<int, Env> envMap,
            Dictionary<int, Channel> channelMap,
            Dictionary<int, Attribute> attrMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            Env env = envMap[id];
            // Parent
            int? parentId = ParseIntNullAttr(element, "parent-Ref");
            if (parentId is not null)
            {
                env.Parent = envMap[(int)parentId];
            }
            // VisAttr and Channel
            foreach (XmlNode subNode in element.ChildNodes)
            {
                XmlElement subElement = (XmlElement)subNode;
                switch (subElement.Name)
                {
                    case nameof(VisAttr):
                        env.Attributes.Add((VisAttr)ParseAttribute(subElement, typeMap, attrMap));
                        break;
                    case nameof(Channel):
                        env.Channels.Add(ParseChannel(subElement, channelMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 解析Channel
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Channel ParseChannel(XmlElement element, Dictionary<int, Channel> channelMap)
        {
            Debug.Assert(element.Name == nameof(Channel));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            bool pub = bool.Parse(element.GetAttribute(nameof(pub)));
            int capacity = int.Parse(element.GetAttribute(nameof(capacity)));
            string description = element.GetAttribute(nameof(description));

            Channel channel = new Channel(identifier, capacity, pub, description) { Id = id };
            channelMap[id] = channel;

            return channel;
        }

        /// <summary>
        /// 解析Proc（仅对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procMap"></param>
        private static void ParseProcObj(XmlElement element, Dictionary<int, Proc> procMap)
        {
            Debug.Assert(element.Name == nameof(Proc));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            string description = element.GetAttribute(nameof(description));
            Proc proc = new Proc(identifier, description) { Id = id };

            procMap[id] = proc;
            ResourceManager.procs.Add(proc);
        }

        /// <summary>
        /// 解析Proc（完整信息
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <param name="portMap"></param>
        /// <param name="procMap"></param>
        private static void ParseProcInfo(XmlElement element,
            Dictionary<int, Type> typeMap,
            Dictionary<int, Port> portMap,
            Dictionary<int, Proc> procMap,
            Dictionary<int, Attribute> attrMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            Proc proc = procMap[id];
            // Parent
            int? parentId = ParseIntNullAttr(element, "parent-Ref");
            if (parentId is not null)
            {
                proc.Parent = procMap[(int)parentId];
            }
            // VisAttr, Method and Port
            foreach (XmlNode subNode in element.ChildNodes)
            {
                XmlElement subElement = (XmlElement)subNode;
                switch (subElement.Name)
                {
                    case nameof(VisAttr):
                        proc.Attributes.Add((VisAttr)ParseAttribute(subElement, typeMap, attrMap));
                        break;
                    case "Method":
                        proc.Methods.Add(ParseCaller(subElement, typeMap));
                        break;
                    case nameof(Port):
                        proc.Ports.Add(ParsePort(subElement, portMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 解析Port
        /// </summary>
        /// <param name="element"></param>
        /// <param name="portMap"></param>
        /// <returns></returns>
        private static Port ParsePort(XmlElement element, Dictionary<int, Port> portMap)
        {
            Debug.Assert(element.Name == nameof(Port));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            bool isOut = bool.Parse(element.GetAttribute(nameof(isOut)));
            string description = element.GetAttribute(nameof(description));

            Port port = new Port(identifier, isOut, description) { Id = id };
            portMap[id] = port;

            return port;
        }

        /// <summary>
        /// 解析IK
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ikMap"></param>
        /// <param name="attrPairMap"></param>
        private static void ParseIK(XmlElement element,
            Dictionary<int, Type> typeMap,
            Dictionary<int, Proc> procMap,
            Dictionary<int, Env> envMap,
            Dictionary<int, IK> ikMap,
            Dictionary<int, AttrPair> attrPairMap,
            Dictionary<int, Attribute> attrMap)
        {
            Debug.Assert(element.Name == nameof(IK));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            string description = element.GetAttribute(nameof(description));
            IK ik = new IK(identifier, description) { Id = id };

            ikMap[id] = ik;
            ResourceManager.iks.Add(ik);

            foreach (XmlNode subNode in element.ChildNodes)
            {
                XmlElement subElement = (XmlElement)subNode;
                switch (subElement.Name)
                {
                    case nameof(ValAttr):
                        ik.Attributes.Add((ValAttr)ParseAttribute(subElement, typeMap, attrMap));
                        break;
                    case nameof(AttrPair):
                        ik.AttrPairs.Add(ParseAttrPair(subElement, procMap, envMap, attrMap, attrPairMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 解析AttrPair
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procMap"></param>
        /// <param name="envMap"></param>
        /// <param name="attrMap"></param>
        /// <param name="attrPairMap"></param>
        /// <returns></returns>
        private static AttrPair ParseAttrPair(XmlElement element,
            Dictionary<int, Proc> procMap,
            Dictionary<int, Env> envMap,
            Dictionary<int, Attribute> attrMap,
            Dictionary<int, AttrPair> attrPairMap)
        {
            Debug.Assert(element.Name == nameof(AttrPair));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            int? procAId = ParseIntNullAttr(element, "procA-Ref");
            int? procAttrAId = ParseIntNullAttr(element, "procAttrA-Ref");
            int? procBId = ParseIntNullAttr(element, "procB-Ref");
            int? procAttrBId = ParseIntNullAttr(element, "procAttrB-Ref");
            int? envAId = ParseIntNullAttr(element, "envA-Ref");
            int? envAttrAId = ParseIntNullAttr(element, "envAttrA-Ref");
            int? envBId = ParseIntNullAttr(element, "envB-Ref");
            int? envAttrBId = ParseIntNullAttr(element, "envAttrB-Ref");
            AttrPair attrPair;
            if (procAId is not null &&
                procAttrAId is not null &&
                procBId is not null &&
                procAttrBId is not null) // P-P型
            {
                attrPair = new AttrPair(
                    procMap[(int)procAId],
                    (VisAttr)attrMap[(int)procAttrAId],
                    procMap[(int)procBId],
                    (VisAttr)attrMap[(int)procAttrBId])
                {
                    Id = id
                };
            }
            else if (procAId is not null &&
                procAttrAId is not null &&
                envBId is not null &&
                envAttrBId is not null) // P-E型
            {
                attrPair = new AttrPair(
                    procMap[(int)procAId],
                    (VisAttr)attrMap[(int)procAttrAId],
                    envMap[(int)envBId],
                    (VisAttr)attrMap[(int)envAttrBId]
                    )
                {
                    Id = id
                };
            }
            else if (envAId is not null &&
                envAttrAId is not null &&
                envBId is not null &&
                envAttrBId is not null) // E-E型
            {
                attrPair = new AttrPair(
                    envMap[(int)envAId],
                    (VisAttr)attrMap[(int)envAttrAId],
                    envMap[(int)envBId],
                    (VisAttr)attrMap[(int)envAttrBId]
                    )
                {
                    Id = id
                };
            }
            else
            {
                throw new System.NotImplementedException();
            }

            attrPairMap[id] = attrPair;
            return attrPair;
        }

        /// <summary>
        /// 解析Axiom
        /// </summary>
        /// <param name="element"></param>
        /// <param name="axiomMap"></param>
        private static void ParseAxiom(XmlElement element, Dictionary<int, Axiom> axiomMap)
        {
            Debug.Assert(element.Name == nameof(Axiom));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string identifier = element.GetAttribute(nameof(identifier));
            string description = element.GetAttribute(nameof(description));
            Axiom axiom = new Axiom(identifier, description) { Id = id };

            axiomMap[id] = axiom;
            ResourceManager.axioms.Add(axiom);

            foreach (XmlNode subNode in element.ChildNodes)
            {
                XmlElement subElement = (XmlElement)subNode;
                axiom.Formulas.Add(ParseFormula(subElement));
            }
        }

        /// <summary>
        /// 解析Formula
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Formula ParseFormula(XmlElement element)
        {
            Debug.Assert(element.Name == nameof(Formula)
                || element.Name == "Guard"
                || element.Name == "Action");

            int id = int.Parse(element.GetAttribute(nameof(id)));
            string content = element.GetAttribute(nameof(content));
            string description = element.GetAttribute(nameof(description));

            return new Formula(content, description) { Id = id };
        }

        /// <summary>
        /// 解析Type_VM
        /// </summary>
        /// <param name="element"></param>
        private static void ParseType_VM(XmlElement element,
            Dictionary<int, Type> typeMap,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            int typeId = int.Parse(element.GetAttribute("type-Ref"));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));
            Type_VM type_VM = new Type_VM(x, y, ResourceManager.mainWindow_VM.ClassDiagram_P_VM, typeMap[typeId]);

            type_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                type_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, type_VM, anchorMap));
            }
            ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add(type_VM);
        }

        /// <summary>
        /// 解析Anchor_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="axiomMap"></param>
        private static void ParseAxiom_VM(XmlElement element, Dictionary<int, Axiom> axiomMap)
        {
            int axiomId = int.Parse(element.GetAttribute("axiom-Ref"));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));
            Axiom_VM axiom_VM = new Axiom_VM(x, y, ResourceManager.mainWindow_VM.ClassDiagram_P_VM, axiomMap[axiomId]);

            ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add(axiom_VM);
        }

        /// <summary>
        /// 解析Env_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="envMap"></param>
        /// <param name="anchorMap"></param>
        private static void ParseEnv_VM(XmlElement element,
            Dictionary<int, Env> envMap,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            int envId = int.Parse(element.GetAttribute("env-Ref"));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));
            Env_VM env_VM = new Env_VM(x, y, ResourceManager.mainWindow_VM.ClassDiagram_P_VM, envMap[envId]);

            env_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                env_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, env_VM, anchorMap));
            }
            ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add(env_VM);
        }

        /// <summary>
        /// 解析Proc_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procMap"></param>
        /// <param name="anchorMap"></param>
        private static void ParseProc_VM(XmlElement element,
            Dictionary<int, Proc> procMap,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            int procId = int.Parse(element.GetAttribute("proc-Ref"));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));
            Proc_VM proc_VM = new Proc_VM(x, y, ResourceManager.mainWindow_VM.ClassDiagram_P_VM, procMap[procId]);

            proc_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                proc_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, proc_VM, anchorMap));
            }
            ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add(proc_VM);
        }

        /// <summary>
        /// 解析IK_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ikMap"></param>
        private static void ParseIK_VM(XmlElement element,
            Dictionary<int, IK> ikMap)
        {
            int ikId = int.Parse(element.GetAttribute("ik-Ref"));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));
            IK_VM ik_VM = new IK_VM(x, y, ResourceManager.mainWindow_VM.ClassDiagram_P_VM, ikMap[ikId]);

            ResourceManager.mainWindow_VM.ClassDiagram_P_VM.DragDrop_VMs.Add(ik_VM);
        }

        /// <summary>
        /// 解析锚点（仅对象，不考虑其维护的Linker引用
        /// </summary>
        /// <param name="element"></param>
        /// <param name="hostVM"></param>
        /// <param name="anchorMap"></param>
        /// <returns></returns>
        private static Anchor_VM ParseAnchorObj(XmlElement element,
            DragDrop_VM hostVM,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            Debug.Assert(element.Name == nameof(Anchor_VM)
                || element.Name == nameof(TopAnchor_VM)
                || element.Name == nameof(BotAnchor_VM));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            double x = double.Parse(element.GetAttribute("x"));
            double y = double.Parse(element.GetAttribute("y"));

            switch (element.Name)
            {
                case nameof(Anchor_VM):
                    Anchor_VM anchor_VM = new Anchor_VM(x, y, hostVM) { Id = id };
                    return anchorMap[id] = anchor_VM;
                case nameof(TopAnchor_VM):
                    TopAnchor_VM topAnchor_VM = new TopAnchor_VM(x, y, hostVM) { Id = id };
                    return anchorMap[id] = topAnchor_VM;
                case nameof(BotAnchor_VM):
                    BotAnchor_VM botAnchor_VM = new BotAnchor_VM(x, y, hostVM) { Id = id };
                    return anchorMap[id] = botAnchor_VM;
                default:
                    throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// 解析连线
        /// </summary>
        /// <param name="element"></param>
        /// <param name="anchorMap"></param>
        /// <param name="linkerMap"></param>
        /// <returns></returns>
        private static Linker_VM ParseLinkerObj(XmlElement element,
            Dictionary<int, Anchor_VM> anchorMap,
            Dictionary<int, Linker_VM> linkerMap,
            DragDrop_P_VM dragDrop_P_VM)
        {
            Debug.Assert(element.Name == nameof(Linker_VM)
                || element.Name == nameof(Arrow_VM));

            int id = int.Parse(element.GetAttribute(nameof(id)));
            int srcId = int.Parse(element.GetAttribute("source-Ref"));
            int destId = int.Parse(element.GetAttribute("dest-Ref"));

            switch (element.Name)
            {
                case nameof(Linker_VM):
                    Linker_VM linker_VM = new Linker_VM(anchorMap[srcId], anchorMap[destId], dragDrop_P_VM) { Id = id };
                    linkerMap[id] = linker_VM;
                    ReBuildAnchorLinkerRelation(linker_VM, anchorMap[srcId]);
                    ReBuildAnchorLinkerRelation(linker_VM, anchorMap[destId]);
                    return linker_VM;
                case nameof(Arrow_VM):
                    Arrow_VM arrow_VM = new Arrow_VM(anchorMap[srcId], anchorMap[destId], dragDrop_P_VM) { Id = id };
                    linkerMap[id] = arrow_VM;
                    ReBuildAnchorLinkerRelation(arrow_VM, anchorMap[srcId]);
                    ReBuildAnchorLinkerRelation(arrow_VM, anchorMap[destId]);
                    return arrow_VM;
                default:
                    throw new System.NotImplementedException();
            }
        }
        
        /// <summary>
        /// 为Linker_VM和Anchor_VM重建互引关系
        /// </summary>
        /// <param name="linker_VM"></param>
        /// <param name="anchor_VM"></param>
        private static void ReBuildAnchorLinkerRelation(Linker_VM linker_VM, Anchor_VM anchor_VM)
        {
            if (anchor_VM is BotAnchor_VM)
            {
                BotAnchor_VM botAnchor_VM = (BotAnchor_VM)anchor_VM;
                botAnchor_VM.AddLinker(linker_VM);
            }
            else
            {
                anchor_VM.LinkerVM = linker_VM;
            }
        }

        /// <summary>
        /// 解析InitState_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procGraph_P_VM"></param>
        private static void ParseInitState_VM(XmlElement element,
            ProcGraph_P_VM procGraph_P_VM,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            InitState_VM initState_VM = new InitState_VM(x, y, procGraph_P_VM);

            initState_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                initState_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, initState_VM, anchorMap));
            }

            procGraph_P_VM.DragDrop_VMs.Add(initState_VM);
        }

        /// <summary>
        /// 解析TinyState_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procGraph_P_VM"></param>
        /// <param name="anchorMap"></param>
        private static void ParseTinyState_VM(XmlElement element,
            ProcGraph_P_VM procGraph_P_VM,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            string name = element.GetAttribute(nameof(name));
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            TinyState_VM tinyState_VM = new TinyState_VM(x, y, procGraph_P_VM);
            tinyState_VM.State.Name = name;
            tinyState_VM.State.Id = id;

            tinyState_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                tinyState_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, tinyState_VM, anchorMap));
            }

            procGraph_P_VM.DragDrop_VMs.Add(tinyState_VM);
        }

        /// <summary>
        /// 解析FinalState_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procGraph_P_VM"></param>
        /// <param name="anchorMap"></param>
        private static void ParseFinalState_VM(XmlElement element,
            ProcGraph_P_VM procGraph_P_VM,
            Dictionary<int, Anchor_VM> anchorMap)
        {
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            FinalState_VM finalState_VM = new FinalState_VM(x, y, procGraph_P_VM);

            finalState_VM.Anchor_VMs.Clear();
            foreach (XmlElement subElement in element.ChildNodes)
            {
                finalState_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, finalState_VM, anchorMap));
            }

            procGraph_P_VM.DragDrop_VMs.Add(finalState_VM);
        }

        /// <summary>
        /// 解析TransNode_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procGraph_P_VM"></param>
        /// <param name="linkerMap"></param>
        /// <param name="transNode_VMMap"></param>
        private static void ParseTransNode_VM(XmlElement element,
            ProcGraph_P_VM procGraph_P_VM,
            Dictionary<int, Linker_VM> linkerMap,
            Dictionary<int, TransNode_VM> transNode_VMMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            int attachedLinkerId = int.Parse(element.GetAttribute("attachedLinker-Ref"));
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));

            TransNode_VM transNode_VM = new TransNode_VM(x, y, procGraph_P_VM, linkerMap[attachedLinkerId]);
            transNode_VM.LocTrans.Id = id;

            foreach (XmlElement subElement in element.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case "Guard":
                        transNode_VM.LocTrans.Guard = ParseFormula(subElement);
                        break;
                    case "Action":
                        transNode_VM.LocTrans.Actions.Add(ParseFormula(subElement));
                        break;
                    default:
                        break;
                }
            }

            transNode_VMMap[id] = transNode_VM;
            procGraph_P_VM.DragDrop_VMs.Add(transNode_VM);
        }

        /// <summary>
        /// 解析ProcInst_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procInst_VMMap"></param>
        private static void ParseProcInst_VM(XmlElement element,
            Dictionary<int, ProcInst_VM> procInst_VMMap,
            Dictionary<int, Proc> procMap,
            Dictionary<int, Anchor_VM> anchorMap,
            Dictionary<int, Type> typeMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            int? procId = ParseIntNullAttr(element, "proc-Ref");
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            ProcInst_VM procInst_VM = new ProcInst_VM(x, y, ResourceManager.mainWindow_VM.TopoGraph_P_VM);
            procInst_VM.Anchor_VMs.Clear();
            procInst_VM.ProcInst.Id = id;
            if (procId is not null)
            {
                procInst_VM.ProcInst.Proc = procMap[(int)procId];
            }
            procInst_VM.ProcInst.Properties.Clear();

            foreach (XmlElement subElement in element.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case nameof(ArrayInstance):
                        procInst_VM.ProcInst.Properties.Add(ParseArrayInstance(subElement, typeMap));
                        break;
                    case nameof(ReferenceInstance):
                        procInst_VM.ProcInst.Properties.Add(ParseReferenceInstance(subElement, typeMap));
                        break;
                    case nameof(ValueInstance):
                        procInst_VM.ProcInst.Properties.Add(ParseValueInstance(subElement, typeMap));
                        break;
                    case nameof(TopAnchor_VM):
                        procInst_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, procInst_VM, anchorMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            procInst_VMMap[id] = procInst_VM;
            ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(procInst_VM);
        }

        /// <summary>
        /// 解析ProcInst_NT_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="procInst_VMMap"></param>
        private static void ParseProcInst_NT_VM(XmlElement element,
            Dictionary<int, ProcInst_VM> procInst_VMMap)
        {
            int procInstId = int.Parse(element.GetAttribute("procInst-Ref"));
            ProcInst_VM procInst_VM = procInst_VMMap[procInstId];
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            ProcInst_NT_VM procInst_NT_VM = new ProcInst_NT_VM(x, y, ResourceManager.mainWindow_VM.TopoGraph_P_VM, procInst_VM.ProcInst);
            procInst_VM.ExtMsg = procInst_NT_VM;
            ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(procInst_NT_VM);
        }

        /// <summary>
        /// 解析EnvInst_NT_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="envInst_VMMap"></param>
        private static void ParseEnvInst_NT_VM(XmlElement element,
            Dictionary<int, EnvInst_VM> envInst_VMMap)
        {
            int envInstId = int.Parse(element.GetAttribute("envInst-Ref"));
            EnvInst_VM envInst_VM = envInst_VMMap[envInstId];
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            EnvInst_NT_VM envInst_NT_VM = new EnvInst_NT_VM(x, y, ResourceManager.mainWindow_VM.TopoGraph_P_VM, envInst_VM.EnvInst);
            envInst_VM.ExtMsg = envInst_NT_VM;
            ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(envInst_NT_VM);
        }

        /// <summary>
        /// 解析EnvInst_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="envInst_VMMap"></param>
        /// <param name="envMap"></param>
        /// <param name="anchorMap"></param>
        /// <param name="typeMap"></param>
        private static void ParseEnvInst_VM(XmlElement element,
            Dictionary<int, EnvInst_VM> envInst_VMMap,
            Dictionary<int, Env> envMap,
            Dictionary<int, Anchor_VM> anchorMap,
            Dictionary<int, Type> typeMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            int? envId = ParseIntNullAttr(element, "env-Ref");
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            EnvInst_VM envInst_VM = new EnvInst_VM(x, y, ResourceManager.mainWindow_VM.TopoGraph_P_VM);
            envInst_VM.Anchor_VMs.Clear();
            envInst_VM.EnvInst.Id = id;
            if (envId is not null)
            {
                envInst_VM.EnvInst.Env = envMap[(int)envId];
            }
            envInst_VM.EnvInst.Properties.Clear();

            foreach (XmlElement subElement in element.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case nameof(ArrayInstance):
                        envInst_VM.EnvInst.Properties.Add(ParseArrayInstance(subElement, typeMap));
                        break;
                    case nameof(ReferenceInstance):
                        envInst_VM.EnvInst.Properties.Add(ParseReferenceInstance(subElement, typeMap));
                        break;
                    case nameof(ValueInstance):
                        envInst_VM.EnvInst.Properties.Add(ParseValueInstance(subElement, typeMap));
                        break;
                    case nameof(BotAnchor_VM):
                        envInst_VM.Anchor_VMs.Add(ParseAnchorObj(subElement, envInst_VM, anchorMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            envInst_VMMap[id] = envInst_VM;
            ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(envInst_VM);
        }

        /// <summary>
        /// 解析ArrayInstance
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <returns></returns>
        private static ArrayInstance ParseArrayInstance(XmlElement element, Dictionary<int, Type> typeMap)
        {
            int typeId = int.Parse(element.GetAttribute("type-Ref"));
            string identifier = element.GetAttribute(nameof(identifier));
            ArrayInstance arrayInstance = new ArrayInstance(typeMap[typeId], identifier, true);
            foreach (XmlElement subElement in element.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case nameof(ReferenceInstance):
                        arrayInstance.ArrayItems.Add(ParseReferenceInstance(subElement, typeMap));
                        break;
                    case nameof(ValueInstance):
                        arrayInstance.ArrayItems.Add(ParseValueInstance(subElement, typeMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
            return arrayInstance;
        }

        /// <summary>
        /// 解析ReferenceInstance
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <returns></returns>
        private static ReferenceInstance ParseReferenceInstance(XmlElement element, Dictionary<int, Type> typeMap)
        {
            int typeId = int.Parse(element.GetAttribute("type-Ref"));
            string identifier = element.GetAttribute(nameof(identifier));
            ReferenceInstance referenceInstance = new ReferenceInstance(typeMap[typeId], identifier, false);
            foreach (XmlElement subElement in element.ChildNodes)
            {
                switch (subElement.Name)
                {
                    case nameof(ReferenceInstance):
                        referenceInstance.Properties.Add(ParseReferenceInstance(subElement, typeMap));
                        break;
                    case nameof(ValueInstance):
                        referenceInstance.Properties.Add(ParseValueInstance(subElement, typeMap));
                        break;
                    case nameof(ArrayInstance):
                        referenceInstance.Properties.Add(ParseArrayInstance(subElement, typeMap));
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
            return referenceInstance;
        }

        /// <summary>
        /// 解析ValueInstance
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeMap"></param>
        /// <returns></returns>
        private static ValueInstance ParseValueInstance(XmlElement element, Dictionary<int, Type> typeMap)
        {
            int typeId = int.Parse(element.GetAttribute("type-Ref"));
            string identifier = element.GetAttribute(nameof(identifier));
            string value = element.GetAttribute(nameof(value));
            ValueInstance valueInstance = new ValueInstance(typeMap[typeId], identifier, false) { Value = value };
            return valueInstance;
        }

        /// <summary>
        /// 解析ProcEnvInst_CT_VM
        /// </summary>
        /// <param name="element"></param>
        /// <param name="linkerMap"></param>
        /// <param name="portMap"></param>
        /// <param name="channelMap"></param>
        private static void ParseProcEnvInst_CT_VM(XmlElement element,
            Dictionary<int, Linker_VM> linkerMap,
            Dictionary<int, Port> portMap,
            Dictionary<int, Channel> channelMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            double x = double.Parse(element.GetAttribute(nameof(x)));
            double y = double.Parse(element.GetAttribute(nameof(y)));
            int attachedLinkerId = int.Parse(element.GetAttribute("attachedLinker-Ref"));
            ProcEnvInst_CT_VM procEnvInst_CT_VM = new ProcEnvInst_CT_VM(x, y, ResourceManager.mainWindow_VM.TopoGraph_P_VM, linkerMap[attachedLinkerId]);
            procEnvInst_CT_VM.ProcEnvInst.Id = id;
            foreach (XmlElement subElement in element.ChildNodes)
            {
                procEnvInst_CT_VM.ProcEnvInst.PortChanInsts.Add(ParsePortChanInst(subElement, portMap, channelMap));
            }
            ResourceManager.mainWindow_VM.TopoGraph_P_VM.DragDrop_VMs.Add(procEnvInst_CT_VM);
        }

        /// <summary>
        /// 解析PortChanInst
        /// </summary>
        /// <param name="element"></param>
        /// <param name="portMap"></param>
        /// <param name="channelMap"></param>
        /// <returns></returns>
        private static PortChanInst ParsePortChanInst(XmlElement element,
            Dictionary<int, Port> portMap,
            Dictionary<int, Channel> channelMap)
        {
            int id = int.Parse(element.GetAttribute(nameof(id)));
            int portId = int.Parse(element.GetAttribute("port-Ref"));
            int chanId = int.Parse(element.GetAttribute("chan-Ref"));
            return new PortChanInst() { Port = portMap[portId], Chan = channelMap[chanId], Id = id };
        }

        /// <summary>
        /// 解析全局_id
        /// </summary>
        /// <param name="element"></param>
        private static void ParseSID(XmlElement element)
        {
            string className = element.GetAttribute(nameof(className));
            int _id = int.Parse(element.GetAttribute(nameof(_id)));
            switch (className)
            {
                case nameof(Axiom):
                    Axiom._id = _id;
                    break;
                case nameof(Channel):
                    Channel._id = _id;
                    break;
                case nameof(Env):
                    Env._id = _id;
                    break;
                case nameof(AttrPair):
                    AttrPair._id = _id;
                    break;
                case nameof(IK):
                    IK._id = _id;
                    break;
                case nameof(Port):
                    Port._id = _id;
                    break;
                case nameof(Proc):
                    Proc._id = _id;
                    break;
                case nameof(State):
                    State._id = _id;
                    break;
                case nameof(LocTrans):
                    LocTrans._id = _id;
                    break;
                case nameof(PortChanInst):
                    PortChanInst._id = _id;
                    break;
                case nameof(ProcEnvInst):
                    ProcEnvInst._id = _id;
                    break;
                case nameof(TopoInst):
                    TopoInst._id = _id;
                    break;
                case nameof(Type):
                    Type._id = _id;
                    break;
                case nameof(Attribute):
                    Attribute._id = _id;
                    break;
                case nameof(Caller):
                    Caller._id = _id;
                    break;
                case nameof(Formula):
                    Formula._id = _id;
                    break;
                case nameof(Anchor_VM):
                    Anchor_VM._id = _id;
                    break;
                case nameof(Linker_VM):
                    Linker_VM._id = _id;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        #endregion
    }
}
