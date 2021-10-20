using Plat._C;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    /// <summary>
    /// 进程模板的实例
    /// </summary>
    public class ProcInst : TopoInst
    {
        private Proc? proc;
        private readonly ObservableCollection<Instance> properties;

        public ProcInst()
            : base()
        {
            this.properties = new ObservableCollection<Instance>();
        }

        /// <summary>
        /// 例化的进程模板
        /// </summary>
        public Proc? Proc
        {
            get => proc;
            set
            {
                this.RaiseAndSetIfChanged(ref proc, value);
                // 当对ProcInst设置新Proc时，要让其连接的所有ProcEnvInst设定的关联信息全失效
                foreach (ProcEnvInst procEnvInst in ResourceManager.procEnvInsts)
                {
                    if (procEnvInst.ProcInst == this)
                    {
                        procEnvInst.PortChanInsts.Clear();
                    }
                }
                // 重新生成实例的Properties
                this.properties.Clear();
                if (proc is null)
                {
                    return;
                }
                foreach (VisAttr attr in proc.Attributes)
                {
                    Instance instance;
                    if (attr.IsArray) // 数组类型
                    {
                        instance = new ArrayInstance(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    else if (attr.Type.IsBase) // 值类型
                    {
                        instance = new ValueInstance(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    else // 引用类型
                    {
                        instance = ReferenceInstance.build(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    this.properties.Add(instance);
                }
                // Tips
                ResourceManager.UpdateTip($"Process template switched for process instance. (1) Clear edge. (2) Regen instance.");
            }
        }
        /// <summary>
        /// 例化参数表
        /// </summary>
        public ObservableCollection<Instance> Properties => properties;
    }
}
