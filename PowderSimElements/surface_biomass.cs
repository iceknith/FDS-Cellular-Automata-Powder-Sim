using Godot;

public class SurfBiomass : Powder
{
	public float nutrient { get; set; }
	public SurfBiomass(float wetness, float nutrient)
	{
		density = 20;
		color = Colors.DarkGreen;
		this.wetness = wetness;
		this.nutrient = nutrient;
	}

	override public string getState()
	{
		return base.getState() + ";" + wetness + ";" + nutrient;
	}

	override public int setState(string state)
	{
		int i = base.setState(state);
		string[] stateArgs = state.Split(";", false);
		wetness = stateArgs[i++].ToFloat();
		nutrient = stateArgs[i++].ToFloat();
		return i;
	}

	override public string inspectInfo()
	{
		return base.inspectInfo() + $"  Wetness: {wetness:F3}\n  Nutrient: {nutrient:F3}\n";
	}
}
