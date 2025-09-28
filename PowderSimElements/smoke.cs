using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
public class Smoke : Gas
{
	public Smoke() : base()
	{
		density = 0.1f;
		color = Colors.DarkGray;
		flammability = 0;
		wetness = 0.0f;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{

		if (rng.Randf() < 0.005f && currentElementArray[x, y] == this) // very small chance to dissipate
		{
			currentElementArray[x, y] = null; // dissipate
			return;
		}

		base.update(oldElementArray, currentElementArray, x, y, maxX, maxY, T); // keep at the end because of returns contained in base method
		
	}
}
