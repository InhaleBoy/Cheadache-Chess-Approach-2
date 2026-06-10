using System;
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
        string error_list = string.Empty;

        foreach (var node in GetTree().GetNodesInGroup("BOARD")) {
            if (node is not Board board) continue;
            bool hasMove = board.HasMove();
            if (hasMove) continue;
            error_list += $"{board.BoardIdx}. {board.BoardName} ";
        }
        
        // Do Something with this so you cant do it
        if (error_list != string.Empty) {
            GD.Print("You did not touch the boards : ",error_list );
            return;
        }

        // send END TURN PACKET ???

        Globe.TwiceTurn++;
        Globe.ColorToMoveNext = !Globe.ColorToMoveNext;
        UpdateInformation();
        // Might add more animations
    }
    
    // ------------------------------ Methods
    
    
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