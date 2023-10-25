using PracticalWork7.ViewModel;
using System.Windows;

namespace PracticalWork7.View
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            DataContext = new HelpViewModel();
        }
    }
}
