using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public partial class Board : Control, IBoard
{
    [Export] private PackedScene _tile;
    [Export] public Control TilesAndPieces;
    public int BoardIdx;

    public void BoardSetup(int boardIdx) {
        BoardIdx = boardIdx;
        CreateTiles(8, 8);
    }
    
    public void CreateTiles(int horizontal, int vertical) {
        bool wob = true;
        Vector3I index = new Vector3I(0,0,BoardIdx);
        
        for(var i = 0; i < horizontal * vertical; i++) {
            wob = i % horizontal == 0 ? wob : !wob; // move further as argument ; delete instantiation
            if (index.X == horizontal) index = new Vector3I(0,index.Y+1,BoardIdx);
            AddTile(index,wob);
            index += new Vector3I(1,0,0);
        }

        Tile tempTile = _tile.Instantiate<Tile>();
        Vector2 size = tempTile.Size;
        tempTile.QueueFree();
        CustomMinimumSize = new Vector2(size.X * (horizontal + 1), size.Y * vertical);
    }
    
    public void CreatePieces() {
        
    }

    public void AddTile(Vector3I index, bool wob) {
        GD.Print("TIle Creation : ", index);
        Tile tile = _tile.Instantiate<Tile>();
        tile.LoadValues(index, wob);
        TilesAndPieces.AddChild(tile);
    }

    public void AddPiece(Pieces type, Vector3I idx) {
        Tile tile = (Tile)GetTree().GetNodesInGroup("TILE").First(x => ((Tile)x).Idx == idx);
        PieceAbstract piAbstract = PiecePathDictionary[type];
        Piece piece = GD.Load<PackedScene>(piAbstract.Scene).Instantiate<Piece>();
        TilesAndPieces.AddChild(piece);
        piece.LoadValues(tile, true, type, piAbstract);
        tile.CurrentPiece = piece;
    }
    
    public override void _Input(InputEvent @event) {
        if (Input.IsKeyPressed(Key.N)) {
            AddPiece(Pieces.Piece, new Vector3I(0, 0, BoardIdx));
        }
    }

    
    private void Flip (bool color) {
        
    }

    private void Win (bool color) {
        
    }
}