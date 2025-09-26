using Godot;

public abstract class Element
{
	public Color color { get; protected set; }
	public double density { get; protected set; }

    protected bool canMoveDownOnElement(Element elementWhereMovement)
    {
        return elementWhereMovement == null || elementWhereMovement.density < density;
    }

    protected bool canMoveUpOnElement(Element elementWhereMovement)
    {
        return elementWhereMovement == null || elementWhereMovement.density > density;
    }

    protected bool canMoveSideOnElement(Element elementWhereMovement)
    {
        return elementWhereMovement == null;
    }

    protected bool gravity(int x, int y, int maxY, Element[,] currentElementArray, Element[,] oldElementArray)
	{
		// returns true if gravity was applied, false if not. 
		if (y + 1 >= maxY) { return false; }

		if (canMoveDownOnElement(oldElementArray[x, y+1]))
		{
            move(x, y, x, y + 1, currentElementArray);
            return true;
		}
		return false;
	}

	protected void move(int current_x, int current_y, int next_x, int next_y, Element[,] currentElementArray)
	{
		// Swaps the current cell with the next cell (can be null)
        (currentElementArray[next_x, next_y], currentElementArray[current_x, current_y]) = (currentElementArray[current_x, current_y], currentElementArray[next_x, next_y]);
	}

    abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);

}
