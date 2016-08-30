using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TMBot.ViewModels;

namespace TMBot.Windows
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsWindow()
        {
            ViewModel = new SettingsViewModel();
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.ActionSelected += ViewModel_ActionSelected;
        }

        private void ViewModel_ActionSelected(bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}
