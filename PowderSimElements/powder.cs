using System;
using Godot;

public class Powder : Element
{
	RandomNumberGenerator rng = new();
	private float wetness;
	public float Wetness   // property
	{
		get { return wetness; }   // get method
		set { wetness = Math.Clamp(value, 0, 1); }  // set method
	}
        
    override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
    {
        if (currentElementArray[x, y] != this) return; // Return if a movement has already been done
        if (y + 1 >= maxY) return; // Return if we are on the bottom of the array

		// Down movement
        gravity(x, y, maxY, currentElementArray, oldElementArray);

		// Diag Left and Diag Right movement possible
		if (x+1 < maxX && canMoveDownOnElement(oldElementArray[x+1, y+1]) && 0 <= x-1 && canMoveDownOnElement(oldElementArray[x+1, y+1]))
		{
			if (rng.RandiRange(0, 1) == 0)
			{
				move(x, y, x+1, y+1, currentElementArray);
			}
			else
			{
				move(x, y, x-1, y+1, currentElementArray);
			}
			return;
		}

		// Diag Right movement
		if (0 <= x-1 && canMoveDownOnElement(oldElementArray[x-1, y+1]))
		{
			move(x, y, x-1, y+1, currentElementArray);
			return;
		}

		// Diag Left movement
		if (x+1 < maxX && canMoveDownOnElement(oldElementArray[x+1, y+1]))
		{
			move(x, y, x+1, y+1, currentElementArray);
			return;
		}
	}
}
