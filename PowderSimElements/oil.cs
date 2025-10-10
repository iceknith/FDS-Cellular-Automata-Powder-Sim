using Godot;

public class Oil : Liquid
{
	public Oil() : base()
	{
		density = 4;
		color = Colors.LightYellow;
		flammability = 5;
		ashCreationPercentage = 0;
		wetness = 0.0f;
		modulationIntensity = 0.02f;
		modulateColor(0.001f);
	}
}
