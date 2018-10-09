using System;

namespace GrapthBuilder.Source.Classes
{
    public class Range : ICloneable
    {
        private double _leftLimit;
        private double _rightLimit;


        public double LeftLimit => _leftLimit;
        public double RightLimit => _rightLimit;


        public Range(double leftLimit, double rightLimit)
        {
           _leftLimit = Math.Min(leftLimit, rightLimit);
           _rightLimit = Math.Max(leftLimit, rightLimit);
        }


        public bool InRange(double point)
        {
            return point >= _leftLimit && point <= _rightLimit;
        }

        public bool SetRange(double newLimit)
        {
            if(InRange(newLimit)) return false;

            var isLeftLimit = newLimit < LeftLimit;

            if (isLeftLimit)
                _leftLimit = newLimit;
            else
                _rightLimit = newLimit;

            return true;

        }

        public Range GetAdditionalRange(double newLimit)
        {
            var inRange = InRange(newLimit);
            if (inRange) return null;

            Range additionalRange;

            var isLeftLimit = newLimit < LeftLimit;
            if (isLeftLimit)
            {
                additionalRange = new Range(newLimit, LeftLimit - 1);
            }
            else
            {
                additionalRange = new Range(RightLimit + 1, newLimit);
            }

            return additionalRange;
        }

        public double Length()
        {
            return RightLimit - LeftLimit;
        }

        public object Clone()
        {
            return new Range(LeftLimit, RightLimit);
        }
    }
}
