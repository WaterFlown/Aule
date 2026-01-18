extends InspectorInput

func get_value():
	return self.button_pressed

func set_value(val):
	if val != null:
		self.button_pressed = val

func _on_toggled(toggled_on):
	updated.emit(self)
