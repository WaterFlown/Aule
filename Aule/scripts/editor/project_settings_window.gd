class_name ProjectSettingsWindow extends Window

@export var TerrainSizeInput: SpinBox
@export var TerrainHeightInput: SpinBox

func _ready():
	if(Globals):
		TerrainHeightInput.value = Globals.terrain_height
		TerrainSizeInput.value = Globals.terrain_size.x
	else:
		queue_free()

func _on_save_pressed():
	Globals.terrain_height = TerrainHeightInput.value
	Globals.terrain_size = Vector2i(TerrainSizeInput.value, TerrainSizeInput.value)
	queue_free()


func _on_close_requested():
	queue_free()
