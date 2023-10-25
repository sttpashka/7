using PracticalWork7.View;
using PracticalWork7.ViewModel.Helpers;
using System.Linq;
using System.Windows;

namespace PracticalWork7.ViewModel
{
    internal class HelpViewModel : BindingHelper
    {
        #region Commands
        public BindableCommand CloseWindowCommand { get; set; }
        #endregion

        public HelpViewModel()
        {
            CloseWindowCommand = new BindableCommand(_ => CloseWindow());
        }

        private void CloseWindow()
        {
            HelpWindow window = Application.Current.Windows.OfType<HelpWindow>().FirstOrDefault();
            window?.Close();
        }
    }
}
