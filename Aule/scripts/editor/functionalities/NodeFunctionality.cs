using Godot;
using System;

[GlobalClass]
public partial class NodeFunctionality : Node {
	public int parentID = -1;
	[Export]
	public int defaultOutputPortID = 2;
	
	public virtual float[,] evaluate(int port){
		float[,] a = {};
		a = getFromInput(0);
		if (a.GetLength(0) == 0) {
			Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
			float[,] flat = new float[(int)terrain_size.X, (int)terrain_size.Y];
			for (int i = 0; i < flat.GetLength(0); i++) {
				for (int j = 0; j < flat.GetLength(1); j++) {
					flat[i, j] = 0;
				}
			}
			a = flat;
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
