using System;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public struct MoveInformation {
    public bool Moved {get;set;}
    public ulong MovedPieceInstanceId {get;set;} // YES ! I can just f*ing save the RIDs and it will work
    public ulong StartTileInstanceId {get;set;}
    public ulong EndTileInstanceId {get;set;}
    public Vector3I StartIdx {get;set;}
    public Vector3I EndIdx {get;set;}
}

public abstract partial class Board : Control {
    
    [Export] public PackedScene TileScene { get; set; }
    [Export] public Control TilesAndPieces { get; set; }
    [Export] public Line2D PointingArrow { get; set; }
    [Export] public Button ResetMoveButton { get; set; }
    
    public int BoardIdx;
    public string BoardName;
    public MoveInformation Move = new MoveInformation {
        Moved = false
    };
    
    public virtual void BoardSetup(int boardIdx, Boards boardType) {
        BoardIdx = boardIdx;
        BoardName = boardType.ToString();
        
        foreach(Node node in TilesAndPieces.GetChildren()) {
            if(node is not Tile tile) continue;
            tile.Idx = tile.Idx + new Vector3I(0,0,boardIdx);
        }
        
        SetArrow();
        ResetMoveButton.Disabled = true;
    }
    
    public bool SetMove(ulong pieceInstanceId, Tile start, Tile end, string action = "To be replaced"){ 
        if(Move.Moved) return false;
        Move.Moved = true;
        Move.MovedPieceInstanceId = pieceInstanceId;

        Move.StartIdx = start.Idx;
        Move.StartTileInstanceId = start.GetInstanceId();
        Move.EndTileInstanceId = end.GetInstanceId();
        Move.EndIdx = end.Idx;

        // Vector2 arrowEnd = end.Idx.Z == start.Idx.Z ? end.Position : PointingArrow.ToLocal(end.GlobalPosition);
        SetArrow(start.Position, PointingArrow.ToLocal(end.GlobalPosition));
        ResetMoveButton.Disabled = false;
        
        return true;
    }
    
    
    public void ResetMove() {
        GD.Print("MOVE RESET"); // DEBUG
        if (!Move.Moved) return;
        Move.Moved = false;

        Piece piece = (Piece)InstanceFromId(Move.MovedPieceInstanceId);
        piece.Move(
            (Tile)InstanceFromId(Move.EndTileInstanceId),
            (Tile)InstanceFromId(Move.StartTileInstanceId)
        );
        piece.UpdatePosition();
        
        Move.MovedPieceInstanceId = default;
        ResetMoveButton.Disabled = true;
        SetArrow();
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
    
    public void SetArrow(params Vector2[] points) {
        PointingArrow.Visible = !points.IsEmpty();
        PointingArrow.Points = points;
    }
    
    public virtual string CommitMoves()  {
        SetArrow();
        Move.Moved = false;
        if (Move.StartIdx.Z != Move.EndIdx.Z) {
            throw new NotImplementedException($"CHANGE BOARD FROM {Move.StartIdx.Z} TO {Move.EndIdx.Z}");
            // Change board from one to another MMMMMMMMMMMMMM ... 
        }
        // If i move the information to a struckt THIS and MORE is going to be JSON in miliseconds
        return $"JSON't {Move.StartIdx}:{Move.EndIdx}";
    }
    
    
    
    
    
    
    
    // Why this exists if moved is private
    // This WILL go to the abstract class later
    // it's existance seems unreasonable until proven otherwise
    // For example there might be boards that u can't move on to or from
    // With u would need to check
    // or smth ... 
    public virtual bool HasMove() {
        return Move.Moved;
    }

    public virtual void BoardNuke() {
        GD.Print(" Board shal be NUKED! ");
        return; // TODO
    }
    
    public virtual void Flip (bool color) {
        return; // TODO
    }

    public virtual void Win (bool color) {
        return; // TODO
    }
    
    
    
    public override void _Input(InputEvent @event) { // DEBUG INPUT
        if (Input.IsKeyPressed(Key.N)) {
            AddPiece(Pieces.Piece, new Vector3I(0, 0, BoardIdx),true);
        }
    }
}