using System;
using System.Collections.Generic;
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
        private readonly double _step;

        private readonly string _strExpression;
        private readonly CompiledExpression _expression;
        private readonly string _variableName;


        #region Properties
        public double Step => _step;

        public string VariableName => _variableName;

        public string StrExpression => _strExpression;

        #endregion


        #region Constructors

        public EquationModel(string strExpr, CompiledExpression optimizedExpression,
                        double step = 0.01, string variableName = "x", double defaultRange = 100)
        {
            _strExpression = strExpr;
            _expression = optimizedExpression;
            _step = step;
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
                var variable = new VariableValue(point, VariableName);
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

            var step = range.Length() / _defaultRange;
            //var currentStep = Step + Step * Math.Round(_range.Length() / _defaultRange);
            for (var i = range.LeftLimit; i <= range.RightLimit; i += step)
            {
                var pointResult = CalculateInPoint(i);
                points.Add(pointResult);
            }

            return points;
        }

        #endregion

    }
}
