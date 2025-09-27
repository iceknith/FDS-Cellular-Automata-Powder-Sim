extends Node

func _process(delta: float) -> void:
	$Control/FPS.text = str(Engine.get_frames_per_second())

func load_file():
	$FileDialog.file_mode = FileDialog.FILE_MODE_OPEN_FILE
	$FileDialog.show()
	# Disconnecting everything
	for connection in $FileDialog.file_selected.get_connections(): 
		$FileDialog.file_selected.disconnect(connection.callable)
	# Connect new signal
	$FileDialog.file_selected.connect($CellularAutomataEngine.LoadGridFromFile)


func save_file():
	$FileDialog.file_mode = FileDialog.FILE_MODE_SAVE_FILE
	$FileDialog.show()
	# Disconnecting everything
	for connection in $FileDialog.file_selected.get_connections(): 
		$FileDialog.file_selected.disconnect(connection.callable)
	# Connect new signal
	$FileDialog.file_selected.connect($CellularAutomataEngine.SaveGridToFile)
