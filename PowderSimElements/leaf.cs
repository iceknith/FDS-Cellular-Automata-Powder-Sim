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

	private (int, int) parentSeed;
	public Leaf((int, int) parentSeed)
	{
		this.parentSeed = parentSeed;
		density = 15;
		color = Colors.Green;
		flammability = 10;
		nutrient = 0f;
		maxNutrient = 5f;
		wetness = 0f;
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
	private bool IsBehindGrowthDirection(int pos, int origin, int dir)
	{
		return dir != 0 && ((dir > 0 && pos <= origin) || (dir < 0 && pos >= origin));
	}

	private bool isValidLeafGrowthPosition(Element[,] currentElementArray, int x, int y, int maxX, int maxY, (int, int) growthDir, (int, int) growthOrigin)
	{
		if (x < 0 || x >= maxX || y < 0 || y >= maxY) return false; // out of bounds
		if (currentElementArray[x, y] != null) return false; // must be empty
		int adjacentLeaves = 0;

		for (int nx = x - 1; nx <= x + 1; nx++)
		{
			for (int ny = y - 1; ny <= y + 1; ny++)
			{
				if ((nx, ny) == (x, y)) continue;
				if (nx >= 0 && nx < maxX && ny >= 0 && ny < maxY)
				{
					// ignore everything behind growth direction
					if (IsBehindGrowthDirection(nx, growthOrigin.Item1, growthDir.Item1) ||
						IsBehindGrowthDirection(ny, growthOrigin.Item2, growthDir.Item2))
					{
						continue;
					}

					if (currentElementArray[nx, ny] is Leaf)
					{
						adjacentLeaves++;
					}
				}
			}
		}
		if (adjacentLeaves > 0) return false; // prevent too dense leaf growth
		return true;
	}

	private int getScoreForGrowthPosition((int, int) pos, int x, int y, int seedX, int seedY)
	{
		int score = 0;

		if (pos.Item1 == seedX)
		{
			if (Math.Abs(pos.Item2 - seedY) < 10)
			{
				score += 30;
			}
			else
			{
				score = -50;
				GD.Print($"Rejecting {pos} because too far from seed vertically");
			}
		}
		
		// Prefer positions closer to the seed
		//int distToSeed = Math.Abs(pos.Item1 - seedX) + Math.Abs(pos.Item2 - seedY);
		//score -= distToSeed * 10;

		// Prefer positions higher up
		//score -= pos.Item2 * 5;

		// Prefer positions more centered above the parent leaf
		//int distToParent = Math.Abs(pos.Item1 - x);
		//score -= distToParent * 2;

		return score;
	}
	private (int, int) getBestGrowthPosition((int, int)[] possiblePositions, int maxX, int maxY, int x, int y, int seedX, int seedY)
	{
		(int, int) bestPos = possiblePositions[0];
		int bestScore = getScoreForGrowthPosition(bestPos, x, y, seedX, seedY);

		foreach (var pos in possiblePositions)
		{
			int score = getScoreForGrowthPosition(pos, x, y, seedX, seedY);
			if (score > bestScore)
			{
				bestScore = score;
				bestPos = pos;
			}
		}

		return bestPos;
	}

	public bool growLeaf(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient < 1) return false;
		if (wetness < 0.5f) return false;
		if (y - 1 < 0) return false;

		List<(int, int)> possibleGrowthPositions = [];

		for (int nx = x - 1; nx <= x + 1; nx++)
		{
			for (int ny = y - 1; ny <= y; ny++)
			{
				if (isValidLeafGrowthPosition(oldElementArray, nx, ny, maxX, maxY, (nx - x, ny - y), (x, y))) // ------------------------------------------ 
				{
					possibleGrowthPositions.Add((nx, ny));
				}
			}
		}
		if (possibleGrowthPositions.Count > 0)
		{
			var rand = new Random();
			var chosenPos = getBestGrowthPosition(possibleGrowthPositions.ToArray(), maxX, maxY, x, y, parentSeed.Item1, parentSeed.Item2);
			currentElementArray[chosenPos.Item1, chosenPos.Item2] = new Leaf(parentSeed);
			childLeafs.Add(chosenPos);
			var parent = getParentSeed(currentElementArray);
			if (parent != null)
			{
				parent.leafCount++;
			}
			nutrient -= 1f;
			wetness -= 0.5f;
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
		if (seed == null || seed?.plantState == PlantState.Dying) // if parent seed is gone or dying, start dying
		{
			leafState = LeafState.Dying;
		}

		if (leafState == LeafState.Dying && rng.Randf() < 0.01f) // 1% chance to die definitively each tick
		{
			Biomass biomass = new Biomass(wetness, nutrient);
			currentElementArray[x, y] = biomass;
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