extends RefCounted
class_name Lobby

var HostID : int
var Players : Dictionary = {}
var mods = []

func _init(id,mods):
	HostID = id
	addMods(mods)
func addPlayer(id, name):
	var color : bool = false
	if Players.is_empty():
		color = true
	Players[id] = {
		"id" : id,
		"name" : name,
		"index" : Players.size() + 1,
		"color" : color
	}
	return Players[id]

func addMods(mods):
	for i in mods:
		if self.mods.has(i):
			pass
	
func removeMods(mods):
	pass
