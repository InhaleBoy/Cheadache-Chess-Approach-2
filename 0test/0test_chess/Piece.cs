using System;
using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public partial class Piece : CharacterBody2D {
    
    [Export] private Sprite2D Sprite { get; set; }
    [Export] private CollisionShape2D CollisionShape { get; set; }

    // -- Conditions
    private bool _mouseontop;
    private bool _follow;
    // private bool _poscorrection;

    // -- Indexing ?? Placement
    public ulong BoardInstanceId;
    public Vector3I Idx;
    public Pieces Type;
    public bool InGameColor;
    
    // -- Move Validation and UI
    public Tile CurrentTile;
    public Tile FloatingTile;

    
    public static Shape2D ClickShape2D = new RectangleShape2D {
        Size = new Vector2(180,180)
    };

    public static Shape2D DragShape2D = new CircleShape2D {
        Radius = 10
    };
    
    
    public void Init(ulong boardInstanceId, Tile tile, bool ingamecolor,Pieces type,PieceAbstract piAbstract) {
        Idx = tile.Idx;
        Type = type;
        InGameColor = ingamecolor;
        CurrentTile = tile;
        BoardInstanceId = boardInstanceId;
        
        Sprite.Texture = GD.Load<Texture2D>(
            ingamecolor ? piAbstract.Texture_W : piAbstract.Texture_B);

        CallDeferred(Node2D.MethodName.SetGlobalPosition,tile.GlobalPosition);
    }
    
    

    // ------------------------ Overrides
    
    public override void _PhysicsProcess(double delta) {
        if (_follow) {
            Vector2 campos = GetViewport().GetCamera2D().GetGlobalMousePosition();
            Velocity = new Vector2(campos.X-GlobalPosition.X, campos.Y - GlobalPosition.Y) * 10;
            MoveAndSlide();
        }
    }

    public override void _Input(InputEvent @event) {
        
        // This is abysmal and it needs just a tiny bit of thinking
        // But i font have the time ╥﹏╥
        
        if (@event is InputEventMouseButton && _mouseontop && @event.IsPressed()) {
            // Check if it's your time to SHINE | WHY HERE IT WORK HERE ?!?!
            if (InGameColor != Globe.YourColor || !Globe.CanYouMove) return;
            CollisionShape.Shape = DragShape2D;
            _follow = true;
            // ColorTiles(AbleToMoveTileLightupColor);
            CurrentTile.SetLightup(PieceOnTopTileLightupColor);
        }
        if(@event is InputEventMouseButton && @event.IsReleased() && _follow) {   
            if(FloatingTile is not null && FloatingTile.CurrentPiece is null
                && MoveValidation.ValidationForPiece(InGameColor, Type, CurrentTile.Idx, FloatingTile.Idx) ) {
                
                // Test Move Print
                Board board = (Board)InstanceFromId(BoardInstanceId);
                if (board is null){
                    GD.Print("DUCK!");
                    return;
                }
                
                bool canMove = board.SetMove(GetInstanceId(), CurrentTile, FloatingTile);
                if (canMove) {
                    Move(CurrentTile,FloatingTile);
                } 
                
            }
            ColorTiles(Colors.Transparent);
            FloatingTile = null;
            _follow = false;
            _mouseontop = false;
            GlobalPosition = CurrentTile.GlobalPosition;
            CollisionShape.Shape = ClickShape2D;
        }
        else if (@event is InputEventMouseMotion && _follow) {
            ColorTiles(AbleToMoveTileLightupColor);

            try {
                // THIS BRAKES COZ OF PIECES MOVIN ON THEIR OWN AAAAAAAAA !!!
                FloatingTile.SetLightup(PieceOnTopTileLightupColor);
            } catch (Exception e) {
                Console.WriteLine("This Method is still CRYING OMFG ?!?! --- ", e);
            }
        }
    }
    
    
    
    // ------------------------------- Methods

    public void Move(Tile from, Tile to){
        from.CurrentPiece = null;
        to.CurrentPiece = this;
        CurrentTile = to;
    }
    
    public void UpdatePosition() {
        SetGlobalPosition(CurrentTile.GlobalPosition);
    }
    
    
    private void ColorTiles(Color color){ // replace with call group
        var tiles = GetTree().GetNodesInGroup("TILE");
        foreach (var apparentTile in tiles) {
            Tile gotoTile = (Tile)apparentTile;
            gotoTile.SetLightup(Colors.Transparent);
            if (!MoveValidation.ValidationForPiece(InGameColor, Type, CurrentTile.Idx, gotoTile.Idx)) continue;
            gotoTile.SetLightup(color);
        }
    }

    

    public void _on_mouse_entered() {
        _mouseontop = true;
    }

    public void _on_mouse_exited() {
        _mouseontop = false;
    }   
}




// ------------------------------ RULESET 
public static class MoveValidation {
    
    public static bool ValidationForPiece(bool ingamecolor, Pieces type, Vector3I startTileIdx, Vector3I endTileIdx) {
        switch (type) {
            case Pieces.Piece: return true;//PawnPieceMoveValidation(ingamecolor, ref startTile, ref endTile);
            case Pieces.Pawn: return PawnPieceMoveValidation(ingamecolor, startTileIdx, endTileIdx);
            default: return false;
        }
    }

    private static bool PawnPieceMoveValidation(bool ingamecolor, Vector3I startTileIdx, Vector3I endTileIdx) {
        int distance = endTileIdx.Y - startTileIdx.Y;
        bool yidx = (ingamecolor ? -distance : distance) is > 0 and <= 2;
        
        bool xidx = endTileIdx.X == startTileIdx.X;
        bool zidx = endTileIdx.Z == startTileIdx.Z;
        return xidx && zidx && yidx;
    } 
}

