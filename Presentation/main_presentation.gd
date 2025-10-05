extends Node

@export var slides:Array[PackedScene]
var current_slide:int = 0

@onready var scene_container: SceneContainer = $SceneContainer

func _ready() -> void:
	# Initiate the first slide
	scene_container.main_node  = slides[0].instantiate()
	$SceneContainer/SubViewport.add_child(scene_container.main_node)

func _process(delta):
	if Input.is_action_just_pressed("ui_left") && current_slide > 0:
		current_slide -= 1
		scene_container.request_scene(slides[current_slide])
	
	if Input.is_action_just_pressed("ui_right") && current_slide + 1 < slides.size():
		current_slide += 1
		scene_container.request_scene(slides[current_slide])
