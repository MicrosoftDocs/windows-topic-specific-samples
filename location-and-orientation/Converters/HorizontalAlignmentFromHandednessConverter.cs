using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;


namespace locationandorientation.Converters
{
    public class HorizontalAlignmentFromHandednessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            bool leftHanded = (bool)value;
            HorizontalAlignment alignment = HorizontalAlignment.Right;
            if (leftHanded)
            {
                alignment = HorizontalAlignment.Left;
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
