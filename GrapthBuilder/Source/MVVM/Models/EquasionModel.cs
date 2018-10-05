using System.Collections.Generic;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;
using GrapthBuilder.Source.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.Practices.Prism.Mvvm;

namespace GrapthBuilder.Source.MVVM.Models
{
    class EquasionModel : BindableBase
    {
        private readonly CompiledExpression _expression;

        private readonly double _step;

        private readonly string _variableName;

        private Range _range;


        #region Properties
        public double Step => _step;

        public string VariableName => _variableName;

        public double LeftLimit
        {
            get => _range.LeftLimit;
            set
            {
                CalculateRange(value);
                _range.LeftLimit = value;
            }
        }

        public double RightLimit
        {
            get => _range.RightLimit;
            set
            {
                CalculateRange(value);
                _range.RightLimit = value;
            }
        }

        public Range Range
        {
            get => _range;
            set
            {
                CalculateRange(value);
                _range = value;

                OnPropertyChanged("LeftLimit");
                OnPropertyChanged("RightLimit");
            }
        }

        public string Expression => _expression.ToString();

        public ChartValues<ObservablePoint> DotSet { get; }

        #endregion


        #region Constructors

        public EquasionModel(CompiledExpression optimizedExpression, Range range, double step = 0.01, string variableName = "x")
        {
            _expression = optimizedExpression;
            _step = step;
            _variableName = variableName;

            DotSet = new ChartValues<ObservablePoint>();

            Range = range;
        }

        #endregion


        #region Public methods

        public void CalculateRange(Range newRange)
        {
            for (var i = newRange.LeftLimit; i <= newRange.RightLimit; i += Step)
            {
                var inRange = _range?.InRange(i) ?? false;

                if (!inRange)
                {
                    var variable = new VariableValue(i, VariableName);
                    var resInPoint = ToolsHelper.Calculator.Calculate(_expression, new List<VariableValue> { variable });
                    var observablePoint = new ObservablePoint(i, resInPoint);
                    DotSet.Add(observablePoint); 
                }
            }
        }

        public void CalculateRange(double newLimit)
        {
            var inRange = _range.InRange(newLimit);
            if (inRange) return;

            Range additionalRange;
            var isLeftLimit = newLimit < LeftLimit;
            if (isLeftLimit)
            {
                additionalRange = new Range(newLimit, LeftLimit - 1);
                LeftLimit = newLimit;
            }
            else
            {
                additionalRange = new Range(RightLimit + 1, newLimit);
                RightLimit = newLimit;
            }

            for (var i = additionalRange.LeftLimit; i <= additionalRange.RightLimit; i += Step)
            {
                var variable = new VariableValue(i, VariableName);
                var resInPoint = ToolsHelper.Calculator.Calculate(_expression, new List<VariableValue> { variable });
                var observablePoint = new ObservablePoint(i, resInPoint);
                DotSet.Add(observablePoint);
            }

        }

        #endregion



    }
}
