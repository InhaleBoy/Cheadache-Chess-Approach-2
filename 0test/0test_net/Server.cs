using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using Godot;
using System.Threading.Tasks;

namespace CHESS2._0test._0test_net;

public class Server {
    public ushort Port;
    public TcpServer TcpServer = new();

    public Queue<Packet> PacketQueue = new();
    
    // ----------------------- Events
    public delegate void ServerActionEventHandler();
    public event ServerActionEventHandler OnServerAction;

    public List<ServerConnection> Connections = [];
    public Dictionary<string, Lobby> Lobbies = new();

    public Server(ushort port){
        Port = port;
        AsyncStartServer();
    }

    private async void AsyncStartServer() { 
        Error err = TcpServer.Listen(Port);
        GD.Print(err, " --- server conn status error");
        await Task.Run(AsyncRequestGrabber);
        // await Task.Run(TaskRequestHandler);
    }

    public void RemoveConnetion(ServerConnection conn) { // ?? GArbace collection in questions ?? idk
        GD.Print(" Host Disconnected ");
        conn.Connection.DisconnectFromHost();
        // Connections.Remove(conn);
    }

    public void AddConnection(ServerConnection conn) {
        if (Connections.Contains(conn)) return;
        Connections.Add(conn);
        GD.Print(Connections.Count, " --- connections count | connection status --- ", conn.Connection.GetStatus());
    }

    private async Task AsyncRequestGrabber() {
        while (true) {
            while(TcpServer.IsConnectionAvailable()){
                ServerConnection newConnection = new ServerConnection {
                    Connection = TcpServer.TakeConnection()
                };
                AddConnection(newConnection);
            }

            // TODO GARBAGE USERS COLLECTION AND STUFF :)
            // IGNORED FOR TIME - ALSO VERY IMPORTANT :<
            
            foreach (ServerConnection client in Connections) {
                client.Connection.Poll();
                int avBytes = client.Connection.GetAvailableBytes();
                if (avBytes <= 0) continue;
                string strInformation = client.Connection.GetUtf8String(avBytes);
                GD.Print("Packet added to Queue ------ " ,strInformation);
                Packet pak;
                try {
                    pak = JsonSerializer.Deserialize<Packet>(strInformation);
                }
                catch (Exception e) {
                    client.Connection.GetData(client.Connection.GetAvailableBytes());
                    continue;
                }
                PacketQueue.Append(pak);
                await Task.Run(() => { PacketHandler(pak,client); });
            }
        }
    }
    
    // -------------------------- METHODS
    
    public void SendData(StreamPeerTcp conn, Packet packet) {
        Error err = conn.PutData(JsonSerializer.Serialize(packet).ToUtf8Buffer());
        GD.Print(" Server Packet Sent : ", err, " ", packet.ToString());
    }
    
    public string GenerateUUID() { 
        return Guid.NewGuid().ToString();
    }
    
    // ------------------------------ PACKET HANDLE

    public void PacketHandler(Packet packet, ServerConnection client) {
        switch (packet.Option) {
            case PacketOptions.Auth : {
                Packet_Auth(packet, client);
                break;
            }
            case PacketOptions.Ping : {
                Packet_Ping(client);
                break;
            }
            case PacketOptions.LobbyCreate : {
                Packet_LobbyCreate(packet,client);
                break;
            }
            case PacketOptions.LobbyUpdate : {
                Packet_LobbyUpdate(packet,client);
                break;
            }
            case PacketOptions.LobbyJoin : {
                Packet_LobbyJoin(packet,client);
                break;
            }
            case PacketOptions.ServerUpdate : {
                Packet_ServerUpdate(packet,client);
                break;
            }
            default: {
                GD.Print(" WHAT HAPPENED MATE --- ", packet.Option);
                break;
            }
        }
        OnServerAction?.Invoke();
    }

    private void Packet_ServerUpdate(Packet packet, ServerConnection client) {
        // What should server give you
        // Lobby list
        // ... now that it is all for now - PUT IT IN ARGS !!
        
        SendData(client.Connection, new Packet{
            Option = PacketOptions.ServerUpdate,
            // FOR NOW WHOLE ARGS CAN BE JUST THE Dict<s,s> of uuid's and names
            Args = new Dictionary<string, string>{
                ["lobbies_json_array"]=JsonSerializer.Serialize(Lobbies.Keys)
            }
        });
    }

    public void Packet_Auth(Packet packet, ServerConnection client) {
        GD.Print(packet, client); // DEBUG
        bool packetWelness = packet.Args.TryGetValue("login", out string login) 
                             && packet.Args.TryGetValue("password", out string password);

        SendData(client.Connection, new Packet {
            Option = packetWelness ? PacketOptions.AuthAnsPositive : PacketOptions.AuthAnsNegative,
            Args = packet.Args
        });
        
        if (packetWelness){
            GD.Print("NEW CONNECTION | name : ",login);
            client.Name = login;
        }
        else {
            // HERE SOME GARBAGE COLLECTION ???
        }
            
    }

    public void Packet_Ping(ServerConnection client) {
        SendData(client.Connection, new Packet {
            Option = PacketOptions.Ping,
            Args = new Dictionary<string, string> {
                ["ans"] = "Pong! to " + client.Name
            }
        });
    }
    
    public void Packet_LobbyCreate(Packet packet, ServerConnection client) {
        packet.Args.TryGetValue("name", out string lobbyName);
        packet.Args.TryGetValue("password", out string lobbyPassword);
        if (lobbyName is null) return;
        packet.Args.TryGetValue("current_lobby_uuid", out string curlobbyuuid);
        if (curlobbyuuid != "") {
            Packet_LobbyLeave(packet,client,curlobbyuuid);
        }

        // The LobbyJoin call for creator is implicated into this sending him updates
        
        string uuid = GenerateUUID();
        Lobbies.Add(uuid, new Lobby(client,uuid,lobbyName,lobbyPassword));
        
        SendData(client.Connection, new Packet{
            Option = PacketOptions.LobbyCreate,
            Args = new Dictionary<string, string>{
                ["ans"]="true",
                ["UUID"]=uuid
            }
        });
        
        GD.Print("Lobby Created"); //DEBUG
    }
    
    public void Packet_LobbyLeave(Packet packet, ServerConnection client, string knownuuid = "") {
        if (knownuuid == string.Empty) return;

        Lobbies.TryGetValue(knownuuid, out Lobby lobby);
        if (lobby.Players.FindIndex((player) => player == client ) == lobby.OwnerId) lobby.OwnerId = -1;
        lobby.Players.Remove(client);
        
        lobby.Players.ForEach((player) => Packet_LobbyUpdate(new Packet{
            Option = PacketOptions.ServerUpdate,
            Args = new Dictionary<string, string>{
                ["UUID"] = lobby.Uuid
            }
        }, player));
    }
    
    public void Packet_LobbyUpdate(Packet packet, ServerConnection client){
        packet.Args.TryGetValue("UUID",out string uuid);
        if (uuid is null) return;
        Lobbies.TryGetValue(uuid, out Lobby lobby);
        if (!lobby.Players.Contains(client)) return;
        string strJsonLobby = JsonSerializer.Serialize(new SendableLobby(lobby));
        
        GD.Print("Lobby String dump : ",strJsonLobby," |||  "); // DEBUG
        
        SendData(client.Connection, new Packet{
            Option = PacketOptions.LobbyUpdate,
            Args = new Dictionary<string, string>{
                ["lobby"] = strJsonLobby
            }
        });
        
        GD.Print("Lobby Update Sent"); //DEBUG
    }
    
    public void Packet_LobbyJoin(Packet packet, ServerConnection client) {
        packet.Args.TryGetValue("name", out string name);
        packet.Args.TryGetValue("uuid", out string uuid);
        packet.Args.TryGetValue("password", out string pass);
        packet.Args.TryGetValue("current_lobby_uuid", out string curlobbyuuid);
        if (curlobbyuuid != "") {
            Packet_LobbyLeave(packet,client,curlobbyuuid);
        } 
        

        if (uuid is null) {
            throw new Exception(" -- I dont send packets with PacketOption.LobbyJoinDenied");
        }
        
        Lobbies.TryGetValue(uuid, out Lobby lobby);
        lobby.Players.Add(client);
        
        Packet pack = new Packet{
            Option = PacketOptions.LobbyJoinAccept,
            Args = new Dictionary<string, string>{
                ["UUID"] = uuid
            }
        };
        
        SendData(client.Connection, pack);
        
        lobby.Players.ForEach((player) => Packet_LobbyUpdate(pack,player) );
    }
    
}