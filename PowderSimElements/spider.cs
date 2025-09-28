using Godot;
using System;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
public class Spider : Life
{
	private (int, int) buildingDirection;
	private (int, int) lastMoveDirection = (0, 0);
	private (int, int) wanderingDirection = (0, 0);
	private int lastMeaningfulStateChangeTick = 0; // To prevent rapid state changes
	private Web onWeb;
	private SpiderFSM currentState = SpiderFSM.FALLING;
	public enum SpiderFSM
	{
		FALLING,
		WANDERING_ON_WEB,
		WANDERING_TO_BUILD_SITE,
		BUILDING
	}
	public Spider() : base()
	{
		density = 10;
		color = Colors.Violet;
		flammability = 0.5f;
	}

	private int getScore((int, int) direction, (int, int) wanderingDirection)
	{
		return - Math.Abs(direction.Item1 - wanderingDirection.Item1) - Math.Abs(direction.Item2 - wanderingDirection.Item2); // the more aligned with wandering direction, the higher the score
	}

	private void handleFallingState(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		bool solidNeighbor = false;
		for (int nx = Math.Max(0, x - 1); nx <= Math.Min(x + 1, maxX - 1); nx++) // including diagonals
		{
			for (int ny = Math.Max(0, y - 1); ny <= Math.Min(y + 1, maxY - 1); ny++)
			{
				if ((nx, ny) == (x, y))
				{
					continue;
				}
				if (oldElementArray[nx, ny] != null && oldElementArray[nx, ny].density >= density && oldElementArray[nx, ny] is not Spider)
				{
					solidNeighbor = true;
					break;
				}
			}
			if (solidNeighbor) break;
		}
		if (solidNeighbor)
		{
			currentState = SpiderFSM.WANDERING_TO_BUILD_SITE;
			lastMeaningfulStateChangeTick = T;

			wanderingDirection = (rng.RandiRange(-1, 1), rng.RandiRange(-1, 1));
		}
		else
		{
			// Continue falling
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, 0, 1);
		}
	}

	private void handleWanderingOnWebState(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (T - lastMeaningfulStateChangeTick > 6 * 60 && rng.Randf() < 0.01f) // After 6 seconds, small chance to start wandering to build site
		{
			currentState = SpiderFSM.WANDERING_TO_BUILD_SITE; // Too long on web, start wandering to build site
			lastMeaningfulStateChangeTick = T;
			return;
		}

		for (int nx = Math.Max(0, x - 1); nx <= Math.Min(x + 1, maxX - 1); nx++) // including diagonals
		{
			for (int ny = Math.Max(0, y - 1); ny <= Math.Min(y + 1, maxY - 1); ny++)
			{
				if ((nx, ny) == (x, y)) { continue; }
				if (oldElementArray[nx, ny] != null && oldElementArray[nx, ny] is Web)
				{
					if (currentElementArray[nx, ny] is not Web) return; // Safety check (can happen if another spider moves onto this cell)
					// Move to a random adjacent web cell
					if (rng.Randf() < 0.1f && (nx - x, ny - y) != lastMoveDirection) // 10% chance to move each tick, but not back the way it came
					{
						specialMove(oldElementArray, currentElementArray, x, y, maxX, maxY, nx - x, ny - y);
						lastMoveDirection = (nx - x, ny - y);
					}
					return; // Stay on the web
				}
			}
		}
		// No adjacent web found, start falling
		currentState = SpiderFSM.FALLING;
	}

	private void handleWanderingToBuildSiteState(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		// get all available cells with webs or that are adjacent to solid cells
		var availableCellsList = new System.Collections.Generic.List<(int, int)>();
		for (int nx = Math.Max(0, x - 1); nx <= Math.Min(x + 1, maxX - 1); nx++)
		{
			for (int ny = Math.Max(0, y - 1); ny <= Math.Min(y + 1, maxY - 1); ny++)
			{
				if (oldElementArray[nx, ny] is Web || (oldElementArray[nx, ny] == null))
				{
					availableCellsList.Add((nx, ny));
				}
			}
		}
		// remove non web cells that are not adjacent to solid cells
		foreach ((int, int) cell in availableCellsList)
		{
			if (oldElementArray[cell.Item1, cell.Item2] == null)
			{
				bool solidNeighbor = false;
				for (int nnx = Math.Max(0, cell.Item1 - 1); nnx <= Math.Min(cell.Item1 + 1, maxX - 1); nnx++) // including diagonals
				{
					for (int nny = Math.Max(0, cell.Item2 - 1); nny <= Math.Min(cell.Item2 + 1, maxY - 1); nny++)
					{
						if ((nnx, nny) == (cell.Item1, cell.Item2)) continue;
						if (oldElementArray[nnx, nny] != null && oldElementArray[nnx, nny].density >= density)
						{
							solidNeighbor = true;
							break;
						}
					}
					if (solidNeighbor) break;
				}
				if (!solidNeighbor)
				{
					availableCellsList.Remove(cell);
				}
			}
		}

		if (availableCellsList.Count == 0)
		{
			// No available cells, start falling, poor thing :(
			currentState = SpiderFSM.FALLING;
			return;
		}

		// Select next cell to move to base on wandering direction bias
		if (availableCellsList.Count == 1)
		{
			(int, int) nextCell = availableCellsList[0];
			move(oldElementArray, currentElementArray, x, y, maxX, maxY, nextCell.Item1 - x, nextCell.Item2 - y);
			return;
		}

		(int, int) bestCell = availableCellsList[0];
		float bestScore = getScore((bestCell.Item1 - x, bestCell.Item2 - y), wanderingDirection);
		foreach ((int, int) cell in availableCellsList) // classic max search
		{
			float score = getScore((cell.Item1 - x, cell.Item2 - y), wanderingDirection);
			if (score > bestScore)
			{
				bestScore = score;
				bestCell = cell;
			}
		}
		specialMove(oldElementArray, currentElementArray, x, y, maxX, maxY, bestCell.Item1 - x, bestCell.Item2 - y);
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		switch (currentState)
		{
			case SpiderFSM.FALLING:
				handleFallingState(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
				break;
			case SpiderFSM.WANDERING_ON_WEB:
				handleWanderingOnWebState(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
				break;
			case SpiderFSM.WANDERING_TO_BUILD_SITE:
				handleWanderingToBuildSiteState(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
				break;
			case SpiderFSM.BUILDING:
				// Handle building state
				break;
		}
	}

	/// <summary>
	/// SOME CHECKS ARE NOT DONE TO SEE IF THE MOVE IS VALID, please do so before calling this function
	/// </summary>
	public bool specialMove(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int movementX, int movementY)
	{

		if (onWeb != null && (movementX != 0 || movementY != 0)) // Moving off the web
		{
			currentElementArray[x, y] = onWeb; // Leave a web behind
		}
		else
		{
			currentElementArray[x, y] = null; // Leave nothing behind
		}
		if (oldElementArray[x + movementX, y + movementY] is Web)
		{
			onWeb = (Web)oldElementArray[x + movementX, y + movementY];
		}

		currentElementArray[movementX + x, movementY + y] = this; // Move to new position
		lastMoveDirection = (movementX, movementY);
		return true;
	}

	public string inspectInfo()
	{
		return $"State: {currentState}\n" +
			$"Last Move Direction: ({lastMoveDirection.Item1}, {lastMoveDirection.Item2})\n" +
			$"Wandering Direction: ({wanderingDirection.Item1}, {wanderingDirection.Item2})\n" +
			$"Ticks since last state change: {lastMeaningfulStateChangeTick}\n" +
			$"On Web: {(onWeb != null ? "Yes" : "No")}\n";
	}
	override public string getState()
	{
		return base.getState()
		+ currentState + ";"
		+ lastMeaningfulStateChangeTick + ";"
		+ lastMoveDirection.Item1 + ";"
		+ lastMoveDirection.Item2 + ";"
		+ wanderingDirection.Item1 + ";"
		+ wanderingDirection.Item2;
	}
}
