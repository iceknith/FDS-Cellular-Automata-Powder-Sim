using Godot;

public class Oil : Liquid
{
	public Oil() : base()
	{
		density = 4;
		color = Colors.LightYellow;
		flammability = 5;
		ashCreationPercentage = 0;
	}
}
