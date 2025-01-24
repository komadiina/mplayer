using CommunityToolkit.Mvvm.ComponentModel;
using FontAwesome.WPF;
using mplayer.src.audio;
using mplayer.src.vm.events;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace mplayer.src.vm
{
	public partial class MainPageViewModel : ObservableObject
	{
		private AudioHandler _audioHandler => AudioHandler.Instance;
		public DispatcherTimer _sliderTimer;

		public void ResetTimer()
		{
			_sliderTimer.Stop();
			_sliderTimer.Start();
		}

		internal void StartTimer()
		{
			_sliderTimer.Start();
		}

		public void InitTimer()
		{
			_sliderTimer = new DispatcherTimer(DispatcherPriority.Background)
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			_sliderTimer.Tick += SliderTimer_Tick;
		}

		private void SliderTimer_Tick(object sender, EventArgs e)
		{
			if (Playing)
			{
				Trace.WriteLine("Slider timer tick");
				SeekTime += 1;
				SliderChangedBySystem = true;
			}
		}

		[ObservableProperty]
		public String nextSongTitle = "";

		[ObservableProperty]
		public Visibility hasNextSongInQueue = Visibility.Hidden;

		[ObservableProperty]
		public Visibility playNextVisibility = Visibility.Hidden;

		[ObservableProperty]
		public Boolean sliderChangedBySystem = true;

		[ObservableProperty]
		private bool playing;

		[ObservableProperty]
		private FontAwesomeIcon togglePlaybackIcon = FontAwesomeIcon.Play;

		[ObservableProperty]
		private string songTitle = AudioHandler.NO_SONG;

		[ObservableProperty]
		private long seekTime = 0L;

		[ObservableProperty]
		private long length = 0;

		[ObservableProperty]
		private float volume = 50f;

		public MainPageViewModel()
		{
			_audioHandler.SongChanged += AudioHandler_SongChanged;
			_audioHandler.SeekTimeChanged += AudioHandler_SeekTimeChanged;
		}

		private void AudioHandler_SongChanged(object sender, SongChangedEventArgs e)
		{
			TogglePlaybackIcon = FontAwesomeIcon.Pause;
			SongTitle = e.NewTitle;
			Length = e.Length;
			SeekTime = 0L;
			Playing = true;
			_audioHandler.Play();
		}

		private void AudioHandler_SeekTimeChanged(object sender, SeekTimeChangedEventArgs e)
		{
			SeekTime = e.NewSeekTime;
		}
		public void Dispose()
		{
			_audioHandler.SongChanged -= AudioHandler_SongChanged;
			_audioHandler.SeekTimeChanged -= AudioHandler_SeekTimeChanged;
		}
	}
}
