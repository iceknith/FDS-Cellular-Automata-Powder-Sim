using Godot;

public class Web : Life
{
	public (int, int) successor;
	public (int, int) predecessor;
	public Web()
	{
		density = 3;
		color = Colors.White;
		flammability = 3;
	}

	public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{

		// if (!(oldElementArray[successor.Item1, successor.Item2] == null) || !(oldElementArray[predecessor.Item1, predecessor.Item2] == null))
		// {
		// 	if (rng.Randf() < 0.02f && currentElementArray[x, y] == this) // small chance to dissipate if its successor or predecessor is gone
		// 	{
		// 		currentElementArray[x, y] = null;
		// 		return;
		// 	}
		// }
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}


}
