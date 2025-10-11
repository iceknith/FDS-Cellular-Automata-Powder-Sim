using Godot;

public class Sand : Powder
{
	public Sand()
	{
		density = 20;
		color = Colors.Yellow;
		modulateColor(0.2f);
	}

	public override void modulateColor(float intensity = 0.05F)
	{
		float w = rng.RandfRange(0.0f, intensity);
		color = color.Lightened(w);
		float z = rng.RandfRange(0.0f, intensity);
		color = color.Darkened(z);
	}
}
