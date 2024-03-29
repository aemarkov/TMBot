﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TMBot.Utilities.MVVM.Converters
{
	/// <summary>
	/// Преобразует null в видимо/невидимо
	/// </summary>
	public sealed class NullToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}