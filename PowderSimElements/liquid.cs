using Godot;

public class Liquid : Element
{
    protected int directionX = 1;
    public bool wet { get; set; }

    override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
    {
        if (currentElementArray[x, y] != this) return;
        // Return if a movement has already been done

        if (y + 1 < maxY) // If we can move down
        {
            // Down movement
            if (canMoveDownOnElement(oldElementArray[x, y + 1]))
            {
                currentElementArray[x, y] = currentElementArray[x, y + 1];
                currentElementArray[x, y + 1] = this;
                return;
            }

            // Left Down movement
            if (0 <= x - 1 && canMoveDownOnElement(oldElementArray[x - 1, y + 1]))
            {
                currentElementArray[x, y] = currentElementArray[x - 1, y + 1];
                currentElementArray[x - 1, y + 1] = this;
                directionX = -1;
                return;
            }

            // Right Down movement
            if (x + 1 < maxX && canMoveDownOnElement(oldElementArray[x + 1, y + 1]))
            {
                currentElementArray[x, y] = currentElementArray[x + 1, y + 1];
                currentElementArray[x + 1, y + 1] = this;
                directionX = 1;
                return;
            }
        }

        // Side Movement
        if (0 <= x + directionX && x + directionX < maxX &&
            canMoveSideOnElement(oldElementArray[x + directionX, y]))
        {
            currentElementArray[x, y] = currentElementArray[x + directionX, y];
            currentElementArray[x + directionX, y] = this;
            return;
        }
        // If we cannot move sideways, change direction
        else
        {
            directionX *= -1;
        }
    }
}