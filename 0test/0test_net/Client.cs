using System.Collections.Generic;
using System.Text.Json;
using Godot;
using System.Threading;
using System.Threading.Tasks;

namespace CHESS2._0test._0test_net;

public class Client {
    
    // ------------------- Config
    private readonly string _serverIp;
    private readonly ushort _serverPort;
    
    
    

    public Client(string serverIp, ushort serverPort) {
        _serverIp = serverIp;
        _serverPort = serverPort;
        _tcpPeer = new StreamPeerTcp();
        GD.Print("Client Started");
    }
    
    // -------------------------------- Main Object --- Inner Object
    // I wanted to inherit from StreemPeer Tcp to make an object with additional functions
    // for some reason i can't
    
    private StreamPeerTcp _tcpPeer;
    
    public void StartClient(string login, string password){
        Task.Run(() => StartClientAsync(login,password));
    }

    private async Task StartClientAsync(string login, string password) {
        if (ConnectionEstablished()) return;

        Error err = _tcpPeer.ConnectToHost(_serverIp ?? "127.0.0.1", _serverPort);
        
        if (err != Error.Ok) {
            GD.Print("TCP SERVER NOT EXISTANT");
            _tcpPeer.DisconnectFromHost();
            return;
        }
        
        GD.Print("TcpServer Found : ", _tcpPeer.GetConnectedHost()," ",err);

        _tcpPeer.Poll();
        bool connected = await Task.Run(() => {
            for (int i = 0; i < 20; i++) {
                GD.Print("Connecting ... | Status ", _tcpPeer.GetStatus());
                bool connected = _tcpPeer.GetStatus() == StreamPeerTcp.Status.Connected;
                if (connected) return true;
                Thread.Sleep(1000 ^ i);
            }
            return false;
        });

        if (!connected) {
            GD.Print("Could Not Connect");
            return;
        }

        IPacket authpacket = new Packet {
            Option = PacketOptions.Auth,
            Args = new Dictionary<string,string> {
                ["login"] = login,
                ["password"] = password
            }
        };
        TcpSendPacket(authpacket);

        // TECHNICALY - THIS IS NEVER ENDING TASK TBH
        // IT SEEMS LIKE IT WILL BREAK ITSELF IF I LEAVE IT FOR TOO LONG
        await Task.Run(async () => {
            while(true) {
                _tcpPeer.Poll();
                IPacket packet = TcpGetPacket();
                if (packet != null) HandlePacket(packet);
            }
        });
    }
    
    // -------------------------- METHODS

    public IPacket TcpGetPacket() {
        if (_tcpPeer.GetStatus() != StreamPeerTcp.Status.Connected || _tcpPeer.GetAvailableBytes() <= 0) return null;
        return JsonSerializer.Deserialize<Packet>(_tcpPeer.GetUtf8String(_tcpPeer.GetAvailableBytes()));
    }

    public void TcpSendPacket(IPacket packet) {
        Error err = _tcpPeer.PutData(JsonSerializer.Serialize(packet).ToUtf8Buffer());
        GD.Print(" Client Packet Sent : ", err, " ", packet);
    }
    
    public bool ConnectionEstablished() {
        GD.Print("Check Connection : ",_tcpPeer.GetStatus());
        return _tcpPeer.GetStatus() != StreamPeerTcp.Status.None;
    }
    
    // ------------------------------ PACKET HANDLE

    private void HandlePacket(IPacket packet) {
        switch(packet.Option){
            case PacketOptions.AuthAnsPositive : {
                Packet_AuthAnsPositive(packet);
                break;
            }
            case PacketOptions.AuthAnsNegative : {
                GD.Print(" You might be a dragon ");
                break;
            }
            case PacketOptions.Ping : {
                packet.Args.TryGetValue("ans", out string value);
                GD.Print(value);
                break;
            }
            case PacketOptions.LobbyCreate : {
                Packet_LobbyCreate(packet);
                break;
            }
            case PacketOptions.LobbyUpdate : {
                Packet_LobbyUpdate(packet);
                break;
            }
            case PacketOptions.ServerUpdate : {
                Packet_ServerUpdate(packet);
                break;
            }
            case PacketOptions.LobbyJoinAccept : {
                Packet_LobbyJoinAccept(packet);
                break;
            }
            default: {
                GD.Print(" WHAT HAPPENED MATE --- PacketOption : ", packet.Option);
                break;
            }
        }
    }
    
    // -------------------------------- Events
    
    public delegate void LobbyUpdateEventHandler(object sender, LobbyUpdateArgs args);
    public event LobbyUpdateEventHandler OnLobbyUpdate;
    
    public delegate void ServerUpdateEventHandler(object sender, ServerUpdateArgs args);
    public event ServerUpdateEventHandler OnServerUpdate;
    
    // --------------------------------------------- Handlers
    
    public void Packet_AuthAnsPositive(IPacket packet) {
        if (packet.Args is null) return;
        packet.Args.TryGetValue("login", out var login);
        packet.Args.TryGetValue("password", out var password);
        GD.Print(" YOU ARE NO DRAGON! YOU ARE ", login, " ", password);
    }
    
    public void Packet_LobbyCreate(IPacket packet) {
        packet.Args.TryGetValue("ans", out string ans);
        if (ans != "true") return;
        packet.Args.TryGetValue("UUID", out string uuid);

        GD.Print("Asking for Lobby Update"); //DEBUG

        TcpSendPacket(new Packet {
            Option = PacketOptions.LobbyUpdate,
            Args = new Dictionary<string, string> {
                ["UUID"] = uuid
            }
        });
    }
    
    public void Packet_LobbyJoinAccept(IPacket packet){
        packet.Args.TryGetValue("UUID", out string uuid);
        if ( uuid == "") return;
        
        // Technicaly u dont need it but i will leave it here for later
        
        // TcpSendPacket(new Packet
        // {
        //     Option = PacketOptions.LobbyUpdate,
        //     Args = new Dictionary<string, string>
        //     {
        //         ["UUID"] = uuid
        //     }
        // });
    }
    
    public void Packet_LobbyUpdate(IPacket packet){
        packet.Args.TryGetValue("lobby", out string lobby);
        if (lobby is null) {
            GD.Print("Lobby is null for some reason");
            return;
        }

        SendableLobby sendableLobby = JsonSerializer.Deserialize<SendableLobby>(lobby);

        OnLobbyUpdate?.Invoke(this, new LobbyUpdateArgs(sendableLobby));
    }
    
    public void Packet_ServerUpdate(IPacket packet) {
        packet.Args.TryGetValue("lobbies_json_array", out string lobby);
        if (lobby is null) {
            GD.Print("Lobby empty ???"); // DEBUG
            return;
        }

        List<string> sendableLobby = JsonSerializer.Deserialize<List<string>>(lobby);

        OnServerUpdate?.Invoke(this, new ServerUpdateArgs(sendableLobby));
    }
    
}
