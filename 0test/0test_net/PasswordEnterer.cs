using Godot;

namespace CHESS2._0test._0test_net;

public partial class PasswordEnterer : ColorRect {
    [Export] public Button EnterButton { get; set; }
    [Export] public LineEdit Input { get; set; }

    public string LobbyUuid;
}
