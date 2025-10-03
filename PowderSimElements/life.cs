public class Life : Element
{
	public float nutrient { get; set; }
	override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
	{
		// Life just exists, as it was always meant to be
	}
}