using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using GrapthBuilder.Source.MVVM.Models;
using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GrapthBuilder.Source.MVVM
{
    internal class MainVM : BindableBase
    {
        private readonly GraphicsModel _graphicsModel;

        private ZoomingOptions _zoomingMode;


        #region Properties

        public SeriesCollection Series => _graphicsModel.Series;
        public IEnumerable<EquationModel> Equations => _graphicsModel.Equations;

        public ZoomingOptions ZoomingMode
        {
            get => _zoomingMode;
            set
            {
                _zoomingMode = value;
                OnPropertyChanged("ZoomingMode");
            }
        }

        public double SelectedX { get; set; }
        public double SelectedY { get; set; }

        public double MaxRange { get; }
        public double MinRange { get; }

        public string MouseX { get; private set; }
        public string MouseY { get; private set; }
        #endregion


        #region Constructor

        public MainVM()
        {
            _graphicsModel = new GraphicsModel();
            ZoomingMode = ZoomingOptions.Xy;

            SelectedX = 0;
            SelectedY = 0;

            MaxRange = 10000;
            MinRange = -10000;

            LoadCommand = new DelegateCommand(LoadFromFile);
            AppendCommand = new DelegateCommand(AppendFromFile);
            DataClickCommand = new DelegateCommand<ChartPoint>(SellectPoint);

            RangeChangedCommand = new DelegateCommand<RangeChangedEventArgs>(Resize);
            UpdateCommand = new DelegateCommand(Update);

            ChangeColorCommand = new DelegateCommand<MouseEventArgs>(ChangeColor);
        }

        #endregion


        #region Commands

        public DelegateCommand LoadCommand { get; }

        public DelegateCommand AppendCommand { get; }

        public DelegateCommand<ChartPoint> DataClickCommand { get; }

        public DelegateCommand<RangeChangedEventArgs> RangeChangedCommand { get; }

        public DelegateCommand UpdateCommand { get; }

        public DelegateCommand<MouseEventArgs> ChangeColorCommand { get; }
        #endregion


        #region Events
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is CartesianChart chart)
            {
                Point point;
                try
                {
                    point = chart.ConvertToChartValues(e.GetPosition(chart));
                }
                catch (Exception)
                {
                    point = new Point(0, 0);
                }

                MouseX = point.X.ToString("F");
                MouseY = point.Y.ToString("F");
                OnPropertyChanged("MouseX");
                OnPropertyChanged("MouseY");
            }
        }

        


        #endregion


        #region Other private functions

        private void LoadFromFile()
        {
            var dialog = new OpenFileDialog { Filter = "txt|*.txt" };

            if (dialog.ShowDialog() == true)
            {
                var patch = dialog.FileName;
                try
                {
                    _graphicsModel.LoadFromFile(patch);
                }
                catch (Exception er)
                {
                    MessageBox.Show("Eror in file : " + er.Message);
                }
            }
        }

        private void AppendFromFile()
        {
            var dialog = new OpenFileDialog { Filter = "txt|*.txt" };

            if (dialog.ShowDialog() == true)
            {
                var patch = dialog.FileName;
                try
                {
                    _graphicsModel.AppendFromFile(patch);
                }
                catch (Exception er)
                {
                    MessageBox.Show("Eror in file : " + er.Message);
                }
            }
        }

        private void SellectPoint(ChartPoint point)
        {
            if (point != null)
            {
                SelectedX = point.X;
                SelectedY = point.Y;
                OnPropertyChanged("SelectedX");
                OnPropertyChanged("SelectedY");
            }
        }

        private void Resize(RangeChangedEventArgs eventArgs)
        {
            if (eventArgs.Axis is Axis axisX)
            {
                try
                {
                    _graphicsModel.RerangeX(axisX.ActualMinValue, axisX.ActualMaxValue);
                }
                catch (Exception)
                {
                    //ignore
                }
            }
        }

        private void Update()
        {
            _graphicsModel.Uppdate();
        }

        private void ChangeColor(MouseEventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                var color = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                var ellipse = e.Source as Ellipse;
                if (ellipse?.DataContext is EquationModel model)
                    model.Brush = new SolidColorBrush(color);

                Update();
            }

        }
        #endregion

    }
}
