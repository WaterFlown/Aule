class_name SerializedNode extends Node

var node_position: Vector2 = Vector2(0, 0)
var filepath: String = ""
var properties = {}
var id = -1

func save():
	var data = {"type":"node", "id":id, "filepath": filepath, "node_position_x": node_position.x, "node_position_y": node_position.y, "properties": properties}
	return data
