extends Control

@export var cell_size:Vector2
var cell_amount:Vector2

var grid:Array[Array]
var auto_process:bool = false
var last_tick = 0;

func _ready() -> void:
	print(size)
	cell_amount = size / cell_size
	cell_amount.x = int(cell_amount.x)
	cell_amount.y = int(cell_amount.y)
	initiate_grid()

func initiate_grid() -> void:
	grid = []
	for x in cell_amount.x:
		grid.append([])
		for y in cell_amount.y:
			grid[x].append(false)

func _draw() -> void:
	for x in cell_amount.x:
		for y in cell_amount.y:
			var cell_rect = Rect2(Vector2(x,y) * cell_size, cell_size)
			draw_rect(cell_rect, Color.GRAY, false)
			if grid[x][y] == true:
				draw_rect(cell_rect, Color.YELLOW, true)

func _process(delta: float) -> void:
	var t = Time.get_ticks_msec()
	if auto_process && t - last_tick >= 30:
		last_tick = t
		update_grid()
	if Input.is_action_pressed("LeftClick"):
		var mouse_pos = get_local_mouse_position() / cell_size
		if mouse_pos.x < cell_amount.x && mouse_pos.y < cell_amount.y:
			grid[int(mouse_pos.x)][int(mouse_pos.y)] = true
			queue_redraw()
	elif Input.is_action_pressed("RightClick"):
		var mouse_pos = get_local_mouse_position() / cell_size
		if mouse_pos.x < cell_amount.x && mouse_pos.y < cell_amount.y:
			grid[int(mouse_pos.x)][int(mouse_pos.y)] = false
			queue_redraw()

func update_grid() -> void:
	var old_grid = grid.duplicate_deep(2)
	
	for x in cell_amount.x:
		for y in cell_amount.y:
			# Rules
			if old_grid[x][y] == false : continue
			if y+1 == cell_amount.y: continue
			
			if old_grid[x][y+1] == false:
				grid[x][y] = false
				grid[x][y+1] = true
				continue
			
			if x+1 < cell_amount.x && x-1 >= 0 && old_grid[x+1][y+1] == false && old_grid[x-1][y+1] == false:
				if randf() > 0.5:
					grid[x][y] = false
					grid[x-1][y+1] = true
					continue
				else:
					grid[x][y] = false
					grid[x+1][y+1] = true
					continue
			elif x+1 < cell_amount.x && old_grid[x+1][y+1] == false:
				grid[x][y] = false
				grid[x+1][y+1] = true
				continue
			elif x-1 >= 0 && old_grid[x-1][y+1] == false:
				grid[x][y] = false
				grid[x-1][y+1] = true
				continue
			
	
	queue_redraw()

func set_auto_process():
	auto_process = true

func set_no_auto_process():
	auto_process = false
