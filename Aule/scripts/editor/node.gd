class_name EditorNode extends Control

signal move_node_to_top
# connect all children signals on parent node. Then move_child(passed node, get_child_count()-1)

var drag_position = null

func _on_gui_input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			drag_position = get_global_mouse_position() - global_position
			emit_signal("move_node_to_top", self)
		else:
			drag_position = null
	if event is InputEventMouseMotion and drag_position:
		global_position = get_global_mouse_position() - drag_position
