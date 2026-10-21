using Godot;
using System;

public partial class LockGreen : StaticBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var global = GetNode<Global>("/root/Global");

		global.RemoveOnMap.TryAdd(this.Name, false);
		if (global.RemoveOnMap[this.Name])
		{
			this.Visible = false;
			this.ProcessMode = ProcessModeEnum.Disabled;
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnUnlock(Node2D obj)
	{
		//CharacterBody2D plCharBody = null;
		try
		{
			//plCharBody = obj as CharacterBody2D;
			if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
			{

				var global = GetNode<Global>("/root/Global");
				global.Inventory.TryAdd("KeyGreen", false);
				if (global.Inventory["KeyGreen"])
                {
					UnlockDoor();
                }
			}
		}
		catch (Exception e)
		{
			GD.Print("Wrong collider " + e);
		}

	}
	
	private async void UnlockDoor()
	{
        
		AnimationPlayer anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
		anim.Play("DoorUnlock");
        // Wait for fade-out animation to finish
        await ToSignal(anim, "animation_finished");

		var global = GetNode<Global>("/root/Global");
		global.RemoveOnMap[this.Name] = true;
		this.ProcessMode = ProcessModeEnum.Disabled;
    }
}
