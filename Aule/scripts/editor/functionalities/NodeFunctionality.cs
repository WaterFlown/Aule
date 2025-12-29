using Godot;
using System;

[GlobalClass]
public partial class NodeFunctionality : Node {
	public virtual float[,] evaluate(int port){
		float[,] a = {};
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
		GodotObject connection = (GodotObject)editor.Call("get_connection_to", (int)((GodotObject)GetParent()).Call("get_id"), to_port);
		NodeFunctionality otherFunctionality = ((NodeFunctionality)((GodotObject)editor.Call("get_node_connected_to", GetParent().Call("get_id"), to_port)).Get("functionality"));
		return otherFunctionality.evaluate((int)connection.Get("connector_from"));
	}
}
