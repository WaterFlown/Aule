class_name NodeAdderWindow extends Window

func _on_close_requested():
	close()

func close():
	queue_free()
