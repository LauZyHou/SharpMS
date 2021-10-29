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

        #region XML Generation and Parsing

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

            #region MetaInfo-Types

            xmlWriter.WriteStartElement("MetaInfo-Types");

            foreach (Type type in ResourceManager.types)
            {
                xmlWriter.WriteStartElement("Type");

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
                    XmlWriteCaller(xmlWriter, caller, "Method");
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

        #endregion
    }
}
