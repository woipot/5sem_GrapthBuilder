﻿using System.Collections.Generic;
using System.Windows.Media;

namespace GrapthBuilder.Source.Classes
{
    public class ColorSet
    {
        private int _current;
        private readonly List<Color> _colors;

        public ColorSet()
        {
            _colors = new List<Color>
            {
                Color.FromRgb(205, 0, 200),
                Color.FromRgb(0, 150, 250),
                Color.FromRgb(255, 20, 147),
                Color.FromRgb(255, 255, 0),
                Color.FromRgb(0, 255, 0),
                Color.FromRgb(255, 69, 0),
                Color.FromRgb(0, 255, 255),
                Color.FromRgb(250, 128, 114),
                Color.FromRgb(255, 218, 185),
                Color.FromRgb(154, 205, 50),
                Color.FromRgb(102, 205, 170)
            };
        }

        public Color GetNext()
        {
            if (_current >= _colors.Count)
                _current = 0;

            return _colors[_current++];
        }
    }
}
