using System;
using System.ComponentModel;
using Godot;

public class Gas : Element
{
	public int cloudLineY = 10;

	public bool sleeping = false;

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