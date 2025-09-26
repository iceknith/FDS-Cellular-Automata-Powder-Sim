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
		if (oldElementArray[x, y + 1] == null)
		{
			currentElementArray[x, y] = null;
			currentElementArray[x, y + 1] = this;
			return;
		}

		// Left and Right movement possible
		if (x + 1 < maxX && oldElementArray[x + 1, y + 1] == null && 0 <= x - 1 && oldElementArray[x - 1, y + 1] == null)
		{
			currentElementArray[x, y] = null;
			RandomNumberGenerator rng = new();
			if (rng.RandiRange(0, 1) == 0)
			{
				currentElementArray[x + 1, y + 1] = this;
			}
			else
			{
				currentElementArray[x - 1, y + 1] = this;
			}

			return;
		}

		// Left movement
		if (0 <= x - 1 && oldElementArray[x - 1, y + 1] == null)
		{
			currentElementArray[x, y] = null;
			currentElementArray[x - 1, y + 1] = this;
			return;
		}

		// Right movement
		if (x + 1 < maxX && oldElementArray[x + 1, y + 1] == null)
		{
			currentElementArray[x, y] = null;
			currentElementArray[x + 1, y + 1] = this;
			return;
		}
	}
}
