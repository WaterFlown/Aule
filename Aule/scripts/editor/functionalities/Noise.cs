using Godot;
using System;
using System.Linq;
using System.Threading;


public partial class Noise : NodeFunctionality
{
	private struct ThreadData
	{
		public int StartY;
		public int EndY;
		public bool CreatePrimaryMap;
		public bool UseDistortion;
		public bool UseMask;
	}
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
		/*properties["DomainWarpAmplitude"] = noise.DomainWarpAmplitude;
		properties["DomainWarpEnabled"] = noise.DomainWarpEnabled;
		properties["DomainWarpFractalGain"] = noise.DomainWarpFractalGain;
		properties["DomainWarpFractalLacunarity"] = noise.DomainWarpFractalLacunarity;
		properties["DomainWarpFractalOctaves"] = noise.DomainWarpFractalOctaves;
		properties["DomainWarpFrequency"] = noise.DomainWarpFrequency;
		properties["DomainWarpFractalType"] = (int)noise.DomainWarpFractalType;
		properties["DomainWarpType"] = (int)noise.DomainWarpType;*/
		properties["FractalGain"] = noise.FractalGain;
		properties["FractalLacunarity"] = noise.FractalLacunarity;
		properties["FractalOctaves"] = noise.FractalOctaves;
		properties["FractalPingPongStrength"] = noise.FractalPingPongStrength;
		properties["FractalType"] = (int)noise.FractalType;
		properties["Frequency"] = noise.Frequency;
		properties["NoiseType"] = (int)noise.NoiseType;
		properties["Offset"] = noise.Offset;

		properties["Strength"] = 0.1f;
    }
	private float GetValue(int x, int y, FastNoiseLite tnoise, bool UseDistortion, bool UseMask, bool CreatePrimaryMap) // x,y positions for 2D noise, distortion and mask coordinates, enable distortion and mask, CreatePrimaryMap - primary map is empty and needs to be created
	{
		float strength = (float)properties["Strength"];
		float current = 0f;
		if (!CreatePrimaryMap)
		{
			current = PrimaryMap[x, y];
		}

		if (UseDistortion)
		{
			Vector2 distortion = DistortionMap[x, y];
			current += (tnoise.GetNoise2D(x + distortion.X, y + distortion.Y)+ 1) / 2 * strength;
		}
		else
		{
			current += (tnoise.GetNoise2D(x, y)+ 1) / 2 * strength;
		}	

		if (UseMask)
		{
			current *= MaskMap[x, y];
		}

		return Math.Clamp(current, 0f, 1f);
	}

	private void ThreadGenerate(object data)
	{
		ThreadData tdata = (ThreadData)data;
		FastNoiseLite tnoise = new FastNoiseLite();
		tnoise.Seed = (int)properties["Seed"];
		tnoise.CellularDistanceFunction = (FastNoiseLite.CellularDistanceFunctionEnum)(int)properties["CellularDistanceFunction"];
		tnoise.CellularJitter = (float)properties["CellularJitter"];
		tnoise.CellularReturnType = (FastNoiseLite.CellularReturnTypeEnum)(int)properties["CellularReturnType"];
		/*tnoise.DomainWarpAmplitude = (float)properties["DomainWarpAmplitude"];
		tnoise.DomainWarpEnabled = (bool)properties["DomainWarpEnabled"];
		tnoise.DomainWarpFractalGain = (float)properties["DomainWarpFractalGain"];
		tnoise.DomainWarpFractalLacunarity = (float)properties["DomainWarpFractalLacunarity"];
		tnoise.DomainWarpFractalOctaves = (int)properties["DomainWarpFractalOctaves"];
		tnoise.DomainWarpFrequency = (float)properties["DomainWarpFrequency"];
		tnoise.DomainWarpFractalType = (FastNoiseLite.DomainWarpFractalTypeEnum)(FastNoiseLite.FractalTypeEnum)(int)properties["DomainWarpFractalType"];
		tnoise.DomainWarpType = (FastNoiseLite.DomainWarpTypeEnum)(int)properties["DomainWarpType"];*/
		tnoise.FractalGain = (float)properties["FractalGain"];
		tnoise.FractalLacunarity = (float)properties["FractalLacunarity"];
		tnoise.FractalOctaves = (int)properties["FractalOctaves"];
		tnoise.FractalPingPongStrength = (float)properties["FractalPingPongStrength"];
		tnoise.FractalType = (FastNoiseLite.FractalTypeEnum)(int)properties["FractalType"];
		tnoise.Frequency = (float)properties["Frequency"];
		tnoise.NoiseType = (FastNoiseLite.NoiseTypeEnum)(int)properties["NoiseType"];
		tnoise.Offset = (Vector3)properties["Offset"];
	
		tnoise.DomainWarpEnabled = false;

		for (int y = tdata.StartY; y < tdata.EndY; y++)
		{
			for (int x = 0; x < PrimaryMap.GetLength(0); x++)
			{
				PrimaryMap[x, y] = GetValue(x, y, tnoise, tdata.UseDistortion, tdata.UseMask, tdata.CreatePrimaryMap);
			}
		}

	}
	public override T[,] Evaluate<T>(int port){
		/*noise.Seed = (int)properties["Seed"];
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
		noise.Offset = (Vector3)properties["Offset"];*/

		
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
		Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
		bool CreatePrimaryMap = false;

		if (PrimaryMap.GetLength(0) == 0) {
			PrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
			CreatePrimaryMap = true;	
		} 
		
		int threadCount = OS.GetProcessorCount() - 1;
		if (threadCount < 1)
		{
			threadCount = 1;
		}
		int rowsPerThread = PrimaryMap.GetLength(1) / threadCount;
		
		Thread[] threads = new Thread[threadCount];
		for (int i = 0; i < threadCount; i++)
		{
			int startY = i * rowsPerThread;
			int endY = startY + rowsPerThread;

			if (i == threadCount - 1)
			{
				endY = PrimaryMap.GetLength(1);
			}

			ThreadData data = new ThreadData();
			data.StartY = startY;
			data.EndY = endY;
			data.CreatePrimaryMap = CreatePrimaryMap;
			data.UseDistortion = UseDistortion;
			data.UseMask = UseMask;

			Thread t = new Thread(ThreadGenerate);
			t.Start(data);
			threads[i] = t;
		}

	for (int i = 0; i < threads.Length; i++)
	{
		threads[i].Join();
	}

		return (T[,])(object)PrimaryMap;
	}
    
}
