using System.Runtime.CompilerServices;
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
        return elementWhereMovement == null || elementWhereMovement.density < density;
    }

    protected bool move(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int movementX, int movementY)
    {
        int newX = x + movementX, newY = y + movementY;

        if (newX >= maxX || newX < 0 ||
            newY >= maxY || newY < 0) return false;

        if (
                (movementY > 0 &&
                canMoveDownOnElement(oldElementArray[x + movementX, y + movementY]))
            ||
                (movementY < 0 &&
                canMoveUpOnElement(oldElementArray[newX, newY]))
            ||
                (movementY == 0 &&
                canMoveSideOnElement(oldElementArray[x + movementX, y + movementY]))
            )
        {
            currentElementArray[x, y] = currentElementArray[newX, newY];
            currentElementArray[newX, newY] = this;
            return true;
        }

        return false;
    }
    abstract public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY);

}
