@tool
extends MeshInstance3D

@export var x_size = 20
@export var z_size = 20
@export var update = false
#@export var visualization = false

func _ready():
	generate_terrain()
	
func generate_terrain():
	var a_mesh: ArrayMesh
	var surfaceTool = SurfaceTool.new()
	
	var noise = FastNoiseLite.new()
	noise.seed = randi()
	noise.noise_type = FastNoiseLite.TYPE_PERLIN
	noise.fractal_octaves = 5
	noise.fractal_type = FastNoiseLite.FRACTAL_RIDGED
	noise.fractal_gain = 0.3
	noise.frequency = 0.005
	var noise_layer_1 = FastNoiseLite.new()
	noise_layer_1.noise_type = FastNoiseLite.TYPE_PERLIN
	noise_layer_1.frequency = 0.01
	
	surfaceTool.begin(Mesh.PRIMITIVE_TRIANGLES)
	for z in range(z_size+1):
		for x in range(x_size+1):
			var y = noise.get_noise_2d(x,z)**2 * 50 + noise_layer_1.get_noise_2d(x,z) * 20
			
			surfaceTool.set_uv(Vector2(inverse_lerp(0, x_size, x), inverse_lerp(0, z_size, z)))
			
			surfaceTool.add_vertex(Vector3(x,y,z))
			#if visualization:
				#draw_sphere(Vector3(x,y,z))
	
	for row in range(z_size):
			for col in range(x_size):
				var upper_left = row * (x_size + 1) + col #row + etc. because indexes are a one dimensional array
				var upper_right = row * (x_size + 1) + col + 1
				var lower_left = (row + 1) * (x_size + 1) + col
				var lower_right = (row + 1) * (x_size + 1) + col + 1
			
				surfaceTool.add_index(upper_left)
				surfaceTool.add_index(upper_right)
				surfaceTool.add_index(lower_left)
				surfaceTool.add_index(upper_right)
				surfaceTool.add_index(lower_right)
				surfaceTool.add_index(lower_left)
	
	surfaceTool.generate_normals()
	a_mesh = surfaceTool.commit()
	mesh = a_mesh
	

#func draw_sphere(pos:Vector3):
	#var sphere = MeshInstance3D.new()
	#add_child(sphere)
	#sphere.position = pos
	#var sphere_mesh = SphereMesh.new()
	#sphere_mesh.height = 0.2
	#sphere_mesh.radius = 0.1
	#sphere.mesh = sphere_mesh

func _process(delta):
	if update:
		update = false
		for i in get_children():
			i.free()
		generate_terrain()
