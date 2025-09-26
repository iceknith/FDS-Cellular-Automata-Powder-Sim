using Godot;

public abstract class Element
{
	public Color color { get; protected set; }
	public double density { get; protected set; }

	abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);
	abstract public void init(Element[,] currentElementArray, int x, int y, int maxX, int maxY);

	
	public static bool meta_is_null(int x, int y, Element[,] currentElementArray, Element[,] oldElementArray) {
		// Needed to check before any movement that there is no cell already planning to move to that place 
		// (can cause weird top - down priority artefacts)
		return currentElementArray[x, y] == null && oldElementArray[x, y] == null;
	}

}
