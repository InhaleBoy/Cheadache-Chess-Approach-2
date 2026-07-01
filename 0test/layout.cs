
using Godot;

namespace CHESS2._0test;

public partial class layout : Control
{
    [ExportCategory("SUB ELEMENTS")]
    [Export] public TextureRect BackgroundTexture { get; set; }
    [Export] public Control BoardHolder { get; set; }
    [Export] public float MagicNumberBorder { get; set; }
    

    public void RefreshLayout() {
        BackgroundTexture.Size = BoardHolder.Size + new Vector2(0,MagicNumberBorder);
        BoardHolder.Position = new Vector2(MagicNumberBorder / 2, MagicNumberBorder / 2);
        
        GetTree().CallGroup("PIECE","UpdatePosition");
    }

    public override void _Input(InputEvent @event) {
        if(Input.IsKeyPressed(Key.Backslash)){
            GD.Print("LayoutRefresh");
            RefreshLayout();
        }
    }
}
