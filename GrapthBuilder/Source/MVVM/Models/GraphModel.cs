using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using ELW.Library.Math;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Mvvm;

namespace GrapthBuilder.Source.MVVM.Models
{
    class GraphicsModel : BindableBase
    {
        private const double DefaultRange = 100;
        private const double Step = 1;


        private ObservableCollection<EquasionModel> _equasions;
        private SeriesCollection _series;
        private Range _range;


        public SeriesCollection Series => _series;


        public GraphicsModel()
        {
            _equasions = new ObservableCollection<EquasionModel>();
            _equasions.CollectionChanged += UppdateSeries;

            _series = new SeriesCollection();
            _range = new Range(-DefaultRange, DefaultRange);
        }



        public void LoadFromFile(string patch)
        {
            using (var sr = new StreamReader(patch))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    var preparedExpression = ToolsHelper.Parser.Parse(str);
                    var compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
                    var optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);

                    var equastion = new EquasionModel(optimizedExpression, (Range)_range.Clone(), Step);

                    _equasions.Add(equastion);
                }
            }
        }


        private void UppdateSeries(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var notTypedItems = notifyCollectionChangedEventArgs.NewItems;
            var items = notTypedItems.Cast<EquasionModel>();


            foreach (var equasion in items)
            {
                var lineSeries = new LineSeries{Values = equasion.DotSet, PointGeometrySize = 1};
                _series.Add(lineSeries);
            }
                
            OnPropertyChanged("Series");
        }
    }

}
