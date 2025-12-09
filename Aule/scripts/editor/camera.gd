class_name EditorCamera extends Camera2D

var target_zoom = zoom
@export var zoom_increment = Vector2(0.33,0.33)
@export var zoom_speed = 24.0
@export var max_zoom = Vector2(1.5,1.5)
@export var min_zoom = Vector2(0.7,0.7)

@export var xy_scroll_limits = Vector2(1600,1600)

@onready var mouse_pos := get_global_mouse_position()
@onready var new_mouse_pos := get_global_mouse_position()

func _unhandled_input(event):
	if event is InputEventMouseMotion:
		if event.button_mask == MOUSE_BUTTON_MASK_LEFT || event.button_mask == MOUSE_BUTTON_MASK_MIDDLE:
			position.x = clamp(position.x - event.relative.x / zoom.x, -xy_scroll_limits.x, xy_scroll_limits.x)
			position.y = clamp(position.y - event.relative.y / zoom.y, -xy_scroll_limits.y, xy_scroll_limits.y)

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				if zoom < max_zoom:
					target_zoom = clamp(zoom+zoom_increment, min_zoom, max_zoom)
					set_physics_process(true)
			elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				if zoom > min_zoom:
					target_zoom = clamp(zoom-zoom_increment, min_zoom, max_zoom)
					set_physics_process(true)

func _physics_process(delta):
	if custom_approx_equal(zoom.x, target_zoom.x) :
		zoom = target_zoom
		set_physics_process(false)
	else:
		mouse_pos = get_global_mouse_position()
		zoom = lerp(zoom, target_zoom, zoom_speed*delta)
		new_mouse_pos = get_global_mouse_position()
		position.x = clamp(position.x + mouse_pos.x - new_mouse_pos.x, -xy_scroll_limits.x, xy_scroll_limits.x)
		position.y = clamp(position.y + mouse_pos.y - new_mouse_pos.y, -xy_scroll_limits.y, xy_scroll_limits.y)
		

func custom_approx_equal(a: float, b:float):
	if (a == b):
		return true;
	return abs(a - b) < 0.002;
