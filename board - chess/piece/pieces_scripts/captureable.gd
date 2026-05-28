extends Piece
class_name Captureable

func capture(gotoTile):
	var X = []
	
	if not gotoTile is Area2D:
		gotoTile = get_parent().TileArray[gotoTile[0]][gotoTile[1]]
	
	if IngameColor : X = gotoTile.tile_index[0]-1
	elif !IngameColor : X = gotoTile.tile_index[0]+1
	
	var TileObj = get_parent().TileArray[X][gotoTile.tile_index[1]]
	TileObj.current_piece.capture(TileObj)
	
	super.capture(gotoTile)

func MoveEventCallHandler():
	self.queue_free()

func setTexture(IngameColor : bool) -> void:
	if IngameColor : $ColorRect.color = Color.WHITE
	else : $ColorRect.color = Color.BLACK


#---------------------------------------------------------------------------------------------------
# CANT MOVE THIS THIS IS JUST THERE
func move(gotoTile):
	pass
func ColorTiles():
	pass
func ColorTilesBack():
	pass
func _input(event):
	pass
func _physics_process(delta):
	pass
func _on_area_2d_mouse_entered():
	pass
func _on_area_2d_mouse_exited():
	pass
