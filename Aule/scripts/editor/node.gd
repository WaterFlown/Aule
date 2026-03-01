class_name EditorNode extends Control
var id = null
@export var functionality: NodeFunctionality ##This node's functionality
@export var inspector = "res://scenes/inspector/inspector_base.tscn" ##Path to this node's inspector in assets
@export var filepath = "res://scenes/editor_node.tscn" ##Path to this node in the filesystem
@export var connectors: Array[NodeConnector] = []

@export var connector_label: RichTextLabel ##This node's connector description display. Already included, changing is not needed.
@export var node_highlight: PanelContainer ##This node's hightlight. Already included, changing is not needed.



var inited = false
var selected = false

signal move_node_to_top
signal moved_node
signal select_node
var drag_position = null

func initing():
	if not inited:
		print(id)
		move_node_to_top.connect(Globals.editor.move_node_to_top.bind())
		select_node.connect(Globals.editor.select_node.bind())
		functionality.parentID = id;
		inited = true

func get_id() -> int:
	return id

func get_connector_by_id(connector_id: int) -> NodeConnector:
	for connector in connectors:
		if connector.id == connector_id:
			return connector
	return null

func select():
	selected = true
	node_highlight.visible = true

func unselect():
	functionality.Deselect()
	selected = false
	node_highlight.visible = false

func _on_gui_input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			drag_position = get_global_mouse_position() - global_position
			emit_signal("move_node_to_top", self)
			if not selected:
				emit_signal("select_node", self)
		else:
			drag_position = null
	if event is InputEventMouseMotion and drag_position:
		global_position = get_global_mouse_position() - drag_position
		emit_signal("moved_node")

func connector_hovering(connector: NodeConnector, enter: bool):
	if connector_label and enter:
		connector_label.text = connector.label
	else:
		connector_label.text = ""

func _process(delta):
	initing()

func set_property(property, value):
	functionality.properties[property] = value

func get_property(property):
	if functionality.properties.has(property):
		return functionality.properties[property]
	return null

func get_properties():
	return functionality.properties
	
