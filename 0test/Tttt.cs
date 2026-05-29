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
    [Export] public Node PieceHolder { get; set; }
    [Export] public Control BoardHolder { get; set; }
    [Export] public layout LayoutHolder { get; set; }

    public override void _Ready() {
        Boards[] boards = [0,0,0,0];
        AddBoars(boards);
    }

    public override void _Input(InputEvent @event) { // DEBUG INPUT
        if (Input.IsKeyPressed(Key.Slash)) {
            LayoutHolder.RefreshLayout();
        }
    }

    public void AddBoars(params Boards[] boards ) {
        int treshold = (int)Math.Round(Math.Sqrt(boards.Length));

        Node temp = new HBoxContainer();
        for(int i = 0; i < boards.Length; i++){
            if (i % treshold == 0) {
                BoardHolder.AddChild(new HBoxContainer());
                temp = BoardHolder.GetChild(-1);
                BoardHolder.AddChild(new Control{CustomMinimumSize = new Vector2(100,100)});
                BoardHolder.GetChild<Control>(-1).Size = new Vector2(100, 100);
            }

            Board board = GD.Load<PackedScene>(BoardDictionary[boards[i]]).Instantiate<Board>();
            board.BoardSetup(i);
            temp.AddChild(board);
        }
        LayoutHolder.RefreshLayout();
    }
}