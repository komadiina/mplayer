namespace mplayer.src.components;

public class ClickableSlider : CustomSlider
{
	public event EventHandler<double> OnSliderTapped;

	public void RaiseOnSliderTapped(double value)
	{
		OnSliderTapped?.Invoke(this, value);
	}
}
