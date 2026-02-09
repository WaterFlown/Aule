using Godot;
using System;

public partial class Combiner : NodeFunctionality
{
	float[,] PrimaryMap = {};

    public override void Initialize()
    {
        defaultOutputPortID = 4;
    }
	
	public override T[,] Evaluate<T>(int port){
		float[,] Map0 = getFromInput<float>(0);
        float[,] Map1 = getFromInput<float>(1);
        float[,] Map2 = getFromInput<float>(2);
        float[,] Map3 = getFromInput<float>(3);

        Vector2I Size0 = new Vector2I(Map0.GetLength(0), Map0.GetLength(1));
        Vector2I Size1 = new Vector2I(Map1.GetLength(0), Map1.GetLength(1));
        Vector2I Size2 = new Vector2I(Map2.GetLength(0), Map2.GetLength(1));
        Vector2I Size3 = new Vector2I(Map3.GetLength(0), Map3.GetLength(1));

		Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
		float[,] CreatedPrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
		for (int i = 0; i < CreatedPrimaryMap.GetLength(0); i++) {
			for (int j = 0; j < CreatedPrimaryMap.GetLength(1); j++) {
				CreatedPrimaryMap[i, j] = 0;
                if (Size0.X > i && Size0.Y > j) {
                    CreatedPrimaryMap[i, j] = Math.Clamp(CreatedPrimaryMap[i, j] + Map0[i, j], 0f, 1f);
                }
                if (Size1.X > i && Size1.Y > j) {
                    CreatedPrimaryMap[i, j] = Math.Clamp(CreatedPrimaryMap[i, j] + Map1[i, j], 0f, 1f);

                }
                if (Size2.X > i && Size2.Y > j) {
                    CreatedPrimaryMap[i, j]  = Math.Clamp(CreatedPrimaryMap[i, j] + Map2[i, j], 0f, 1f);
                }
                if (Size3.X > i && Size3.Y > j) {
                    CreatedPrimaryMap[i, j] = Math.Clamp(CreatedPrimaryMap[i, j] + Map3[i, j], 0f, 1f);
                }
			}
		}
		PrimaryMap = CreatedPrimaryMap;
		
		return (T[,])(object)PrimaryMap;
	}
    
}
