class_name Editor extends Node

@export var connection_line_manager: ConnectionLineManager
@onready var global_node_id = 0

var connecting: bool = false
var connecting_from: NodeConnector = null
var hovering_connector: NodeConnector = null
var connecting_to: NodeConnector = null


var nodes := {}
var connections := []

var selected_node: EditorNode = null

@export var camera: Camera2D
@export_group("Inspector")
@export var inspector_container: Control
@export var close_button: Button
var node_inspector: NodeInspector = null
var node_inspector_enabled = true
var node_adder_window: NodeAdderWindow = null
@export_group("Misc")
var generation_popup: GenerationPopup = null

#Setting windows
var project_settings: ProjectSettingsWindow = null

#@export_subgroup("Toolbar")
#@export var close_button: Button
#@export var add_button: Button

func _ready():
	Globals.editor = self

func select_node(node: EditorNode):
	if selected_node:
		selected_node.unselect()
	selected_node = node
	node.select()
	set_inspector()

func set_inspector():
	if (node_inspector):
		node_inspector.queue_free()
	var inspector_scene = load(selected_node.inspector)
	node_inspector = inspector_scene.instantiate()
	inspector_container.add_child(node_inspector)

func run():
	if selected_node:
		open_generation_popup()
		generation_popup.set_text("Generating heightmap...")
		await get_tree().process_frame
		await get_tree().process_frame
		Globals.terrain.call_deferred("generate_terrain", selected_node.functionality.Output()) #Globals.terrain.generate_terrain(selected_node.functionality.Output())
	else:
		pass

#func _unhandled_input(event):
	#if event is InputEventMouseButton:
		#if event.button_mask == MOUSE_BUTTON_LEFT:
			#if selected_node:
				#selected_node.unselect()
				#selected_node = null

func register_node(node: EditorNode):
	node.id = global_node_id
	nodes[node.id] = node
	global_node_id += 1

func start_connecting(from: NodeConnector):
	connecting = true
	connecting_from = from
	connecting_to = null
	
	for c_connection in connections:
		if (c_connection.from_connector == from.get_id() and c_connection.from_node == from.get_node_id()) or (c_connection.to_connector == from.get_id() and c_connection.to_node == from.get_node_id()):
			remove_connection(connections.find(c_connection))

func stop_connecting():
	connecting = false
	if connecting_from and connecting_to:
		create_connection(connecting_from, connecting_to)


func remove_connection(index: int):
	var connection: Connection = connections.pop_at(index)
	var from_node: EditorNode = nodes.get(connection.from_node)
	var to_node: EditorNode = nodes.get(connection.to_node)
	from_node.get_connector_by_id(connection.from_connector).disconnect_connectors()
	to_node.get_connector_by_id(connection.to_connector).disconnect_connectors()
	
	connection_line_manager.remove_line(connection.from_node, connection.to_node, connection.from_connector, connection.to_connector)
	

func create_connection(from: NodeConnector, to: NodeConnector):
	if is_cycle(from.parent.get_id(), to.parent.get_id()):
		return
	#elif is_cycle(to.parent.get_id(), from.parent.get_id()):
	#	return
	
	if from.IOCategory != to.IOCategory:
		return
	
	for c_connection in connections:
		if (c_connection.to_connector == to.get_id() and c_connection.to_node == to.get_node_id()) or (c_connection.from_connector == to.get_id() and c_connection.from_node == to.get_node_id()):
			remove_connection(connections.find(c_connection))
	
	
	var connection: Connection = Connection.new()
	if from.IOtype == Globals.NodeConnectorIOType.OUTPUT and to.IOtype == Globals.NodeConnectorIOType.INPUT: #FROM is not output, TO is not input
		connection.from_node = from.get_node_id()
		connection.to_node = to.get_node_id()
		connection.from_connector = from.get_id()
		connection.to_connector = to.get_id()
	else:
		connection.from_node = to.get_node_id()
		connection.to_node = from.get_node_id()
		connection.from_connector = to.get_id()
		connection.to_connector = from.get_id()
	
	
	connections.append(connection)
	from.connect_connectors(to)
	to.connect_connectors(from)
	
	print(connections)

##Gets connection to node on a specific connector
func get_connection_to(node_id: int, connector_id: int) -> Connection:
	for c_connection: Connection in connections:
		if (c_connection.to_node == node_id && c_connection.to_connector == connector_id):
			return c_connection
	return null

func get_node_from_id(id: int) -> EditorNode:
	if nodes[id]:
		return nodes[id]
	return null

## get_connection_to and get_node_from_id call in one function
func get_node_connected_to(node_id: int, connector_id: int) -> EditorNode:
	var connection: Connection = get_connection_to(node_id, connector_id);
	if connection:
		return get_node_from_id(connection.from_node)
	return null

#region Cycle detection
##Returns if a path already exists between two nodes.
func is_cycle(from_node: int, to_node: int):
	return path_exists_forward(from_node, to_node, {}) or path_exists_backwards(from_node, to_node, {})
## Recursive function that checks path existance. CALL is_cycle INSTEAD.
func path_exists_forward(from_node: int, to_node: int, visited: Dictionary) -> bool:
	if from_node == to_node:
		return true
	visited[to_node] = true
	for c in connections:
		if c.from_node == to_node:
			var next_id = c.to_node
			if not visited.has(next_id):
				if path_exists_forward(from_node, next_id, visited):
					return true
	return false
## Recursive function that checks path existance. CALL is_cycle INSTEAD.
func path_exists_backwards(from_node: int, to_node: int, visited: Dictionary) -> bool:
	if from_node == to_node:
			return true
	visited[from_node] = true
	for c in connections:
		if c.to_node == from_node:
			var next_id = c.from_node
			if not visited.has(next_id):
				if path_exists_backwards(next_id, to_node, visited):
					return true
	return false

func connector_hovering(connector:NodeConnector, enter:bool):
	if enter:# and hovering_connector != connector:
		print("add")
		hovering_connector = connector
	else:
		print("remove")
		hovering_connector = null

func handle_connecting():
	if connecting and connecting_from:
		if hovering_connector and connecting_to != hovering_connector:
			if connecting_from.IOtype != hovering_connector.IOtype and (connecting_from.parent != hovering_connector.parent): #check compatibility and if nodes are already connected too later
				connecting_to = hovering_connector

func move_node_to_top(node: EditorNode):
	move_child(node, get_child_count()-1)

func _process(delta):
	handle_connecting()
	
func _unhandled_input(event):
	if event.is_action_pressed("remove"):
		delete_selected_node()
	elif event.is_action_pressed("add_node"):
		open_node_adder()

func add_node(path: String): ##Places an editor node where the camera is
	var node: EditorNode = load(path).instantiate()
	node.position = camera.position - camera.get_viewport_rect().size / 2.0
	add_child(node)

func delete_selected_node():
	if selected_node:
		var connections_to_delete: Array[Connection]
		for connection: Connection in connections:
			if connection.from_node == selected_node.id or connection.to_node == selected_node.id:
				connections_to_delete.append(connection)
		for connection in connections_to_delete:
			remove_connection(connections.find(connection))
		
		selected_node.unselect()
		node_inspector.queue_free()
		selected_node.queue_free()

func open_node_adder():
	if !node_adder_window:
		node_adder_window = load("res://scenes/node_adder/node_adder_window.tscn").instantiate()
		add_child(node_adder_window)
func toggle_inspector():
	if node_inspector_enabled:
		inspector_container.position.x = -1 * inspector_container.size.x
		node_inspector_enabled = false
	else:
		inspector_container.position.x = 0
		node_inspector_enabled = true

func open_generation_popup(): #Await 2 frames after
	if !generation_popup:
		generation_popup = load("res://scenes/generation_popup.tscn").instantiate()
		add_child(generation_popup)
		generation_popup.popup_centered(generation_popup.min_size)

func close_generation_popup():
	if generation_popup:
		generation_popup.hide()
		generation_popup.queue_free()

func open_project_settings():
	if(!project_settings):
		project_settings = load("res://scenes/project_settings_window.tscn").instantiate()
		add_child(project_settings)

func _on_run_button_pressed():
	run()


func _on_add_button_pressed():
	open_node_adder()

func _on_close_button_pressed():
	toggle_inspector()
	var icon: TextureRect = close_button.get_child(0)
	icon.flip_h = !icon.flip_h 
