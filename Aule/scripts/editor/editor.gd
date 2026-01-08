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

func _ready():
	Globals.editor = self

func select_node(node: EditorNode):
	if selected_node:
		selected_node.unselect()
	selected_node = node
	node.select()


func run():
	if selected_node:
		print("run")
		Globals.terrain.generate_terrain(selected_node.functionality.output())
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
	elif is_cycle(to.parent.get_id(), from.parent.get_id()):
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

##Returns if a path already exists between two nodes.
func is_cycle(from_node: int, to_node: int):
	return path_exists(from_node, to_node, {})
## Recursive function that checks path existance. CALL is_cycle INSTEAD.
func path_exists(from_node: int, to_node: int, visited: Dictionary) -> bool:
	if from_node == to_node:
		return true
	visited[from_node] = true
	for c in connections:
		if c.to_node == from_node:
			var next_id = c.from_node
			if not visited.has(next_id):
				if path_exists(next_id, to_node, visited):
					return true
	return false

func connector_hovering(connector:NodeConnector, enter:bool):
	if enter and hovering_connector != connector:
		hovering_connector = connector
	else:
		hovering_connector = null

func handle_connecting():
	if connecting and connecting_from:
		if not connecting_to and hovering_connector:
			if connecting_from.IOtype != hovering_connector.IOtype and (connecting_from.parent != hovering_connector.parent): #check compatibility and if nodes are already connected too later
				connecting_to = hovering_connector

func move_node_to_top(node: EditorNode):
	move_child(node, get_child_count()-1)

func _process(delta):
	handle_connecting()

func _on_run_button_pressed():
	run()
	
