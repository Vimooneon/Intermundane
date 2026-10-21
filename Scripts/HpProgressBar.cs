using Godot;
using System;

public partial class HpProgressBar : TextureProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DamageCooldownTimerEnd()
	{
		Engine.TimeScale = 1;
        var global = GetNode<Global>("/root/Global");
        global.DamageTakeCooldown = false;
    }
}
