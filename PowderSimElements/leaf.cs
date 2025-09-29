using Godot;
public class Leaf : Seed
{
	private (int, int) parentSeed;
	public Leaf((int, int) parentSeed)
	{
		this.parentSeed = parentSeed;
		density = 10;
		color = Colors.Green;
		flammability = 10;
		ashCreationPercentage = 0.8f;
	}
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		burn(oldElementArray, currentElementArray, x, y, maxX, maxY, T);
		updateColor(T);
	}
}