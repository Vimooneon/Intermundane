using Godot;

public partial class Global : Node
{
    private Control _fader;
    private AnimationPlayer _faderAnim;

    public Node CurrentScene { get; set; }

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        // Using a negative index counts from the end, so this gets the last child node of `root`.
        CurrentScene = root.GetChild(-1);

        InitFader();
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

    private void InitFader()
    {
        var faderScene = (PackedScene)ResourceLoader.Load("res://Scenes/Fader.tscn");
        _fader = (Control)faderScene.Instantiate();

        AddChild(_fader);
        _faderAnim = (AnimationPlayer)_fader.GetNode("AnimationPlayer");

        _fader.Visible = false; // Hide fader in beginning
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
}