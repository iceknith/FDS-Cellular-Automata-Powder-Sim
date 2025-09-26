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

        // Down movement
        if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1)) { lifetime = maxLifetime; return; }

        // Diag movements
        if (rng.RandiRange(0, 1) == 0)
        {
            if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) { lifetime = maxLifetime; return; }
            if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) { lifetime = maxLifetime; return; }
		}
        else
        {
            if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) { lifetime = maxLifetime; return; }
            if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) { lifetime = maxLifetime; return; }
        }

        // Side Movement
        if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, directionX, 0)) { lifetime--; return; }
        // If we cannot move sideways, change direction
        else
        {
            directionX *= -1;
        }
    }
}
