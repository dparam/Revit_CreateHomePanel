using Autodesk.Revit.DB;
using Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.ViewModels;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Revit_CreateHomePanel.App.Plugins.CreateHomePanel.UserControls.Views
{
    public partial class Window_CreateHomePanel : Window
    {
        private MainViewModel _mainViewModel = null;

        public Window_CreateHomePanel(Document document)
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel(document, this);
            DataContext = _mainViewModel;
        }


        // Hyperlink

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        // Validation

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        // Events

        private void OnSetDefaultValues(object sender, RoutedEventArgs e)
        {
            _mainViewModel.OnSetDefaultValues(sender, e);
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            _mainViewModel.OnStart(sender, e);
        }


        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _mainViewModel.OnCancel(sender, e);
        }
    }
}
