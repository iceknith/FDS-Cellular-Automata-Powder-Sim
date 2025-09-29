using Godot;
using System;
using System.Collections.Generic;
public class Leaf : Seed
{
	private int lastGrowthTick = 0;
	private int growthInterval = 2 * 60; // ticks
	private LeafState leafState = LeafState.Growing;
	private List<(int, int)> childLeafs = [];
	private enum LeafState
	{
		Growing,
		Sleeping,
		Dying
	}

	private float maxNutrient = 5f;

	private (int, int) parentSeed;
	public Leaf((int, int) parentSeed)
	{
		this.parentSeed = parentSeed;
		density = 15;
		color = Colors.Green;
		flammability = 10;
		nutrient = 0f;
		ashCreationPercentage = 0.8f;
	}
	public Seed getParentSeed(Element[,] currentElementArray)
	{
		if (currentElementArray[parentSeed.Item1, parentSeed.Item2] is Seed seed)
		{
			return seed;
		}
		return null;
	}
	private bool isValidLeafGrowthPosition(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (x < 0 || x >= maxX || y < 0 || y >= maxY) return false;
		if (currentElementArray[x, y] != null) return false;
		int adjacentLeaves = 0;
		(int, int)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
		foreach (var dir in directions)
		{
			int newX = x + dir.Item1;
			int newY = y + dir.Item2;
			if (newX >= 0 && newX < maxX && newY >= 0 && newY < maxY)
			{
				if (currentElementArray[newX, newY] is Leaf)
				{
					adjacentLeaves++;
				}
			}
		}
		if (adjacentLeaves > 1) return false; // prevent too dense leaf growth
		return true;
	}

	public bool growLeaf(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient <= 0) return false;
		if (y - 1 < 0) return false;

		List<(int, int)> possibleGrowthPositions = [];

		for (int nx = x - 1; nx <= x + 1; nx++)
		{
			for (int ny = y - 1; ny <= y; ny++)
			{
				if (isValidLeafGrowthPosition(oldElementArray, nx, ny, maxX, maxY))
				{
					possibleGrowthPositions.Add((nx, ny));
				}
			}
		}
		if (possibleGrowthPositions.Count > 0)
		{
			var rand = new Random();
			var chosenPos = possibleGrowthPositions[rand.Next(possibleGrowthPositions.Count)]; // found this online
			currentElementArray[chosenPos.Item1, chosenPos.Item2] = new Leaf(parentSeed);
			childLeafs.Add(chosenPos);
			nutrient -= 1f;
			return true;
		}
		else
		{
			leafState = LeafState.Sleeping; // to prevent constant growth attempt
			return false;
		}
	}

	public void transferNutrientsToChildLeafs(Element[,] currentElementArray)
	{
		if (childLeafs.Count == 0) return;
		float maxNutrientTransferAmountPerChild = Math.Min(nutrient / childLeafs.Count, 0.5f); // max 0.5 nutrient per child per activity tick
		float maxWetnessTransferAmountPerChild = Math.Min(wetness / childLeafs.Count, 0.1f); // max 0.1 wetness per child per activity tick
		foreach (var pos in childLeafs)
		{
			if (currentElementArray[pos.Item1, pos.Item2] is Leaf childLeaf)
			{
				// transfer nutrients
				if (childLeaf.nutrient < childLeaf.maxNutrient && nutrient - maxNutrientTransferAmountPerChild > 0)
				{
					float actualTransfer = Math.Min(maxNutrientTransferAmountPerChild, childLeaf.maxNutrient - childLeaf.nutrient);
					nutrient -= actualTransfer;
					childLeaf.nutrient += actualTransfer;
				}

				// transfer wetness
				if (childLeaf.wetness < 1f && wetness - maxWetnessTransferAmountPerChild > 0)
				{
					float actualWetnessTransfer = Math.Min(maxWetnessTransferAmountPerChild, 1f - childLeaf.wetness);
					wetness -= actualWetnessTransfer;
					childLeaf.wetness += actualWetnessTransfer;
				}
			}
		}
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		Seed seed = getParentSeed(currentElementArray);
		if (seed == null && seed?.plantState == PlantState.Dying) // if parent seed is gone or dying, start dying
		{
			leafState = LeafState.Dying;
		}

		if (leafState == LeafState.Dying && rng.Randf() < 0.1f) // 10% chance to die definitively each tick
		{
			Soil soil = new Soil();
			soil.nutrient = nutrient;
			soil.wetness = wetness;
			currentElementArray[x, y] = soil;
			return;
		}
		// Try to grow leaves if possible
		if (leafState == LeafState.Growing && T - lastGrowthTick >= growthInterval)
		{
			lastGrowthTick = T;
			growLeaf(oldElementArray, currentElementArray, x, y, maxX, maxY);

		}

		if (leafState == LeafState.Sleeping)
		{
			transferNutrientsToChildLeafs(currentElementArray);
		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
	
	override public string inspectInfo()
	{
		return $"  Wetness: {wetness:F3}\n  Nutrient: {nutrient:F3}\n  Leaf State: {leafState}\n  Child Leafs: {childLeafs.Count}\n";
	}
}