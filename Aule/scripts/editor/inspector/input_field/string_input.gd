extends InspectorInput

func get_value():
	return self.text
func set_value(val):
	self.text = val;

func _on_text_changed(new_text):
	updated.emit(self)


func _on_text_submitted(new_text):
	updated.emit(self)
