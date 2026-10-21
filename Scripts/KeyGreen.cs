using Godot;
using System;

public partial class KeyGreen : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        var global = GetNode<Global>("/root/Global");
        global.Inventory.TryAdd("KeyGreen", false);
        if (global.Inventory["KeyGreen"])
        {
            this.Visible = false;
            this.ProcessMode = ProcessModeEnum.Disabled;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void KeyCollected(Node2D obj)
    {
        try
        {
            if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
            {

                var global = GetNode<Global>("/root/Global");
                global.Inventory["KeyGreen"] = true;
                AnimationPlayer anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
                anim.Play("KeyPickup");
            }
        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }

    }
}
