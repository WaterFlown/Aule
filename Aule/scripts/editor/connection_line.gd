class_name ConnectionLine extends Line2D

var from_node: int #output
var to_node: int   #input
var from_connector: int #output connector
var to_connector: int	#input connector


var from_connector_extracted: NodeConnector
var to_connector_extracted: NodeConnector


var from_offset: Vector2
var to_offset: Vector2


func _ready():
	var from_node_extracted: EditorNode = Globals.editor.nodes.get(from_node)
	from_connector_extracted = from_node_extracted.get_connector_by_id(from_connector)
	
	var to_node_extracted: EditorNode = Globals.editor.nodes.get(to_node)
	to_connector_extracted = to_node_extracted.get_connector_by_id(to_connector)
	
	begin_cap_mode = Line2D.LINE_CAP_ROUND
	end_cap_mode = Line2D.LINE_CAP_ROUND
	joint_mode = Line2D.LINE_JOINT_ROUND
	
	width = 2
	default_color = Color(1.0, 1.0, 1.0, 1.0)
	z_index = 5
	
	from_offset = from_connector_extracted.center_offset * from_connector_extracted.get_parent().scale
	to_offset = to_connector_extracted.center_offset * to_connector_extracted.get_parent().scale
	
	add_point(from_connector_extracted.global_position+from_offset)
	add_point(to_connector_extracted.global_position+to_offset)
	
	to_node_extracted.connect("moved_node", update_positions)
	from_node_extracted.connect("moved_node", update_positions)
	get_viewport().size_changed.connect(update_positions)

func update_positions():
	points[0] = from_connector_extracted.global_position+from_offset
	points[1] = to_connector_extracted.global_position+to_offset
