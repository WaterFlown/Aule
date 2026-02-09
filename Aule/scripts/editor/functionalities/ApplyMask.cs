using Godot;
using System;

public partial class ApplyMask : NodeFunctionality
{
    float[,] PrimaryMap = {};
	float[,] MaskMap = {};
    public override void Initialize()
	{

	}

    private float GetValue(int x, int y, bool UseMask)
	{
		float current = 0f;
        if (!PrimaryMap.GetLength(0).Equals(0))
        current = PrimaryMap[x, y];
		
		if (UseMask)
		{
            
			//current *= Mathf.InverseLerp(-1f, 1f, MaskMap[x, y]);
            current *= MaskMap[x, y];
		}

		return Math.Clamp(current, 0f, 1f);
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
