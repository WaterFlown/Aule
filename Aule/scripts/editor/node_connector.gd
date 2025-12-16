class_name NodeConnector extends TextureRect
@export var id: int = 0
@export var IOtype: Globals.NodeConnectorIOType = Globals.NodeConnectorIOType.INPUT
@export var label: String = "This connector does not have a description"
@onready var parent: EditorNode = get_parent().get_parent()

var inited: bool = false
var connected: bool = false

var connection_line: Line2D = null

var dragging: bool = false
var drag_line: Line2D = null
var center_offset: Vector2 = pivot_offset

signal start_connecting(from: NodeConnector)
signal stop_connecting()
signal mouse_hover(connector: NodeConnector, enter: bool)

func _ready():
	pass

func get_id() -> int:
	return id

func get_node_id() -> int:
	return parent.get_id()

func connect_connectors(other: NodeConnector):
	connected = true
	if(IOtype == Globals.NodeConnectorIOType.OUTPUT):
		Globals.editor.connection_line_manager.add_line(parent.get_id(), other.parent.get_id(), get_id(), other.get_id())
	
func disconnect_connectors():
	connected = false
	if connection_line:
		connection_line.queue_free()


func start_drag():
	dragging = true
	drag_line = Line2D.new()
	drag_line.position = position
	drag_line.add_point(center_offset-position)
	drag_line.add_point(center_offset-position)
	
	drag_line.begin_cap_mode = Line2D.LINE_CAP_ROUND
	drag_line.end_cap_mode = Line2D.LINE_CAP_ROUND
	drag_line.joint_mode = Line2D.LINE_JOINT_ROUND
	
	drag_line.width = 20
	drag_line.default_color = Color(1.0, 1.0, 1.0, 0.8)
	
	drag_line.z_index = 5
	add_child(drag_line)
	start_connecting.emit(self)

func stop_drag():
	if drag_line:
		drag_line.queue_free()
	dragging = false
	stop_connecting.emit()

func _on_gui_input(event):
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_MASK_LEFT:
		if event.pressed:
			start_drag()
		elif dragging:
			stop_drag()

func _process(delta):
	if Globals.editor and not inited:
		start_connecting.connect(Globals.editor.start_connecting.bind())
		stop_connecting.connect(Globals.editor.stop_connecting.bind())
		mouse_hover.connect(Globals.editor.connector_hovering.bind())
		mouse_hover.connect(parent.connector_hovering.bind())
		inited = true
	
	if drag_line:
		drag_line.set_point_position(1, get_local_mouse_position()-position)


func _on_mouse_entered():
	mouse_hover.emit(self, true)


func _on_mouse_exited():
	mouse_hover.emit(self, false)
