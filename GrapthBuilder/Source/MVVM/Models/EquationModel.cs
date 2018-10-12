using System;
using System.Windows.Media;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Mvvm;
using org.mariuszgromada.math.mxparser;

namespace GrapthBuilder.Source.MVVM.Models
{
    internal class EquationModel : BindableBase
    {
        private readonly double _pointsInRange;

        private readonly Expression _expression;
        private readonly Argument _argument;

        private readonly string _variableName;
        private Brush _brush;
        private uint _lineWidth; 


        #region Properties

        public uint LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value; 
                OnPropertyChanged("LineWidth");
            }
        }

        public string StrExpression => _expression.getExpressionString();

        public bool IsEnabled { get; set; } = true;

        public string VariableName => _variableName;

        public Brush Brush
        {
            get =>_brush;
            set
            {
                _brush = value;
                OnPropertyChanged("Brush");
            }
        }

        #endregion


        #region Constructors

        public EquationModel(Expression optimizedExpression, Color color, 
            string variableName = "x", double pointsInRange = 100)
        {
            _variableName = variableName;
            _pointsInRange = pointsInRange;
            _lineWidth = 1;

            _expression = optimizedExpression;
            _argument = new Argument(_variableName, 0);
            _expression.addArguments(_argument);

            Brush = new SolidColorBrush(color);

        }

        #endregion


        #region Public methods

        public LineSeries GetSeriesInRange(Range range)
        {
            var chartVales = CalculateRange(range);

            var lineSeries = new LineSeries{Values = chartVales};
            return lineSeries;
        }

        public ObservablePoint CalculateInPoint(double point)
        {
            try
            {
                _argument.setArgumentValue(point);

                var resInPoint = _expression.calculate();
                var observablePoint = new ObservablePoint(point, resInPoint);

                return observablePoint;
            }
            catch (Exception)
            {
                return new ObservablePoint(point, double.NaN);
            }
        }

        public ChartValues<ObservablePoint> CalculateRange(Range range)
        {
            var points = new ChartValues<ObservablePoint>();

            var step = range.Length() / _pointsInRange;

            for (var i = range.LeftLimit; i <= range.RightLimit; i += step)
            {
                var pointResult = CalculateInPoint(i);

                points.Add(pointResult);
            }
            return points;
        }

        public double DerivativeResult(double x)
        {
            var derStr = "der(" + StrExpression + "," + VariableName + ")";
            var derivativeExpression = new Expression(derStr);

            var argument = new Argument(VariableName, x);
            derivativeExpression.addArguments(argument);

            var derivativeResult = derivativeExpression.calculate();
            return derivativeResult;
        }
        #endregion

    }
}
