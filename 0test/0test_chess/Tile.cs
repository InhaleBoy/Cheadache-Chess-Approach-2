
using Godot;
using Color = Godot.Color;

namespace CHESS2._0test._0test_chess;

public partial class Tile : Area2D {
    
    [Export] public Vector2 Size { get; set; }
    [Export] private ColorRect LightupRect { get; set; }
    [Export] private ColorRect TileColorRect { get; set; }
    [Export] private Label DebugLabel { get; set; }
    
    public Vector3I Idx;
    public Color TileColor;
    
    public Piece CurrentPiece = null;

    public void _on_body_entered(Node2D body) {
        if (body is Piece piece) piece.FloatingTile = this;
    }

    public void Init(Vector3I index, Color color){
        Idx = index;
        TileColor = color;
        Position = new Vector2(index.X,index.Y) * Size + Size/4;
        TileColorRect.Color = color;

        // GD.Print("Tile Constructed"); // DEBUG

        UpdateDebug();
        SetLightup(Colors.Transparent);
    }
    

    // -------------------------------------- Methods
    
    public void SetLightup(Color color) {
        LightupRect.Color = color;
    }
    
    public void UpdateDebug() {
        DebugLabel.Text = Idx.ToString();
        if (TileColor == Colors.Black) DebugLabel.AddThemeColorOverride("font_color",Colors.White);
    }
}
