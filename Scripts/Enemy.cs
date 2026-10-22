using Godot;
using System;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

public partial class Enemy : Node2D
{
    const int Speed = 300;
	int direction = 1;
    public bool DamageTakeCooldown = false;

    public bool Alive = true;

    [Export] public int Health = 100;

    public bool Paralyzed = false;
    
    public void EnemyTakeDamage(int amount, float paralyzed_time)
	{
		if (DamageTakeCooldown)
		{
			return;
		}
		DamageTakeCooldown = true;

		if (Health > amount)
		{
			Health -= amount;
		}
		else
        {
            
            Health = 0;
            Dissapear();
            return;
		}
        Engine.TimeScale = 0.8;

        
        Paralyzed = true;
        this.GetNode<Godot.Timer>("ParalyzedCooldown").WaitTime = paralyzed_time;
        this.GetNode<Godot.Timer>("ParalyzedCooldown").Start();
		this.GetNode<AnimationPlayer>("AnimationPlayer").Play("TakeDamage");
		
		this.GetNode<Godot.Timer>("DamageCooldown").Start();
	}

    public void DamageCooldownTimerEnd()
    {
        Engine.TimeScale = 1;
        DamageTakeCooldown = false;
    }

    public void ParalyzedCooldownTimerEnd()
    {
        Paralyzed = false;
    }

    public async void Dissapear()
    {
        Alive = false;
        Engine.TimeScale = 0.5;
        AnimationPlayer anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        anim.Play("Dissapear");
        this.Position = new Godot.Vector2((float)(this.Position.X - direction * Speed / 8), this.Position.Y);
        await ToSignal(anim, "animation_finished");
        this.ProcessMode = ProcessModeEnum.Disabled;
        Engine.TimeScale = 1;
    }

}