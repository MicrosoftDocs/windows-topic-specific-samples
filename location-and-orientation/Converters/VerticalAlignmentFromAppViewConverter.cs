using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace locationandorientation.Converters
{
    public class VerticalAlignmentFromAppViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            bool portraitOrientation = (bool)value;
            VerticalAlignment alignment = VerticalAlignment.Top;
            if (portraitOrientation)
            {
                alignment = VerticalAlignment.Center;
            }
            return alignment;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
