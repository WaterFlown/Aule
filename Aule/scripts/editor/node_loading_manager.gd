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
		snode.id = cur_node
		var node_data = snode.call("save")
		var json_string = JSON.stringify(node_data)
		save_file.store_line(json_string)
	for connection: Connection in Globals.editor.connections:
		var connection_data = {"type": "connection", "from_node": connection.from_node, "to_node": connection.to_node, "from_connector": connection.from_connector, "to_connector": connection.to_connector}
		save_file.store_line(JSON.stringify(connection_data))
	
	save_file.store_line(JSON.stringify({"type": "project_settings", "value": {"terrain_height": Globals.terrain_height, "terrain_size": Globals.terrain_size.x}}))
	
	save_file.close()

func load_project():
	if FileAccess.file_exists("user://auleSave.save"):
		var save_file = FileAccess.open("user://auleSave.save", FileAccess.READ)
		Globals.editor.clear_project()
		while save_file.get_position() < save_file.get_length():
			var json_string = save_file.get_line()
			var json = JSON.new()
			var parse_result = json.parse(json_string)
			if not parse_result == OK:
				print("JSON Parse Error: ", json.get_error_message(), " in ", json_string, " at line ", json.get_error_line())
				continue
			match (json.data["type"]):
				"node":
					var cnode: EditorNode = Globals.editor.add_node(json.data["filepath"], json.data["id"])
					var position:Vector2 = Vector2(json.data["node_position_x"], json.data["node_position_y"])
					cnode.initing()
					cnode.position = position
					for property in json.data["properties"]:
						cnode.set_property(property, json.data["properties"][property])
				"connection":
					var from_connector: NodeConnector = Globals.editor.get_node_from_id(json.data["from_node"]).get_connector_by_id(json.data["from_connector"])
					var to_connector: NodeConnector = Globals.editor.get_node_from_id(json.data["to_node"]).get_connector_by_id(json.data["to_connector"])
					Globals.editor.create_connection(from_connector, to_connector)
					
				"project_settings":
					Globals.terrain_size = Vector2i(json.data["value"]["terrain_size"], json.data["value"]["terrain_size"])
					Globals.terrain_height = json.data["value"]["terrain_height"]
		
