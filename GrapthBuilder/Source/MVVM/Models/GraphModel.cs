using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ELW.Library.Math;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Mvvm;

namespace GrapthBuilder.Source.MVVM.Models
{
    internal class GraphicsModel : BindableBase
    {
        private const double DefaultRange = 100;
        private const double Step = 1;


        private ObservableCollection<EquationModel> _equations;
        private SeriesCollection _series;
        private Range _range;


        public SeriesCollection Series => _series;
        public IEnumerable<EquationModel> Equations => _equations;


        public GraphicsModel()
        {
            _equations = new ObservableCollection<EquationModel>();
            _equations.CollectionChanged += UppdateSeries;

            _series = new SeriesCollection();
            _range = new Range(-DefaultRange, DefaultRange);
        }



        public void LoadFromFile(string patch)
        {
            if(_equations.Any())
                _equations.Clear();

            var result = Load(patch);

            foreach (var equation in result)
            {
                _equations.Add(equation);
            }

        }

        public void AppendFromFile(string patch)
        {
            var result = Load(patch);

            foreach (var equasionModel in result)
            {
                _equations.Add(equasionModel);          
            }
        }



        private IEnumerable<EquationModel> Load(string patch)
        {
            var resultList = new List<EquationModel>();

            using (var sr = new StreamReader(patch))
            {

                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    var preparedExpression = ToolsHelper.Parser.Parse(str);
                    var compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
                    var optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);

                    var equastion = new EquationModel(str + "= y", optimizedExpression, (Range)_range.Clone(), Step);

                    resultList.Add(equastion);
                }

            }

            return resultList;

        }

        private void UppdateSeries(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var eventAction = notifyCollectionChangedEventArgs.Action;

            var notTypedItems = notifyCollectionChangedEventArgs.NewItems;
            var items = notTypedItems.Cast<EquationModel>();


            if (eventAction == NotifyCollectionChangedAction.Add)
            {
                foreach (var equation in items)
                {
                    var lineSeries = new LineSeries {Fill = Brushes.Transparent, Values = equation.DotSet, PointGeometrySize = 1, Tag = equation};
                    _series.Add(lineSeries);
                }
            }
            else if (eventAction == NotifyCollectionChangedAction.Remove)
            {
                foreach (var equation in items)
                {
                    for(var i = 0; i < _series.Count; i++)
                    {
                        var series = (LineSeries) _series[i];

                        if (series.Tag == equation)
                        {
                            _series.Remove(series);
                            break;
                        }
                    }
                }
            }
            else if (eventAction == NotifyCollectionChangedAction.Reset)
            {
                _series.Clear();
            }
            else if (eventAction == NotifyCollectionChangedAction.Replace)
            {
                var oldItems = notifyCollectionChangedEventArgs.OldItems;
                var counter = 0;
                foreach (var oldEquation in oldItems)
                {
                    for (var i = 0; i < _series.Count; i++)
                    {
                        var series = (LineSeries)_series[i];

                        if (series.Tag == oldEquation)
                        {
                            _series.Remove(series);
                            var newEquasion = items.ElementAt(counter);

                            var lineSeries = new LineSeries { Fill = Brushes.Transparent, Values = newEquasion.DotSet, PointGeometrySize = 1, Tag = newEquasion};
                            _series.Add(lineSeries);

                            break;
                        }
                    }
                    counter++;
                }
            }
                
            OnPropertyChanged("Series");
        }
    }
    
}
