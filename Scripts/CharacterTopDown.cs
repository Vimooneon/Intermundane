using Godot;
using System;

public partial class CharacterTopDown : CharacterBody2D
{
    public const float Speed = 400.0f;

    public AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
        sprite = GetNode<AnimatedSprite2D>("Sprite2D");
	}

    public override void _PhysicsProcess(double delta)
    {

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (direction != Vector2.Zero)
        {
            direction.X = direction.X * Speed;
            direction.Y = direction.Y * Speed;
        }
        else
        {
            direction.X = Mathf.MoveToward(Velocity.X, 0, Speed / 10);
            direction.Y = Mathf.MoveToward(Velocity.Y, 0, Speed / 10);
        }
        //TODO: add animations
        if (direction.X > 1)
        {
            //sprite.Animation = "right";
        }

        if (direction.X < -1)
        {
            //sprite.Animation = "left";
        }

        if (direction.Y > 1)
        {
            //sprite.Animation = "down";
        }

        if (direction.Y < -1)
        {
            //sprite.Animation = "up";
        }

        Velocity = direction;
        MoveAndSlide();

        //TODO: remove when proper animations are added
        bool isLeft = direction.X < -1;
        sprite.FlipH = !isLeft;
    }
}
