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

        public double SelectedX { get; set; }
        public double SelectedY { get; set; }

        public IEnumerable<EquationModel> Equations => _graphicsModel.Equations;

        #endregion


        #region Constructor

        public MainVM()
        {
            _graphicsModel = new GraphicsModel();

            SelectedX = 0;
            SelectedY = 0;

            LoadCommand = new DelegateCommand(LoadFromFile);
            AppendCommand = new DelegateCommand(AppendFromFile);
            DataClickCommand = new DelegateCommand<ChartPoint>(SellectPoint);
        }

        #endregion


        #region Commands

        public DelegateCommand LoadCommand { get; }

        public DelegateCommand AppendCommand { get; }

        public DelegateCommand<ChartPoint> DataClickCommand { get; }

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

        #endregion
        
    }
}
