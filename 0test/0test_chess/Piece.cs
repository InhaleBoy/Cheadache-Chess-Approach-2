using System;
using Godot;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public partial class Piece : CharacterBody2D {
    
    [Export] private Sprite2D Sprite { get; set; }
    [Export] private CollisionShape2D CollisionShape { get; set; }

    private bool _mouseontop;
    private bool _follow;
    private bool _poscorrection;

    public Vector3I Idx;
    public bool InGameColor;
    public Pieces Type;

    public Tile CurrentTile;
    public Tile FloatingTile;

    public static Shape2D ClickShape2D = new RectangleShape2D {
        Size = new Vector2(180,180)
    };

    public static Shape2D DragShape2D = new CircleShape2D {
        Radius = 10
    };
    
    public void LoadValues(Tile tile, bool ingamecolor,Pieces type,PieceAbstract piAbstract) {
        Idx = tile.Idx;
        Type = type;
        InGameColor = ingamecolor;
        
        Sprite.Texture = GD.Load<Texture2D>(piAbstract.Texture);

        CurrentTile = tile;
        
        // Yes, this additional simulated click is working as a position correction mechanizm.
        // Please do not ask.
        _poscorrection = true;
    }
    
    

    // ------------------------ Overrides
    
    public override void _PhysicsProcess(double delta) {
        if (_poscorrection) {
            Velocity = Vector2.Down * 10;
            MoveAndSlide();
            GlobalPosition = CurrentTile.GlobalPosition;
            _poscorrection = false;
        }
        if (_follow) {
            Vector2 campos = GetViewport().GetCamera2D().GetGlobalMousePosition();
            Velocity = new Vector2(campos.X-GlobalPosition.X, campos.Y - GlobalPosition.Y) * 10;
            MoveAndSlide();
        }
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton && _mouseontop && @event.IsPressed()) {
            CollisionShape.Shape = DragShape2D;
            _follow = true;
            ColorTiles(Tile.ColoringType.AbleToMove);
            CurrentTile.Lightup(Tile.ColoringType.OnTop, true);
        }
        else if(@event is InputEventMouseButton && @event.IsReleased()) {   
            if(FloatingTile is not null && FloatingTile.CurrentPiece is null
                                        && MoveValidation.ValidationForPiece(InGameColor,ref Type, ref CurrentTile, ref FloatingTile) ) {
                CurrentTile.CurrentPiece = null;
                FloatingTile.CurrentPiece = this;
                CurrentTile = FloatingTile;
            }
            ColorTiles(Tile.ColoringType.Nothing);
            FloatingTile = null;
            _follow = false;
            GlobalPosition = CurrentTile.GlobalPosition;
            CollisionShape.Shape = ClickShape2D;
        }
        if (@event is InputEventMouseMotion && _follow) {
            ColorTiles(Tile.ColoringType.AbleToMove);
            try{
                FloatingTile.Lightup(Tile.ColoringType.OnTop, true);
            }
            catch (Exception e) {
                // ignored
            }
        }
    }
    
    
    
    // ------------------------------- Methods

    private void ColorTiles(Tile.ColoringType coloringType){ // replace with call group
        var tiles = GetTree().GetNodesInGroup("TILE");
        foreach (var t in tiles) {
            Tile tile = (Tile)t;
            tile.Lightup(coloringType, MoveValidation.ValidationForPiece(InGameColor,ref Type, ref CurrentTile, ref tile));
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
    
    public static bool ValidationForPiece(bool ingamecolor, ref Pieces type, ref Tile startTile, ref Tile endTile) {
        switch (type) {
            case Pieces.Piece: return true;//PawnPieceMoveValidation(ingamecolor, ref startTile, ref endTile);
            default: return false;
        }
    }

    private static bool PawnPieceMoveValidation(bool ingamecolor, ref Tile startTile, ref Tile endTile) {
        int distance = endTile.Idx.Y - startTile.Idx.Y;
        return endTile.Idx.X == startTile.Idx.X && endTile.Idx.Z == startTile.Idx.Z && distance is > 0 and <= 2;
        
    } 
}

