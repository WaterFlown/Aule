extends InspectorInput

@export var numeric_input: SpinBox

func _ready():
	if numeric_input:
		numeric_input.editable = self.editable
		numeric_input.step = self.step
		numeric_input.min_value = self.min_value
		numeric_input.max_value = self.max_value
		
		numeric_input.get_line_edit().add_theme_constant_override("minimum_character_width", 3)
		numeric_input.size = Vector2.ZERO

func get_value():
	return self.value
func set_value(val):
	self.value = val
	numeric_input.value = val

func _on_drag_ended(value_changed):
	updated.emit(self)

func _on_value_changed(value):
	numeric_input.value = value
	updated.emit(self)

func _on_numeric_input_value_changed(value):
	self.value = value
	updated.emit(self)
