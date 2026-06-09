using Godot;
using System;

public partial class HeightmapImport : NodeFunctionality
{
    float[,] PrimaryMap = {};
    public override void Initialize()
	{
        properties["ImportPath"] =  "user://".PathJoin("exported_heightmap.png");
	}
    public override T[,] Evaluate<T>(int port){
                if(((string)properties["ImportPath"]).StartsWith("\"") && ((string)properties["ImportPath"]).EndsWith("\""))
                {
                    properties["ImportPath"] = ((string)properties["ImportPath"]).Trim('"');
                }

            Godot.Image heightmap = new Godot.Image();
            if((string)properties["ImportPath"] != "" && ((string)properties["ImportPath"]).GetExtension() == "png" || ((string)properties["ImportPath"]).GetExtension() == "exr") {

            heightmap.Load(properties["ImportPath"].ToString());
            if(heightmap.IsCompressed()){
                heightmap.Decompress();
            }

            Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
			float[,] newMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
			for (int i = 0; i < newMap.GetLength(0); i++) {
				for (int j = 0; j < newMap.GetLength(1); j++) {
                    if (i >= heightmap.GetWidth() || j >= heightmap.GetHeight()) {
                        newMap[i, j] = 0f;
                        continue;
                    }
					newMap[i, j] = heightmap.GetPixel(i, j).R;
				}
			}
			PrimaryMap = newMap;
            }
            else
            {
                Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
                float[,] CreatedPrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
                for (int i = 0; i < CreatedPrimaryMap.GetLength(0); i++) {
                    for (int j = 0; j < CreatedPrimaryMap.GetLength(1); j++) {
                        CreatedPrimaryMap[i, j] = 0f;
                    }
                }
                PrimaryMap = CreatedPrimaryMap;
            }

            return (T[,])(object)PrimaryMap;
        }
}
