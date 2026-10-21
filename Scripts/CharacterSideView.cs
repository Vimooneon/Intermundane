using Godot;
using Microsoft.VisualBasic;
using System;
using System.Runtime.CompilerServices;

public partial class CharacterSideView : CharacterBody2D
{

    public const float Speed = 100.0f;
    public const float MaxSpeed = 300.0f;
    public const float JumpAccel = 1.2f;
    public const float SpeedIncrease = 5.0f;
    public const float JumpVelocity = -700.0f;
    public const float LowJumpMult = 0.5f; // Cuts velocity to 50% if tapped
    public const float CameraAheadMult = 1.5f;

    private AnimatedSprite2D sprite2d;
    public Area2D attack;
    private Camera2D camera;

    private bool attacking = false;


    //var CoyoteLength = 6;  // How many in-air frames to allow jumping
    Boolean CoyoteTime = false;  // Track whether we're in coyote time or not
    Boolean LastFloor = false;  // Last frame's on-floor state
    Boolean Jumping = false;
 
    public override void _Ready()
    {
        sprite2d = GetNode<AnimatedSprite2D>("Sprite2D");
        attack = GetNode<Area2D>("Scratch");

        camera = GetTree().Root.GetChild(1, false).GetNode<Camera2D>("Camera2D");
        camera.GlobalPosition = this.GlobalPosition;
    }
 
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();



    public override void _PhysicsProcess(double delta)
    {
        if (!Visible)
        {
            return;
        }
        Vector2 velocity = Velocity;


        // Animations (TODO)
        //if (Math.Abs(velocity.X) > 1)
        //    sprite2d.Animation = "running";
        //else
        //    sprite2d.Animation = "default";

        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
            //sprite2d.Animation = "jumping";
        }
        else
        {
            Jumping = false;
        }

        if (!IsOnFloor() && !Jumping)
        {
            CoyoteTime = true;
            GetNode<Godot.Timer>("CoyoteTimer").Start();
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && (IsOnFloor() || CoyoteTime))
        {
            velocity.Y = JumpVelocity;
            Jumping = true;
            CoyoteTime = false;
        }
        //Less jump height if button tapped instead of held
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
        {
            velocity.Y *= LowJumpMult;
        }

        

        // Get the input direction and handle the movement/deceleration.
        float direction = Input.GetAxis("move_left", "move_right");
        if (direction != 0)
        {
            if (Jumping && direction * velocity.X < Speed * JumpAccel) //starting speed when jumping
            {
                velocity.X += direction * Speed * JumpAccel;
            }
            else if (direction * velocity.X < Speed) //starting speed when walking
            {
                velocity.X += direction * Speed;
            }
            else if (direction * velocity.X < MaxSpeed) //speed is increased until reached max speed
            {
                velocity.X += direction * SpeedIncrease;
            }
            // Flip the sprite based on the direction.
            bool isLeft = velocity.X < 0;
            sprite2d.FlipH = !isLeft;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, 12); //slides a bit when input is no longer held
        }

        Velocity = velocity;
        MoveAndSlide();

        
        

        //updates camera position to character position + moves it a bit forward (player can see a bit ahead in direction they move)
        camera.GlobalPosition = this.GlobalPosition;
        Godot.Vector2 vc = new Godot.Vector2(direction * Speed * CameraAheadMult, 0);
        camera.Position += vc;

        if (Input.IsActionJustPressed("attack"))
        {
            AttackInFront();
        }
    }

    public void CoyteTimerEnd()
    {
        CoyoteTime = false;
    }

    public async void AttackInFront()
    {
        if (attacking)
            return;

        Godot.Vector2 pos = attack.Position;
        if (sprite2d.FlipH)
        {
            attack.Position = new Godot.Vector2(pos.X+320, pos.Y);
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

                    //var global = GetNode<Global>("/root/Global");
                    //global.GotoScene("res://Scenes/"+this.Name+".tscn", ModeSwap);
                    GD.Print(obj.Name);
                    Enemy enmy = (Enemy)obj;
                    enmy.EnemyTakeDamage(34);
                }
            }
            
        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }
    }
    
}