class_name SerializedNode extends Node

var node_position: Vector2 = Vector2(0, 0)
var filepath: String = ""
var properties = {}

func save():
	var data = {"type":"node", "filepath": filepath, "node_position": node_position, "properties": properties}
	return data
