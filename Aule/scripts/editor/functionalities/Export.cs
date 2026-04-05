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
                editor.Call("open_generation_popup");
                GodotObject generationPopup = (GodotObject)editor.Get("generation_popup");
                if(properties["ExportPath"].ToString() == "" || properties["ExportPath"].ToString().GetExtension() == "png") {

                    generationPopup.Call("set_text", "Exporting heightmap...");
                    ExportHeightmap(0);
                }
                else if (properties["ExportPath"].ToString().GetExtension() == "glb") {
                    generationPopup.Call("set_text", "Generating heightmap...");
                    ExportHeightmap(1);
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


    public void ExportHeightmap(int exportType)
    {
        GodotObject connection = (GodotObject)editor.Call("get_connection_to", parentID, 0);
        if (connection == null) {
            editor.Call("close_generation_popup");
            ExportingHeightmap = false;
            return;
        }

        ExportingHeightmap = true;

        if(exportType == 0) {

        Timer timer = new Timer();
        timer.WaitTime = 0.1f;
        timer.OneShot = true;
        timer.Connect("timeout", Callable.From(() => {
        float[,] heightmap = getFromInput<float>(0);
        if (heightmap.GetLength(0) == 0 || heightmap.GetLength(1) == 0) {
            editor.Call("close_generation_popup");
            ExportingHeightmap = false;
            return false;
        }

        Image image = Image.CreateEmpty(heightmap.GetLength(0), heightmap.GetLength(1), false, Image.Format.Rgb8);
        for (int x = 0; x < heightmap.GetLength(0); x++)
        {
            for (int y = 0; y < heightmap.GetLength(1); y++)
            {
                float value = heightmap[x, y];
                image.SetPixel(x, y, new Color(value, value, value));
            }
        }

        if ((string)properties["ExportPath"] == "")
        {
            image.SavePng("user://".PathJoin("exported_heightmap.png"));
        }
        else {
            GD.Print("Exporting heightmap to: " + (string)properties["ExportPath"]);
            image.SavePng((string)properties["ExportPath"]);
        }
        ExportingHeightmap = false;
        editor.Call("close_generation_popup");
        return true;
        
        }));
        AddChild(timer);
        timer.Start();
        }
        else if (exportType == 1) {
            Timer timer = new Timer();
            timer.WaitTime = 0.1f;
            timer.OneShot = true;
            timer.Connect("timeout", Callable.From(() => {
            float[,] heightmap = getFromInput<float>(0);
            if (heightmap.GetLength(0) == 0 || heightmap.GetLength(1) == 0) {
                editor.Call("close_generation_popup");
                ExportingHeightmap = false;
                return false;
            }

            editor.Call("run_export_model", ConvertToGDArray(heightmap), properties["ExportPath"].ToString());
            ExportingHeightmap = false;
            return true;
            
            }));
            AddChild(timer);
            timer.Start();
        }
    }

}

