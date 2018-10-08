using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using GrapthBuilder.Source.MVVM.Models;
using LiveCharts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;

namespace GrapthBuilder.Source.MVVM
{
    internal class MainVM : BindableBase
    {

        private GraphicsModel _graphicsModel;

        private ZoomingOptions _zoomingMode;


        #region Properties

        public SeriesCollection Series => _graphicsModel.Series;

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

        public IEnumerable<EquationModel> Equations => _graphicsModel.Equations;

        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        #endregion


        #region Constructor

        public MainVM()
        {
            _graphicsModel = new GraphicsModel();
            ZoomingMode = ZoomingOptions.Xy;

            SelectedX = 0;
            SelectedY = 0;

            LoadCommand = new DelegateCommand(LoadFromFile);
            AppendCommand = new DelegateCommand(AppendFromFile);
            DataClickCommand = new DelegateCommand<ChartPoint>(SellectPoint);
            ResetZoominCommand = new DelegateCommand<RoutedEventArgs>(ResetZooming);

            XFormatter = RerangeX;
            YFormatter = RerangeY;
        }

        #endregion


        #region Commands

        public DelegateCommand LoadCommand { get; }

        public DelegateCommand AppendCommand { get; }

        public DelegateCommand<ChartPoint> DataClickCommand { get; }
    
        public DelegateCommand<RoutedEventArgs> ResetZoominCommand { get; }

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
            SelectedX = point.X;
            SelectedY = point.Y;
            OnPropertyChanged("SelectedX");
            OnPropertyChanged("SelectedY");
        }

        private void ResetZooming(RoutedEventArgs e)
        {
            //X.MinValue = double.NaN;
            //X.MaxValue = double.NaN;
            //Y.MinValue = double.NaN;
            //Y.MaxValue = double.NaN;
        }

        private string RerangeX(double val)
        {
            return val.ToString("N");
        }

        private string RerangeY(double val)
        {
            return val.ToString("N");
        }
        #endregion

    }
}
