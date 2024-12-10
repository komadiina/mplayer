namespace mplayer.src.vm.events
{
	public class SongChangedEventArgs(string newTitle, long length) : EventArgs
	{
		public string NewTitle { get; set; } = newTitle;
		public long Length { get; set; } = length;
	}
}
