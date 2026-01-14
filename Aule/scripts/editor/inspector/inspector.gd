class_name NodeInspector extends Panel

func _ready():
	pass

func connect_signals():
	for child in get_children():
		if child is InspectorInput:
			child.connect("updated", field_updated)

func field_updated():
	pass
