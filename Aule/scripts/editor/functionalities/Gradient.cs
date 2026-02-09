using Godot;
using System;

public partial class Gradient : NodeFunctionality
{
    float[,] PrimaryMap = {};
    [ExportGroup("Linear")]
    [Export]
    Curve LinearP = new Curve();
    [ExportGroup("Smooth")]
    [Export]
    Curve Smooth = new Curve();

    public override void Initialize()
    {
        properties["GradientType"] = 0;
    }

    public override T[,] Evaluate<T>(int port){
        Curve curveX = LinearP;
        Curve curveY = LinearP;
        switch ((int)properties["GradientType"]) {
            case 0:
            //Linear Pyramid
                break;
            case 1:
                curveX = Smooth;
                curveY = Smooth;
                break;
            default:
                break;
        }

		Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
		PrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
        Vector2 size = new Vector2(PrimaryMap.GetLength(0), PrimaryMap.GetLength(1));

		for (int i = 0; i < size.X; i++) {
			for (int j = 0; j < size.Y; j++) {
				PrimaryMap[i, j] = (curveX.Sample((float)i / size.X) * curveY.Sample((float)j / size.Y));
			}
		}
		return (T[,])(object)PrimaryMap;
	}

}
