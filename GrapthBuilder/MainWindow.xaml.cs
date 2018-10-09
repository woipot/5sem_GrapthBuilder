using System;
using System.Windows;
using System.Windows.Input;
using GrapthBuilder.Source.MVVM;
using LiveCharts.Wpf;

namespace GrapthBuilder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is MainVM vm)
            {
                MainChart.MouseMove += vm.MouseMove;
            }

        }
    }
}
