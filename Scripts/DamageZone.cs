using Godot;
using System;

public partial class DamageZone : Area2D
{
    [Export] private int DamageAmount = 1;
    [Export] private bool EnemyAttack = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DamageZoneEntered(Node2D obj)
    {
        //Check if the damage is done by enemy
        if (EnemyAttack)
        {   
            //check if enemy is currently alive and not paralyzed
            var enmy = (Enemy)GetParent();
            if (enmy.Paralyzed || !enmy.Alive)
            {
                return;
            }
        }
        try
        {
            //if the area collides with player deals attack damage
            if (obj.Name == "Player" || obj.Name == "PlayerTopDown")
            {
				var global = GetNode<Global>("/root/Global");
				global.PlayerTakeDamage(DamageAmount);
            }
        }
        catch (Exception e)
        {
            GD.Print("Wrong collider " + e);
        }

    }
}
