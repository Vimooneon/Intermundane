using Godot;
using System;
using System.Net.Mail;
using System.Numerics;

public partial class DespondencyEnemy : Enemy
{
	const int Speed = 300;
	int direction = 1;

	private Sprite2D sprite2d;

	private RayCast2D ray_right;
	private RayCast2D ray_left;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		ray_right = this.GetNode<RayCast2D>("RayCastRight");
		ray_left = this.GetNode<RayCast2D>("RayCastLeft");
		sprite2d = this.GetNode<Sprite2D>("Sprite2D");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Alive && !Paralyzed) {
            if (ray_right.IsColliding())
			{
				direction = -1;
				sprite2d.FlipH = false;
			}
			if (ray_left.IsColliding())
			{
				direction = 1;
				sprite2d.FlipH = true;
			}
			this.Position = new Godot.Vector2((float)(this.Position.X + direction * Speed * delta), this.Position.Y);
        }else if (Alive && Paralyzed) //knockback mechanic, pushes enemy to opposite direction unless it colides with something
		{
			if (!(ray_right.IsColliding() || ray_left.IsColliding() ))
			{
				this.Position = new Godot.Vector2((float)(this.Position.X - direction * Speed * delta / 2), this.Position.Y);
			}
        }
	}
}
