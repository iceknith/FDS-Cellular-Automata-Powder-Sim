using Godot;

public abstract class Element
{
	public Color color { get; protected set; }
	public double density { get; protected set; }

	abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);
	abstract public void init(Element[,] currentElementArray, int x, int y, int maxX, int maxY);

	public static bool metaIsNull(int x, int y, Element[,] currentElementArray, Element[,] oldElementArray)
	{
		// Needed to check before any movement that there is no cell already planning to move to that place 
		// (can cause weird top - down priority artefacts)
		return currentElementArray[x, y] == null && oldElementArray[x, y] == null;
	}

	public bool gravity(int x, int y, int maxY, Element[,] currentElementArray, Element[,] oldElementArray)
	{
		// returns true if gravity was applied, false if not. 
		if (y + 1 >= maxY) { return false; }

		if (oldElementArray[x, y + 1] != null && oldElementArray[x, y + 1].density >= oldElementArray[x, y].density)
		{
			return false; // there is something beneath the cell that is higher or equal density
		}
		move(x, y, x, y + 1, currentElementArray);
		return true;
	}

	public void move(int current_x, int current_y, int next_x, int next_y, Element[,] currentElementArray)
	{
		// Swaps the current cell with the next cell (can be null)
		(currentElementArray[next_x, next_y], currentElementArray[current_x, current_y]) = (currentElementArray[current_x, current_y], currentElementArray[next_x, next_y]);
	}

}
