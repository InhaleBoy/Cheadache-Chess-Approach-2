extends Piece
class_name Bishop

func moveValid(gotoTile) -> bool:
	if !Global.SemiInfiniteBasicChessMovmentCheck(get_parent(),CurrentTile,gotoTile): return false
	if ((gotoTile.tile_index[0] == CurrentTile.tile_index[0]) or (gotoTile.tile_index[1] == CurrentTile.tile_index[1])) : return false
	return true
	
func captureValid(gotoTile) -> bool:
	if moveValid(gotoTile): return true
	return false

func setTexture(IngameColor : bool) -> void:
	if IngameColor : $Sprite2D.texture = load("res://board - chess/piece/textures/goniec_white.png")
	else : $Sprite2D.texture = load("res://board - chess/piece/textures/goniec_black.png")
