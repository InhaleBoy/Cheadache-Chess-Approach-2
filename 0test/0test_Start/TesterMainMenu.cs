using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_Start;

public partial class TesterMainMenu : Control {

    [Export] public PackedScene GameScene { get; set; }

    public bool WoB;
    [Export] public Label ColorLabel { get; set; }
    [Export] public Button PickWhiteButton { get; set; }
    [Export] public Button PickBlackButton { get; set; }

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
        
        // I need to keep ingformtion in Globe for now I guess
        // BETTER ! Im nt gonna give any info at all as of now XD
        
        // Hardcoded Boards
        Boards[] boards = [
            Boards.RrgularChess,
            Boards.RrgularChess,
        ];
        
        Tttt gamesc = GameScene.Instantiate<Tttt>();
        // There will be more Net Stuff to combine theese
        // For now it is single player tho
        gamesc.Init(boards);
        GetTree().Root.AddChild(gamesc);
        
        
        Globe.IsThereAGamePresent = true;
        Globe.YourColor = true; // i make it true for safety 
        // Globe.YourColor = WoB;
        
        
        QueueFree();
    }
}
