class_name NodeLoadingManager extends Node

var node_clipboard: Array[SerializedNode] = []
var paste_offset = 8

func _input(event):
	if event.is_action_pressed("ui_copy"):
		paste_offset = 8
		copy()
	elif event.is_action_pressed("ui_paste"):
		paste()
		paste_offset += 8

func copy():
	var snode: SerializedNode = SerializedNode.new()
	snode.filepath = Globals.editor.selected_node.filepath
	snode.node_position = Globals.editor.selected_node.position
	snode.properties = Globals.editor.selected_node.get_properties()
	node_clipboard = [snode]
	
func paste():
	for csnode in node_clipboard:
		var cnode = Globals.editor.add_node(csnode.filepath)
		cnode.position = csnode.node_position + Vector2(paste_offset, paste_offset)
		for property in csnode.properties:
			cnode.set_property(property, csnode.properties[property])
