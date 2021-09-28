using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    public class Axiom : ReactiveObject
    {
        private readonly ObservableCollection<Formula> formulas;
        private string identifier;
        private string description;

        public Axiom(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.formulas = new ObservableCollection<Formula>();
            this.description = description;
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
