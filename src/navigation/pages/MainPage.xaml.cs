using FontAwesome.WPF;
using mplayer.src.audio;
using mplayer.src.vm;
using System.Diagnostics;
using System.Windows.Controls;

namespace mplayer.src.navigation.pages
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class MainPage : Page
	{
		private Boolean Playing = false;
		private long _currentTime = 0L;

		public MainPage()
		{
			InitializeComponent();
			this.DataContext = new MainPageViewModel();
			AudioHandler.Context = this.DataContext as MainPageViewModel;

			var _vm = (MainPageViewModel)this.DataContext;
			_vm.InitTimer();
			_vm.StartTimer();
		}

		private void TogglePlayback(object sender, System.Windows.RoutedEventArgs e)
		{
			var _vm = (MainPageViewModel)this.DataContext;
			if (_vm.Playing == false)
			{
				if (AudioHandler.Instance.IsInitialized == false)
					return;

				AudioHandler.Instance.Play();
				_vm.TogglePlaybackIcon = FontAwesomeIcon.Pause;

				_vm.Playing = true;
				_vm._sliderTimer?.Start();
			}
			else
			{
				AudioHandler.Instance.PausePlayback();
				_vm.TogglePlaybackIcon = FontAwesomeIcon.Play;
				_vm.Playing = false;
			}
		}

		public void Slider_DragStarted(object sender, EventArgs e)
		{
			var ViewModel = (MainPageViewModel)this.DataContext;
			ViewModel.SliderChangedBySystem = false;
		}

		public void Slider_DragCompleted(object sender, EventArgs e)
		{
			// Handle user-initiated seek
			var newPosition = ((Slider)sender).Value;
			AudioHandler.Instance.SetPosition(newPosition);
		}

		private void btnStop_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var _vm = (MainPageViewModel)this.DataContext;
			AudioHandler.Instance.StopPlayback();
			Playing = false;
			_vm._sliderTimer?.Stop();
			SongDurationSlider.Value = 0;
			_vm.TogglePlaybackIcon = FontAwesomeIcon.Play;
		}

		private void VolumeSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
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

		private void SongDurationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			var _vm = (MainPageViewModel)this.DataContext;

			var s = sender as Slider;
			Trace.WriteLine($"{s}");
			if (s.IsFocused)
			{
				_vm.SliderChangedBySystem = false;
				s.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
			}

			if (_vm.SliderChangedBySystem)
				return;

			try
			{
				AudioHandler.Instance.SetPosition((float)SongDurationSlider.Value);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Unable to set position: {ex.Message}");
			}
		}

		private void ButtonDebug_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//var _vm = this.DataContext as MainPageViewModel;
			AudioHandler.Instance.Debug();
		}
	}
}
