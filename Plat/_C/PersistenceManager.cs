using Avalonia.Controls;
using Plat._M;
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
                xmlWriter.WriteAttributeString("parentId", type.Parent?.Id.ToString());
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
                xmlWriter.WriteAttributeString("parentId", env.Parent?.Id.ToString());
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
                xmlWriter.WriteAttributeString("parentId", proc.Parent?.Id.ToString());
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
            xmlWriter.WriteAttributeString("idValue", idValue.ToString());

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
            xmlWriter.WriteAttributeString("typeId", attribute.Type.Id.ToString());
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
            xmlWriter.WriteAttributeString("returnTypeId", caller.ReturnType.Id.ToString());
            xmlWriter.WriteAttributeString("description", caller.Description);

            foreach (Type type in caller.ParamTypes)
            {
                xmlWriter.WriteStartElement("ParamType");

                xmlWriter.WriteAttributeString("typeId", type.Id.ToString());

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

        #endregion
    }
}
