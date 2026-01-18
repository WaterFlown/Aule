class_name NodeInspector extends Panel

@export var inputs: Array[InspectorInput]

func _ready():
	initialize_fields()

func initialize_fields():
	for child in inputs:
		if child is InspectorInput:
			child.connect("updated", field_updated)
			child.set_value(Globals.editor.selected_node.get_property(child.property))

func field_updated(field: InspectorInput):
	Globals.editor.selected_node.set_property(field.property, field.get_value())
