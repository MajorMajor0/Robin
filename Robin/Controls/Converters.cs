using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Robin
{
	//public class BooleanToCollapsedVisibilityConverter : IValueConverter
	//{
	//    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	//    {
	//        //reverse conversion (false=>Visible, true=>collapsed) on any given parameter
	//        bool input = (null == parameter) ? (bool)value : !((bool)value);
	//        return (input) ? Visibility.Visible : Visibility.Collapsed;
	//    }

	//    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	//    {
	//        throw new NotImplementedException();
	//    }
	////}

	public class IntToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int i = (int)(value ?? 0);
			if (i < 1)
			{
				return Visibility.Hidden;
			}
			else
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

    public class BooleanToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool tf = (bool)value;
            if (tf)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value== null)
			{
				return Visibility.Hidden;
			}
			else
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
#if DEBUG
            Stopwatch Watch = Stopwatch.StartNew();
#endif
            var path = value as string;
			try
			{
				if (path != null && File.Exists(path))
				{
					BitmapImage bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.UriSource = new Uri(path);			
					bitmap.EndInit();
#if DEBUG
                    Debug.WriteLine(path + " " + Watch.ElapsedMilliseconds);
#endif
                    return bitmap;
				}
				else
				{
#if DEBUG
                    Debug.WriteLine("Image failure: " + Watch.ElapsedMilliseconds);
#endif
                    return DependencyProperty.UnsetValue;
				}
			}
			catch (Exception)
			{
				return DependencyProperty.UnsetValue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class PreferredMultiValueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (Equals(values[0], values[1]))
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Hidden;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class FileToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string filePath = (string)(value ?? "");
			if (File.Exists(filePath))
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
