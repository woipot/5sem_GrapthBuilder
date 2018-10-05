using System;

namespace GrapthBuilder.Source.Classes
{
    public class Range : ICloneable
    {
        private double _leftLimit;
        private double _rightLimit;


        public double LeftLimit
        {
            get => _leftLimit;
            set
            {
                if (value > RightLimit)
                {
                    RightLimit = value;
                }
                else
                {
                    _leftLimit = value;
                }
            }
        }
        
        public double RightLimit
        {
            get => _rightLimit;
            set
            {
                if (value < LeftLimit)
                {
                    LeftLimit = value;
                }
                else
                {
                    _rightLimit = value;
                }
            }
        }



        public Range(double leftLimit, double rightLimit)
        {
           _leftLimit = Math.Min(leftLimit, rightLimit);
           _rightLimit = Math.Max(leftLimit, rightLimit);
        }

        public bool InRange(double point)
        {
            return point >= _leftLimit && point <= _rightLimit;
        }


        public object Clone()
        {
            return new Range(LeftLimit, RightLimit);
        }
    }
}
