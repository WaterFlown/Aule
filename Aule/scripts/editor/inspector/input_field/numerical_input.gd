extends InspectorInput

@export var scaling: float = 1

func get_value():
	return self.value / scaling

func _on_value_changed(value):
	updated.emit(self)

func set_value(val):
	if val != null:
		self.value = val * scaling
