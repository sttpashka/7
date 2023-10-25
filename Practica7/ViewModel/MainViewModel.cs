using PracticalWork7.ViewModel.Helpers;
using PracticalWork7.Model;
using System;
using PracticalWork7.View;
using System.Windows;
using System.Windows.Input;

namespace PracticalWork7.ViewModel
{
    internal class MainViewModel : BindingHelper
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

        private Visibility _isDataGridVisible;
        public Visibility IsDataGridVisible
        {
            get { return _isDataGridVisible; }
            set
            {
                _isDataGridVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public BindableCommand AddSymbolCommand { get; set; }
        public BindableCommand GetAnswerCommand { get; set; }
        public BindableCommand TakeAwayTruthTableCommand { get; set; }
        public BindableCommand OpenHelpWindowCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            Expression = new ExpressionHandler();
            Expression.Text = "a∧b∨c→d";
            AddSymbolCommand = new BindableCommand(AddSymbol);
            GetAnswerCommand = new BindableCommand(_ => GetAnswer());
            TakeAwayTruthTableCommand = new BindableCommand(_ => TakeAwayTruthTable());
            OpenHelpWindowCommand = new BindableCommand(OpenHelpWindow);
        }

        private void AddSymbol(object parameter)
        {
            string symbol = parameter?.ToString();
            Expression.Text += symbol;
        }

        private void GetAnswer()
        {
            Expression.Table = Expression.SetTable();
        }

        private void TakeAwayTruthTable()
        {
            TruthTableWindow truthTableWindow = new TruthTableWindow();

            truthTableWindow.Closed += (sender, e) =>
            {
                IsDataGridVisible = Visibility.Visible;
            };

            IsDataGridVisible = Visibility.Collapsed;

            truthTableWindow.Show();
        }

        private void OpenHelpWindow(object parameter)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }
    }
}
