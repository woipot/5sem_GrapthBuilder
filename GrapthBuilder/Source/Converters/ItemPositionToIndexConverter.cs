using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace GrapthBuilder.Source.Converters
{
    public class ItemPositionToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = 0;
            if (value is ListViewItem lvItem)
            {
                //нумерацию будем вести с единицы
                if (ItemsControl.ItemsControlFromItemContainer(lvItem) is ListView listView)
                    index = listView.ItemContainerGenerator.IndexFromContainer(lvItem) + 1;
            }

            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
