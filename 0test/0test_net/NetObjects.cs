using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CHESS2._0test._0test_net;

public enum PacketOptions {
    // Random
    Ping,
    ServerError,
    Disconnected,

    // Establish Connection
    Connect,

    // Authentication
    Auth,
    AuthAnsPositive,
    AuthAnsNegative,

    // Game Menu
    ServerUpdate, // C | S + lobby list, player list, ... 
    LobbyJoin, // C
    LobbyCreate, // C + creation information
            // there is no reason to make more than two responses to LobbyJoin Calls
    LobbyJoinAccept, // S + lobby information
    LobbyJoinDenied, // S
            // += FREN ASK TO PLAY OPTIONS

    // In Lobby
    LobbyUpdate, // C | S + lobby information
    ChangeRole, // C
    ChangeRoleAccept, // S + ???
    ChangeRoleDenied, // S 
    StartGame, // C[Owner?] -> S | S -> All C

    // In Game
            // THIS IS NOT GOING TO BE FUNC AT ALL
    GameUpdate

}

public interface IPacket {
    public PacketOptions Option { get; set; }
    public Dictionary<string,string> Args { get; set; }
}

public struct Packet : IPacket {
    public PacketOptions Option { get; set; }
    public Dictionary<string,string> Args { get; set; }

    // STATIC PACKET THAT IDK HOW TO ACC MAKE
    // public static Packet Disconnection = new Packet {
    //     Option = PacketOptions.Disconnected,
    //     Args = new Dictionary<string, string> {
    //         ["reason"] = "Reason unknow or now importat"
    //     }
    // };
}

public class LobbyUpdateArgs : EventArgs {
    public SendableLobby Lobby { get; set; }
    
    public LobbyUpdateArgs(SendableLobby lobby) {
        Lobby = lobby;
    }
}

public class ServerUpdateArgs : EventArgs {
    public List<string> LobbyList { get; set; }
    
    public ServerUpdateArgs(List<string> lobby) {
        LobbyList = lobby;
    }
}

public struct Lobby(ServerConnection owner, string uuid, string name, string password, int ownerid = 0)
{
    public bool Active { get; set; } = true;
    public string Uuid { get; set; } = uuid;
    public string Name { get; set; } = name;
    public string Password { get; set; } = password;
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public int OwnerId { get; set; } = ownerid; // -1 is the no owner groupoid
    public int PlayerId { get; set; }
    public List<ServerConnection> Players { get; set; } = [owner];
}

public struct SendableLobby(Lobby lobby)
{
    public bool Active { get; set; } = lobby.Active;
    public string Uuid { get; set; } = lobby.Uuid;
    public string Name { get; set; } = lobby.Name;
    public string Password { get; set; } = lobby.Password;
    public DateTime CreationTime { get; set; } = lobby.CreationTime;
    public int OwnerId { get; set; } = lobby.OwnerId;
    public int PlayerId { get; set; } = lobby.PlayerId;
    public List<string> Players { get; set; } = [..lobby.Players.Select(player => player.Name)];
}

public class ServerConnection {
    public StreamPeerTcp Connection { get; set; }
    public string Name { get; set; }
}