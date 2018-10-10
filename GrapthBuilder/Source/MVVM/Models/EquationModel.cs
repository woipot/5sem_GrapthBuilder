using System;
using System.Collections.Generic;
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
        private readonly double _defaultRange;
        private readonly double _stepMult;

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
                        double stepMult = 1, string variableName = "x", double defaultRange = 100)
        {
            _stepMult = stepMult;
            _variableName = variableName;
            _defaultRange = defaultRange;
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

        public LineSeries GetSeriesInRange(double leftLimit, double rightLimit)
        {
            var range = new Range(leftLimit, rightLimit);
            return GetSeriesInRange(range);
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

            var step = range.Length() / (_defaultRange * _stepMult);

            for (var i = range.LeftLimit; i <= range.RightLimit; i += step)
            {
                var pointResult = CalculateInPoint(i);

                //if (points.Count != 0)
                //{
                //    //TODO Y scale
                //    var lastPoint = points.Last();
                //    var difference = Math.Abs(pointResult.Y - lastPoint.Y);
                //    var differenceLimit = range.Length() / (10 );
                //    if (difference > differenceLimit)
                //    {
                //        lastPoint.Y = double.NaN;
                //    }
                //}

                points.Add(pointResult);
            }
            return points;
        }
        #endregion

    }
}
