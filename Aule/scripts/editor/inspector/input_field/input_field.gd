class_name InspectorInput extends Control
@export var property: String = ""

signal updated(field: InspectorInput)

func get_value():
	pass

func set_value(val):
	if val != null:
		self.value = val
