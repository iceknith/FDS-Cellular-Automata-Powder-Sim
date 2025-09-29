using System;
using Godot;
public class Root : Seed
{
	private (int, int) parentSeed;
	private float maxNutrient = 10f;

	private int lastNutrientAbsorbTick = 0;
	private int nutrientAbsorbInterval = 1 * 60;
	public Root((int, int) parentSeed)
	{
		this.parentSeed = parentSeed;
		density = 1500;
		color = Colors.Brown;
		flammability = 10;
		ashCreationPercentage = 0.8f;
	}
	
	/// <summary>
	/// Returns the parent seed if it still exists, otherwise null
	/// </summary>
	public Seed getParentSeed(Element[,] currentElementArray, int maxX, int maxY)
	{
		if (currentElementArray[parentSeed.Item1, parentSeed.Item2] is Seed seed)
		{
			return seed;
		}
		return null;
	}
	private bool absorbNutrientsAndWetness(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		// absorb nutrients and wetness from adjacent soil in cardinal directions
		float absorbedNutrients = 0f;
		float absorbedWetness = 0f;
		(int, int)[] directions = new (int, int)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
		foreach (var dir in directions)
		{
			int newX = x + dir.Item1;
			int newY = y + dir.Item2;
			if (newX >= 0 && newX < maxX && newY >= 0 && newY < maxY)
			{
				if (currentElementArray[newX, newY] is Soil soil)
				{
					float availableNutrients = Math.Min(soil.nutrient, maxNutrient);
					float availableWetness = soil.wetness; // max wetness is 1

					soil.nutrient -= availableNutrients / 4;
					soil.wetness -= availableWetness / 4;

					absorbedNutrients += availableNutrients / 4;
					absorbedWetness += availableWetness / 4;
				}
			}
		}

		return false;
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		// if parent seed no longer exists, turn into soil with same nutrient and wetness to not lose resources
		if (getParentSeed(currentElementArray, maxX, maxY) == null)
		{
			Soil soil = new Soil();
			soil.nutrient = nutrient;
			soil.wetness = wetness;
			currentElementArray[x, y] = soil;
			return;
		}

		// take nutrients from soil adjacent to root in cardinal directions
		if (lastNutrientAbsorbTick - T >= nutrientAbsorbInterval)
		{
			lastNutrientAbsorbTick = T;
			absorbNutrientsAndWetness(currentElementArray, x, y, maxX, maxY);
		}
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
}