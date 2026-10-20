using Godot;
using System;
using System.Threading.Tasks.Dataflow;

public partial class NextRoom : Area2D
{
    //[Export] private CustomClass customExample;
    [Export] private bool ModeSwap;
    //public bool isInTopview = true;

    public void NextRoomGo(Node2D obj)
    {
        //CharacterBody2D plCharBody = null;
        try
        {
            //plCharBody = obj as CharacterBody2D;
            if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
            {

                var global = GetNode<Global>("/root/Global");
                global.GotoScene("res://Scenes/"+this.Name+".tscn", ModeSwap);

                //if (isInTopview)
                //{
                    //GD.Print("Test Real Real");
                    //isInTopview = false;

                    //Node simultaneousScene = ResourceLoader.Load<PackedScene>("res://scenes/topdownScene.tscn").Instantiate();
                    //await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
                    //GetTree().Root.AddChild(simultaneousScene);
                    //GetTree().Root.GetChild(0, false).Free();

                    
                //}

            }
        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }

    }
}
