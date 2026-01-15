extends InspectorInput

func get_value():
	return self.value

func _on_value_changed(value):
	updated.emit(self)
