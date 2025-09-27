using Godot;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using Godot.Collections;
using static Godot.GD;
public partial class CellularAutomataEngine : Node2D
{

	// --- Private element instantiation --- //
	private Element[,] elementArray;
	private int cellWidth;
	private int cellHeight;
	private int gridWidth;
	private int gridHeight;

	private enum DrawinState
	{
		None,
		Drawing,
		Erasing
	}

	private DrawinState drawinState = DrawinState.None;
    
	private ButtonGroup buttonGroup;
	public string selectedElement; // TODO idk how to do differently

	private Slider gameSpeedSlider;
	public int gameSpeed = 1;

	private Slider brushSizeSlider;
	public int brushSize = 1;


	// --- Public (exported) element instantiation --- //
	[ExportCategory("Simulation Size")]
	[Export]
	public Vector2 cellSize { get; set; } = new Vector2(8, 8);
	[Export]
	public Vector2 gridSize { get; set; } = new Vector2(144, 81);

	// --- Methods --- //
	public override void _Ready()
	{
		base._EnterTree();
		cellWidth = (int)cellSize.X;
		cellHeight = (int)cellSize.Y;
		gridWidth = (int)gridSize.X;
		gridHeight = (int)gridSize.Y;

		Button firstButton = GetNode<Button>("%Sand");
		buttonGroup = firstButton.ButtonGroup;

		brushSizeSlider = GetNode<Slider>("%BrushSize");
		gameSpeedSlider = GetNode<Slider>("%GameSpeed");

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
		UiHandler();
		PlacementHandler();
		for (int _ = 0; _ < gameSpeed; _++) // game speed just skips steps
		{
			CellUpdateHandler();
		}
		
		QueueRedraw();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: true } eventMouseButton)
		{
			switch (eventMouseButton.ButtonIndex)
			{
				case MouseButton.Left:
					drawinState = DrawinState.Drawing;
					break;
				case MouseButton.Right:
					drawinState = DrawinState.Erasing;
					break;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: false } eventMouseButton)
		{
			switch (eventMouseButton.ButtonIndex)
			{
				case MouseButton.Left:
					if (drawinState == DrawinState.Drawing)
					{
						drawinState = DrawinState.None;
					}
					break;
				case MouseButton.Right:
					if (drawinState == DrawinState.Erasing)
					{
						drawinState = DrawinState.None;
					}
					break;
			}
		}
	}

	private void UiHandler() {
		foreach (BaseButton button in buttonGroup.GetButtons())
		{
			if (button.ButtonPressed)
			{
				selectedElement = (string)button.GetMeta("element");
				break;
			}
		}

		gameSpeed = (int)gameSpeedSlider.Value;
		brushSize = (int)brushSizeSlider.Value;
	}

	private void PlacementHandler()
	{

		if (drawinState != DrawinState.None)
		{
			Vector2 pos = GetViewport().GetMousePosition() / cellSize;
			int xStart = Math.Clamp((int)pos.X - brushSize/2, 0, gridWidth);
			int xStop = Math.Clamp((int)pos.X + brushSize/2 + brushSize%2 + 1, 0, gridWidth);
			int yStart = Math.Clamp((int)pos.Y - brushSize/2, 0, gridWidth);
			int yStop = Math.Clamp((int)pos.Y + brushSize/2 + brushSize%2 +1, 0, gridHeight);

			for (int x = xStart; x < xStop; x++)
			{
				for (int y = yStart; y < yStop; y++)
				{
					if (drawinState == DrawinState.Erasing)
					{
						elementArray[x, y] = null;
						continue;
					}
					switch (selectedElement) // ugly but was the only thing on my mind
					{

						case "Nutrient":
							if (elementArray[x,y] is Soil soil)
							{
								soil.nutrient += 1.0F;
							}
							break;

						default:
							createElement(x, y, selectedElement);
							break;
					}
				}
			}
		}
	}

	private void createElement(int x, int y, string elementType)
	{
		elementArray[x, y] = (Element)Activator.CreateInstance(Type.GetType(elementType));
	}

	private void CellUpdateHandler()
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



	public void SaveGridToFile(string fileName)
	{
		using (StreamWriter writer = new StreamWriter(fileName))
		{
			// Write header line
			writer.WriteLine(gridWidth + " " + gridHeight + " " + cellWidth + " " + cellHeight);

			// Write rest of file
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					if (elementArray[x, y] != null) writer.Write(elementArray[x, y].GetType() + " ");
					else writer.Write("- ");
				}
				writer.Write("\n");
			}
		}
	}

	public void LoadGridFromFile(string fileName)
	{
		if (File.Exists(fileName)) {
			// Store each line in array of strings
			string[] lines = File.ReadAllLines(fileName);

			string[] header = lines[0].Split(" ", false);
			if (header.Length < 4) throw new DataException("The file header isn't formatted correctly");

			// Initializing every variable according to the headers
			gridWidth = header[0].ToInt();
			gridHeight = header[1].ToInt();
			gridSize = new Vector2(gridWidth, gridHeight);
			cellWidth = header[2].ToInt();
			cellHeight = header[3].ToInt();
			cellSize = new Vector2(cellWidth, cellHeight);

			elementArray = new Element[gridWidth, gridHeight];

			// Read rest of file
			if (lines.Length < gridWidth) throw new DataException("The file doesn't have the correct amount of rows");

			for (int x = 0; x < gridWidth; x++)
			{
				string[] line = lines[x +1].Split(" ", false);
				if (line.Length < gridHeight)
					throw new DataException("The file doesn't have the correct amount of lines on row " + x + " : " + line.Length + " instead of " + gridHeight);
				for (int y = 0; y < gridHeight; y++)
				{
					// If element non null
					if (line[y] != "-") createElement(x, y, line[y]);
				}
			}
		}
	}

}