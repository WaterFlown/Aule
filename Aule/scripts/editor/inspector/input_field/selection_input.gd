extends InspectorInput


@export var values: Dictionary[int, Variant] ##values associated with selection IDs.

func get_value():
	return values[self.selected]

func set_value(val):
	if val != null:
		var vali: int = val
		if values.find_key(vali) != null:
			self.selected = values.find_key(vali)

func _on_item_selected(index):
	updated.emit(self)
