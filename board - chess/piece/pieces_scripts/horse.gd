extends Piece

func moveValid(gotoTile) -> bool:
	var x = self.CurrentTile.tile_index[0]
	var y = self.CurrentTile.tile_index[1]
	
	# TODO I GOD DAMN KNOW I CAN SIMPLIFY - but it works and i propably should not try to change that LMAO
	var a = 1
	for i in 2:
		if i == 1 : a *= -1
		if [x+(1 * a),y-(2 * a)] == gotoTile.tile_index :
			return true
		if [x+(2 * a),y-(1 * a)] == gotoTile.tile_index :
			return true
		if [x-(1 * a),y-(2 * a)] == gotoTile.tile_index :
			return true
		if [x-(2 * a),y-(1 * a)] == gotoTile.tile_index :
			return true
	return false

func captureValid(gotoTile) -> bool:
	if moveValid(gotoTile): return true
	return false

func setTexture(IngameColor : bool) -> void:
	if IngameColor : $Sprite2D.texture = load("res://board - chess/piece/textures/horse_white.png")
	else : $Sprite2D.texture = load("res://board - chess/piece/textures/horse_black.png")
