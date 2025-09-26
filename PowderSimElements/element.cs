using Godot;

public abstract class Element
{
    public Color color { get; protected set; }
    public double density { get; protected set; }

    protected bool canMoveDownOnElement(Element elementWhereMovement)
    {
        return elementWhereMovement == null || elementWhereMovement.density < density;
    }

    protected bool canMoveSideOnElement(Element elementWhereMovement)
    {
        return elementWhereMovement == null;
    }

    abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);
}