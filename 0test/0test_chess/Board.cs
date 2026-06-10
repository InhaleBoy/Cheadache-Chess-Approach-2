using System.Linq;
using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public partial class Board : Control, IBoard {
    
    [Export] private PackedScene TileScene { get; set; }
    [Export] public Control TilesAndPieces { get; set; }
    [Export] public Line2D PointingArrow { get; set; }
    [Export] public Button ResetMoveButton { get; set; }
    public int BoardIdx;
    public string BoardName;

    public void BoardSetup(int boardIdx, Boards boardType) {
        BoardIdx = boardIdx;
        BoardName = boardType.ToString();
        // min 1 : 4
        CreateTiles(8, 8);
        CreatePieces(8,8);
        
        // Move to some Init for all abstract Boards ??
        SetArrow();
        ResetMoveButton.Disabled = true;
    }

    // Move it to Struckt ??
    public string MoveStr;
    public bool Moved;
    public ulong MovedPieceInstanceId; // YES ! I can just fukcing save the RIDs and it will work
    public Vector3I StartIdx;
    public Vector3I EndIdx;
    
    public bool SetMove(ulong pieceInstanceId, Tile start, Tile end, string action = "To be replaced"){ 
        if(Moved) return false;
        Moved = true;
        MovedPieceInstanceId = pieceInstanceId;

        StartIdx = start.Idx;
        EndIdx = end.Idx;

        // Vector2 arrowEnd = end.Idx.Z == start.Idx.Z ? end.Position : PointingArrow.ToLocal(end.GlobalPosition);
        SetArrow(start.Position, PointingArrow.ToLocal(end.GlobalPosition));
        ResetMoveButton.Disabled = false;
        
        return true;
    }
    
    public void ResetMove() {
        Moved = false;
        
        // Piece Needs to go back
        
        MovedPieceInstanceId = default;
        ResetMoveButton.Disabled = true;
        SetArrow();
    }
    
    public bool HasMove() {
        return Moved;
    }
    
    public void SetArrow(params Vector2[] points) {
        PointingArrow.Visible = !points.IsEmpty();
        PointingArrow.Points = points;
        // PointingArrow.QueueRedraw();
    }
    
    
    
    
    
    
    
    public void CreateTiles(int horizontal, int vertical) {
        if (vertical < 4 || horizontal < 1) {
            BoardNuke();
            return;
        }
        bool wob = true;
        Vector3I index = new Vector3I(0,0,BoardIdx);
        
        for(var i = 0; i < horizontal * vertical; i++) {
            wob = i % horizontal == 0 ? wob : !wob; // move further as argument ; delete instantiation
            if (index.X == horizontal) index = new Vector3I(0,index.Y+1,BoardIdx);
            AddTile(index,wob);
            index += new Vector3I(1,0,0);
        }

        Tile tempTile = TileScene.Instantiate<Tile>();
        Vector2 size = tempTile.Size;
        tempTile.QueueFree();
        CustomMinimumSize = new Vector2(size.X * (horizontal + 1), size.Y * vertical);
    }
    
    public void CreatePieces(int horizontal, int vertical) {
        if (vertical < 4 || horizontal < 1) {
            BoardNuke();
            return;
        }
        
        
        // There might be a more elegant solution to this
        // Like for all other code in my projects
        // For now it's good enought
        
        
        // Pawns * the Y in vector sohuld be based of of vertical
        for (int i = 0; i < horizontal; i++) {
            AddPiece(Pieces.Pawn, new Vector3I(i,1,BoardIdx),false);
            AddPiece(Pieces.Pawn, new Vector3I(i, 6, BoardIdx), true);
        }

        int pivot = horizontal / 2; 
        
        // King
        AddPiece(Pieces.Piece, new Vector3I(pivot, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot, 7, BoardIdx), true);
        
        if(horizontal < 2) return;
        // Queen
        AddPiece(Pieces.Piece, new Vector3I(pivot - 1, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot- 1, 7, BoardIdx), true);
        
        // Bishop
        AddPiece(Pieces.Piece, new Vector3I(pivot + 1, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 2, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot + 1, 7, BoardIdx), true);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 2, 7, BoardIdx), true);
        
        // Horse
        AddPiece(Pieces.Piece, new Vector3I(pivot + 2, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 3, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot + 2, 7, BoardIdx), true);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 3, 7, BoardIdx), true);
        
        // Rook
        AddPiece(Pieces.Piece, new Vector3I(pivot + 3, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 4, 0, BoardIdx), false);
        AddPiece(Pieces.Piece, new Vector3I(pivot + 3, 7, BoardIdx), true);
        AddPiece(Pieces.Piece, new Vector3I(pivot - 4, 7, BoardIdx), true);
    }

    
    
    
    
    
    // ---------------------------------------- Methods
    
    public void AddTile(Vector3I index, bool wob) {
        // GD.Print("TIle Creation : ", index); // DEBUG
        Tile tile = TileScene.Instantiate<Tile>();
        tile.Init(index, wob ? Colors.White : Colors.Black);
        TilesAndPieces.AddChild(tile);
    }

    public void AddPiece(Pieces type, Vector3I idx, bool wob)
    {
        Tile tile = (Tile)GetTree().GetNodesInGroup("TILE").First(x => ((Tile)x).Idx == idx);
        if (tile.CurrentPiece is not null)
        {
            GD.Print(" ---- This Tile is Occupied"); // ?DEBUG+
            return;
        }

        PieceAbstract piAbstract = PiecePathDictionary[type];
        Piece piece = GD.Load<PackedScene>(piAbstract.Scene).Instantiate<Piece>();
        TilesAndPieces.AddChild(piece);
        piece.Init(GetInstanceId(),tile, wob, type, piAbstract);
        tile.CurrentPiece = piece;
    }
    
    
    
    
    public override void _Input(InputEvent @event) { // DEBUG INPUT
        if (Input.IsKeyPressed(Key.N)) {
            AddPiece(Pieces.Piece, new Vector3I(0, 0, BoardIdx),true);
        }
    }

    public void BoardNuke() { // TODO ? IDK
        GD.Print(" Board shal be NUKED! ");
    }
    
    private void Flip (bool color) {
        
    }

    private void Win (bool color) {
        
    }
}