extends Tree
@export var entries: Array[NodeAddEntry]
@onready var entry_id_map: Dictionary[String, TreeItem]
@onready var root = create_item()

func _ready():
	hide_root = true

	for entry: NodeAddEntry in entries:
		var category = root
		if entry.child_of != "":
			category = entry_id_map.get(entry.child_of)
		var child = create_item(category)
		
		entry_id_map[entry.id] = child
		
		child.set_text(0, entry.text)
		if entry.category:
			child.set_selectable(0, false)

func _on_item_activated():
	if get_selected():
		var id: String = entry_id_map.find_key(get_selected())
		for entry in entries:
			if id == entry.id:
				Globals.editor.add_node(entry.node)
				break
	Globals.editor.node_adder_window.close()
