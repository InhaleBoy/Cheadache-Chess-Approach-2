extends Modifier
class_name DiceRollMod

var RNG = RandomNumberGenerator.new()

func _init():
	ModifiersBoard = load("res://board - dice roll/dice-roll board.tscn")

func _ready():
	RNG.randomize()

func Validate(current_piece,floating_piece,gotoTile) -> bool:
	
	Offer()
	
	if gotoTile == null:
		print("DiceRoll | there is no tile in this equation")
	if current_piece == null:
		print("DiceRoll | there is no piece in tile")
	if floating_piece == null:
		print("DiceRoll | no piece ?")
		return false
	
	DiceRoll(current_piece,floating_piece,gotoTile)
	
	return true

func Offer():
	pass

func Ansver(ans : bool):
	pass

signal roll(number)

func DiceRoll(current_piece : Piece, floating_piece : Piece, gotoTile : ChessTile):
	var DependanciesValue_MINValue = 19
	var DependanciesValue_RNGMAXValue = 20
	
	if current_piece != null and current_piece is King :
		DependanciesValue_RNGMAXValue *= 20
		
		DependanciesValue_MINValue = 19
		DependanciesValue_MINValue *= 20
	
	var IsTherePieceDependence = current_piece != null
	#var PieceColorDependence = floating_piece.IngameColor != current_piece.IngameColor
	#var PieceTypeDependance = 1
	
	
	var RNGValue = RNG.randi_range(0,DependanciesValue_RNGMAXValue)
	var result =  RNGValue >= DependanciesValue_MINValue
	
	print("DiceRoll attept | DPV_MINV = " + str(DependanciesValue_MINValue) + " | DPV_RNGMAX = " + str(DependanciesValue_RNGMAXValue) + " | RNGV = " + str(RNGValue))
	
	ModifiersBoard.SetNumber.rpc(RNGValue,DependanciesValue_MINValue)
	
	if result and !IsTherePieceDependence:
		floating_piece.move.rpc(gotoTile.tile_index)
	elif result and IsTherePieceDependence:
		current_piece.capture.rpc(gotoTile.tile_index)
		floating_piece.move.rpc(gotoTile.tile_index)
	Global.IngameColorToMoveNext_Change()
