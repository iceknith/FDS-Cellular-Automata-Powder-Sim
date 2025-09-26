using Godot;

public class Liquid : Element
{
    protected int directionX = 1;
    private int maxLifetime = 60 * 3;
	private int lifetime;
    private RandomNumberGenerator rng = new();

    public Liquid()
    {
        lifetime = maxLifetime;
        directionX = 2 * rng.RandiRange(0, 1) - 1; // Random start direction
    }

    override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
    {
        if (currentElementArray[x, y] != this) return; // Return if a movement has already been done

        if (lifetime <= 0)
        {
            currentElementArray[x, y] = null; // Delete the element
            return;
        }

        if (y + 1 < maxY) // If we can move down
        {
            // Down movement
            if (canMoveDownOnElement(oldElementArray[x, y + 1]))
            {
                lifetime = maxLifetime;
                currentElementArray[x, y] = currentElementArray[x, y + 1];
                currentElementArray[x, y + 1] = this;
                return;
            }

            // Diag Left and Diag Right movement possible
            if (x+1 < maxX && canMoveDownOnElement(oldElementArray[x+1, y+1]) && 0 <= x-1 && canMoveDownOnElement(oldElementArray[x-1, y+1]))
            {
                lifetime = maxLifetime;
                if (rng.RandiRange(0, 1) == 0)
                {
                    move(x, y, x + 1, y + 1, currentElementArray);
                    directionX = 1;
                }
                else
                {
                    move(x, y, x - 1, y + 1, currentElementArray);
                    directionX = -1;
                }
                return;
            }

            // Left Down movement
            if (0 <= x-1 && canMoveDownOnElement(oldElementArray[x-1, y+1]))
            {
                lifetime = maxLifetime;
                move(x, y, x-1, y+1, currentElementArray);
                directionX = -1;
                return;
            }

            // Right Down movement
            if (x + 1 < maxX && canMoveDownOnElement(oldElementArray[x + 1, y + 1]))
            {
                lifetime = maxLifetime;
                move(x, y, x + 1, y + 1, currentElementArray);
                directionX = 1;
                return;
            }
        }

        // Side Movement
        if (0 <= x + directionX && x + directionX < maxX &&
            canMoveSideOnElement(oldElementArray[x + directionX, y]))
        {
            lifetime--;
            move(x, y, x + directionX, y, currentElementArray);
            return;
        }
        // If we cannot move sideways, change direction
        else
        {
            directionX *= -1;
        }
    }
}
