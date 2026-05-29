
using Godot;
using Color = Godot.Color;

namespace CHESS2._0test._0test_chess;

public partial class Tile : Area2D
{
    public bool InGameColor;
    public Vector3I Idx;

    public Piece CurrentPiece = null;

    public enum ColoringType {
        Nothing,
        AbleToMove,
        OnTop
    }

    public static Color AbleToMoveLightupColor = new(0, 255, 0);
    public static Color OnTopLightupColor = new(0, 0, 255);

    [Export] public Vector2 Size { get; set; }
    [Export] private ColorRect LightupRect { get; set; }
    [Export] private ColorRect TileColor { get; set; }
    [Export] private Label DebugLabel { get; set; }

    public void LoadValues(Vector3I index, bool color){
        Idx = index;
        InGameColor = color;
        LightupRect = GetNode<ColorRect>("ColorRect");
        Position = new Vector2(index.X,index.Y) * Size + Size/4;

        if (color) TileColor.Color = Colors.Black;

        GD.Print("Tile Constructed");

        DebugLabel.Text = Idx.ToString();
        UpdateDebug();
    }

    public void Lightup(ColoringType coloringType,bool lightup) {
        switch (coloringType) {
            case ColoringType.AbleToMove : {
                LightupRect.Color = AbleToMoveLightupColor;
                break;
            }
            case ColoringType.OnTop : {
                LightupRect.Color = OnTopLightupColor;
                break;
            }
        }
        LightupRect.Visible = coloringType != ColoringType.Nothing && lightup;
    }

    public void _on_body_entered(Node2D body) {
        if (body is Piece piece) piece.FloatingTile = this;
    }

    ///////////////////////////////////////////////////////////////////////////
    
    public void UpdateDebug() {
        DebugLabel.Text = Idx.ToString();
        if (InGameColor) DebugLabel.AddThemeColorOverride("font_color",Colors.White);
    }
}
