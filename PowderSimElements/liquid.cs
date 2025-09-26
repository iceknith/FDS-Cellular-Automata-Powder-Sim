using Godot;

public class Liquid : Element
{
	private int lifetime = 60 * 10;
	private RandomNumberGenerator rng = new();
	private bool flowingLeft;

	override public void init(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{

		if (rng.RandiRange(0, 1) == 0)
		{
			flowingLeft = false;
		}
		else
		{
			flowingLeft = true;
		}
		;
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{

		// to prevent the fluid of bouncing indefinitely searching for a stable place
		if (lifetime <= 0)
		{
			currentElementArray[x, y] = null;
		}

		// Down movement
		if (y + 1 < maxY && Element.meta_is_null(x, y+1, currentElementArray, oldElementArray))
		{
			currentElementArray[x, y] = null;
			currentElementArray[x, y + 1] = this;
			return;
		}

		//Diag Left movement
		if (0 <= x - 1 && y + 1 < maxY && Element.meta_is_null(x - 1, y+1, currentElementArray, oldElementArray))
		{
			currentElementArray[x, y] = null;
			currentElementArray[x - 1, y + 1] = this;
			return;
		}

		// Diag Right movement
		if (x + 1 < maxX && y + 1 < maxY && Element.meta_is_null(x + 1, y+1, currentElementArray, oldElementArray))
		{
			currentElementArray[x, y] = null;
			currentElementArray[x + 1, y + 1] = this;
			return;
		}

		if (x + 1 < maxX && oldElementArray[x + 1, y] == null & 0 <= x - 1 && oldElementArray[x - 1, y] == null) // If the cell is isolated, it means that it is still searching for a stable place
		{
			lifetime -= 1;
		}

		// Flowing left
		if (flowingLeft && 0 <= x - 1 && Element.meta_is_null(x - 1, y, currentElementArray, oldElementArray))
		{
			currentElementArray[x, y] = null;
			currentElementArray[x - 1, y] = this;

			return;
		}
		else if (flowingLeft) // Left side is blocked
		{
			flowingLeft = false;
		}

		// Flowing right
		if (!flowingLeft && x + 1 < maxX && Element.meta_is_null(x + 1, y, currentElementArray, oldElementArray))
		{
			currentElementArray[x, y] = null;
			currentElementArray[x + 1, y] = this;
			return;
		}
		else if (!flowingLeft) // Right side is blocked
		{
			flowingLeft = true;
		}
	}
}
