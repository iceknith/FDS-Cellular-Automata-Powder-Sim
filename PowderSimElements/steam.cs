using Godot;
using System;
using System.Runtime.InteropServices;
public class Steam : Gas
{
	public Steam() : base()
	{
		density = 1;
		color = Colors.Gray;
		flammability = 0;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
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
			if (neighbourCount >= 3) // condensate only if there are many other steam particles around
			{
				currentElementArray[x, y] = new Water(); // condensate
				return;
			}
		}

		base.update(oldElementArray, currentElementArray, x, y, maxX, maxY); // keep at the end because of returns contained in base method
	}
}
