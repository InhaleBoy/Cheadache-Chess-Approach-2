using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.WebSockets;
using Godot;
using Godot.Collections;
using static CHESS2._0test.Information;

namespace CHESS2._0test._0test_chess;

public partial class ChessBoard : Board {

    public override void BoardSetup(int boardIdx, Boards boardType) {
        base.BoardSetup(boardIdx,boardType);
        CreatePieces(8,8);
    }
    
    public void CreatePieces(int horizontal, int vertical) {
        
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

    
    
    
    
    // ------------------- Later to be repurposed to something else
    
    
    
    [Obsolete] public void AddTile(Vector3I index, bool wob) {
        // GD.Print("TIle Creation : ", index); // DEBUG
        Tile tile = TileScene.Instantiate<Tile>();
        // tile.Init(index, wob ? Colors.White : Colors.Black);
        TilesAndPieces.AddChild(tile);
    }
    
    [Obsolete] public void CreateTiles(int horizontal, int vertical) {
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

}