using System.Security.Cryptography.X509Certificates;
using Godot;

public class Water : Liquid
{
	public Water() : base()
	{
		density = 5;
		color = Colors.Blue;
		flammability = 0;
		wetness = 1.0f;
	}

	public override void onEvaporate(Element[,] currentElementArray, int x, int y)
	{
		currentElementArray[x, y] = new Steam();
	}

    public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
    {
		if (wetness <= 0)
		{
			currentElementArray[x, y] = null;
			return;
		}

		if (rng.Randf() < 0.002f && y - 1 > 0 && oldElementArray[x, y - 1] == null) // can add water to the system if wetness < 1
		{
			onEvaporate(currentElementArray, x, y);
			return;
		}

        base.update(oldElementArray, currentElementArray, x, y, maxX, maxY);
    }

}
