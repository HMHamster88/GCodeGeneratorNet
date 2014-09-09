using CSScriptLibrary;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Graphics;
using Microsoft.Win32;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GCodeGeneratorNet
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Workspace workspace;
        ViewWindow pathView;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext =
            workspace = new Workspace();
            ShowPathView();
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPathView();
        }

        private void ShowPathView()
        {
            if (pathView == null)
            {
                Task.Factory.StartNew(() =>
                {
                    pathView = new ViewWindow();
                    pathView.Run(60.0, 0.0);
                    pathView = null;
                });
                CompileAndView();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            CompileAndView();
        }

        private void CompileAndView()
        {
            var gcodes = workspace.Compiler.Compile(workspace.TextEditManager.Text);
            var points = workspace.GCodeToPointsConverter.Convert(gcodes);
            if (pathView != null)
                pathView.LoadPoints(points);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            workspace.TextEditManager.New();
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(workspace.TextEditManager.FilePath))
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "C# script (*.csc)|*.csc";
                if (dialog.ShowDialog() == true)
                {
                    workspace.TextEditManager.Save(dialog.FileName);
                }
            }
            else
            {
                workspace.TextEditManager.Save(workspace.TextEditManager.FilePath);
            }
            CompileAndView();
        }

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "C# script (*.csc)|*.csc";
            if (dialog.ShowDialog() == true)
            {
                workspace.TextEditManager.Open(dialog.FileName);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            CompileAndView();
            var dialog = new SaveFileDialog();
            dialog.Filter = "GCode (*.nc)|*.nc";
            if (dialog.ShowDialog() == true)
            {
                var gcodes = workspace.Compiler.Compile(workspace.TextEditManager.Text);
                using(var file = dialog.OpenFile())
                {
                    var sr = new StreamWriter(file);
                    foreach(var code in gcodes)
                    {
                        sr.WriteLine(code.ToString());
                    }
                    sr.Flush();
                }
            }
        }
    }
}
