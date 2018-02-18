/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
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

	public class PlatformToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Platform)
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

	public class DateToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
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
					//Debug.WriteLine(path + " " + Watch.ElapsedMilliseconds);
#endif
					return bitmap;
				}
				else
				{
#if DEBUG
					Debug.WriteLine("Image failure: " + path);
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

	public class MatchGroupMultiValueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			MatchWindowViewModel MWVM = values[0] as MatchWindowViewModel;

			LocalDB db = (LocalDB)values[1];

			string name = Enum.GetName(typeof(LocalDB), db);

			string id = "";

			string sep1 = " (";
			string sep2 = ")";
			switch (db)
			{
				case LocalDB.GamesDB:
					if (MWVM.ID_GDB != null)
					{
						id = sep1 + MWVM.ID_GDB + sep2;
					}
					break;
				case LocalDB.GiantBomb:
					if (MWVM.ID_GB != null)
					{
						id = sep1 + MWVM.ID_GB + sep2;
					}
					break;
				case LocalDB.OpenVGDB:
					if (MWVM.ID_OVG != null)
					{
						id = sep1 + MWVM.ID_OVG + sep2;
					}
					break;
				case LocalDB.LaunchBox:
					if (MWVM.ID_LB != null)
					{
						id = sep1 + MWVM.ID_LB + sep2;
					}
					break;
				default:
					break;
			}

			return name + id;
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

	public class StarConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return 1;
			}
			else
			{
				return (value as decimal?) * 15 - 1;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RatingColorConverter : IValueConverter
	{
		SolidColorBrush gold = new SolidColorBrush(Colors.Gold);
		SolidColorBrush silver = new SolidColorBrush(Colors.Silver);
		SolidColorBrush bronze = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#CD7F32"));
		//SolidColorBrush copper = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#8C5B53"));
		SolidColorBrush white = new SolidColorBrush(Colors.White);
		//SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			decimal? rating = value as decimal?;

			if (rating == null)
			{
				return null;
			}

			if (rating == 5)
			{
				return gold;
			}

			if (rating >= 4)
			{
				return silver;
			}
			if (rating >= 3)
			{
				return bronze;
			}

			return white;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
