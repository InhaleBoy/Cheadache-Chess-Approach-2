
using Godot;

namespace CHESS2._0test;

public partial class layout : Control
{
    [ExportCategory("SUB ELEMENTS")]
    [Export] public TextureRect BackgroundTexture { get; set; }
    [Export] public Control PlayerCenterContainer { get; set; }
    [Export] public int MagicNumberBorder { get; set; }

    public void RefreshLayout() {
        GD.Print(PlayerCenterContainer.Size);
        BackgroundTexture.Size = PlayerCenterContainer.Size + new Vector2(0,MagicNumberBorder);
        BackgroundTexture.Position = new Vector2(-MagicNumberBorder / 2, -MagicNumberBorder / 2);
    }

    public override void _Input(InputEvent @event) {
        if(Input.IsKeyPressed(Key.Backslash)){
            GD.Print("LayoutRefresh");
            RefreshLayout();
        }
    }
}
