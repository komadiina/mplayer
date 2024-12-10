using Microsoft.Win32;
using mplayer.src.audio;
using mplayer.src.model;
using StylesTemplatesBinding.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace mplayer.src.vm
{
	public class ContextBarViewModel
	{
		public ICommand OpenFileCommand { get; set; }
		public ICommand OpenFolderCommand { get; set; }
		public ICommand AboutDialogCommand { get; set; }
		public ICommand ContactDialogCommand { get; set; }

		public ContextBarViewModel()
		{
			OpenFileCommand = new RelayCommand(OpenFileDialog);
			OpenFolderCommand = new RelayCommand(OpenFolderDialog);
			AboutDialogCommand = new RelayCommand(AboutDialog);
			ContactDialogCommand = new RelayCommand(ContactDialog);
		}

		private void OpenFileDialog()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
			bool? result = dialog.ShowDialog();

			if (result ??= true && dialog.FileName != null)
			{
				AudioHandler.Instance.InitSong(new Song(dialog.FileName));
			}
		}

		private void OpenFolderDialog()
		{
			var dialog = new OpenFolderDialog();
			bool? result = dialog.ShowDialog();

			if (result ??= true && dialog.FolderName != null)
			{
				AudioHandler.Instance.InitPlaylist(new Playlist(dialog.FolderName, false));
			}
		}

		private void AboutDialog()
		{
			MessageBox.Show(
				App.Current.MainWindow,
				"A university project for \"Human-Computer Interaction\" (HCI, 2277)",
				"About",
				MessageBoxButton.OK
				);
		}

		private void ContactDialog()
		{
			MessageBox.Show(
				App.Current.MainWindow,
				"komadina.ognjen@gmail.com | github.com/komadiina",
				"Contact",
				MessageBoxButton.OK
				);
		}
	}
}
