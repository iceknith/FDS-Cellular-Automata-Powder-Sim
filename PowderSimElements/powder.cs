using System;
using Godot;

public class Powder : Element
{
	private float wetness;
	public float Wetness   // property
	{
		get { return wetness; }   // get method
		set { wetness = Math.Clamp(value, 0, 1); }  // set method
	}

	override public void init(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		// empty
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		if (y + 1 >= maxY) return;

		// Down movement
		base.gravity(x, y, maxY, currentElementArray, oldElementArray);

		// Diag Left and Diag Right movement possible
		if (x + 1 < maxX && oldElementArray[x + 1, y + 1] == null && 0 <= x - 1 && oldElementArray[x - 1, y + 1] == null)
		{
			
			RandomNumberGenerator rng = new();
			if (rng.RandiRange(0, 1) == 0)
			{
				base.move(x, y, x + 1, y + 1, currentElementArray);
			}
			else
			{
				base.move(x, y, x - 1, y + 1, currentElementArray);
			}

			return;
		}

		// Diag Left movement
		if (0 <= x - 1 && oldElementArray[x - 1, y + 1] == null)
		{
			base.move(x, y, x - 1, y + 1, currentElementArray);
			return;
		}

		// Diag Right movement
		if (x + 1 < maxX && oldElementArray[x + 1, y + 1] == null)
		{
			base.move(x, y, x + 1, y + 1, currentElementArray);
			return;
		}
	}
}
