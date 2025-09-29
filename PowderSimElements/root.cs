using System;
using System.Collections.Generic;
using Godot;
public class Root : Seed
{
	private (int, int) parentSeed;
	private float maxNutrient = 10f;

	private int lastActivity = 0;
	private int activityInterval = 30;
	private int lastGrowthTick = 0;
	private int growthInterval = 2 * 60; // ticks
	public Root((int, int) parentSeed)
	{
		this.parentSeed = parentSeed;
		density = 15;
		color = Colors.Brown;
		flammability = 10;
		ashCreationPercentage = 0.8f;
		nutrient = 0f;
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

	private void transferNutrientsUpwards(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		// transfer nutrients to parent seed if it exists
		Seed seed = getParentSeed(currentElementArray, maxX, maxY);
		if (seed != null)
		{
			float transferableNutrients = Math.Max(nutrient - 1, 0); // keep at least 1 nutrient in root
			seed.nutrient += transferableNutrients;
			nutrient -= transferableNutrients;
		}
	}

	private bool isValidRootGrowthPosition(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (x < 0 || x >= maxX || y < 0 || y >= maxY) return false;
		if (currentElementArray[x, y] is not Soil) return false;
		int adjacentRoots = 0;
		(int, int)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
		foreach (var dir in directions)
		{
			int newX = x + dir.Item1;
			int newY = y + dir.Item2;
			if (newX >= 0 && newX < maxX && newY >= 0 && newY < maxY)
			{
				if (currentElementArray[newX, newY] is Root)
				{
					adjacentRoots++;
				}
			}
		}
		if (adjacentRoots > 1) return false;
		return true;
	}

	public bool growRoot(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient <= 0) return false;
		if (y + 1 >= maxY) return false;

		List<(int, int)> possibleGrowthPositions = [];

		for (int nx = x - 1; nx <= x + 1; nx++)
		{
			for (int ny = y; ny <= y + 1; ny++)
			{
				if (isValidRootGrowthPosition(oldElementArray, nx, ny, maxX, maxY))
				{
					possibleGrowthPositions.Add((nx, ny));
				}
			}
		}
		if (possibleGrowthPositions.Count > 0)
		{
			var rand = new Random();
			var chosenPos = possibleGrowthPositions[rand.Next(possibleGrowthPositions.Count)]; // found this online
			currentElementArray[chosenPos.Item1, chosenPos.Item2] = new Root(parentSeed);
			nutrient -= 1f;
			return true;
		}
		else
		{
			return false;
		}
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
		if (lastActivity - T >= activityInterval)
		{
			lastActivity = T;
			absorbNutrientsAndWetness(currentElementArray, x, y, maxX, maxY);
			transferNutrientsUpwards(currentElementArray, x, y, maxX, maxY);
		}

		// Try to grow roots if possible
		if (T - lastGrowthTick >= growthInterval)
		{
			lastGrowthTick = T;
			growRoot(oldElementArray, currentElementArray, x, y, maxX, maxY);
		}
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
	
	override public string inspectInfo()
	{
		return $"  Wetness: {wetness:F3}\n  Nutrient: {nutrient:F3}\n  Parent Seed: ({parentSeed.Item1}, {parentSeed.Item2})\n";
	}
} 