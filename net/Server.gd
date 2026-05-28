extends Control
class_name NetServerModule

enum Message {
	id,
	join,
	userConnected,
	userDisconnected,
	lobby,
	candidate,
	offer,
	answer,
	checkIn
}

var peer = WebSocketMultiplayerPeer.new()
var users = {}
var lobbies = {}

func _ready():
	peer.connect("peer_connected", peer_connected)
	peer.connect("peer_disconnected", peer_disconnected)

func _on_button_button_down():
	startServer()

func startServer():
	peer.create_server(56473)
	print("Started Server")

func sendToPlayer(userID, data):
	peer.get_peer(userID).put_packet(JSON.stringify(data).to_utf16_buffer())

#---------------------------------------------------------------------------------------------------

func peer_connected(id):
	print("Peer Connected " + str(id))
	users[id] = {
		"message" : Message.id,
		"id" : id
	}
	sendToPlayer(id,users[id])
	$LineEdit.text = ""
	for i in users:
		$LineEdit.text += str(i) + "\n"

func peer_disconnected(id):
	print("Peer Disconnected " + str(id))
	users.erase(id)

func _process(delta):
	peer.poll()
	if peer.get_available_packet_count() > 0:
		var packet = peer.get_packet()
		if peer != null:
			var dataString = packet.get_string_from_utf16()
			var data = JSON.parse_string(dataString)
			print(data)
			if data.message == Message.lobby:
				joinLobby(data)
				
			if data.message == Message.offer || data.message == Message.answer || data.message == Message.candidate:
				print("source id is " + str(data.orgPeer))
				sendToPlayer(data.peer, data)


func joinLobby(user):
	if user.lobbyValue == "" or lobbies[user.lobbyValue].Players.size() == 2:
		user.lobbyValue = generateRandomString()
		lobbies[user.lobbyValue] = Lobby.new(user.id,user.mods)
	
	var player = lobbies[user.lobbyValue].addPlayer(user.id, user.name)
	
	for p in lobbies[user.lobbyValue].Players:
		
		var data = {
			"message" : Message.userConnected,
			"id" : user.id
		}
		sendToPlayer(p, data)
		
		var data2 = {
			"message" : Message.userConnected,
			"id" : p
		}
		sendToPlayer(user.id, data2)
		
		var lobbyInfo = {
			"message" : Message.lobby,
			"players" : JSON.stringify(lobbies[user.lobbyValue].Players),
			"host" : lobbies[user.lobbyValue].HostID,
			"lobbyValue" : user.lobbyValue
		}
		sendToPlayer(p,lobbyInfo)
		
		
	
	var data = {
		"message" : Message.userConnected,
		"id" : user.id,
		"player" : lobbies[user.lobbyValue].Players[user.id],
		"lobbyValue" : user.lobbyValue
	}
	sendToPlayer(data.id,data)



#---------------------------------------------------------------------------------------------------

func generateRandomString() -> String:
	var result = ""
	for i in range(32):
		result += str(randi_range(0,9))
	print("result : " + str(result))
	return result
