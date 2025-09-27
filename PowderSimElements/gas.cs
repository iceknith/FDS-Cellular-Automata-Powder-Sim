using System;
using System.ComponentModel;
using Godot;

public class Gas : Element
{
	public RandomNumberGenerator rng = new();
	public int cloudLineY = 10;

	public bool sleeping = false;

	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
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

		int decision = rng.RandiRange(0, 3);
		int distFromCloudLine = Math.Abs(cloudLineY - y) + 1;

		if (decision == 0) // drift right
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, 1, 0);
			return;
		}
		else if (decision == 1) // drift left
		{
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, -1, 0);
			return;
		}
		else // vertical movement 
		{
			float distr = rng.Randf();

			int dir = 1; // default move down
			if (cloudLineY - y >= 0)
			{
				dir = -1; // move up if below cloud line
			}

			if (distr < (1f / distFromCloudLine)) // more likely to move the closer to the cloud line the farther away from it
			{
				move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, dir);
			}
			else // if it doesn't move towards the cloud line, it has a small chance to move away from it
			{
				if (rng.Randf() > 0.0f)
				{
					move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, -dir);
				}

			}

		}

	}
}