class_name NodeAddEntry extends Node
@export var category: bool = false ## Is a category - doesn't place a node
@export var text: String ## Displayed text.
@export var id: String ##ID String. Must be unique!
@export var node: String ## Path to the editor node that'll be placed when activated
@export var child_of: String ##Category id
