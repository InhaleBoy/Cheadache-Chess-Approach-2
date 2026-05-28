extends Control
class_name NetClientModule

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
var id = 0
var rtcPeer : WebRTCMultiplayerPeer = WebRTCMultiplayerPeer.new()
var hostID:int
var lobbyValue = ""

func _ready():
	multiplayer.connected_to_server.connect(RTCServerConnected)
	multiplayer.peer_connected.connect(RTCPeerConnected)
	multiplayer.peer_disconnected.connect(RTCPeerDisconnected)

func RTCServerConnected():
	print("RTC server connected")

func RTCPeerConnected(id):
	print("rtc peer connected " + str(id))
	
func RTCPeerDisconnected(id):
	print("rtc peer disconnected " + str(id))


func _on_start_client_button_down():
	connectToServer("")

var server_ip = "127.0.0.1"
func connectToServer(ip) :
	peer.create_client("ws://" + str($"Server IP".text)+ ":56473")
	print("Started Client")

#---------------------------------------------------------------------------------------------------

func _on_join_lobby_button_down():
	if $Name.text.strip_edges() == "" : return
	
	var mods = []
	
	
	
	var message = {
		"id" : id,
		"message" : Message.lobby,
		"lobbyValue" : $LineEdit.text,
		"name" : $Name.text,
		"mods" : mods
	}
	peer.put_packet(JSON.stringify(message).to_utf16_buffer())


func _on_start_game_button_down():
	StartGame.rpc()

@rpc("any_peer","call_local")
func StartGame():
	if lobbyValue == "" : return
	var scene = load("res://main_scene.tscn").instantiate()
	get_tree().root.add_child(scene)
	get_parent().hide()


#---------------------------------------------------------------------------------------------------

func _process(delta):
	peer.poll()
	if peer.get_available_packet_count() > 0:
		var packet = peer.get_packet()
		if peer != null:
			var dataString = packet.get_string_from_utf16()
			var data = JSON.parse_string(dataString)
			print(data)
			if data.message == Message.id:
				id = data.id
				connected()
			
			if data.message == Message.userConnected:
				createPeer(data.id)
				pass
			#-------------------------------------------------------------------
			if data.message == Message.lobby :
				GameManager.Players = JSON.parse_string(data.players)
				hostID = data.host
				lobbyValue = data.lobbyValue
				$"Lobby ID".text = "lobby id : " + str(lobbyValue)
				$TextEdit.text = ""
				for i in GameManager.Players:
					$TextEdit.text += str(GameManager.Players[i].name) + "\n"
				Global.YourColor = GameManager.Players[str(id)].color
			#-------------------------------------------------------------------
			
			if data.message == Message.candidate: 
				if rtcPeer.has_peer(data.orgPeer):
					print("Got Candidate: " + str(data.orgPeer) + " | my id : " +  str(id))
					rtcPeer.get_peer(data.orgPeer).connection.add_ice_candidate(data.mid, data.index, data.sdp)
			if data.message == Message.offer:
				if rtcPeer.has_peer(data.orgPeer):
					rtcPeer.get_peer(data.orgPeer).connection.set_remote_description("offer", data.data)
			if data.message == Message.answer:
				if rtcPeer.has_peer(data.orgPeer):
					rtcPeer.get_peer(data.orgPeer).connection.set_remote_description("answer", data.data)

func connected():
	rtcPeer.create_mesh(id)
	multiplayer.multiplayer_peer = rtcPeer

#web rtc connection
func createPeer(id):
	if id != self.id:
		var peer : WebRTCPeerConnection = WebRTCPeerConnection.new()
		peer.initialize({
			"iceServers" : [{ "urls" : ["stun:stun.l.google.com:19302"]}]
		})
		print("binding id " + str(id) + " my id is : " + str(self.id) )
		peer.session_description_created.connect(self.offerCreated.bind(id))
		peer.ice_candidate_created.connect(self.iceCandidateCreated.bind(id))
		
		rtcPeer.add_peer(peer, id)
		
		if id < rtcPeer.get_unique_id():
			peer.create_offer()

func offerCreated(type, data, id):
	if !rtcPeer.has_peer(id):
		return
	
	rtcPeer.get_peer(id).connection.set_local_description(type,data)
	
	if type == "offer":
		sendOffer(id, data)
	else:
		sendAnswer(id, data)


func iceCandidateCreated(midNname, indexName, sdpName, id):
	var message = {
		"peer" : id,
		"orgPeer" : self.id,
		"message" : Message.candidate,
		"mid" : midNname,
		"index" : indexName,
		"sdp" : sdpName,
		"lobby" : lobbyValue
	}
	peer.put_packet(JSON.stringify(message).to_utf16_buffer())
	pass



func sendOffer(id, data):
	var message = {
		"peer" : id,
		"orgPeer" : self.id,
		"message" : Message.offer,
		"data" : data,
		"lobby" : lobbyValue
	}
	peer.put_packet(JSON.stringify(message).to_utf16_buffer())

func sendAnswer(id, data):
	var message = {
		"peer" : id,
		"orgPeer" : self.id,
		"message" : Message.answer,
		"data" : data,
		"lobby" : lobbyValue
	}
	peer.put_packet(JSON.stringify(message).to_utf16_buffer())
