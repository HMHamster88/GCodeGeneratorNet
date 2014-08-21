using CSScriptLibrary;
using GCodeGeneratorNet.Core;
using GCodeGeneratorNet.Graphics;
using OpenTK;
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
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var gcodes = workspace.Compiler.Compile(workspace.TextEditManager.Text);
            foreach (var g in gcodes)
                g.ToString();
            var points = workspace.GCodeToPointsConverter.Convert(gcodes);
            if(pathView != null)
                pathView.LoadPoints(points);
        }
    }
}
