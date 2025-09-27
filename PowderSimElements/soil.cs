using Godot;
using System;


public class Soil : Powder
{
	private Color baseColor;
	private float _nutrient;
	public float nutrient
	{
		get { return _nutrient; }   // get method
		set { _nutrient = Math.Max(value, 0); }  // set method
	}
	public Soil()
	{
		density = 10;
		color = Colors.DarkKhaki;
		baseColor = color;
		flammability = 0;
		nutrient = 0.0F;
	}

	private void changeColor()
	{
		float colorChange = float.Lerp(1, 0, 1/(nutrient + 0.001F));
		float clampedColorChange = Math.Clamp(colorChange, 0.15F, 0.8F);
		color = baseColor.Darkened(clampedColorChange);
	}

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		for (int nx = Math.Max(0, x - 1); nx < Math.Min(x + 1, maxX); nx++)
		{
			for (int ny = Math.Max(0, y - 1); ny < Math.Min(y + 1, maxY); ny++)
			{
				if ((nx, ny) != (x, y) && oldElementArray[nx, ny] is Soil soil)
				{
					float diff = soil.nutrient - nutrient;
					soil.nutrient -= diff / 2;
					nutrient += diff / 2;
				}
			}
		}

		changeColor();
	
		base.update(oldElementArray, currentElementArray, x, y, maxX, maxY); // keep at the end because of returns contained in base method
	}
}
