using System.Collections.Generic;

namespace CHESS2._0test;

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

    public static Dictionary<Pieces, PieceAbstract> PiecePathDictionary = new Dictionary<Pieces, PieceAbstract>
    {
        [Pieces.Piece] = new PieceAbstract
        {
            Scene = "res://0test/0test_chess/Piece.tscn",
            Texture = "res://board - chess/piece/textures/goniec_black.png"
        },
        [Pieces.Pawn] = new PieceAbstract
        {
            Scene = "",
            Texture = ""
        }
    };

    public enum Message {
        Id,
        Join,
        UserConnected,
        UserDisconnected,
        Lobby,
        Candidate,
        Offer,
        Answer,
        CheckIn
    }
}

public class PieceAbstract {
    public string Scene { get; set; }
    public string Texture { get; set; }
}


