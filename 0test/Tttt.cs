using System;
using System.Linq;
using Godot;
using CHESS2._0test._0test_chess;
using static CHESS2._0test.Information;
// using CHESS2._0test._0test_net;

namespace CHESS2._0test;

public partial class Tttt : Node2D
{
    [ExportCategory(" MAGIC ")]
    [Export] public Camera2D Camera { get; set; }
    // [Export] public Node PieceHolder { get; set; }
    [Export] public Control BoardHolder { get; set; }
    [Export] public layout LayoutHolder { get; set; }
    [Export] public Label InformationLabel { get; set; }

    public Boards[] Boards;

    public void Init(Boards[] boards){
        Boards = boards;
    }
    
    public override void _Ready() {
        AddBoars(Boards);
        
        // Updates not as important to be seperated
        CallDeferred(MethodName.DefferableUpdateAnDRefresh);
        UpdateInformation();
        
    }

    public void AddBoars(params Boards[] boards ) {
        int treshold = (int)Math.Ceiling(Math.Sqrt(boards.Length));

        Node temp = new HBoxContainer();
        for(int i = 0; i < boards.Length; i++){
            if (i % treshold == 0) {
                BoardHolder.AddChild(new HBoxContainer());
                temp = BoardHolder.GetChild(-1);
                BoardHolder.AddChild(new Control{CustomMinimumSize = new Vector2(100,100)});
                BoardHolder.GetChild<Control>(-1).Size = new Vector2(100, 100);
            }

            Board board = GD.Load<PackedScene>(BoardDictionary[boards[i]]).Instantiate<Board>();
            temp.AddChild(board);
            board.BoardSetup(i,boards[i]);
        }
    }
    
    public void EndTurn() { // Change here so those are calls to nodes in gtoup maybe ?

        var boards = GetTree().GetNodesInGroup("BOARD");

        try {
            Board eBoard = (Board)boards.First(x => x is Board b && !b.HasMove());
            GD.Print("You did not touch the boards : ", eBoard.BoardName);
            return; 
        } catch (Exception e) { // So BIZZARE that Exception is what i want
            GD.Print("Empty sounds nice !" );
        }


        string mov = "Da Moves : \n";
        foreach (var node in boards) {
            if (node is not Board b) return;
            mov += $"{b.CommitMoves()} \n";
        }
        
        
        // Finalize moves and save them here
        GD.Print(mov);
        // send END TURN PACKET for check with srv ??? Later ╥﹏╥
        

        Globe.TwiceTurn++;
        Globe.ColorToMoveNext = !Globe.ColorToMoveNext;
        UpdateInformation();
        // Might add more animations
        
        // DELETE DEBUG
        AdminDebugUpdate();
    }
    
    // ------------------------------ Methods

    [Export] public Label AdminDebugLabel { get; set; }
    public void ChangeMyColorDebugAdmin(){
        GD.Print("Network Administrator is one step behind GOD ~ chestnut horder TinfoilHatMgrTG " );
        Globe.YourColor = !Globe.YourColor;
        AdminDebugUpdate();
    }
    public void AdminDebugUpdate(){
        AdminDebugLabel.Text = $"{Globe.IsThereAGamePresent}\n{Globe.ColorToMoveNext}\n{Globe.YourColor}\n" +
                               $"{Globe.TwiceTurn}\n{Globe.CanYouMove}\n";
    }
    
    public override void _Input(InputEvent @event) { // DEBUG INPUT
        if (Input.IsKeyPressed(Key.L)) {
            GD.Print("-- Refresh Layout");
            LayoutHolder.RefreshLayout();
        }
    }
    
    public void DefferableUpdateAnDRefresh() {
        LayoutHolder.RefreshLayout();
    }
    
    public void UpdateInformation() {
        string color = Globe.ColorToMoveNext ? "White" : "Black";
        RenderingServer.SetDefaultClearColor(Globe.ColorToMoveNext ? Colors.White : Colors.Black);
        InformationLabel.AddThemeColorOverride("font_color", Globe.ColorToMoveNext ? Colors.Black : Colors.White);
        InformationLabel.Text = $"TURN {Math.Ceiling(Globe.TwiceTurn/2)} \n {color}'s move";
    }
}