extends Node

enum NodeConnectorIOType {INPUT, OUTPUT}
enum NodeConnectorCategory {DEFAULT, VECTOR2}

@onready var editor: Editor = null
@onready var terrain_view: TerrainView = null
@onready var terrain: Terrain = null
@onready var terrain_size: Vector2i = Vector2(512, 512)
@onready var terrain_height = 128
