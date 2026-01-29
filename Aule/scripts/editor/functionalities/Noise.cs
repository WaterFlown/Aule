using Godot;
using System;

public partial class Noise : NodeFunctionality
{
	FastNoiseLite noise = new FastNoiseLite();
	float[,] PrimaryMap = {};
	Vector2[,] DistortionMap = {};
	float[,] MaskMap = {};

    public override void Initialize()
    {
		if (seed_set)
		{
			noise.Seed = (int)properties["Seed"];
		}
		properties["CellularDistanceFunction"] = (int)noise.CellularDistanceFunction;
		properties["CellularJitter"] = noise.CellularJitter;
		properties["CellularReturnType"] = (int)noise.CellularReturnType;
		properties["DomainWarpAmplitude"] = noise.DomainWarpAmplitude;
		properties["DomainWarpEnabled"] = noise.DomainWarpEnabled;
		properties["DomainWarpFractalGain"] = noise.DomainWarpFractalGain;
		properties["DomainWarpFractalLacunarity"] = noise.DomainWarpFractalLacunarity;
		properties["DomainWarpFractalOctaves"] = noise.DomainWarpFractalOctaves;
		properties["DomainWarpFrequency"] = noise.DomainWarpFrequency;
		properties["DomainWarpFractalType"] = (int)noise.DomainWarpFractalType;
		properties["DomainWarpType"] = (int)noise.DomainWarpType;
		properties["FractalGain"] = noise.FractalGain;
		properties["FractalLacunarity"] = noise.FractalLacunarity;
		properties["FractalOctaves"] = noise.FractalOctaves;
		properties["FractalPingPongStrength"] = noise.FractalPingPongStrength;
		properties["FractalType"] = (int)noise.FractalType;
		properties["Frequency"] = noise.Frequency;
		properties["NoiseType"] = (int)noise.NoiseType;
		properties["Offset"] = noise.Offset;
    }
	private float GetValue(int x, int y, bool UseDistortion, bool UseMask, bool CreatePrimaryMap) // x,y positions for 2D noise, distortion and mask coordinates, enable distortion and mask, CreatePrimaryMap - primary map is empty and needs to be created
	{
		float current = 0f;
		if (!CreatePrimaryMap)
		{
			current = PrimaryMap[x, y];
		}
		if (UseDistortion)
		{
			Vector2 distortion = DistortionMap[x, y];
			current += noise.GetNoise2D(x + distortion.X, y + distortion.Y);
		}
		else
		{
			current += noise.GetNoise2D(x, y);
		}	

		if (UseMask)
		{
			current *= MaskMap[x, y];
		}

		return current;
	}
	public override T[,] Evaluate<T>(int port){
		noise.Seed = (int)properties["Seed"];
		noise.CellularDistanceFunction = (FastNoiseLite.CellularDistanceFunctionEnum)(int)properties["CellularDistanceFunction"];
		noise.CellularJitter = (float)properties["CellularJitter"];
		noise.CellularReturnType = (FastNoiseLite.CellularReturnTypeEnum)(int)properties["CellularReturnType"];
		noise.DomainWarpAmplitude = (float)properties["DomainWarpAmplitude"];
		noise.DomainWarpEnabled = (bool)properties["DomainWarpEnabled"];
		noise.DomainWarpFractalGain = (float)properties["DomainWarpFractalGain"];
		noise.DomainWarpFractalLacunarity = (float)properties["DomainWarpFractalLacunarity"];
		noise.DomainWarpFractalOctaves = (int)properties["DomainWarpFractalOctaves"];
		noise.DomainWarpFrequency = (float)properties["DomainWarpFrequency"];
		noise.DomainWarpFractalType = (FastNoiseLite.DomainWarpFractalTypeEnum)(FastNoiseLite.FractalTypeEnum)(int)properties["DomainWarpFractalType"];
		noise.DomainWarpType = (FastNoiseLite.DomainWarpTypeEnum)(int)properties["DomainWarpType"];
		noise.FractalGain = (float)properties["FractalGain"];
		noise.FractalLacunarity = (float)properties["FractalLacunarity"];
		noise.FractalOctaves = (int)properties["FractalOctaves"];
		noise.FractalPingPongStrength = (float)properties["FractalPingPongStrength"];
		noise.FractalType = (FastNoiseLite.FractalTypeEnum)(int)properties["FractalType"];
		noise.Frequency = (float)properties["Frequency"];
		noise.NoiseType = (FastNoiseLite.NoiseTypeEnum)(int)properties["NoiseType"];
		noise.Offset = (Vector3)properties["Offset"];

		
		PrimaryMap = getFromInput<float>(0);
		DistortionMap = getFromInput<Vector2>(2);
		MaskMap = getFromInput<float>(1);

		bool UseDistortion = false;
		if (DistortionMap.GetLength(0) != 0)
		{
			UseDistortion = true;
		}
		bool UseMask = false;
		if (MaskMap.GetLength(0) != 0)	
		{
			UseMask = true;
		}

		if (PrimaryMap.GetLength(0) == 0) {
			Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
			float[,] CreatedPrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
			for (int i = 0; i < CreatedPrimaryMap.GetLength(0); i++) {
				for (int j = 0; j < CreatedPrimaryMap.GetLength(1); j++) {
					CreatedPrimaryMap[i, j] = GetValue(i, j, UseDistortion, UseMask, true);
				}
			}
			PrimaryMap = CreatedPrimaryMap;
		} 
		else {
			for (int i = 0; i < PrimaryMap.GetLength(0); i++) {
				for (int j = 0; j < PrimaryMap.GetLength(1); j++) {
					PrimaryMap[i, j] += GetValue(i, j, UseDistortion, UseMask, false);
				}	
			}
		}
		return (T[,])(object)PrimaryMap;
	}
    
}
