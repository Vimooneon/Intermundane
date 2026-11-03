using Godot;
using System;

public partial class DespondencyEnemyTopDown : Enemy
{
	private Sprite2D sprite2d;
	private bool Detected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite2d = this.GetNode<Sprite2D>("Sprite2D");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Alive && !Paralyzed && Detected)
		{
			var global = GetNode<Global>("/root/Global");
			Vector2 currentAgentPosition = GlobalTransform.Origin;
			Velocity = currentAgentPosition.DirectionTo(global.GetPlayer().Position) * Speed;
			MoveAndSlide();
		}
		else if (Alive && Paralyzed) //knockback mechanic, pushes enemy to opposite direction unless it colides with something
		{
			var global = GetNode<Global>("/root/Global");
			Vector2 currentAgentPosition = GlobalTransform.Origin;
			Velocity = currentAgentPosition.DirectionTo(global.GetPlayer().Position) * -Speed;
			MoveAndSlide();
		}
	}

	public void OnDetected(Node2D obj)
	{
		try
		{
			if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
			{
				Detected = true;
			}
		}
		catch (Exception e)
		{
			GD.Print("Wrong collider " + e);
		}
	}

	public void OnUndetected(Node2D obj)
	{
		try
		{
			if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
			{
				Detected = false;
			}
		}
		catch (Exception e)
		{
			GD.Print("Wrong collider " + e);
		}
	}
}
