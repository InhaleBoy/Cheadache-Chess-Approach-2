extends Piece
class_name Queen

func moveValid(gotoTile) -> bool:
	if !Global.SemiInfiniteBasicChessMovmentCheck(get_parent(),CurrentTile,gotoTile): return false
	return true
	
func captureValid(gotoTile) -> bool:
	if moveValid(gotoTile): return true
	return false

func setTexture(IngameColor : bool) -> void:
	if IngameColor : $Sprite2D.texture = load("res://board - chess/piece/textures/queen_white.png")
	else : $Sprite2D.texture = load("res://board - chess/piece/textures/queen_black.png")
