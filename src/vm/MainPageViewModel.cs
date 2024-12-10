using CommunityToolkit.Mvvm.ComponentModel;
using FontAwesome.WPF;
using mplayer.src.audio;
using mplayer.src.vm.events;

namespace mplayer.src.vm
{
	public partial class MainPageViewModel : ObservableObject
	{
		private AudioHandler _audioHandler => AudioHandler.Instance;

		[ObservableProperty]
		private bool playing = false;

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
			TogglePlaybackIcon = FontAwesomeIcon.Play;
			SongTitle = e.NewTitle;
			Length = e.Length;
			SeekTime = 0L;
			Playing = false;
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
