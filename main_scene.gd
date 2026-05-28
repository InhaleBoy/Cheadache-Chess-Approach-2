extends Node2D

@onready var WhoMovesColor = $CanvasLayer/GameControll/ColorRect


func _ready():
	Global.GameObject = self
	setColorToMoveNextAddnotations(true)
	for i in 4 :
		print(i)
		var mod = Global.GameMods[i]
		if !mod : continue
		$CanvasLayer/Boards.addBoard(mod)

func setColorToMoveNextAddnotations(IngameColorToMoveNext):
	if IngameColorToMoveNext:
		WhoMovesColor.color = Color.WHITE
	else:
		WhoMovesColor.color = Color.BLACK
