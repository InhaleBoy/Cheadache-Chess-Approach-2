using System.Collections.Generic;
using Godot;

namespace CHESS2._0test;

public partial class ThePlayerCamera : Camera2D {
    // For later so I can make different ways to move the camera
    public List<Vector2> BoardPositions;
    [Export] public float Speed { get; set; }

    public override void _PhysicsProcess(double delta) {
        if (!Input.IsAnythingPressed()) return;
        Vector2 moveVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Position += moveVector * Speed * (float)delta;
    }
}
