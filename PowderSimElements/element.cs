using Godot;

public abstract class Element
{
    public Color color { get; protected set; }
    public double density { get; protected set; }

    abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);
}