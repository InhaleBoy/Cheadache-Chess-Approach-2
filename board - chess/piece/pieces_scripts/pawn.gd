extends Piece
class_name Pawn

var FirstMove2xBuff = true

func setTexture(IngameColor : bool) -> void:
	if IngameColor : $Sprite2D.texture = load("res://board - chess/piece/textures/pawn_white.png")
	else : $Sprite2D.texture = load("res://board - chess/piece/textures/pawn_black.png")

func _new(CurrentTile:Object,IngameColor:bool,Chessboard:ChessBoard) -> void:
	super._new(CurrentTile,IngameColor,Chessboard)
	UpgradePieceType = "queen"

func move(gotoTile):
	FirstMove2xBuff = false
	super.move(gotoTile)


func checkUpgrade() -> bool :
	if IngameColor and CurrentTile.tile_index[0] == 0 : return true
	elif !IngameColor and CurrentTile.tile_index[0] == 7 : return true
	return false

func moveValid(gotoTile) -> bool:
	if gotoTile.current_piece != null : return false
	#if !Global.SemiInfiniteBasicChessMovmentCheck(get_parent(),CurrentTile,gotoTile): return false
	if self.IngameColor && [self.CurrentTile.tile_index[0] - 1, self.CurrentTile.tile_index[1]] == gotoTile.tile_index :
		return true
	if !self.IngameColor && [self.CurrentTile.tile_index[0] + 1, self.CurrentTile.tile_index[1]] == gotoTile.tile_index :
		return true
	return false

func captureValid(gotoTile) -> bool:
	if self.IngameColor && ([self.CurrentTile.tile_index[0] - 1, self.CurrentTile.tile_index[1] - 1] == gotoTile.tile_index || [self.CurrentTile.tile_index[0] - 1, self.CurrentTile.tile_index[1] + 1] == gotoTile.tile_index):
		return true
	if !self.IngameColor && ([self.CurrentTile.tile_index[0] + 1, self.CurrentTile.tile_index[1] - 1] == gotoTile.tile_index || [self.CurrentTile.tile_index[0] + 1, self.CurrentTile.tile_index[1] + 1] == gotoTile.tile_index):
		return true
	return false

#---------------------------------------------------------------------------------------------------

func specialValid(gotoTile) -> bool:
	if gotoTile.current_piece != null : return false
	if IngameColor and FirstMove2xBuff and [self.CurrentTile.tile_index[0] - 2, self.CurrentTile.tile_index[1]] == gotoTile.tile_index : 
		return true
	if !IngameColor and FirstMove2xBuff and [self.CurrentTile.tile_index[0] + 2, self.CurrentTile.tile_index[1]] == gotoTile.tile_index :
		return true
	
	return false

func special(gotoTile,current_piece):
	gotoTile = get_parent().TileArray[gotoTile[0]][gotoTile[1]]
	self.move(gotoTile)
	EnPassant()

func EnPassant() : 
	var Tile = []
	
	if IngameColor : Tile = [CurrentTile.tile_index[0]+1,CurrentTile.tile_index[1]]
	else : Tile = [CurrentTile.tile_index[0]-1,CurrentTile.tile_index[1]]
	
	ChessBoard.addPiece(Tile,"captureable",Global.IngameColorToMoveNext)
