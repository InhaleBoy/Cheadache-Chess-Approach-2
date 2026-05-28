extends Area2D
class_name ChessTile

var ChessBoard


var texture_load : Texture
var texture_lightup : Texture

var TileColor : bool
var tile_index : Array

func new(tile_index : Array, color : bool, position : Vector2, Chessboard:ChessBoard):
	self.ChessBoard = Chessboard
	self.position = position
	self.name = "tile " + str(tile_index)
	self.tile_index = tile_index
	self.TileColor = color
	
	texture_lightup = load("res://board - chess/tile/textures/test.tres")
	if color : self.texture_load = load("res://board - chess/tile/textures/white_tile.tres")
	else : self.texture_load = load("res://board - chess/tile/textures/black_tile.tres")
	$MeshInstance2D.texture = texture_load
	$ColorRect.visible = false

#---------------------------------------------------------------------------------------------------

func MoveHelpColoring(a : bool):
	if a : $ColorRect.visible = true
	else: $ColorRect.visible = false

#---------------------------------------------------------------------------------------------------
	
var floating_piece
var current_piece = null


func _process(delta):
	if floating_piece == null or floating_piece.dragging : return
	$MeshInstance2D.texture = texture_load

func _on_body_entered(body):
	if body.dragging :
		$MeshInstance2D.texture = texture_lightup
		floating_piece = body
		body.FloatingTile = self

func _on_body_exited(body):
	$MeshInstance2D.texture = texture_load
	floating_piece = null
