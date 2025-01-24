using mplayer.src.model;
using mplayer.src.vm;
using mplayer.src.vm.events;
using NAudio.Wave;
using System.Diagnostics;
using System.Windows;

namespace mplayer.src.audio
{
	internal class AudioHandler
	{
		public static readonly String NO_SONG = "No song loaded.";
		public static AudioHandler Instance { get; } = new AudioHandler();
		protected WaveOutEvent OutputDevice { get; private set; } = null;
		protected Song CurrentSong { get; private set; } = null;
		protected Playlist Playlist { get; private set; }
		public Boolean IsInitialized => OutputDevice != null && CurrentSong != null;
		private Boolean SongChange = false;
		public event EventHandler<SongChangedEventArgs> SongChanged;
		public event EventHandler<SeekTimeChangedEventArgs> SeekTimeChanged;
		public static MainPageViewModel Context { get; set; }

		private AudioHandler() { }

		public void InitSong(Song song)
		{
			if (song == null || song.Data == null) return;

			try
			{
				if (OutputDevice != null)
				{
					SongChange = true;
					OutputDevice.Stop();
					OutputDevice.Dispose();
					OutputDevice = null;
				}
				else SongChange = false;

				CurrentSong?.Dispose();
				CurrentSong = null;

				CurrentSong = song;
				OutputDevice = new WaveOutEvent();
				OutputDevice.PlaybackStopped += OnPlaybackStopped;

				Context.Playing = true;
				Context.SongTitle = song.Title;
				Context.SeekTime = 0;
				Context.Length = (long)song.Data.TotalTime.TotalSeconds;
				Context.ResetTimer();

				OutputDevice.Init(CurrentSong.Data);
				OnSongChanged(CurrentSong.Title, (long)CurrentSong.Data.TotalTime.TotalSeconds);
			}
			catch (Exception)
			{
				Trace.WriteLine("Unable to load audio file: " + song.Path);
			}
		}

		public void PlayNext(Song song)
		{
			//OutputDevice?.Stop();
			OutputDevice?.Dispose();
			OutputDevice = null;

			CurrentSong?.Dispose();
			CurrentSong = song;

			if (song == null || song.Data == null)
			{
				Context.PlayNextVisibility = Visibility.Hidden;
				Trace.WriteLine("No song to play.");
				return;
			}

			OutputDevice = new WaveOutEvent();
			OutputDevice.Init(CurrentSong.Data);
			OutputDevice.PlaybackStopped += OnPlaybackStopped;
			OutputDevice.Play();

			Context.Playing = true;
			Context.SongTitle = song.Title;
			Context.SeekTime = 0;
			Context.Length = (long)song.Data.TotalTime.TotalSeconds;
			Context.TogglePlaybackIcon = FontAwesome.WPF.FontAwesomeIcon.Pause;
			Context.ResetTimer();

			SeekTimeChanged?.Invoke(this, new SeekTimeChangedEventArgs(0L));
			SongChanged?.Invoke(this, new SongChangedEventArgs(CurrentSong.Title, (long)CurrentSong.Data.TotalTime.TotalSeconds));
		}

		public void SetVolume(float value)
		{
			if (OutputDevice != null)
			{
				OutputDevice.Volume = value / 100f;
			}
		}

		public void InitPlaylist(Playlist playlist)
		{
			StopPlayback();
			Playlist = playlist;
			playlist.Save();
			Song next = playlist.Next();
			if (playlist.First() != null)
			{
				Song peeked = playlist.First();
				Context.PlayNextVisibility = Visibility.Visible;
				Context.HasNextSongInQueue = Visibility.Visible;
				Context.NextSongTitle = "Next in queue: " + peeked.Title;
			}
			else
			{
				Context.PlayNextVisibility = Visibility.Hidden;
				Context.HasNextSongInQueue = Visibility.Hidden;
				Context.NextSongTitle = "";
			}

			InitSong(next);
		}

		public void Play()
		{
			OutputDevice?.Play();
		}

		public void PausePlayback()
		{
			OutputDevice?.Pause();
		}

		public void StopPlayback()
		{
			if (Playlist != null)
			{
				if (Playlist.Songs.Count == 0)
				{
					SeekTimeChanged?.Invoke(this, new SeekTimeChangedEventArgs(0L));
					SongChanged?.Invoke(this, new SongChangedEventArgs(NO_SONG, 0));
				}
			}
			else if (CurrentSong == null)
			{
				SeekTimeChanged?.Invoke(this, new SeekTimeChangedEventArgs(0L));
				SongChanged?.Invoke(this, new SongChangedEventArgs(NO_SONG, 0));
			}

			if (OutputDevice != null)
			{
				OutputDevice.Stop();
				OutputDevice.Dispose();
				OutputDevice = null;
			}
			CurrentSong?.Dispose();
			CurrentSong = null;
		}

		public void SetPosition(double seekTime)
		{
			if (CurrentSong == null || OutputDevice == null) return;

			if (CurrentSong.Data != null)
			{
				CurrentSong.Data.CurrentTime = TimeSpan.FromSeconds(seekTime);
				OnSeekTimeChanged((long)seekTime);
			}
		}

		public long GetPosition()
		{
			if (CurrentSong == null || OutputDevice == null) return 0L;
			return (long)CurrentSong.Data.CurrentTime.TotalSeconds;
		}

		private void OnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			// stop playing current song
			if (CurrentSong != null && !SongChange)
			{
				CurrentSong.Data.Dispose();
				CurrentSong.Dispose();
				CurrentSong = null;
			}
			else
			{
				Context.SongTitle = "No songs are currently playing.";
			}


			// get next song from current playlist (if exists)
			if (Playlist != null)
			{
				Song nextSong = Playlist.Next();
				if (nextSong != null)
				{
					PlayNext(nextSong);

					if (Playlist.First() != null)
					{
						Song peeked = Playlist.First();
						Context.PlayNextVisibility = Visibility.Visible;
						Context.HasNextSongInQueue = Visibility.Visible;
						Context.NextSongTitle = "Next in queue: " + peeked.Title;
					}
					else
					{
						Context.PlayNextVisibility = Visibility.Hidden;
						Context.HasNextSongInQueue = Visibility.Visible;
						Context.NextSongTitle = "No songs left in queue.";
					}
				}
				else
				{
					Context.PlayNextVisibility = Visibility.Hidden;
					Context.HasNextSongInQueue = Visibility.Hidden;
					Context.NextSongTitle = "";
				}
			}

			// dispose of output device when no songs left
			if (OutputDevice != null && CurrentSong == null && !SongChange)
			{
				OutputDevice.Dispose();
				OutputDevice = null;

				Context.Playing = false;
				Context.SongTitle = "No songs are currently playing.";
				Context.SeekTime = 0;
				Context.Length = 0;
				Context.TogglePlaybackIcon = FontAwesome.WPF.FontAwesomeIcon.Play;
			}
		}

		private void OnSongChanged(string title, long length)
		{
			SongChanged?.Invoke(this, new SongChangedEventArgs(title, length));
			SeekTimeChanged?.Invoke(this, new SeekTimeChangedEventArgs(0L));
		}

		private void OnSeekTimeChanged(long time)
		{
			SeekTimeChanged?.Invoke(this, new SeekTimeChangedEventArgs(time));
		}

		public void Debug()
		{

		}
	}
}
