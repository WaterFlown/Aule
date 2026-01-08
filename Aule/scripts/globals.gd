extends Node

enum NodeConnectorIOType {INPUT, OUTPUT}
enum NodeConnectorCategory {DEFAULT}

@onready var editor: Editor = null
@onready var terrain: Terrain = null
@onready var terrain_size: Vector2i = Vector2(512, 512)
