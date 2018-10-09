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
                vm.AxisX = AxisX;
                vm.AxisY = AxisY;

                MainChart.MouseMove += vm.MouseMove;
                MainChart.ManipulationCompleted += vm.Resize;
            }

        }

    }
}
