using Godot;
using System;

public partial class MultiplyOutput : NodeFunctionality
{
	float[,] PrimaryMap = {};

	public override T[,] Evaluate<T>(int port){
		PrimaryMap = getFromInput<float>(0);
		return (T[,])(object)PrimaryMap;
	}
    
}

