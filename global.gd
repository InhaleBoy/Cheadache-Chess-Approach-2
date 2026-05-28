extends Node

var YourColor : bool = true

var ChessBoardObject : ChessBoard
var GameObject : Object

var GameMods = {
	0 : BaseGameMod.new(),
	1 : DiceRollMod.new(),
	2 : null,
	3 : null
}

var IngameColorToMoveNext = true
func IngameColorToMoveNext_Change() -> void:
	ICTMN_Change_RPC.rpc()

@rpc("any_peer","call_local")
func ICTMN_Change_RPC():
	IngameColorToMoveNext = !IngameColorToMoveNext
	if GameObject ==  null : return;
	GameObject.setColorToMoveNextAddnotations(IngameColorToMoveNext)


#---------------------------------------------------------------------------------------------------


func color(color:bool) -> String:
	if color: return "white"
	else: return "black"


func SemiInfiniteBasicChessMovmentCheck(tiles_and_pieces,from_tile,to_tile) -> bool:
	if from_tile == to_tile : return false 
	
	var TileArray = tiles_and_pieces.TileArray
	
	var from_point = [from_tile.tile_index[0],from_tile.tile_index[1]]
	var to_point = to_tile.tile_index
	var direction = [to_point[0] - from_point[0], to_point[1] - from_point[1]]
	
	var vector = [0,0]
	if direction[0] != 0 : vector[0] = direction[0]/abs(direction[0])
	if direction[1] != 0 : vector[1] = direction[1]/abs(direction[1])
	
	#print(str(from_point) + " | " + str(to_point) + " | " + str(vector))
	#print(str(from_tile.tile_index) + " | " + str(to_tile.tile_index) + " | " + str(vector))
	
	while from_point != to_point:
		#print(from_point)
		
		if from_point[0] > 7 or from_point[0] < 0 or from_point[1] > 7 or from_point[1] < 0 : 
			return false
		
		if from_point != from_tile.tile_index and TileArray[from_point[0]][from_point[1]].current_piece != null:
			return false 
		
		from_point[0] += vector[0]
		from_point[1] += vector[1]
		
	return true
