using Godot;
using System;
public class Steam : Gas
{
	public Steam() : base()
	{
		density = 1;
		color = new Color(Colors.WhiteSmoke.R, Colors.WhiteSmoke.G, Colors.WhiteSmoke.B, 0.05f);
		flammability = 0;
		wetness = 1.0f;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{

		// Move up if possible
		if (rng.Randf() < 0.01f) // very small chance to condensate
		{
			// check if there is other steam particles around
			int neighbourCount = 0;
			for (int nx = Math.Max(0, x - 1); nx < Math.Min(x + 1, maxX); nx++)
			{
				for (int ny = Math.Max(0, y - 1); ny < Math.Min(y + 1, maxY); ny++)
				{
					if ((nx, ny) != (x, y) && oldElementArray[nx, ny] is Steam)
					{
						neighbourCount++;
					}
				}
			}
			// there is problems with the neighbour count being low because of particles moving, but it should be fine for now

			if (neighbourCount >= 3) // condensate only if there are many other steam particles around
			{
				currentElementArray[x, y] = new Water(); // condensate

				// small chance to create a water particle above if possible
				if (y - 1 >= 0 && currentElementArray[x, y - 1] == null)
				{
					if (rng.Randf() < 0.1f) // 10% chance to create water above
					{
						currentElementArray[x, y - 1] = new Water();
					}
				}
				return;
			}
		}

		base.update(oldElementArray, currentElementArray, x, y, maxX, maxY, T); // keep at the end because of returns contained in base method
	}
}
