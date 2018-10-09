using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Mvvm;

namespace GrapthBuilder.Source.MVVM.Models
{
    internal class EquationModel : BindableBase
    {
        private readonly double _defaultRange;
        private readonly double _stepMult;

        private readonly CompiledExpression _expression;
        private readonly string _variableName;


        #region Properties

        public string StrExpression { get; }

        public bool IsEnabled { get; set; } = true;

        #endregion


        #region Constructors

        public EquationModel(string strExpr, CompiledExpression optimizedExpression,
                        double stepMult = 1, string variableName = "x", double defaultRange = 100)
        {
            StrExpression = strExpr;
            _expression = optimizedExpression;
            _stepMult = stepMult;
            _variableName = variableName;
            _defaultRange = defaultRange;

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
        #endregion


        #region PrivateMethods

        private ObservablePoint CalculateInPoint(double point)
        {
            try
            {
                var variable = new VariableValue(point, _variableName);
                var resInPoint = ToolsHelper.Calculator.Calculate(_expression, new List<VariableValue> { variable });
                var observablePoint = new ObservablePoint(point, resInPoint);

                return observablePoint;
            }
            catch (Exception)
            {
                return new ObservablePoint(point, double.NaN);
            }
        }

        private ChartValues<ObservablePoint> CalculateRange(Range range)
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
