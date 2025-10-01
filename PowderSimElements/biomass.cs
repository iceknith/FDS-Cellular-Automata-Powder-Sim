using Godot;

public class Biomass : Powder
{
	public float nutrient { get; set; }
	public Biomass(float wetness, float nutrient)
	{
		density = 20;
		color = Colors.Khaki;
		this.wetness = wetness;
		this.nutrient = nutrient;
	}
}
