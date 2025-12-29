using Godot;
using System;

[GlobalClass]
public partial class NodeFunctionality : Node {
	public int parentID = -1;
	
	public virtual float[,] evaluate(int port){
		float[,] a = {};
		a = getFromInput(0);
		return a;
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
