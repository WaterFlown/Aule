class_name TerrainView extends Node3D
@export_group("Required")
@export var cameraPivot: Node3D
@export var camera: Camera3D

@export_group("View Settings")
@export var cameraRotateSensitivity = 0.01
@export var cameraZoomSensitivity = 1
@export var zoom_speed = 20
@export var zoom_increment = 250

var camera_target_y: float = 1500
var camera_max_y = 2000
var camera_min_y = 10


func _ready():
	Globals.terrain_view = self
	setCameraProperties()

func setCameraProperties():
	cameraPivot.position.x = Globals.terrain_size.x/2
	cameraPivot.position.z = Globals.terrain_size.y/2
	
	print(Globals.terrain.x_size)
	
	camera.position.y = Globals.terrain_size.x * 2
	camera_target_y = camera.position.y
	camera_max_y = camera.position.y * 1.25 + 250
	
	cameraPivot.rotation.x = 45
	cameraPivot.rotation.y = 45
	
	

func _unhandled_input(event):
	if event is InputEventMouseMotion:
		if event.button_mask == MOUSE_BUTTON_MASK_LEFT || event.button_mask == MOUSE_BUTTON_MASK_MIDDLE:
			cameraPivot.rotation.y = cameraPivot.rotation.y - event.relative.x * cameraRotateSensitivity
			cameraPivot.rotation.x = clamp(cameraPivot.rotation.x - event.relative.y * cameraRotateSensitivity, 0, PI/2.5)
func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				if camera.position.y > camera_min_y:
					camera_target_y = clamp(camera.position.y-zoom_increment, camera_min_y, camera_max_y)
					set_physics_process(true)
			elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				if camera.position.y < camera_max_y:
					camera_target_y = clamp(camera.position.y+zoom_increment, camera_min_y, camera_max_y)
					set_physics_process(true)

func _physics_process(delta):
	if is_equal_approx(camera.position.y, camera_target_y):
		camera.position.y = camera_target_y
		set_physics_process(false)
	else:
		camera.position.y = lerp(camera.position.y, camera_target_y, zoom_speed*delta)
