using Godot;
using System;
using System.Threading;

public partial class CellularAutomataEngine : Node2D
{

	// --- Private element instantiation --- //
	private Element[,] elementArray;
	private int cellWidth;
	private int cellHeight;
	private int gridWidth;
	private int gridHeight; 


	// --- Public (exported) element instantiation --- //
	[ExportCategory("Simulation Size")]
	[Export]
	public Vector2 cellSize { get; set; } = new Vector2(8, 8);
	[Export]
	public Vector2 gridSize { get; set; } = new Vector2(144, 81);

	// --- Mathods --- //
	public override void _Ready()
	{
		base._EnterTree();
		cellWidth = (int)cellSize.X;
		cellHeight = (int)cellSize.Y;
		gridWidth = (int)gridSize.X;
		gridHeight = (int)gridSize.Y;

		elementArray = new Element[gridWidth, gridHeight];
	}


	public override void _Draw()
	{
		base._Draw();

		// Draw outline
		DrawRect(new Rect2(Vector2.Zero, cellSize * gridSize), Colors.SlateGray, false);

		// Draw every cell
		Rect2 cellRect = new Rect2(Vector2.Zero, cellSize);
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				if (elementArray[x, y] != null)
				{
					// If you want to override to have a draw method inside the Element Class, you can,
					// But I am concerned with slight optimisation issues tho
					cellRect.Position = cellSize * new Vector2(x, y);
					DrawRect(cellRect, elementArray[x, y].color);
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		PlacementHandler();
		CellMovementHandler();

		QueueRedraw();
	}

	private void PlacementHandler()
	{
		if (Input.IsActionPressed("LeftClick"))
		{
			Vector2 pos = GetViewport().GetMousePosition() / cellSize;
			if (0 <= pos.X && pos.X < gridWidth && 0 <= pos.Y && pos.Y < gridHeight)
			{
				elementArray[(int)pos.X, (int)pos.Y] = new Sand();
			}
		}
		if (Input.IsActionPressed("RightClick"))
		{
			Vector2 pos = GetViewport().GetMousePosition() / cellSize;
			if (0 <= pos.X && pos.X < gridWidth && 0 <= pos.Y && pos.Y < gridHeight)
			{
				Water cell = new Water();
				cell.init(elementArray, (int)pos.X, (int)pos.Y, gridWidth, gridHeight);
				elementArray[(int)pos.X, (int)pos.Y] = cell;
			}
		}
	}

	private void CellMovementHandler()
	{
		Element[,] oldElementArray = (Element[,]) elementArray.Clone();

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				if (oldElementArray[x, y] != null)
				{
					oldElementArray[x, y].update(oldElementArray, elementArray, x, y, gridWidth, gridHeight);
				}
			}
		}
	}

}
