using FiretruckRouteGenerator.Utils;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace FiretruckRouteGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string RESOURCES_PATH = @"Resources\";
        private const string CONFIG_FILE_FILTER = "txt files (*.txt)|*.txt";
        private const string OUTPUT_FILE_NAME = "Output";

        private ViewModel _vm = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _vm;
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(Environment.CurrentDirectory, RESOURCES_PATH),
                Filter = CONFIG_FILE_FILTER
            };

            if (openFileDialog.ShowDialog() != true) return;

            try
            {
                _vm.GenerateCases(openFileDialog.FileName);
            }
            catch (MapConfigFormatException exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Path.Combine(Environment.CurrentDirectory, RESOURCES_PATH),
                FileName = OUTPUT_FILE_NAME,
                Filter = CONFIG_FILE_FILTER
            };

            if (saveFileDialog.ShowDialog() != true) return;

            _vm.SaveLog(saveFileDialog.FileName);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) => _vm.Close();
    }
}
