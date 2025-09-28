using Godot;
public class Wood : Life
{
	public Wood()
	{
		density = 1500;
		color = Colors.Brown;
		flammability = 10;
		ashCreationPercentage = 0.8f;
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
		// Wood just peacefully exists
	}
}