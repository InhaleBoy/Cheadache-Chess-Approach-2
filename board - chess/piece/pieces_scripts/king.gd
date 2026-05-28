extends Piece
class_name King

var CastleValidMem : bool = true
var Castle_Rook1 : Piece
var Castle_Rook2 : Piece


#func setTexture(IngameColor : bool) -> void:
	#if IngameColor : $MeshInstance2D.texture = load("res://piece/textures/king_white.png")
	#else : $MeshInstance2D.texture = load("res://piece/textures/king_black.png")

func move(gotoTile):
	super.move(gotoTile)
	CastleValidMem = false

func capture(gotoTile):
	ChessBoard.win(!IngameColor)
	super.capture(gotoTile)

func moveValid(gotoTile) -> bool:
	
	if gotoTile.tile_index == [CurrentTile.tile_index[0],CurrentTile.tile_index[1] + 1] : return true
	if gotoTile.tile_index == [CurrentTile.tile_index[0],CurrentTile.tile_index[1] - 1] : return true
	
	if gotoTile.tile_index == [CurrentTile.tile_index[0] + 1,CurrentTile.tile_index[1]] : return true
	if gotoTile.tile_index == [CurrentTile.tile_index[0] + 1,CurrentTile.tile_index[1] + 1] : return true
	if gotoTile.tile_index == [CurrentTile.tile_index[0] + 1,CurrentTile.tile_index[1] - 1] : return true
	
	if gotoTile.tile_index == [CurrentTile.tile_index[0] - 1,CurrentTile.tile_index[1]] : return true
	if gotoTile.tile_index == [CurrentTile.tile_index[0] - 1,CurrentTile.tile_index[1] + 1] : return true
	if gotoTile.tile_index == [CurrentTile.tile_index[0] - 1,CurrentTile.tile_index[1] - 1] : return true
	
	return false
	
func captureValid(gotoTile) -> bool:
	if moveValid(gotoTile): return true
	return false

#---------------------------------------------------------------------------------------------------

func specialValid(gotoTile) -> bool:
	if !CastleValidMem : return false
	if gotoTile.current_piece != null : return false
	
	var X = CurrentTile.tile_index[0] # names might be none sense (X is Y btw XD)
	var Y = CurrentTile.tile_index[1] # names mighht have less than sense(Y is X btw XD)
	
	var Yadd
	
	if gotoTile.tile_index == [X,Y + 2] and get_parent().TileArray[X][Y+3].current_piece is Rook and get_parent().TileArray[X][Y + 3].current_piece.CastleValid : 
		Yadd = +3
	if gotoTile.tile_index == [X,Y - 2] and get_parent().TileArray[X][Y-4].current_piece is Rook and get_parent().TileArray[X][Y - 4].current_piece.CastleValid : 
		Yadd = -4
	
	if Yadd == null : return false 
	
	if Global.SemiInfiniteBasicChessMovmentCheck(get_parent(),CurrentTile,get_parent().TileArray[X][Y + Yadd]): return true
		
	return false

func special(gotoTile,current_piece):
	gotoTile = get_parent().TileArray[gotoTile[0]][gotoTile[1]]
	if !Castle(gotoTile) : return
	self.move(gotoTile)

func Castle(gotoTile) -> bool :
	if !CastleValidMem : return false
	
	var MoveTile1_index = [CurrentTile.tile_index[0],CurrentTile.tile_index[1] + 2]
	var MoveTile2_index = [CurrentTile.tile_index[0],CurrentTile.tile_index[1] - 2] 
	var RookCurrTile
	var RookMoveTile
	
	if gotoTile.tile_index == MoveTile1_index :
		RookCurrTile = get_parent().TileArray[MoveTile1_index[0]][MoveTile1_index[1]+1]
		RookMoveTile = get_parent().TileArray[MoveTile1_index[0]][MoveTile1_index[1]-1]
	elif gotoTile.tile_index == MoveTile2_index :
		RookCurrTile = get_parent().TileArray[MoveTile2_index[0]][MoveTile2_index[1]-2]
		RookMoveTile = get_parent().TileArray[MoveTile2_index[0]][MoveTile2_index[1]+1]
	
	print(RookCurrTile)
	print(RookCurrTile.current_piece)
	print(RookMoveTile)
	
	
	if RookCurrTile.current_piece is Rook :
		RookCurrTile.current_piece.move(get_parent().TileArray[RookMoveTile.tile_index[0]][RookMoveTile.tile_index[1]])
		return true
	
	return false
