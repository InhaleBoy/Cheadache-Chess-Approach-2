extends Control

var TileArray := Array()

func _ready():
	for i in range(8):
		TileArray.append([]) 
		for j in range(8):
			TileArray[i].append(j) 
