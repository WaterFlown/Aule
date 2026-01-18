extends InspectorInput


@export var values: Dictionary[int, Variant] ##values associated with selection IDs.

func get_value():
	return values[self.selected]

func set_value(val):
	if val != null:
		if self.selected == values.find_key(val) != null:
			self.selected = values.find_key(val)

func _on_item_selected(index):
	updated.emit(self)
