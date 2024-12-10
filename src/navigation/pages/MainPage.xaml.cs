using FontAwesome.WPF;
using mplayer.src.audio;
using mplayer.src.vm;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace mplayer.src.navigation.pages
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class MainPage : Page
	{
		private Boolean Playing = false;
		private readonly DispatcherTimer _sliderTimer;
		private long _currentTime = 0L;
		private MainPageViewModel _vm;

		public MainPage()
		{
			InitializeComponent();

			_sliderTimer = new DispatcherTimer(DispatcherPriority.Normal)
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			_sliderTimer.Tick += SliderTimer_Tick;
			_vm = (this.DataContext as MainPageViewModel);
		}

		private void SliderTimer_Tick(object sender, EventArgs e)
		{
			if (_vm.Playing)
			{
				SongSeekSlider.Value += 1f;
				_vm.SeekTime = (long)SongSeekSlider.Value;
			}
		}

		private void TogglePlayback(object sender, System.Windows.RoutedEventArgs e)
		{
			if (_vm.Playing == false)
			{
				if (AudioHandler.Instance.IsInitialized == false)
					return;

				AudioHandler.Instance.Play();
				_vm.TogglePlaybackIcon = FontAwesomeIcon.Pause;

				_vm.Playing = true;
				_sliderTimer?.Start();
			}
			else
			{
				AudioHandler.Instance.PausePlayback();
				_vm.TogglePlaybackIcon = FontAwesomeIcon.Play;
				_vm.Playing = false;
			}
		}

		private void btnStop_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			AudioHandler.Instance.StopPlayback();
			Playing = false;
			_sliderTimer?.Stop();
			SongSeekSlider.Value = 0;
			_vm.TogglePlaybackIcon = FontAwesomeIcon.Play;
		}
		private void SongSeekSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			try
			{
				AudioHandler.Instance.SetPosition(SongSeekSlider.Value);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Unable to set position: {ex.Message}");
			}
		}

		private void VolumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			try
			{
				AudioHandler.Instance.SetVolume((float)VolumeSlider.Value);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Unable to set volume: {ex.Message}");
			}
		}
	}
}
