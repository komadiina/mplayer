namespace mplayer.src.vm.events
{
	public class SeekTimeChangedEventArgs(long newSeekTime) : EventArgs
	{
		public long NewSeekTime { get; set; } = newSeekTime;
	}
}
