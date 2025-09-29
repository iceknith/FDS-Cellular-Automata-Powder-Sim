using Godot;

public class Water : Liquid
{
    private float evaporationChance = 0.0004f; // chance of evaporating each tick if the conditions are right

    public Water() : base()
    {
        density = 5;
        color = Colors.Blue;
        flammability = 0;
        wetness = 1.0f;
    }

    public override void onEvaporate(Element[,] currentElementArray, int x, int y)
    {
        currentElementArray[x, y] = new Steam(); // evaporate into steam
    }

    public override void ignite(Element[,] currentElementArray, int x, int y)
    {
        if (currentElementArray[x, y] == this)
        {
            currentElementArray[x, y] = new Steam(); // water turns to steam when ignited
        }
        base.ignite(currentElementArray, x, y);
    }


    public override void update(Element[,] oldElementArray, Element[,] currentElementArray, int x, int y, int maxX, int maxY, int T)
    {
        if (wetness <= 0 && currentElementArray[x, y] == this) // disappear if completely dry (dry water is not water) 
        {
            currentElementArray[x, y] = null;
            return;
        }

        if (rng.Randf() < evaporationChance && y - 1 > 0 && oldElementArray[x, y - 1] == null) // can add water to the system if wetness < 1
        {
            onEvaporate(currentElementArray, x, y);
            return;
        }

        base.update(oldElementArray, currentElementArray, x, y, maxX, maxY, T); // keep at the end because of returns contained in base method
    }

}
