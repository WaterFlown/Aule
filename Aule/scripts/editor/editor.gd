class_name Editor extends Node

@onready var global_node_id = 0

var connecting: bool = false
var connecting_from: NodeConnector = null
var hovering_connector: NodeConnector = null
var connecting_to: NodeConnector = null


var nodes := {}
var connections := []

func _ready():
	Globals.editor = self

func register_node(node: EditorNode):
	node.id = global_node_id
	nodes[node.id] = node
	global_node_id += 1
	print(nodes)

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
	from_node.get_connector_by_id(connection.from_connector).connected = false
	to_node.get_connector_by_id(connection.to_connector).connected = false
	
	print("removed conn")
	

func create_connection(from: NodeConnector, to: NodeConnector):
	for c_connection in connections:
		if (c_connection.to_connector == to.get_id() and c_connection.to_node == to.get_node_id()) or (c_connection.from_connector == to.get_id() and c_connection.from_node == to.get_node_id()):
			remove_connection(connections.find(c_connection))
	
	
	var connection: Connection = Connection.new()
	connection.from_node = from.get_node_id()
	connection.to_node = to.get_node_id()
	connection.from_connector = from.get_id()
	connection.to_connector = to.get_id()
	connections.append(connection)
	
	from.connected = true
	to.connected = true
	
	from.connected = true
	to.connected = true
	
	print(connections)
	

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
