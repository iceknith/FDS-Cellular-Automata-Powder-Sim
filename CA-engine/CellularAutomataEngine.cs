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

	private DrawingState _drawingState = DrawingState.None;

	private ButtonGroup buttonGroup;
	public string selectedElement; // TODO idk how to do differently

	private Slider gameSpeedSlider;
	public int gameSpeed = 1;

	private Slider brushSizeSlider;
	public int brushSize = 1;

	public int tick = 0;


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

		tick ++;
		base._Process(delta);
		UiHandler();
		PlacementHandler();
		for (int _ = 0; _ < gameSpeed; _++) // game speed just skips steps
		{
			CellUpdateHandler();
		}

		// Commented code to prevent stuff
		//float totalWetness = GetTotalWetness();
		//GD.Print($"Total wetness in simulation: {totalWetness:F3}");

		QueueRedraw();
	}

	//Those inputs may be ignored by filters
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: true } eventMouseButton)
		{
			switch (eventMouseButton.ButtonIndex)
			{
				case MouseButton.Left:
					_drawingState = DrawingState.Drawing;
					break;
				case MouseButton.Right:
					_drawingState = DrawingState.Erasing;
					break;
			}
		}
	}

	//Those inputs are always called
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: false } eventMouseButton)
		{
			switch (eventMouseButton.ButtonIndex)
			{
				case MouseButton.Left:
					if (_drawingState == DrawingState.Drawing)
					{
						_drawingState = DrawingState.None;
					}
					break;
				case MouseButton.Right:
					if (_drawingState == DrawingState.Erasing)
					{
						_drawingState = DrawingState.None;
					}
					break;
				case MouseButton.Middle:
					string cellInfo = GetCellInfoAtCursor();
					GD.Print(cellInfo);
					break;
			}
		}
	}

	private void UiHandler()
	{
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

		((Label)GetNode("%InspectLabel")).Text = GetCellInfoAtCursor();
	}

	private void PlacementHandler()
	{

		if (_drawingState != DrawingState.None)
		{
			Vector2 pos = GetViewport().GetMousePosition() / cellSize;
			int xStart = Math.Clamp((int)pos.X - brushSize / 2, 0, gridWidth);
			int xStop = Math.Clamp((int)pos.X + brushSize / 2 + brushSize % 2 + 1, 0, gridWidth);
			int yStart = Math.Clamp((int)pos.Y - brushSize / 2, 0, gridWidth);
			int yStop = Math.Clamp((int)pos.Y + brushSize / 2 + brushSize % 2 + 1, 0, gridHeight);

			for (int x = xStart; x < xStop; x++)
			{
				for (int y = yStart; y < yStop; y++)
				{
					if (_drawingState == DrawingState.Erasing)
					{
						elementArray[x, y] = null;
						continue;
					}
					switch (selectedElement) // ugly but was the only thing on my mind
					{

						case "Nutrient":
							if (elementArray[x, y] is Soil soil)
							{
								soil.nutrient += 1.0F;
							}
							break;
						
						case "Fire":
							elementArray[x, y]?.ignite(elementArray, x, y);
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
	
	private void createElement(int x, int y, string elementType, string state)
	{
		elementArray[x, y] = (Element)Activator.CreateInstance(Type.GetType(elementType), state);
	}

	private void CellUpdateHandler()
	{
		Element[,] oldElementArray = (Element[,])elementArray.Clone();

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				if (oldElementArray[x, y] != null)
				{
					oldElementArray[x, y].update(oldElementArray, elementArray, x, y, gridWidth, gridHeight, tick);
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
					if (elementArray[x, y] != null)
					{
						// Write element and state (if exists)
						string elementStoredText = elementArray[x, y].GetType().ToString();
						string elementState = elementArray[x, y].getState();
						if (elementState != null) elementStoredText += "|" + elementState;
						elementStoredText += " ";
						writer.Write(elementStoredText);
					}
					else writer.Write("- ");
				}
				writer.Write("\n");
			}
		}
	}

	public void LoadGridFromFile(string fileName)
	{
		if (File.Exists(fileName))
		{
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
				string[] line = lines[x + 1].Split(" ", false);
				if (line.Length < gridHeight)
					throw new DataException("The file doesn't have the correct amount of lines on row " + x + " : " + line.Length + " instead of " + gridHeight);
				for (int y = 0; y < gridHeight; y++)
				{
					// If element non null
					if (line[y] != "-")
					{
						// If has state
						if (line[y].Contains("|"))
						{
							string[] storedElement = line[y].Split("|");
							createElement(x, y, storedElement[0], storedElement[1]);
						}
						else createElement(x, y, line[y]);
					}
				}
			}
		}
	}

	public string GetCellInfoAtCursor()
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 gridPos = mousePos / cellSize;
		
		int x = (int)gridPos.X;
		int y = (int)gridPos.Y;
		
		// Check if cursor is within grid bounds
		if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
		{
			return "Cursor outside grid bounds";
		}
		
		Element cell = elementArray[x, y];
		
		if (cell == null)
		{
			return $"Position ({x}, {y}): Empty cell";
		}
		
		// Get the class name
		string className = cell.GetType().Name;
		
		// Build attribute string
		string attributes = $"Position ({x}, {y}): {className}\n";
		attributes += $"  Flammability: {cell.flammability}\n";
		attributes += $"  Wetness: {cell.wetness:F3}\n";
		attributes += $"  Burning: {cell.burning}\n";
		
		if (cell.burning)
		{
			attributes += $"  Burning Lifetime: {cell.burningLifetime}\n";
		}
		
		// Add specific attributes for different element types
		if (cell is Soil soil)
		{
			attributes += $"  Nutrient: {soil.nutrient:F3}\n";
		}
		
		return attributes.StripEdges();
	}

	public float GetTotalWetness()
	{
		float totalWetness = 0f;

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				if (elementArray[x, y] != null)
				{
					totalWetness += elementArray[x, y].wetness;
				}
			}
		}

		return totalWetness;
	}
	
	private enum DrawingState
	{
		None,
		Drawing,
		Erasing
	}

}
