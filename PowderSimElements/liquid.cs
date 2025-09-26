using Godot;

public class Liquid : Element
{
	private int maxLifetime = 60 * 5;
	private int lifetime;
	private RandomNumberGenerator rng = new();
	private bool flowingLeft;

	override public void init(Element[,] currentElementArray, int x, int y, int maxX, int maxY)
	{
		lifetime = maxLifetime;

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
		bool gravity_status = base.gravity(x, y, maxY, currentElementArray, oldElementArray);
		if (gravity_status)
		{
			lifetime = maxLifetime; // reset lifetime (there was a meaningful movement)
		}

		//Diag Left movement
		if (0 <= x - 1 && y + 1 < maxY && Element.metaIsNull(x - 1, y + 1, currentElementArray, oldElementArray))
		{
			base.move(x, y, x - 1, y + 1, currentElementArray);
			lifetime = maxLifetime; // reset lifetime (there was a meaningful movement)
			return;
		}

		// Diag Right movement
		if (x + 1 < maxX && y + 1 < maxY && Element.metaIsNull(x + 1, y+1, currentElementArray, oldElementArray))
		{
			base.move(x, y, x + 1, y + 1, currentElementArray);
			lifetime = maxLifetime; // reset lifetime (there was a meaningful movement)
			return;
		}

		// Flowing left
		if (flowingLeft && 0 <= x - 1 && Element.metaIsNull(x - 1, y, currentElementArray, oldElementArray))
		{
			base.move(x, y, x - 1, y, currentElementArray);
			lifetime -= 1;
			return;
		}
		else if (flowingLeft) // Left side is blocked
		{
			flowingLeft = false;
		}

		// Flowing right
		if (!flowingLeft && x + 1 < maxX && Element.metaIsNull(x + 1, y, currentElementArray, oldElementArray))
		{
			base.move(x, y, x + 1, y, currentElementArray);
			lifetime -= 1;
			return;
		}
		else if (!flowingLeft) // Right side is blocked
		{
			flowingLeft = true;
		}
	}
}
