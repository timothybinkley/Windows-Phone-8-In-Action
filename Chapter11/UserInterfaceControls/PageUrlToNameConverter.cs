using System;
using System.Windows.Data;

namespace UserInterfaceControls
{
    public class PageUrlToNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (!(value is string))
                return value;

            string result = value.ToString();
            int pos = result.LastIndexOf("/");
            if( pos > 0)
                result = result.Substring(pos+1);
            pos = result.IndexOf(".xaml");
            if( pos > 0)
                result = result.Substring(0, pos);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
