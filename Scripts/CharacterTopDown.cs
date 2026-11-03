using Godot;
using System;

public partial class CharacterTopDown : CharacterBody2D
{
    public const float Speed = 400.0f;

    public AnimatedSprite2D sprite;
    public Area2D attack;

    private bool attacking = false;
    public bool choosingAnAttack = false;
	
	public override void _Ready()
	{
        sprite = GetNode<AnimatedSprite2D>("Sprite2D");
        attack = GetNode<Area2D>("Scratch");
	}

    public override void _PhysicsProcess(double delta)
    {
        if (!Visible)
        {
            return;
        }
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

        if (direction.X > 1)
        {
            sprite.Animation = "walk_right";
        }

        if (direction.X < -1)
        {
            sprite.Animation = "walk_left";
        }

        if (direction.Y > 1)
        {
            sprite.Animation = "walk_down";
        }

        if (direction.Y < -1)
        {
            sprite.Animation = "walk_up";
        }

        if (direction.X == 0 && direction.Y == 0)
        {
            sprite.Pause();
        }
        else
        {
            sprite.Play();
        }

        Velocity = direction;
        MoveAndSlide();

        if (Input.IsActionJustPressed("attack"))
        {
            AttackInFront();
        }
    }

    public async void AttackInFront()
    {
        //prevents from attacking again if currently executing an attack
        if (attacking || choosingAnAttack)
            return;

        Godot.Vector2 pos = attack.Position;
        //offsets the attack to the right if player is facing the opposite direction
        
        if (sprite.Animation == "walk_right")
        {
            attack.Position = new Godot.Vector2(pos.X + 320, pos.Y);
        }
        if (sprite.Animation == "walk_up")
        {
            attack.Position = new Godot.Vector2(pos.X + 160, pos.Y-160);
        }
        if (sprite.Animation == "walk_down")
        {
            attack.Position = new Godot.Vector2(pos.X + 160, pos.Y+160);
        }

        attacking = true;
        attack.Visible = true;
        var anim = attack.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Frame = 0;
        anim.Play();
        await ToSignal(anim, "animation_finished");
        attack.Visible = false;
        attacking = false;
        attack.Position = new Godot.Vector2(pos.X, pos.Y);
    }

    public void OnScratchInAttackRange(Node2D obj)
    {
        try
        {
            if (attacking)
            {
                string name = obj.Name;
                if (name.Contains("Enemy") || name.Contains("enemy"))
                {
                    Enemy enmy = (Enemy)obj;
                    enmy.EnemyTakeDamage(34, (float)0.4);
                }
            }

        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }
    }

    public void DamageCooldownTimerEnd()
    {
        var global = GetNode<Global>("/root/Global");
        global.DamageTakeCooldown = false;
        SetCollisionLayerValue(2, true);
        SetCollisionMaskValue(2, true);

    }
}
