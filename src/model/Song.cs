using NAudio.Wave;
using System.Diagnostics;

namespace mplayer.src.model
{
	public class Song
	{
		public String Path { get; private set; }
		public AudioFileReader Data { get; private set; }
		public String Title { get; private set; }

		public Song(String path)
		{
			try
			{
				Path = path;
				Data = new AudioFileReader(path);
				Title = System.IO.Path.GetFileNameWithoutExtension(path);
			}
			catch (Exception e)
			{
				Trace.WriteLine(e);
				Data = null;
			}
		}

		public void Dispose()
		{
			if (Data == null) return;
			Data.Dispose();
		}

		public override bool Equals(object obj)
		{
			if (obj is Song s)
				return s.Path.Equals(Path);

			return false;
		}
	}
}
