extends Node

func _process(_delta: float) -> void:
	$Control/Label.text = str(Engine.get_frames_per_second())
