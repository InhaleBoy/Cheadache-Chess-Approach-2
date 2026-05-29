using System.Collections.Generic;

namespace CHESS2._0test;

public interface IBoard {
    void BoardSetup(int boardIdx);
}

public static class Information {
    
    public enum Boards {
        RrgularChess,
        DiceRoll
    }

    public enum Pieces {
        Piece,
        Pawn,
    }

    public static Dictionary<Boards, string> BoardDictionary = new Dictionary<Boards, string> {
        [Boards.RrgularChess] = "res://0test/0test_chess/Board.tscn",
        [Boards.DiceRoll] = ""
    };

    public static Dictionary<Pieces, PieceAbstract> PiecePathDictionary = new() {
        [Pieces.Piece] = new() {
            Scene = "res://0test/0test_chess/Piece.tscn",
            Texture_W = "res://board - chess/piece/textures/goniec_black.png",
            Texture_B = "board - chess/piece/textures/rook_black.png"
        },
        [Pieces.Pawn] = new() {
            Scene = "",
            Texture_W = "",
            Texture_B = ""
        }
    };
}

public class PieceAbstract {
    public string Scene { get; set; }
    public string Texture_W { get; set; }
    public string Texture_B { get; set; }
}


