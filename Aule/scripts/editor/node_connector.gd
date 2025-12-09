class_name NodeConnector extends TextureRect
@export var IOtype: Globals.NodeConnectorIOType = Globals.NodeConnectorIOType.INPUT

var dragging: bool = false
var drag_line: Line2D = null
var center_offset: Vector2 = Vector2(32,64)


func _ready():
	if IOtype == Globals.NodeConnectorIOType.INPUT:
		pass
	elif IOtype == Globals.NodeConnectorIOType.OUTPUT:
		flip_h = true

func start_drag():
	dragging = true
	drag_line = Line2D.new()
	drag_line.position = position
	drag_line.add_point(center_offset-position)
	drag_line.add_point(center_offset-position)
	
	drag_line.begin_cap_mode = Line2D.LINE_CAP_ROUND
	drag_line.end_cap_mode = Line2D.LINE_CAP_ROUND
	drag_line.joint_mode = Line2D.LINE_JOINT_ROUND
	
	drag_line.antialiased = true
	drag_line.width = 10
	drag_line.default_color = Color(1.0, 1.0, 1.0, 0.8)
	add_child(drag_line)

func stop_drag():
	if drag_line:
		drag_line.queue_free()
	dragging = false

func _on_gui_input(event):
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_MASK_LEFT:
		if event.pressed:
			start_drag()
		else:
			stop_drag()

func _process(delta):
	if drag_line:
		drag_line.set_point_position(1, get_local_mouse_position()-position)
