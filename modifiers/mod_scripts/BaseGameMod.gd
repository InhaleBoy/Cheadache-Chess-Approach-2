extends Modifier
class_name BaseGameMod

func _init():
	ModifiersBoard = load("res://board - chess/chess board.tscn")

func Validate(current_piece,floating_piece,gotoTile) -> bool:
	
	if gotoTile == null:
		print("BaseGameMod | there is no tile in this equation")
	if current_piece == null:
		print("BaseGameMod | there is no piece in tile")
	if floating_piece == null:
		print("BaseGameMod | no piece ?")
		return false
	
	if current_piece == null and floating_piece.moveValid(gotoTile):
		floating_piece.move.rpc(gotoTile.tile_index)
		Global.IngameColorToMoveNext_Change()
		return true
	elif current_piece != null and floating_piece.captureValid(gotoTile) and floating_piece.IngameColor != current_piece.IngameColor:
		current_piece.capture.rpc(gotoTile.tile_index)
		floating_piece.move.rpc(gotoTile.tile_index)
		Global.IngameColorToMoveNext_Change()
		return true
	elif floating_piece.specialValid(gotoTile):
		floating_piece.special.rpc(gotoTile.tile_index,current_piece)
		Global.IngameColorToMoveNext_Change()
		return true
	
	return false
