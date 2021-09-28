using Plat._M;
using ReactiveUI;
using Plat._C;
using System.Collections.ObjectModel;

namespace Plat._VM
{
    /// <summary>
    /// Axiom面板VM
    /// </summary>
    public class AxiomPanel_VM : ViewModelBase
    {
        private readonly ObservableCollection<Axiom> axiomList;
        private Axiom? currentAxiom;
        private Formula? currentFormula;

        public AxiomPanel_VM()
        {
            this.axiomList = ResourceManager.axioms;
        }

        public Axiom? CurrentAxiom { get => currentAxiom; set => this.RaiseAndSetIfChanged(ref currentAxiom, value); }
        public Formula? CurrentFormula { get => currentFormula; set => this.RaiseAndSetIfChanged(ref currentFormula, value); }
        public ObservableCollection<Axiom> AxiomList => axiomList;
        #region Command

        /// <summary>
        /// 添加新的Axiom
        /// </summary>
        private void OnCreateNewAxiom()
        {
            Axiom axiom = new Axiom("NewAxiom");
            ResourceManager.axioms.Add(axiom);
            // 同步操作结果到类图
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            classDiagram_P_VM.DragDrop_VMs.Add(new Axiom_VM(200, 200, classDiagram_P_VM, axiom));
            ResourceManager.UpdateTip($"Add new axiom, op synced to class diagram.");
        }

        /// <summary>
        /// 删除选中的Axiom
        /// </summary>
        private void OnDeleteSelectedAxiom()
        {
            if (this.currentAxiom is null)
            {
                ResourceManager.UpdateTip($"An axiom must be selected!");
                return;
            }
            Axiom axiom = this.currentAxiom;
            ResourceManager.axioms.Remove(axiom);
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            foreach (DragDrop_VM item in classDiagram_P_VM.DragDrop_VMs)
            {
                if (item is Axiom_VM)
                {
                    Axiom_VM axiom_VM = (Axiom_VM)item;
                    if (axiom_VM.Axiom == axiom)
                    {
                        classDiagram_P_VM.DragDrop_VMs.Remove(axiom_VM);
                        goto OVER;
                    }
                }
            }
        OVER:
            ResourceManager.UpdateTip($"Delete axiom [{axiom.Identifier}], op synced to class diagram.");
        }

        /// <summary>
        /// 创建新的公理公式
        /// </summary>
        private void OnCreateNewFormula()
        {
            if (this.currentAxiom is null)
            {
                ResourceManager.UpdateTip($"An axiom must be selected!");
                return;
            }
            this.currentAxiom.Formulas.Add(new Formula("f(g(x))=x"));
            ResourceManager.UpdateTip($"Create new formula for axiom [{this.currentAxiom.Identifier}].");
        }

        /// <summary>
        /// 删除选中的公理公式
        /// </summary>
        private void OnDeleteSelectedFormula()
        {
            if (this.currentAxiom is null)
            {
                ResourceManager.UpdateTip($"An axiom must be selected!");
                return;
            }
            if (this.currentFormula is null)
            {
                ResourceManager.UpdateTip($"A formula must be selected!");
                return;
            }
            this.currentAxiom.Formulas.Remove(this.currentFormula);
            ResourceManager.UpdateTip($"Delete selected formula for axiom [{this.currentAxiom}].");
        }

        /// <summary>
        /// 向上移动公理公式
        /// </summary>
        /// <param name="fPos"></param>
        private void OnMoveUpFormula(int? fPos)
        {
            if (this.currentAxiom is null)
            {
                ResourceManager.UpdateTip($"An axiom must be selected!");
                return;
            }
            if (fPos is null)
            {
                ResourceManager.UpdateTip($"A formula must be selected!");
                return;
            }
            if (fPos < 0 || fPos >= this.currentAxiom.Formulas.Count)
            {
                ResourceManager.UpdateTip($"The formula position exceed!");
                return;
            }
            if (fPos == 0)
            {
                ResourceManager.UpdateTip($"The formula is the top one, no need to move up!");
                return;
            }
            int pos = (int)fPos;
            Formula formula = this.currentAxiom.Formulas[pos];
            this.currentAxiom.Formulas.RemoveAt(pos);
            this.currentAxiom.Formulas.Insert(pos - 1, formula);
            ResourceManager.UpdateTip($"Move up the formula [{formula.Content}] in axiom [{this.currentAxiom.Identifier}]");
        }

        /// <summary>
        /// 向下移动公理公式
        /// </summary>
        /// <param name="fPos"></param>
        private void OnMoveDownFormula(int? fPos)
        {
            if (this.currentAxiom is null)
            {
                ResourceManager.UpdateTip($"An axiom must be selected!");
                return;
            }
            if (fPos is null)
            {
                ResourceManager.UpdateTip($"A formula must be selected!");
                return;
            }
            if (fPos < 0 || fPos >= this.currentAxiom.Formulas.Count)
            {
                ResourceManager.UpdateTip($"The formula position exceed!");
                return;
            }
            if (fPos == this.currentAxiom.Formulas.Count - 1)
            {
                ResourceManager.UpdateTip($"The formula is the bottom one, no need to move down!");
                return;
            }
            int pos = (int)fPos;
            Formula formula = this.currentAxiom.Formulas[pos];
            this.currentAxiom.Formulas.RemoveAt(pos);
            this.currentAxiom.Formulas.Insert(pos + 1, formula);
            ResourceManager.UpdateTip($"Move down the formula [{formula.Content}] in axiom [{this.currentAxiom.Identifier}]");
        }

        #endregion
    }
}
