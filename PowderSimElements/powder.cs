public class Powder : Element
{

	public override bool canMoveDownOnElement(Element elementWhereMovement)
	{
		return elementWhereMovement == null || elementWhereMovement is Gas || elementWhereMovement is Liquid;
	}

	public override bool canMoveSideOnElement(Element elementWhereMovement)
	{
		return elementWhereMovement == null || elementWhereMovement is Gas || elementWhereMovement is Liquid;
	}

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (currentElementArray[x, y] != this) return; // Return if a movement has already been done

		// Down movement
		if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1)) return;

		// Diag movements
		if (T % 2 == 0)
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) return;
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) return;
		}
		else
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) return;
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) return;
		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
}
