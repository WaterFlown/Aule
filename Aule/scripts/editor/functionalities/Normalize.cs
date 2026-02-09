using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
public partial class Normalize : NodeFunctionality
{
    float[,] PrimaryMap = {};
    float maxValue = 1f;
    float minValue = -1f;
    public override void Initialize()
	{
		defaultOutputPortID = 1;
	}

    private float GetValue(int x, int y)
    {
        float current = 0f;
        current = Mathf.InverseLerp(minValue, maxValue, PrimaryMap[x, y]);
        return Math.Clamp(current, 0f, 1f);
    }
    public override T[,] Evaluate<T>(int port){
            PrimaryMap = getFromInput<float>(0);


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
                minValue = PrimaryMap.Cast<float>().Min();
                maxValue = PrimaryMap.Cast<float>().Max();  
                for (int i = 0; i < PrimaryMap.GetLength(0); i++) {
                    for (int j = 0; j < PrimaryMap.GetLength(1); j++) {
                        PrimaryMap[i, j] = GetValue(i, j);
                    }	
                }
            }
            return (T[,])(object)PrimaryMap;
        }
}
