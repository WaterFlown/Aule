using Godot;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class Export : NodeFunctionality
{
    bool ExportingHeightmap = false;
    bool SelectingHeightmapExportPath = false;
    GodotObject editor = null;
    
    [Export]
    FileDialog heightmapFileDialog = null;
    public override void Initialize()
	{
        properties["ExportPath"] = "";
        
        properties["ExportPressed"] = false;
        properties["SelectPrimaryExportPathPress"] = false;

        editor = (GodotObject)GetNode<Node>("/root/Globals").Get("editor");

	}

    public override void _Process(double delta)
    {
        base._Process(delta);
        if ((bool)properties["SelectPrimaryExportPathPress"])
        {
            properties["SelectPrimaryExportPathPress"] = false;
            if (!ExportingHeightmap && !SelectingHeightmapExportPath) { 
                SelectPrimaryExportPath();
            }
        }
        if ((bool)properties["ExportPressed"])
        {
            properties["ExportPressed"] = false;
            if (!ExportingHeightmap) {

            float[,] heightmap = getFromInput<float>(0);

            if (heightmap.GetLength(0) != 0 && heightmap.GetLength(1) != 0) {
                editor.Call("open_generation_popup");
                GodotObject generationPopup = (GodotObject)editor.Get("generation_popup");
                generationPopup.Call("set_text", "Exporting heightmap...");

                Timer timer = new Timer();
                timer.WaitTime = 0.1f;
                timer.OneShot = true;
                timer.Timeout += () => // Use a timer to allow the generation popup to update before starting the export process. Ugly workaround.
                {
                    ExportHeightmap();
                    timer.QueueFree();
                };
                AddChild(timer);
                timer.Start();
            }

            }
        }
    }

    public override void Deselect()
    {
        SelectingHeightmapExportPath = false;
        heightmapFileDialog.Hide();
    }


    public void SelectPrimaryExportPath()
    {
        SelectingHeightmapExportPath = true;
        heightmapFileDialog.PopupCentered();
    }

    public void HeightmapFileDialogCanceled()
    {
        SelectingHeightmapExportPath = false;
        heightmapFileDialog.Hide();
        
    }

    public void HeightmapFileDialogCloseRequested()
    {
        SelectingHeightmapExportPath = false;
        heightmapFileDialog.Hide();
    }
    public void HeightmapFileDialogConfirmed()
    {
        string path = heightmapFileDialog.GetCurrentPath();
        GD.Print("Selected export path: " + path);
        DirAccess dirAccess = DirAccess.Open(path.GetBaseDir());
        if (dirAccess != null && dirAccess.DirExists(path.GetBaseDir()))
        {
            properties["ExportPath"] = path;
        }
        
        SelectingHeightmapExportPath = false;
        heightmapFileDialog.Hide();
    }

    public void HeightmapFileDialogConfirmed(string path)
    {
        GD.Print("Selected export path: " + path);
        DirAccess dirAccess = DirAccess.Open(path.GetBaseDir());
        if (dirAccess != null && dirAccess.DirExists(path.GetBaseDir()))
        {
            properties["ExportPath"] = path;
        }
        
        SelectingHeightmapExportPath = false;
        heightmapFileDialog.Hide();
    }


    public bool ExportHeightmap()
    {
        ExportingHeightmap = true;
        float[,] heightmap = getFromInput<float>(0);
        if (heightmap.GetLength(0) == 0 || heightmap.GetLength(1) == 0) {
            ExportingHeightmap = false;
            return false;
        }

        editor.Call("open_generation_popup");
        GodotObject generationPopup = (GodotObject)editor.Get("generation_popup");
        generationPopup.Call("set_text", "Exporting heightmap...");

        Image image = Image.CreateEmpty(heightmap.GetLength(0), heightmap.GetLength(1), false, Image.Format.Rgb8);
        for (int x = 0; x < heightmap.GetLength(0); x++)
        {
            for (int y = 0; y < heightmap.GetLength(1); y++)
            {
                float value = (heightmap[x, y] + 1) / 2;
                image.SetPixel(x, y, new Color(value, value, value));
            }
        }

        GD.Print(properties["ExportPath"]);
        if ((string)properties["ExportPath"] == "")
        {
            image.SavePng("user://".PathJoin("exportheightmap.png"));
        }
        else {
            GD.Print("Exporting heightmap to: " + (string)properties["ExportPath"]);
            image.SavePng((string)properties["ExportPath"]);
        }
        ExportingHeightmap = false;
        editor.Call("close_generation_popup");
        return true;
    }

}
