using Godot;

public class Powder : Element
{

    public bool wet { get; set; }

    override public void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY)
    {
        if (y + 1 >= maxY) return;

        // Down movement
        if (oldElementArray[x, y + 1] == null)
        {
            currentElementArray[x, y] = null;
            currentElementArray[x, y + 1] = this;
            return;
        }

        // Left movement
        if (0 <= x - 1 && oldElementArray[x - 1, y + 1] == null)
        {
            currentElementArray[x, y] = null;
            currentElementArray[x - 1, y + 1] = this;
            return;
        }

        // Right movement
        if (x + 1 < maxX && oldElementArray[x + 1, y + 1] == null)
        {
            currentElementArray[x, y] = null;
            currentElementArray[x + 1, y + 1] = this;
            return;
        }
    }
}