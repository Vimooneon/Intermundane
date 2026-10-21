using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class Global : Node
{
    private CanvasLayer _fader;
    private AnimationPlayer _faderAnim;
    private TextureProgressBar hpbar;

    public bool DamageTakeCooldown = false;

    public int PlayerHP = 100;

    public Node CurrentScene { get; set; }

    public Dictionary<string, bool> Inventory = new Dictionary<string, bool>();

    public Dictionary<string, bool> RemoveOnMap = new Dictionary<string, bool>();

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        // Using a negative index counts from the end, so this gets the last child node of `root`.
        CurrentScene = root.GetChild(-1);

        InitFader();
        InitHP();
    }

    private void InitFader()
    {
        var faderScene = (PackedScene)ResourceLoader.Load("res://Scenes/Fader.tscn");
        _fader = (CanvasLayer)faderScene.Instantiate();

        AddChild(_fader);
        _faderAnim = _fader.GetNode<AnimationPlayer>("AnimationPlayer");

        _fader.Visible = false; // Hide fader in beginning
    }

    private void InitHP()
    {
        var hpScene = (PackedScene)ResourceLoader.Load("res://Scenes/PapercutHP.tscn");
        var scene = (CanvasLayer)hpScene.Instantiate();
        AddChild(scene);
        hpbar = scene.GetNode<TextureProgressBar>("hpProgressBar");
    }

    public void GotoScene(string path, bool fade) //Global function of changing scenes called from other functions, has FadeIn on new scene option
    {
        if (fade)
        {
            GotoSceneFader(path);
        }
        else
        {
            CallDeferred(MethodName.DeferredGotoScene, path);
        }
    }

    //The current scene may still be active so the scene deletion is deferred
    public void DeferredGotoScene(string path)
    {
        var name = CurrentScene.Name;

        CurrentScene.Free();
        // Load, Instance, Add new scene.
        var nextScene = GD.Load<PackedScene>(path);
        CurrentScene = nextScene.Instantiate();
        GetTree().Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        //GetTree().CurrentScene = CurrentScene;

        //Set player location to level entrance if possible (for either side view or topdown view)
        CharacterBody2D player = CurrentScene.GetNodeOrNull<CharacterBody2D>("Player");
        CharacterBody2D playerTD = CurrentScene.GetNodeOrNull<CharacterBody2D>("PlayerTopDown");
        Marker2D Pos = CurrentScene.GetNodeOrNull<Marker2D>(name + "Entrance");

        if (GodotObject.IsInstanceValid(player) && GodotObject.IsInstanceValid(Pos))
        {
            player.GlobalPosition = Pos.GlobalPosition;
        }
        else if (GodotObject.IsInstanceValid(playerTD) && GodotObject.IsInstanceValid(Pos))
        {
            playerTD.GlobalPosition = Pos.GlobalPosition;
        }
    }

    //Adds FadeOut and FadeIn effects upon switching scenes
    private async void GotoSceneFader(string path)
    {
        // Fade out current screen
        _fader.Visible = true;
        _faderAnim.Play("FadeOut");
        // Wait for fade-out animation to finish
        await ToSignal(_faderAnim, "animation_finished");

        // Change scene
        CallDeferred(MethodName.DeferredGotoScene, path);

        _faderAnim.Play("FadeIn");
        await ToSignal(_faderAnim, "animation_finished");
        _fader.Visible = false; // Hide fader after fade in
    }

    public void PlayerTakeDamage(int amount)
    {
        if (DamageTakeCooldown)
        {
            return;
        }
        DamageTakeCooldown = true;

        if (PlayerHP > amount)
        {
            PlayerHP -= amount;
        }
        else
        {
            PlayerHP = 0;
            RestartGame();
            
        }
        hpbar.Value = PlayerHP;
        Engine.TimeScale = 0.8;

        hpbar.GetNode<AnimationPlayer>("AnimationPlayer").Play("TakeDamage");

        CharacterBody2D player = CurrentScene.GetNodeOrNull<CharacterBody2D>("Player");
        CharacterBody2D playerTD = CurrentScene.GetNodeOrNull<CharacterBody2D>("PlayerTopDown");
        if (GodotObject.IsInstanceValid(player))
        {
            player.GetNode<AnimationPlayer>("AnimationPlayer").Play("TakeDamage");
            player.Velocity = new Godot.Vector2(-player.Velocity.X/2, -player.Velocity.Y/2);
        }
        else if (GodotObject.IsInstanceValid(playerTD))
        {
            playerTD.GetNode<AnimationPlayer>("AnimationPlayer").Play("TakeDamage");
            player.Velocity = new Godot.Vector2(-player.Velocity.X, -player.Velocity.Y);
        }
        hpbar.GetNode<Godot.Timer>("DamageCooldown").Start();
    }

    public async void RestartGame()
    {
        var gameOver = _fader.GetNode<Label>("Label");
        gameOver.Visible = true;
        GotoScene("res://Scenes/Side_level_1.tscn", true);
        await ToSignal(_faderAnim, "animation_finished");
        gameOver.Visible = false;
        PlayerHP = 100;
        hpbar.Value = PlayerHP;
    }
}