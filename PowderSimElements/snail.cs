using Godot;
using System;
using System.Collections.Generic;
public class Snail : Life
{
	private int moveInterval = 30; // ticks
	private int lastMoveTick = 0;
	private (int, int) lastMoveDir = (0, 0); // to avoid going back and forth

	enum SnailState
	{
		Falling,
		Moving,
		Idle
	}
	private SnailState snailState = SnailState.Falling;
	public Snail()
	{
		ashCreationPercentage = 0.0f;
		density = 3;
		color = Colors.Beige;
		flammability = 3;
	}
	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		switch (snailState)
		{
			case SnailState.Falling:
				// check if can fall down
				if (y + 1 < maxY && (currentElementArray[x, y + 1] == null || currentElementArray[x, y + 1] is Water))
				{
					// fall down
					currentElementArray[x, y + 1] = this;
					currentElementArray[x, y] = null;
				}
				else
				{
					snailState = SnailState.Idle; // landed
				}
				break;
			case SnailState.Idle:
				// check if can start moving
				if (T - lastMoveTick >= moveInterval)
				{
					snailState = SnailState.Moving;
					lastMoveTick = T;
				}
				break;

			case SnailState.Moving:
				// try to move in a random direction on the surface
				List<(int, int)> possibleDirs = [(-1, 0), (1, 0), (0, -1), (0, 1)];
				possibleDirs.Remove((-lastMoveDir.Item1, -lastMoveDir.Item2)); // avoid going back and forth

				// shuffle directions
				Random rng = new Random();
				int n = possibleDirs.Count;
				while (n > 1)
				{
					int k = rng.Next(n--);
					var temp = possibleDirs[n];
					possibleDirs[n] = possibleDirs[k];
					possibleDirs[k] = temp;
				}

				bool moved = false;
				foreach (var dir in possibleDirs)
				{
					int newX = x + dir.Item1;
					int newY = y + dir.Item2;
					if (newX >= 0 && newX < maxX && newY >= 0 && newY < maxY)
					{
						if (currentElementArray[newX, newY] == null)
						{
							// check if there's solid ground below the target position
							if (newY + 1 < maxY && currentElementArray[newX, newY + 1] != null && !(currentElementArray[newX, newY + 1] is Water))
							{
								// move to the new position
								currentElementArray[newX, newY] = this;
								currentElementArray[x, y] = null;
								lastMoveDir = dir;
								moved = true;
								break;
							}
						}
					}
				}
				if (!moved)
				{
					snailState = SnailState.Idle; // couldn't move, go back to idle
				}
				break;
		}


		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}


}
