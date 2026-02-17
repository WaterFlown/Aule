using Godot;
using System;

public partial class Terrace : NodeFunctionality
{
    float[,] PrimaryMap = {};
	float[,] MaskMap = {};

    float terraceWidth = 1f;
    float terraces = 1f;
	float steepness = 10f;
    public override void Initialize()
	{
		properties["Terraces"] = 1;
        properties["TerraceWidth"] = 0.2f;
		properties["Steepness"] = 1f;

        terraceWidth = (float)properties["TerraceWidth"];
        terraces = (float)properties["Terraces"];
	}
	private float GetValue(int x, int y, bool UseMask)
	{
		float current = 0f;
        if (!PrimaryMap.GetLength(0).Equals(0))
        current = PrimaryMap[x, y] * terraces;
		
		float output = (float)(Mathf.Round(current) + 0.5 * Mathf.Pow(2*(current - Mathf.Round(current)), 2 * steepness - 1)); //Formula for terracing
		if (UseMask)
		{
			output = Mathf.Lerp(current, output, MaskMap[x, y]) / terraces; //if using mask, interpolate between original and terraced by mask.
		}
		else
		{
			output /= terraces; //Dividing because current is primarymap * terraces ig
		}

		return Math.Clamp(output, 0f, 1f);

    }

	private float round(float current)
	{
		float k = (float)Math.Floor(current / terraceWidth);
        float f = (current - k*terraceWidth) / terraceWidth;
        float s = Mathf.Min(2 * f, 1f);
        
        return (k+s) * terraceWidth;
	}

public override T[,] Evaluate<T>(int port){
		PrimaryMap = getFromInput<float>(0);
		MaskMap = getFromInput<float>(1);

        terraces = (float)properties["Terraces"];
        terraceWidth = (float)properties["TerraceWidth"];
		steepness = (float)properties["Steepness"];

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
