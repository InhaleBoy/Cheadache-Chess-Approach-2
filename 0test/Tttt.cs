using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Godot;
using CHESS2._0test._0test_chess;
using static CHESS2._0test.Information;
using CHESS2._0test._0test_net;

namespace CHESS2._0test;

public partial class Tttt : Node2D
{
    [ExportCategory(" MAGIC ")]
    [Export] public Camera2D Camera { get; set; }
    [Export] public Node PieceHolder { get; set; }
    [Export] public Control BoardHolder { get; set; }
    [Export] public layout LayoutHolder { get; set; }

    public Server RandomAssServer;
    public Client[] RandomAssClient = new Client[10 ];

    public override void _Ready() {
        Boards[] boards = [0,0,0,0];
        AddBoars(boards);
    }

    public override void _Input(InputEvent @event) {
        if (Input.IsKeyPressed(Key.M)) {
            AddPiece(Pieces.Piece, new Vector3I(0, 0, 2));
        }
        if (Input.IsKeyPressed(Key.Slash)) {
            LayoutHolder.RefreshLayout();
        }

        if (Input.IsKeyPressed(Key.H)) {
            RandomAssServer = new Server(9999);
        }
        if (Input.IsKeyPressed(Key.J)) {
            Client client = new Client("127.0.0.1",9999);
            RandomAssClient.Append(client);
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

    public void AddPiece(Pieces type, Vector3I idx) {
        Tile tile = (Tile)GetTree().GetNodesInGroup("TILE").Where(x => ((Tile)x).Idx == idx).First();
        PieceAbstract piAbstract = PiecePathDictionary[type];
        Piece piece = GD.Load<PackedScene>(piAbstract.Scene).Instantiate<Piece>();   
        PieceHolder.AddChild(piece);
        piece.LoadValues(tile, true, type, piAbstract);
        tile.CurrentPiece = piece;
    }
}