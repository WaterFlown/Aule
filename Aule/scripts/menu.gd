extends MenuBar
@export var File: PopupMenu
@export var Edit: PopupMenu

func _ready():
	##File
	File.add_item("New Project", 0)
	File.add_item("Save", 1)
	
	##Edit
	Edit.add_item("Project Settings", 0)


func _on_edit_index_pressed(index):
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
		_:
			pass
