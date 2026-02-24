extends MenuBar
@export var File: PopupMenu
@export var Edit: PopupMenu
@export var Project: PopupMenu
@export var View: PopupMenu

func _ready():
	##File
	File.add_item("New Project", 0)
	File.add_item("Save Project", 1)
	File.add_item("Load Project", 2)
	
	##Projects
	Project.add_item("Project Settings", 0)
	
	##Edit
	Edit.add_item("Copy Selected Node", 0)
	Edit.add_item("Paste", 1)
	
	##View
	View.add_item("Toggle Inspector", 0)
	View.add_item("Reset Terrain View", 1)


func _on_project_index_pressed(index):
	match(index):
		0:
			if(Globals.editor):
				Globals.editor.open_project_settings()
		_:
			pass



func _on_file_index_pressed(index):
	match(index):
		0:
			if(Globals.editor):
				Globals.editor.clear_project()
		1:
			if(Globals.editor && Globals.editor.node_loading_manager):
				Globals.editor.node_loading_manager.save_project()
		2:
			if(Globals.editor && Globals.editor.node_loading_manager && Globals.editor.connection_line_manager):
				Globals.editor.node_loading_manager.load_project()
				Globals.editor.connection_line_manager.update_all_lines()
		_:
			pass


func _on_edit_index_pressed(index):
	match(index):
		0:
			if(Globals.editor && Globals.editor.node_loading_manager && Globals.editor.selected_node):
				Globals.editor.node_loading_manager.copy()
		1:
			if(Globals.editor && Globals.editor.node_loading_manager):
				Globals.editor.node_loading_manager.paste()
		_:
			pass


func _on_view_index_pressed(index):
	match(index):
		0:
			if(Globals.editor):
				Globals.editor.toggle_inspector()
		1:
			if(Globals.terrain_view):
				Globals.terrain_view.setCameraProperties()
		_:
			pass
