using Godot;
using System.Collections.Generic;
public class Worm : Life
{
	int lastActivity = 0;
	int activityInterval = 10;

	private Queue<(int, int)> recentPositions = new();

	Soil inSoil = null;
	public Worm()
	{
		ashCreationPercentage = 0.0f;
		density = 30;
		color = Colors.Pink;
		flammability = 3;
	}

	enum WormState
	{
		Moving,
		Falling,
	}
	private WormState wormState = WormState.Falling;
	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		if (T - lastActivity < activityInterval)
		{
			// Not time to act yet
			burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
			updateColor(T);
			return;
		}
		else
		{
			lastActivity = T;
		}

		switch (wormState)
		{
			case WormState.Falling:
				// check if can fall down
				if (y + 1 < maxY && (currentElementArray[x, y + 1] == null || currentElementArray[x, y + 1] is Water || currentElementArray[x, y + 1] is Soil))
				{
					// fall down
					specialMove(currentElementArray, x, y, maxX, maxY, 0, 1);
				}

				if (inSoil != null)
				{
					wormState = WormState.Moving; // landed
				}

				break;

			case WormState.Moving:
				// try to move in a random direction on the surface
				List<(int, int)> possibleDirs = [(-1, 0), (1, 0), (0, -1), (0, 1)];
				// shuffle directions
				for (int i = 0; i < possibleDirs.Count; i++)
				{
					int j = rng.RandiRange(0, possibleDirs.	Count - 1);
					(possibleDirs[i], possibleDirs[j]) = (possibleDirs[j], possibleDirs[i]);
				}

				foreach ((int, int) dir in possibleDirs)
				{
					if (x + dir.Item1 < 0 || x + dir.Item1 >= maxX || y + dir.Item2 < 0 || y + dir.Item2 >= maxY)
						continue; // out of bounds

					if (currentElementArray[x + dir.Item1, y + dir.Item2] is Biomass biomass) // try to eat biomass
					{
						consumeBiomass(currentElementArray, x + dir.Item1, y + dir.Item2);
						specialMove(currentElementArray, x, y, maxX, maxY, dir.Item1, dir.Item2);
						break;
					}

					if (currentElementArray[x + dir.Item1, y + dir.Item2] is Soil
					&& specialMove(currentElementArray, x, y, maxX, maxY, dir.Item1, dir.Item2)
					&& !recentPositions.Contains((x + dir.Item1, y + dir.Item2))) // try to move onto soil
					{
						break;
					}
				}
				
				break;
		}

		// just move around in the dirt
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}

	private void addToPositionHistory(int x, int y)
	{
		recentPositions.Enqueue((x, y));
		if (recentPositions.Count > 6) // keep only last 6 positions
		{
			recentPositions.Dequeue();
		}
	}

	public void consumeBiomass(Element[,] currentElementArray, int x, int y)
	{
		Biomass biomass = currentElementArray[x, y] as Biomass; // checks are done before calling this function
		float biomassNutrient = biomass.nutrient;
		float biomassWetness = biomass.wetness;

		// Create soil with the nutrient and wetness of the biomass
		Soil newSoil = new Soil();
		newSoil.nutrient = biomassNutrient;
		newSoil.wetness = biomassWetness;    // max wetness is 1, there may be a problem here if biomass wetness > 1

		currentElementArray[x, y] = newSoil;
	}

	public bool specialMove(Element[,] currentElementArray, int x, int y, int maxX, int maxY, int movementX, int movementY)
	{
		// Safety: Only move if this worm is still at (x, y)
		if (currentElementArray[x, y] != this)
			return false;

		int targetX = x + movementX;
		int targetY = y + movementY;

		// Bounds check
		if (targetX < 0 || targetX >= maxX || targetY < 0 || targetY >= maxY)
			return false;

		// Check if target cell is occupied by something other than a soil
		Element targetElem = currentElementArray[targetX, targetY];
		if (targetElem != null && targetElem is not Soil)
			return false;

		// Handle leaving a soil behind if moving off a soil
		if (inSoil != null && (movementX != 0 || movementY != 0))
		{
			currentElementArray[x, y] = inSoil;
			inSoil = null;
		}
		else
		{
			currentElementArray[x, y] = null;
		}

		// If moving onto a soil, "pick it up"
		if (currentElementArray[targetX, targetY] is Soil soil)
		{
			inSoil = soil;
		}
		else
		{
			inSoil = null;
		}

		// Move worm to new position
		currentElementArray[targetX, targetY] = this;
		addToPositionHistory(x, y); // Add the old position to history

		return true;
	}

	public override string inspectInfo()
	{
		return base.inspectInfo() + $"\nState: {wormState} \nRecent Positions: {string.Join("-", recentPositions)}";
	}

}
