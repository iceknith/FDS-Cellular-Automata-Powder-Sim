extends Node

@onready var scene_container: SceneContainer = $SceneContainer

var switched := false

func _process(delta):
	if Input.is_action_just_pressed("ui_accept") and not switched:
		scene_container.request_scene("res://addons/scene_container/examples/big_scene.tscn")
		
		# Alternatively, you can load using UID:
		#scene_container.request_scene("uid://6saipaosg07p")
		# You can use already-loaded scenes (PackedScene) too:
		#scene_container.request_scene(some_packed_scene)
		
		switched = true
