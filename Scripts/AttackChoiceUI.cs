using Godot;
using System;

public partial class AttackChoiceUI : CanvasLayer
{
	private TextureProgressBar timebar;
	public int maxTimeLeft = 400;
	public int timeLeft = 400;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        timebar = GetNode<TextureProgressBar>("ThinkFast");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (timeLeft > 0)
        {
			timeLeft -= (int)(200 * delta / Engine.TimeScale);
			timebar.Value = timeLeft;
        }
        else
        {
			HideUI();
        }
    }

	public void HideUI()
	{
		var global = GetNode<Global>("/root/Global");
		global.resetAttack();
	}


	public void ShowUp()
	{
		timeLeft = maxTimeLeft;
		ProcessMode = ProcessModeEnum.Inherit;
		this.Visible = true;
	}
	
	public void HealPlayer()
	{
		var global = GetNode<Global>("/root/Global");
		global.PlayerHeal(20);
		global.resetAttack();
		
	}
}
