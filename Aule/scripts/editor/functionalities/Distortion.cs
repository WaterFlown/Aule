using Godot;
using System;

public partial class Distortion : NodeFunctionality
{
    FastNoiseLite NoiseX = new FastNoiseLite();
    FastNoiseLite NoiseY = new FastNoiseLite();
    
    public override void Initialize()
	{
		NoiseX.SetFrequency(0.005f);
		NoiseY.SetFrequency(NoiseX.GetFrequency());
		NoiseX.Seed = (int)properties["Seed"];
        NoiseY.Seed = (int)properties["Seed"] + 1000;

		NoiseX.SetFractalType(FastNoiseLite.FractalTypeEnum.None);
		NoiseY.SetFractalType(FastNoiseLite.FractalTypeEnum.None);

		properties["Strength"] = 25.0f;
        properties["Frequency"] = NoiseX.GetFrequency();

	}

	public override T[,] Evaluate<T>(int port){
		Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
		Vector2[,] PrimaryDistortionMap = new Vector2[(int)terrain_size.X, (int)terrain_size.Y];
		for (int i = 0; i < PrimaryDistortionMap.GetLength(0); i++) {
			for (int j = 0; j < PrimaryDistortionMap.GetLength(1); j++) {
				PrimaryDistortionMap[i, j].X = NoiseX.GetNoise2D(i, j) * (float)properties["Strength"];
                PrimaryDistortionMap[i, j].Y = NoiseY.GetNoise2D(i, j) * (float)properties["Strength"];
			}
		}
		return (T[,])(object)PrimaryDistortionMap;
	}
	
	public override Godot.Collections.Array Output() {
        float[,] emptyArray = new float[0, 0];
		return ConvertToGDArray(emptyArray);
	}
}
