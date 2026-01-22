using Godot;
using System;

[GlobalClass]
public partial class NodeFunctionality : Node {
	public int parentID = -1;
	public bool seed_set = false;
	[Export]
	public int defaultOutputPortID = 2;

	public Godot.Collections.Dictionary properties = new Godot.Collections.Dictionary{};
	
	public override void _Ready() {
		if (seed_set == false)
		{
			int seed = 0;
			seed = (int)GD.Randi();
			properties["seed"] = seed;
			properties["foo"] = 0;
			seed_set = true;
		}
	}

	public virtual float[,] evaluate(int port){
		float[,] a = {};
		a = getFromInput(0);
		if (a.GetLength(0) == 0) {
			/*FastNoiseLite noise = new FastNoiseLite();
			noise.Seed = (int)properties["seed"];
			noise.SetFrequency(0.0005f);
			noise.SetFractalType(FastNoiseLite.FractalTypeEnum.Fbm);
			
			noise.SetFractalLacunarity(2f);
			noise.SetFractalGain(0.7f);
			noise.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Simplex);*/
			
			
			Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
			float[,] flat = new float[(int)terrain_size.X, (int)terrain_size.Y];
			for (int i = 0; i < flat.GetLength(0); i++) {
				for (int j = 0; j < flat.GetLength(1); j++) {
					flat[i, j] = (float)properties["foo"];// +noise.GetNoise2D(i, j) * 100;
				}
			}
			a = flat;
		} 
		else {
			FastNoiseLite noise = new FastNoiseLite();
			noise.Seed = (int)properties["seed"];
			noise.SetFrequency(0.0005f);
			noise.SetFractalType(FastNoiseLite.FractalTypeEnum.Fbm);
			
			noise.SetFractalLacunarity(2f);
			noise.SetFractalGain(0.7f);
			noise.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Simplex);
			for (int i = 0; i < a.GetLength(0); i++) {
				for (int j = 0; j < a.GetLength(1); j++) {
					a[i, j] += (float)properties["foo"];// + noise.GetNoise2D(i, j) * 100;
				}	
			}
		}
		return a;
	}
	
	public Godot.Collections.Array output() {
		return convertToGDArray(evaluate(defaultOutputPortID));
	}
	
	public Godot.Collections.Array convertToGDArray(float[,] arrayToConvert) {
		Godot.Collections.Array convertedArray = [];
		for (int i = 0; i < arrayToConvert.GetLength(0); i++) {
			Godot.Collections.Array innerArray = [];
			for (int j = 0; j < arrayToConvert.GetLength(1); j++) {
				innerArray.Add(arrayToConvert[i, j]);
			}
			convertedArray.Add(innerArray);
		}
		return convertedArray;
	}
	public float[,] getFromInput(int to_port) {
		GodotObject editor = (GodotObject)GetNode<Node>("/root/Globals").Get("editor");
		GodotObject connection = (GodotObject)editor.Call("get_connection_to", parentID, to_port);
		if (connection == null) {
			float[,] empty = {};
			return empty;
		}
		NodeFunctionality otherFunctionality = ((NodeFunctionality)((GodotObject)editor.Call("get_node_connected_to", parentID, to_port)).Get("functionality"));
		return otherFunctionality.evaluate((int)connection.Get("connector_from"));
	}
}
