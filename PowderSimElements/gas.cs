using System;
using System.ComponentModel;
using Godot;

public class Gas : Element
{
	public int cloudLineY = 10;

	public bool sleeping = false;

	public override bool canMoveUpOnElement(Element elementWhereMovement)
	{
		return elementWhereMovement == null || elementWhereMovement.density < density || elementWhereMovement is Gas;
	}

	override public bool move(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int movementX, int movementY)
	{
		int newX = x + movementX, newY = y + movementY;
		if (newY < 0 || newY >= maxY || newX < 0 || newX >= maxX) return false; // Prevent moving out of bounds

		if (currentElementArray[newX, newY] is Web)
		{
			currentElementArray[x, y] = null;
			currentElementArray[newX, newY] = this;
			return true;
		}  // Webs get destroyed by gases
		return base.move(oldElementArray, currentElementArray, x, y, maxX, maxY, movementX, movementY);
	}

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (sleeping)
		{
			sleeping = false;
			return;
		}
		else
		{
			sleeping = true;
		}

		if (currentElementArray[x, y] != this) return; // Return if a movement has already been done

		// Use a simple but perfectly balanced approach
		float decision = rng.Randf(); // 0.0 to 1.0
		int distFromCloudLine = Math.Abs(cloudLineY - y) + 1;

		if (decision < 0.25f) // 25% chance - drift left (changed order to eliminate any potential bias)
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 0);
		}
		else if (decision < 0.5f) // 25% chance - drift right
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 0);
		}
		else // 50% chance - vertical movement 
		{
			float distr = rng.Randf();

			int dir = 1; // default move down
			if (cloudLineY - y >= 0)
			{
				dir = -1; // move up if below cloud line
			}

			if (distr < (1f / distFromCloudLine)) // more likely to move towards cloud line when farther away
			{
				move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, dir);
			}
			else if (rng.Randf() > 0.7f) // 30% chance to move away from cloud line
			{
				move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, -dir);
			}
			// else: 70% chance to stay put for vertical movement
		}

		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
}