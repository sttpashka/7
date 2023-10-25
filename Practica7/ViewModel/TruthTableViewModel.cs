using PracticalWork7.ViewModel.Helpers;
using PracticalWork7.Model;
using System.Windows;
using PracticalWork7.View;
using System.Linq;

namespace PracticalWork7.ViewModel
{
    internal class TruthTableViewModel : BindingHelper
    {
        #region Properties
        private ExpressionHandler _expression;
        public ExpressionHandler Expression
        {
            get { return _expression; }
            set
            {
                if (_expression != value)
                {
                    _expression = value;
                }
                OnPropertyChanged();
            }
        }
        #endregion
        #region Commands
        public BindableCommand CloseWindowCommand { get; set; }
        #endregion

        public TruthTableViewModel()
        {
            Expression = new ExpressionHandler();
            CloseWindowCommand = new BindableCommand(_ => CloseWindow());
            Expression = Expression.GetInstance();
        }

        private void CloseWindow()
        {
            TruthTableWindow window = Application.Current.Windows.OfType<TruthTableWindow>().FirstOrDefault();
            window?.Close();
        }
    }
}
