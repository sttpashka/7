using PracticalWork7.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace PracticalWork7.View
{
    /// <summary>
    /// Логика взаимодействия для TruthTableWindow.xaml
    /// </summary>
    public partial class TruthTableWindow : Window
    {
        public TruthTableWindow()
        {
            InitializeComponent();
            DataContext = new TruthTableViewModel();
        }
    }
}
