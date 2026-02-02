using Godot;
using System;

public partial class Gain : NodeFunctionality
{
    float[,] PrimaryMap = {};
	float[,] MaskMap = {};
    public override void Initialize()
	{
		properties["Gain"] = 1f;
		properties["Bias"] = 1f;

	}
	private float GetValue(int x, int y, bool UseMask)
	{
		float current = 0f;
        if (!PrimaryMap.GetLength(0).Equals(0))
        current = PrimaryMap[x, y];
		bool negative = false;
		if (current < 0f)
		{
			current = Math.Abs(current);
			negative = true;
		}

		if (UseMask)
		{
            float currentGain = (float)properties["Gain"] * MaskMap[x, y];
			current = (float)Math.Pow(current, currentGain);
		}
        else
        {
            current = (float)Math.Pow(current, (float)properties["Gain"]);
        }
		if (negative)	
		{
			current = -current;
		}

		return Math.Clamp(current * (float)properties["Bias"], -1f, 1f);
	}

public override T[,] Evaluate<T>(int port){
		PrimaryMap = getFromInput<float>(0);
		MaskMap = getFromInput<float>(1);

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
					CreatedPrimaryMap[i, j] = 0f;
				}
			}
			PrimaryMap = CreatedPrimaryMap;
		} 
		else {
			for (int i = 0; i < PrimaryMap.GetLength(0); i++) {
				for (int j = 0; j < PrimaryMap.GetLength(1); j++) {
					PrimaryMap[i, j] = GetValue(i, j, UseMask);
				}	
			}
		}
		return (T[,])(object)PrimaryMap;
	}

}
