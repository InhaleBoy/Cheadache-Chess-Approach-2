using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_Start;

public partial class TesterMainMenu : Control {

    [Export] public PackedScene GameScene { get; set; }
    [Export] public Label ColorLabel { get; set; }
    [Export] public Button PickWhiteButton { get; set; }
    [Export] public Button PickBlackButton { get; set; }

    public bool WoB;
    
    public override void _Ready() {
        ChangeColorWhite();
        UpdateColorLabel();
    }
    
    public void ChangeColorWhite() {
        PickWhiteButton.Disabled = true;
        PickBlackButton.Disabled = false;
        WoB = true;
        UpdateColorLabel();
    }
    
    public void ChangeColorBlack() {
        PickBlackButton.Disabled = true;
        PickWhiteButton.Disabled = false;
        WoB = false;
        UpdateColorLabel();
    }
    
    public void UpdateColorLabel() {
        string color = WoB ? "White" : "Black";
        ColorLabel.Text = $"Yo Color : {color}";
    }
    
    public void StartGame() {
        
        Globe.YourColor = WoB;
        Globe.IsThereAGamePresent = true;
        
        // ACHTUNG! Hardcoded Boards
        Boards[] boards = [
            Boards.RrgularChess,
            Boards.RrgularChess,
        ];
        
        Tttt gamesc = GameScene.Instantiate<Tttt>();
        
        // There will be more Networking Stuff here
        // For now it is single player tbh
        
        gamesc.Init(boards);
        GetTree().Root.AddChild(gamesc);
        QueueFree();
        
    }
}
