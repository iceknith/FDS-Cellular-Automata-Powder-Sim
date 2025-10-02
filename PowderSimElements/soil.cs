using Godot;
using System;
using System.Runtime.InteropServices;


public class Soil : Powder
{
	public (int, int)[] cardinals = [(0, 1), (1, 0), (0, -1), (-1, 0)];
	new private Color baseColor = Color.FromHtml("#8d7267ff");
	private Color wetColor = Color.FromHtml("#3a1008ff");
	private Color richColor = Color.FromHtml("#394e35ff");
	private float _nutrient;
	public float nutrient
	{
		get { return _nutrient; }   // get method
		set { _nutrient = Math.Max(value, 0); }  // set method
	}

	public Soil()
	{
		density = 20;
		color = baseColor;
		flammability = 0;
		wetness = 0.0f;
		nutrient = 0.0f;
	}

	override public void updateColor(int T)
	{
		base.updateColor(T);

		Color nutriHue = baseColor.Lerp(richColor, Math.Min(nutrient, 2)); // more nutrient = darker color
		Color wetHue = baseColor.Lerp(wetColor, wetness); // more wet = darker color

		color = nutriHue.Lerp(wetHue, 0.5f); // blend both effects TODO: adjust colors
	}

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{

		// Handle nutrient diffusion (uniform in all directions)
		for (int nx = Math.Max(0, x - 1); nx < Math.Min(x + 1, maxX); nx++) // including diagonals
		{
			for (int ny = Math.Max(0, y - 1); ny < Math.Min(y + 1, maxY); ny++)
			{
				if ((nx, ny) == (x, y))
				{
					continue;
				}
				if (oldElementArray[nx, ny] is Soil soil)
				{
					float nutriDiff = soil.nutrient - nutrient;
					soil.nutrient -= nutriDiff / 2;
					nutrient += nutriDiff / 2;
				}
			}
		}

		// Handle wetness propagation (limited distance, cardinal directions only)
		if (wetness > 0.3f) // Only propagate if we have significant wetness
		{
			foreach ((int dx, int dy) in cardinals)
			{
				int neighborX = x + dx;
				int neighborY = y + dy;

				if (neighborX >= 0 && neighborX < maxX && neighborY >= 0 && neighborY < maxY)
				{
					if (oldElementArray[neighborX, neighborY] is Soil neighborSoil)
					{
						float wetnessDiff = wetness - neighborSoil.wetness;
						if (wetnessDiff > 0.1f) // Only transfer if significant difference and if wetness greater than neighbor's
						{
							float transferAmount = wetnessDiff * 0.1f; // Slower transfer rate
							float maxTransfer = Math.Min(transferAmount, wetness * 0.3f); // Limit how much can be transferred

							wetness -= maxTransfer;
							neighborSoil.wetness += maxTransfer;
						}
					}
				}
			}
		}

		// Handle water absorption from adjacent water elements
		foreach ((int nx, int ny) in cardinals)
		{
			int neighborX = x + nx;
			int neighborY = y + ny;
			if (neighborX >= 0 && neighborX < maxX && neighborY >= 0 && neighborY < maxY)
			{
				if (oldElementArray[neighborX, neighborY] is Water water)
				{
					float wetnessCap = Math.Min(water.wetness, 1 - wetness); // only absorb what we can take
					water.wetness -= wetnessCap;
					wetness += wetnessCap;
				}
			}
		}

		updateColor(T);

		base.update(oldElementArray, currentElementArray, x, y, maxX, maxY, T); // keep at the end because of returns contained in base method
	}

	override public string getState()
	{
		return base.getState() + ";" + wetness + ";" + nutrient;
	}

	override public int setState(string state)
	{
		int i = 0;
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
