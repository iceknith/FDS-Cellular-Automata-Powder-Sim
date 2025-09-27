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

		// Down movement
		if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1)) return;

		// Diag movements
		if (rng.RandiRange(0, 1) == 0)
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) return;
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) return;
		}
		else
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) return;
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) return;
		}
	}
}
