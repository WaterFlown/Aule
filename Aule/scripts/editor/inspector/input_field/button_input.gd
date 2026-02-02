extends InspectorInput

func get_value():
	return true

func set_value(val):
	return

func _on_pressed():
	updated.emit(self)
