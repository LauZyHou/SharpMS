using Plat._C;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    /// <summary>
    /// 环境模板的实例
    /// </summary>
    public class EnvInst : TopoInst
    {
        private Env? env;
        private readonly ObservableCollection<Instance> properties;

        public EnvInst()
            : base()
        {
            this.properties = new ObservableCollection<Instance>();
        }

        /// <summary>
        /// 例化的环境模板
        /// </summary>
        public Env? Env
        {
            get => env;
            set
            {
                this.RaiseAndSetIfChanged(ref env, value);
                // 当对EnvInst设置新Env时，要让其连接的所有ProcEnvInst设定的关联信息全失效
                foreach (ProcEnvInst procEnvInst in ResourceManager.procEnvInsts)
                {
                    if (procEnvInst.EnvInst == this)
                    {
                        procEnvInst.PortChanInsts.Clear();
                    }
                }
                // 重新生成实例的Properties（需要考虑Env级的继承关系
                this.properties.Clear();
                if (env is null)
                {
                    return;
                }
                Env? hyperEnv = env;
                do
                {
                    foreach (VisAttr attr in hyperEnv.Attributes)
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
                    hyperEnv = hyperEnv.Parent;
                } while (hyperEnv is not null);
                // Tips
                ResourceManager.UpdateTip($"Environment template switched for environment instance. (1) Clear edge. (2) Regen instance.");
            }
        }

        /// <summary>
        /// 由Attributes例化而来的参数表
        /// </summary>
        public ObservableCollection<Instance> Properties => properties;
    }
}
