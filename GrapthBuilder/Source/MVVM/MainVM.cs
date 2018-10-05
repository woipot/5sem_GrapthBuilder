using System;
using System.Windows;
using GrapthBuilder.Source.MVVM.Models;
using LiveCharts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;

namespace GrapthBuilder.Source.MVVM
{
    class MainVM : BindableBase
    {
        private GraphicsModel _graphicsModel;

        public SeriesCollection Series => _graphicsModel.Series;


        public MainVM()
        {
            _graphicsModel = new GraphicsModel();

            LoadCommand = new DelegateCommand(LoadFromFile);
        }




        public DelegateCommand LoadCommand { get; }

        public void LoadFromFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt|*.txt";

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


    }
}
