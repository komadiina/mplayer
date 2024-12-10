using System.Globalization;
using System.Windows.Data;

namespace mplayer.src.vm.converters
{
	public class LongToMMSSConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				return TimeSpan.FromSeconds(long.Parse(value.ToString())).ToString(@"mm\:ss");
			}
			catch (Exception e)
			{
				return "00:00";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
