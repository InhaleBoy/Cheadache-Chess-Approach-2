using System.Collections.Generic;
using Godot;

namespace CHESS2._0test;

public static class Globe {
    public static bool IsThereAGamePresent = false ;
    public static bool ColorToMoveNext = true;
    public static bool YourColor = true; // Base color is true=white
    public static bool CanYouMove = YourColor == ColorToMoveNext;
    
    // It's juts twice the current tourtn to mod 2 it for who sohuld move XD
    public static double TwiceTurn = 1;
}

public interface IBoard {
    void BoardSetup(int boardIdx, Information.Boards boardType);
}

public abstract partial class AbstractBoard : Control {
    // Thik bout it ??? Replace IBoard coz this is absolute bgarbage
}

public static class Information {
    
    public static Color AbleToMoveTileLightupColor = new(0, 255, 0);
    public static Color PieceOnTopTileLightupColor = new(0, 0, 255);
    
    public enum Boards {
        RrgularChess,
        DiceRoll
    }

    public enum Pieces {
        Piece,
        King,
        Queen,
        Bishop,
        Horse,
        Rook,
        Pawn
    }

    public static Dictionary<Boards, string> BoardDictionary = new() {
        [Boards.RrgularChess] = "res://0test/0test_chess/Board.tscn",
        [Boards.DiceRoll] = ""
    };

    public static Dictionary<Pieces, PieceAbstract> PiecePathDictionary = new() {
        [Pieces.Piece] = new() {
            Scene = "res://0test/0test_chess/Piece.tscn",
            Texture_W = "res://board - chess/piece/textures/goniec_black.png",
            Texture_B = "board - chess/piece/textures/rook_black.png"
        },
        [Pieces.King] = new(){
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        },
        [Pieces.Queen] = new(){
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        },
        [Pieces.Bishop] = new(){
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        },
        [Pieces.Horse] = new(){
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        },
        [Pieces.Rook] = new(){
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        },
        [Pieces.Pawn] = new() {
            Scene = "res://0test/0test_chess/Piece.tscn",
            Texture_W = "board - chess/piece/textures/pawn_white.png",
            Texture_B = "board - chess/piece/textures/pawn_black.png"
        }
    };
}

public class PieceAbstract {
    public string Scene { get; set; }
    public string Texture_W { get; set; }
    public string Texture_B { get; set; }
}


