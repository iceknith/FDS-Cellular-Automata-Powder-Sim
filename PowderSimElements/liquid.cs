using System.Runtime.InteropServices;
using Godot;

public class Liquid : Element
{
	protected int directionX = 1;
	private int maxLifetime = 60 * 3;
	private int lifetime;
	public Liquid()
	{
		lifetime = maxLifetime;
		directionX = 2 * rng.RandiRange(0, 1) - 1; // Random start direction
	}

	/// <summary>
	/// Will trigger when the liquid tries to evaporates.
	/// To override if needed.
	/// </summary>
	public virtual void onEvaporate(Element[,] currentElementArray, int x, int y)
	{
		if (currentElementArray[x, y] != this) return;
		currentElementArray[x, y] = null;
	}

	override public bool move(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int movementX, int movementY)
	{
		int newX = x + movementX, newY = y + movementY;
		if (newY < 0 || newY >= maxY || newX < 0 || newX >= maxX) return false; // Prevent moving out of bounds (recheck but allows using the base function)

		if (currentElementArray[newX, newY] is Web)
		{
			currentElementArray[x, y] = null;
			currentElementArray[newX, newY] = this;
			return true;
		}  // Webs get destroyed by liquids
		return base.move(oldElementArray, currentElementArray, x, y, maxX, maxY, movementX, movementY);
	}

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (currentElementArray[x, y] != this) return; // Return if a movement has already been done

		// evaporate if lifetime is over and we are not below another liquid or at the bottom
		if (lifetime <= 0
		&& !burning
		&& (y - 1 == maxY
		|| (y + 2 < maxY
		&& currentElementArray[x, y + 1] is not Liquid
		&& currentElementArray[x, y + 2] is not Liquid)))
		{
			onEvaporate(currentElementArray, x, y);
			return;
		}

		if (y + 2 < maxY && oldElementArray[x, y + 1] is Leaf)
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, -2); // move down through leaves
		}
		// Down movement
		if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1)) { lifetime = maxLifetime; return; }

		// Diag movements
		float randomFloat = rng.Randf();
		if (randomFloat < 0.5f)
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) { lifetime = maxLifetime; return; }
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) { lifetime = maxLifetime; return; }
		}
		else
		{
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 1)) { lifetime = maxLifetime; return; }
			if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 1)) { lifetime = maxLifetime; return; }
		}

		if (randomFloat < 0.1f) // small chance to change direction randomly
		{
			directionX *= -1;
		}

		// Side Movement
		if (move(oldElementArray, currentElementArray, x, y, maxX, maxY, directionX, 0)) { lifetime--; return; }
		// If we cannot move sideways, change direction
		else
		{
			directionX *= -1;
		}


		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
}
