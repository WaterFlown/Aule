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

	
func save_project():
	var save_file = FileAccess.open("user://auleSave.save", FileAccess.WRITE)
	for cur_node in Globals.editor.nodes:
		var snode: SerializedNode = SerializedNode.new()
		var cnode: EditorNode = Globals.editor.get_node_from_id(cur_node)
		snode.filepath = cnode.filepath
		snode.node_position = cnode.position
		snode.properties = cnode.get_properties()
		var node_data = snode.call("save")
		var json_string = JSON.stringify(node_data)
		save_file.store_line(json_string)
	for connection: Connection in Globals.editor.connections:
		var connection_data = {"type": "connection", "from_node": connection.from_node, "to_node": connection.to_node, "from_connector": connection.from_connector, "to_connector": connection.to_connector}
		save_file.store_line(JSON.stringify(connection_data))
	
	save_file.store_line(JSON.stringify({"type": "project_settings", "value": {"terrain_height": Globals.terrain_height, "terrain_size": Globals.terrain_size.x}}))
	
	save_file.close()
