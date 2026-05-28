using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace CHESS2._0test._0test_net;

public partial class NetTester : Control
{
    [Export] public TextEdit OutputConnectionList { get; set; }
    [Export] public TextEdit OutputLobbyList { get; set; }
    
    [Export] public LineEdit InputLogin { get; set; }
    [Export] public LineEdit InputPassword { get; set; }

    [Export] public LineEdit InputLobbyName { get; set; }
    [Export] public LineEdit InputLobbyPassword { get; set; }
    
    [Export] public LineEdit OutputLobbyName { get; set; }
    [Export] public LineEdit OutputLobbyPassword { get; set; }
    [Export] public LineEdit OutputLobbyUuid { get; set; }
    [Export] public TextEdit OutputLobbyPlayerList { get; set; }
    [Export] public CheckBox OutputLobbyActive { get; set; }
    
    [Export] public LineEdit InputLobbyJoinName { get; set; }
    [Export] public LineEdit InputLobbyJoinUuid { get; set; }
    [Export] public LineEdit InputLobbyJoinPassword { get; set; }
    
    [Export] public VBoxContainer OutputLobbyListClient { get; set; }
    [Export] public PackedScene LobbyListClientButtonScene { get; set; }

    [Export] public PackedScene PasswodEntererScene { get; set; }
    public PasswordEnterer PasswodEntererObj;
    public Timer RefreshTimer = new();
    

    public Server NetServer;
    public Client NetClient;

    public SendableLobby CurrentLobby;
    public List<string> LobbyList;

    public ushort ClientPort = 9999;
    public ushort ServerPort = 9998;
    
    public void  StartClient() {
        if (InputLogin is null || InputPassword is null) {
            GD.Print("ERROR : NO LOGIN INPUTS");
            return;
        }

        NetClient = new Client("127.0.0.1", ServerPort);
        NetClient.StartClient(InputLogin.Text, InputPassword.Text);
        
        NetClient.OnLobbyUpdate += (_,e) => {
            CurrentLobby = e.Lobby;
            CallDeferred(MethodName.UpdateLobbyInfo);
        };
        
        NetClient.OnServerUpdate += (_,e) => {
            LobbyList = e.LobbyList;
            CallDeferred(MethodName.UpdateServerLobbyList);
        };
        
        // TIME REFRESH ADD LATER
        // RefreshTimer.Timeout += () => {
        //     ServerUpdate();
        //     RefreshTimer.Start(10);
        // };
        // AddChild(RefreshTimer);
        // RefreshTimer.Start(2);
    }
    
    public void StartServer() {
        NetServer = new Server(ServerPort);

        OutputConnectionList.GetChild<Label>(0).Visible = true;
        OutputLobbyList.GetChild<Label>(0).Visible = true;

        NetServer.OnServerAction += () => CallDeferred(MethodName.ServerDiagnostics);
    }
    
    
    // --------------- Buttons

    public void _on_ping_clients_button_down() 
        => throw new NotImplementedException(" --- Cant Ping Clients");
    
    // --------------- Actions / Methods
    
    public void Createlobby() {
        if (NetClient is null) return;
        if (!NetClient.ConnectionEstablished()) return;
        if (InputLobbyName.Text.Length <= 0) return;

        string lobbyName = InputLobbyName.Text;
        string lobbyPassword = InputLobbyPassword.Text;

        GD.Print($"Create lobby {lobbyName} {lobbyPassword}");
        
        GD.Print(NetClient);

        NetClient.TcpSendPacket(new Packet {
            Option = PacketOptions.LobbyCreate,
            Args = new Dictionary<string, string>{
                ["name"] = lobbyName,
                ["password"] = lobbyPassword,
                ["current_lobby_uuid"] = CurrentLobby.Uuid ?? ""
            }
        });
    }
    
    public void JoinLobby() {
        if (NetClient is null) return;
        if (!NetClient.ConnectionEstablished()) return;
        if (InputLobbyJoinName.Text.Length <= 0 && InputLobbyJoinUuid.Text.Length <= 0) return;
            
        NetClient.TcpSendPacket(new Packet{
            Option = PacketOptions.LobbyJoin,
            Args = new Dictionary<string, string>{
                ["name"] = InputLobbyJoinName.Text,
                ["uuid"] = InputLobbyJoinUuid.Text,
                ["password"] = InputLobbyJoinPassword.Text,
                ["current_lobby_uuid"] = CurrentLobby.Uuid ?? ""
            }
        });
    }
    
    public void JoinLobby(string uuid, string password) {
        if (NetClient is null) return;
        if (!NetClient.ConnectionEstablished()) return;
            
        NetClient.TcpSendPacket(new Packet{
            Option = PacketOptions.LobbyJoin,
            Args = new Dictionary<string, string>{
                ["name"] = "name is not important XD",
                ["uuid"] = uuid,
                ["password"] = password,
                ["current_lobby_uuid"] = CurrentLobby.Uuid ?? ""
            }
        });
    }
    
    public void ServerUpdate() {
        if (NetClient is null) return;
        if (!NetClient.ConnectionEstablished()) return;
        NetClient.TcpSendPacket(new Packet {
            Option = PacketOptions.ServerUpdate
        });
    }
    
    
    // ----------------------------------- GUI

    public void UpdateServerLobbyList() {
        GD.Print("TO OSTATNIA NIEDZIELA - DZISIAJ SIĘ ROZSTANIEMY - DZISIAJ SIĘ ROZEJDZIEMY - NA LEPSZY CZAS"); // DEBUG SLOP
        foreach (var child in OutputLobbyListClient.GetChildren()) {
            child.QueueFree();
        }

        foreach (var lobby in LobbyList) {
            LobbyButton lb = LobbyListClientButtonScene.Instantiate<LobbyButton>();
            lb.Text = lobby;
            lb.ButtonObj.ButtonDown += () => {
                if (PasswodEntererObj is not null) {
                    PasswodEntererObj.QueueFree();
                }

                PasswodEntererObj = PasswodEntererScene.Instantiate<PasswordEnterer>();
                PasswodEntererObj.EnterButton.ButtonDown += () => {
                    CallDeferred(MethodName.JoinLobby, lb.Text, PasswodEntererObj.Input.Text);
                    PasswodEntererObj.QueueFree();
                    PasswodEntererObj = null;
                };
                AddChild(PasswodEntererObj);
            };
            OutputLobbyListClient.AddChild(lb);
        }
    }
    
    public void UpdateLobbyInfo() {
        GD.Print("BLACK WHOLE SUN - WONT U COME - WAHS AWAY THE PAIN"); // DEBUG SLOP
        OutputLobbyActive.ButtonPressed = CurrentLobby.Active;
        OutputLobbyName.Text = CurrentLobby.Name;
        OutputLobbyPassword.Text = CurrentLobby.Password;
        OutputLobbyUuid.Text = CurrentLobby.Uuid;
        OutputLobbyPlayerList.Text = "";
        foreach (var name in CurrentLobby.Players) {
            OutputLobbyPlayerList.Text += $"{name}\n";
        }
    }

    // public void PasswordEntererFunc(string uuid) {
    //     PasswodEntererObj.Visible = true;
    //     PasswodEntererObj.EnterButton.ButtonDown += PasswordEntererCorrection;
    //     // some things that stupid ppl dont do stupid things
    // }
    
    // public void PasswordEntererCorrection(){
    //     CallDeferred(MethodName.JoinLobby, uuid, PasswodEntererObj.Input.Text);
    //     PasswodEntererObj.Visible = false;
    // }
    
    
    
    // --------------- INFORMATION -- LOG -- REFRESH -- :))))))))
    public override void _Input(InputEvent @event) {
        if (!Input.IsKeyPressed(Key.A)) return;
        GD.Print("---- the info ",JsonSerializer.Serialize(CurrentLobby));
    }

    public void ServerDiagnostics() {
        if (NetServer == null) return;

        OutputConnectionList.Text = "";
        OutputLobbyList.Text = "";
        
        foreach (var conn in NetServer.Connections) {
            OutputConnectionList.Text += $"@{conn.Name}\n";
        }
        
        foreach (var lobby in NetServer.Lobbies) {
            OutputLobbyList.Text += $"{lobby.Value.Name}\n";
        }
    }
    
    public void PingServer()
    {
        if (NetClient is null) return;
        if (!NetClient.ConnectionEstablished()) return;
        GD.Print("Client Ping!");
        NetClient.TcpSendPacket(new Packet
        {
            Option = PacketOptions.Ping
        });
    }
    
    
}
