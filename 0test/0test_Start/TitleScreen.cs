using System;
using Godot;

namespace CHESS2._0test._0test_Start;

public partial class TitleScreen : Control {
    [Export] public PackedScene GemeMenuScene { get; set; }

    public void StartGame() {
        Error error = GetTree().ChangeSceneToPacked(GemeMenuScene);
        if (error != Error.Ok) throw new Exception(" -- THE MENU SCENE DIED");
    }
}
