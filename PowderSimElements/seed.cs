using System;
using System.Runtime.InteropServices;
using Godot;

public class Seed : Life
{
	public float nutrient { get; set; }
	private int lastGrowthTick = 0;
	private int growthInterval = 1 * 60; // ticks

	private (int, int) startingLeaf = (-1, -1);
	public PlantState plantState = PlantState.Falling;

	public enum PlantState
	{
		Falling,
		Seed,
		Growing,
		Mature,
		Dying
	}
	public Seed()
	{
		ashCreationPercentage = 0.2f;
		density = 15;
		color = Colors.Burlywood;
		flammability = 4;
		nutrient = 10f;
		wetness = 1f;
	}

	private bool growStartingRoot(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		// Try to grow root downwards if there's space
		if (y + 1 < maxY && currentElementArray[x, y + 1] is Soil)
		{
			currentElementArray[x, y + 1] = new Root((x, y));
			nutrient -= 1f;
			return true;
		}
		return false;
	}

	private (int, int) growStartingLeaf(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient < 1) return (-1, -1); // not enough nutrient to grow
		if (y - 1 < 0) return (-1, -1); // no space above

		// Try to grow leaves upwards if there's space
		if (currentElementArray[x, y - 1] == null)
		{
			currentElementArray[x, y - 1] = new Leaf((x, y));
			startingLeaf = (x, y - 1);
			nutrient -= 1f;
			return (x, y - 1);
		}

		return (-1, -1);
	}

	private void transferNutrientsUpwards(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (y - 1 < 0) return; // no space above
		if (startingLeaf == (-1, -1)) return; // no starting leaf to transfer to

		if (currentElementArray[startingLeaf.Item1, startingLeaf.Item2] is Leaf firstLeaf)
		{
			if (firstLeaf.nutrient < 5f)
			{
				float transferAmount = Math.Min(nutrient, 0.5f); // transfer up to 0.5 nutrient per tick
				nutrient -= transferAmount;
				firstLeaf.nutrient += transferAmount;
				GD.Print($"Transferring {transferAmount} nutrient to leaf at {startingLeaf}");
			}

			// same thing with wetness
			if (firstLeaf.wetness < 1f)
			{
				float wetnessTransfer = Math.Min(wetness, 0.5f); // transfer up to 0.5 wetness per tick
				wetness -= wetnessTransfer;
				firstLeaf.wetness += wetnessTransfer;
				GD.Print($"Transferring {wetnessTransfer} wetness to leaf at {startingLeaf}");
			}
			

		}
		else
		{
			plantState = PlantState.Dying; // first leaf no longer exists, die
		}
	}
	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (y + 1 < maxY && plantState == PlantState.Falling)
		{
			if (!move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1)) // if cannot fall further
			{
				plantState = PlantState.Seed; // become a seed
			}
		}

		if (plantState == PlantState.Seed && T - lastGrowthTick >= growthInterval)
		{
			lastGrowthTick = T;
			// Try to grow roots first
			if (!growStartingRoot(currentElementArray, x, y, maxX, maxY))
			{
				// try to grow leaves
				growStartingLeaf(currentElementArray, x, y, maxX, maxY);
				if (startingLeaf != (-1, -1))
				{
					plantState = PlantState.Growing; // finished initial growth
				}
			}
		}

		if (plantState == PlantState.Growing && nutrient > 0)
		{
			transferNutrientsUpwards(currentElementArray, x, y, maxX, maxY);
		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
	
	override public string inspectInfo()
	{
		return base.inspectInfo() + $"  Seed Nutrient: {nutrient:F3}\n  Plant State: {plantState}\n";
	}

}
