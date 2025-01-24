using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace mplayer.src.model
{
	public class Playlist
	{
		public String Title { get; set; }
		public Queue<Song> Songs { get; set; }

		public Song this[int index]
		{
			get => Songs.ElementAt(index);
			set => Songs.ElementAt(index);
		}

		protected Playlist() { }

		public Playlist(String path, bool fromXML = false)
		{
			Songs = new Queue<Song>();
			Title = path;

			if (fromXML)
			{
				Playlist playlist = Load(path);
				Songs = playlist.Songs;
			}
			else
			{
				// path represents a directory
				foreach (String file in Directory.GetFiles(path))
					if (file.EndsWith(".mp3") || file.EndsWith(".wav"))
						Songs.Enqueue(new Song(file));
			}
		}

		public void AddSong(Song song) => Songs.Enqueue(song);
		public void RemoveSong() => Songs.Dequeue();

		public Song Next()
		{
			if (Songs.Count == 0) return null;

			Trace.WriteLine($"Playing: ${First().Title}");

			return Songs.Dequeue();
		}

		public Boolean HasNext() { return Songs.Count > 0; }

		public Song First()
		{
			return Songs.Count == 0 ? null : Songs.Peek();
		}

		public void Save()
		{
			Save($"./storage/playlist_${DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.xml");
		}

		public void Save(String path)
		{
			// check if all path directories exist
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));

			TextWriter textWriter = new StreamWriter(path);
			textWriter.Close();
		}

		public static Playlist Load(String path)
		{
			XmlSerializer serializer = new(typeof(Playlist));
			TextReader textReader = new StreamReader(path);

			Playlist playlist = (Playlist)serializer.Deserialize(textReader);

			textReader.Close();
			return playlist;
		}

		public Song PeekNext()
		{
			return Songs.Count == 0 ? null : Songs.Peek();
		}
	}
}
