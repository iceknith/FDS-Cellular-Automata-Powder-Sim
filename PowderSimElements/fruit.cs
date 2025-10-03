using Godot;

public class Fruit : Life
{
	public bool pollinated = false;
	private int lifetimeOnSoil = 300 * 60; // ticks
	public Fruit()
	{
		ashCreationPercentage = 0.0f;
		density = 3;
		color = Colors.Red;
		flammability = 3;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (y + 1 !< maxY) return; // out of bounds below

		if (oldElementArray[x, y + 1] is Soil)
		{
			lifetimeOnSoil--;
		}
		if (lifetimeOnSoil <= 0 && currentElementArray[x, y] == this)
		{
			// Fruit has withered away
			SurfBiomass biomass = new SurfBiomass(wetness, nutrient);
			currentElementArray[x, y] = biomass;
			return;
		}

		if (currentElementArray[x, y + 1] == null || currentElementArray[x, y + 1] is Gas || currentElementArray[x, y + 1] is Liquid)
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1);
		}

		// if polliated and on soil, try to grow a seed
		if (pollinated && oldElementArray[x, y + 1] is Soil)
		{
			// 1% chance each tick to grow a seed
			if (GD.Randf() < 0.01f)
			{
				currentElementArray[x, y] = new Seed();
				return;
			}
		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}


}
