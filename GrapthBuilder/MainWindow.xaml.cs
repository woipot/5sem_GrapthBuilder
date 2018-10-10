using System.Windows;
using GrapthBuilder.Source.MVVM;

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
