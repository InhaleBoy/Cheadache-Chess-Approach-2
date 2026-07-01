
using Godot;
using Color = Godot.Color;

namespace CHESS2._0test._0test_chess;

[Tool]
public partial class Tile : Area2D {
    
    [Export] public Vector2 Size { get; set; }
    [Export] private ColorRect LightupRect { get; set; }
    [Export] private ColorRect TileColorRect { get; set; }
    [Export] private Label DebugLabel { get; set; }
    
    private Vector3I _idx;
    [Export] public Vector3I Idx {
        get => _idx;
        set {
            _idx = value;
            UpdateDebug();
        }
    }
    
    public Piece CurrentPiece = null;

    public override void _Ready() {        
        UpdateDebug();
        SetLightup(Colors.Transparent);
    }


    public void _on_body_entered(Node2D body) {
        if (body is Piece piece) piece.FloatingTile = this;
    }

    // -------------------------------------- Methods
    
    public void SetLightup(Color color) {
        LightupRect.Color = color;
    }
    
    public void UpdateDebug() {
        DebugLabel.Text = Idx.ToString();
        // if (TileColor == Colors.Black) DebugLabel.AddThemeColorOverride("font_color",Colors.White);
    }
}
