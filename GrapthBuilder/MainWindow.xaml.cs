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
            }

        }

        private void ChartMouseMove(object sender, MouseEventArgs e)
        {

            Point point;
            try
            {
                point = MainChart.ConvertToChartValues(e.GetPosition(MainChart));
            }
            catch (Exception)
            {
                point = new Point(0, 0);
            }

            X.Text = point.X.ToString("N");
            Y.Text = point.Y.ToString("N");
        }
    }
}
