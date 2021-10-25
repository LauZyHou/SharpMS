using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    public class Axiom : ReactiveObject
    {
        private static int _id = 0;
        private readonly ObservableCollection<Formula> formulas;
        private string identifier;
        private string description;

        /// <summary>
        /// 无参构造
        /// </summary>
        public Axiom()
        {
            this.identifier = $"Ax{++_id}";
            this.description = "";
            this.formulas = new ObservableCollection<Formula>();
        }

        /// <summary>
        /// 带标识的构造
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="description"></param>
        public Axiom(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.description = description;
            this.formulas = new ObservableCollection<Formula>();
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public ObservableCollection<Formula> Formulas => formulas;
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region Have xxx

        public bool HaveFormula
        {
            get
            {
                return this.formulas is not null && this.formulas.Count > 0;
            }
        }

        #endregion
    }
}
