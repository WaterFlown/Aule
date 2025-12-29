class_name EditorNode extends Control
var id = null
@export var functionality: NodeFunctionality
@export var connector_label: RichTextLabel
@export var connectors: Array[NodeConnector] = []
@export var node_highlight: Panel
var inited = false
var selected = false

signal move_node_to_top
signal moved_node
signal select_node
var drag_position = null

func initing():
	if not inited:
		Globals.editor.register_node(self)
		move_node_to_top.connect(Globals.editor.move_node_to_top.bind())
		select_node.connect(Globals.editor.select_node.bind())
		inited = true

func get_id() -> int:
	return id

func get_connector_by_id(id: int) -> NodeConnector:
	for connector in connectors:
		if connector.id == id:
			return connector
	return null

func select():
	selected = true
	node_highlight.visible = true

func unselect():
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
	
