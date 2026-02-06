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
			properties["Seed"] = seed;
			properties["foo"] = 0;
			seed_set = true;
		}
		Initialize();
	}
	public virtual void Initialize()
	{
		
	}
	public virtual void Deselect()
	{
		
	}
	public virtual T[,] Evaluate<T>(int port){
		float[,] a = {};
		a = getFromInput<float>(0);
		if (a.GetLength(0) == 0) {
			
			Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
			float[,] flat = new float[(int)terrain_size.X, (int)terrain_size.Y];
			for (int i = 0; i < flat.GetLength(0); i++) {
				for (int j = 0; j < flat.GetLength(1); j++) {
					flat[i, j] = (float)properties["foo"];
				}
			}
			a = flat;
		} 
		else {
			for (int i = 0; i < a.GetLength(0); i++) {
				for (int j = 0; j < a.GetLength(1); j++) {
					a[i, j] += (float)properties["foo"];
				}	
			}
		}
		return (T[,])(object)a;
	}
	
	public virtual Godot.Collections.Array Output() {
		return ConvertToGDArray(Evaluate<float>(defaultOutputPortID));
	}
	
	public static Godot.Collections.Array ConvertToGDArray(float[,] arrayToConvert) {
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
	public T[,] getFromInput<T>(int to_port) {
		GodotObject editor = (GodotObject)GetNode<Node>("/root/Globals").Get("editor");
		GodotObject connection = (GodotObject)editor.Call("get_connection_to", parentID, to_port);
		if (connection == null) {
			T[,] empty = {};
			return empty;
		}
		NodeFunctionality otherFunctionality = ((NodeFunctionality)((GodotObject)editor.Call("get_node_connected_to", parentID, to_port)).Get("functionality"));
		return otherFunctionality.Evaluate<T>((int)connection.Get("connector_from"));
	}
}
