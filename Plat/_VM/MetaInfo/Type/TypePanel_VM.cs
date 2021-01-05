using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._M;
using ReactiveUI;

namespace Plat._VM
{
    public class TypePanel_VM : ViewModelBase
    {

        private Type? currentType;
        private ObservableCollection<Type> typeList;

        public TypePanel_VM()
        {
            this.typeList = new ObservableCollection<Type>();
            this.typeList.Add(Type.TYPE_INT);
            this.typeList.Add(Type.TYPE_MSG);
        }

        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }
        public Type? CurrentType { get => currentType; set => this.RaiseAndSetIfChanged(ref currentType, value); }

        #region Button Command

        private void DeleteType(Type type)
        {
            if (type is null || type.IsBase)
            {
                return;
            }
            this.typeList.Remove(type);
        }

        private void CreateType()
        {
            this.typeList.Add(new Type("TodoName"));
        }

        #endregion
    }
}
