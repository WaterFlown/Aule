class_name Editor extends Node

var connecting: bool = false
var connecting_from: NodeConnector = null
var hovering_connector: NodeConnector = null
var connecting_to: NodeConnector = null


func _ready():
	Globals.editor = self

func start_connecting(from: NodeConnector):
	connecting = true
	connecting_from = from
	connecting_to = null

func stop_connecting():
	connecting = false
	if connecting_from and connecting_to:
		print("comp")
		pass
	
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


func _process(delta):
	handle_connecting()
