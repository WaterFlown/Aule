using Godot;
using System;

[GlobalClass]
public partial class NodeFunctionality : Node {
	public virtual Godot.Collections.Array evaluate(int port){
		float[,] a = {};
		return convertToGDArray(a);
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
}
