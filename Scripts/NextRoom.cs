using Godot;
using System;
using System.Threading.Tasks.Dataflow;

public partial class NextRoom : Area2D
{
    [Export] private bool ModeSwap;

    public void NextRoomGo(Node2D obj)
    {
        try
        {
            if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
            {

                var global = GetNode<Global>("/root/Global");
                global.GotoScene("res://Scenes/"+this.Name+".tscn", ModeSwap);
            }
        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }

    }
}
