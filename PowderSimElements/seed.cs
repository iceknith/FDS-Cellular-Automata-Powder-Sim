using System.Dynamic;
using Godot;

public class Seed : Life
{
	public float nutrient { get; set; }
	private int lastGrowthTick = 0;
	private int growthInterval = 1 * 60; // ticks
	public Seed()
	{
		ashCreationPercentage = 0.2f;
		density = 3;
		color = Colors.Burlywood;
		flammability = 4;
		nutrient = 10f;
	}

	public virtual bool growRoot(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient <= 0) return false;
		if (y + 1 >= maxY) return false;

		// Try to grow roots downwards if there's space
		if (currentElementArray[x, y + 1] is Soil)
		{
			currentElementArray[x, y + 1] = new Root((x,y));
			nutrient -= 1f;
			return true;
		}
		
		return false;
	}

	public virtual bool growLeaf(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (nutrient <= 0) return false;
		if (y - 1 < 0) return false;

		// Try to grow leaves upwards if there's space
		if (currentElementArray[x, y - 1] == null)
		{
			currentElementArray[x, y - 1] = new Leaf((x,y));
			nutrient -= 1f;
			return true;
		}

		return false;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (lastGrowthTick - T >= growthInterval)
		{
			lastGrowthTick = T;
			// Try to grow roots first
			if (!growRoot(oldElementArray, currentElementArray, x, y, maxX, maxY))
			{
				// If root growth fails, try to grow leaves
				growLeaf(oldElementArray, currentElementArray, x, y, maxX, maxY);
			}

		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}


}
