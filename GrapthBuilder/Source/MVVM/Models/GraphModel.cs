using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Mvvm;
using org.mariuszgromada.math.mxparser;

namespace GrapthBuilder.Source.MVVM.Models
{
    internal class GraphicsModel : BindableBase
    {
        private static readonly ColorSet Colors;

        private const double DefaultRange = 100;
           
        private readonly ObservableCollection<EquationModel> _equations;


        private Range _currentRange;
        private double _defaultPointsInRange = 150;
        

        #region Properties
        public SeriesCollection Series { get; }

        public IEnumerable<EquationModel> Equations => _equations;

        public double DefaultPointsInRange
        {
            get => _defaultPointsInRange;
            set
            {
                _defaultPointsInRange = value;
                OnPropertyChanged("DefaultPointsInRange");
            }
        }

        #endregion


        #region Constructor

        static GraphicsModel()
        {
            Colors = new ColorSet();
        }

        public GraphicsModel()
        {
            _equations = new ObservableCollection<EquationModel>();
            _equations.CollectionChanged += UppdateSeries;

            _currentRange = new Range(-DefaultRange, DefaultRange);
           
            Series = new SeriesCollection();
        }

        #endregion


        #region Public methods

        public void LoadFromFile(string patch)
        {
            if (_equations.Any())
                _equations.Clear();

            if (Series.Any())
                Series.Clear();


            var result = GetEqFromFile(patch);
            foreach (var equation in result)
            {
                _equations.Add(equation);
            }

            OnPropertyChanged("Equations");
        }

        public void AppendFromFile(string patch)
        {
            var result = GetEqFromFile(patch);
            foreach (var equation in result)
            {
                _equations.Add(equation);
            }

            OnPropertyChanged("Equations");
        }

        public void RerangeX(double axisXActualMinValue, double axisXActualMaxValue)
        {
            var seriesList = new List<LineSeries>();
            _currentRange = new Range(axisXActualMinValue, axisXActualMaxValue);

            foreach (var equation in _equations)
            {
                if(!equation.IsEnabled) continue;

                var lineSeries = GetSeries(equation);
                seriesList.Add(lineSeries);
            }

            if (Series.Any())
                Series.Clear();

            Series.AddRange(seriesList);
            OnPropertyChanged("Series");
        }

        public void Uppdate()
        {
            RerangeX(_currentRange.LeftLimit, _currentRange.RightLimit);
        }

        public void CreateTangentFromPoint(double x, double y)
        {
            var equation = FindByPoint(x, y);

            var derivativeResult = equation.DerivativeResult(x);


            var tangentumEqStr =
                $"{y.ToString(CultureInfo.InvariantCulture)}+({derivativeResult.ToString(CultureInfo.InvariantCulture)})*" +
                $"(x-({x.ToString(CultureInfo.InvariantCulture)}))";

            AddEquation(tangentumEqStr);
        }

        public void CreateNormalFromPoint(double x, double y)
        {
            var equation = FindByPoint(x, y);

            var derivativeResult = equation.DerivativeResult(x);

            var normalEqStr =
                $"{y.ToString(CultureInfo.InvariantCulture)}+(-1/{derivativeResult.ToString(CultureInfo.InvariantCulture)})*" +
                $"(x-({x.ToString(CultureInfo.InvariantCulture)}))";

            AddEquation(normalEqStr);
        }

        #endregion


        #region Private methods

        private IEnumerable<EquationModel> GetEqFromFile(string patch)
        {
            var resultList = new List<EquationModel>();

            using (var sr = new StreamReader(patch))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    var equastion = CreateEquation(str);
                    resultList.Add(equastion);
                }
            }
            return resultList;

        }

        private EquationModel CreateEquation(string equationStr)
        {
            var expression = new Expression(equationStr);

            var equastion = new EquationModel(expression, Colors.GetNext(), pointsInRange: _defaultPointsInRange);

            return equastion;
        }

        private void UppdateSeries(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var equation = item as EquationModel;
                    var series = GetSeries(equation);
                    Series.Add(series);
                }
                OnPropertyChanged("Series");
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                
                OnPropertyChanged("Series");
            }
        }

        private LineSeries GetSeries(EquationModel equation)
        {
            var lineSeries = equation.GetSeriesInRange(_currentRange);

            lineSeries.Fill = Brushes.Transparent;
            lineSeries.Stroke = equation.Brush;
            lineSeries.PointGeometrySize = equation.LineWidth;
            lineSeries.Tag = equation;

            return lineSeries;
        }

        private EquationModel FindByPoint(double x, double y)
        {
            foreach (var equation in _equations)
            {
                var result = equation.CalculateInPoint(x);

                if (y.Equals(result.Y))
                    return equation;
            }

            return null;
        }

        private void AddEquation(string tangentumEqStr)
        {
            var tangentumEqModel = CreateEquation(tangentumEqStr);
            _equations.Add(tangentumEqModel);
            OnPropertyChanged("Equations");
        }
        #endregion

    }
    
}
