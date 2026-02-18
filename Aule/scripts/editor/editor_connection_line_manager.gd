class_name ConnectionLineManager extends Node

var lines: Array[ConnectionLine] = []

func add_line(from_node: int, to_node: int, from_connector: int, to_connector: int):
	var line = ConnectionLine.new()
	line.from_node = from_node
	line.to_node = to_node
	line.from_connector = from_connector
	line.to_connector = to_connector
	lines.append(line)
	add_child(line)


func remove_line(from_node: int, to_node: int, from_connector: int, to_connector: int):
	for line in lines:
		if line.from_node == from_node and line.to_node == to_node and line.from_connector == from_connector and line.to_connector == to_connector:
			var removed_line = lines.pop_at(lines.find(line))
			removed_line.queue_free()

#func remove_all_lines():
	#for line in lines:
			#var removed_line = lines.pop_at(lines.find(line))
			#removed_line.queue_free()
