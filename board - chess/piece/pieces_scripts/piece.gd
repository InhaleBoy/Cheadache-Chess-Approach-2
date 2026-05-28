extends CharacterBody2D
class_name Piece

var ChessBoard

var IngameColor : bool # true = white | black = false
var CurrentTile

func _new(CurrentTile:Object,IngameColor:bool,Chessboard:ChessBoard) -> void:
	self.ChessBoard = Chessboard
	self.CurrentTile = CurrentTile
	self.IngameColor = IngameColor
	setTexture(IngameColor)
	self.position = CurrentTile.position
	CurrentTile.current_piece = self

@rpc("any_peer","call_local")
func capture(gotoTile):
	if not gotoTile is Area2D:
		gotoTile = get_parent().TileArray[gotoTile[0]][gotoTile[1]]
	gotoTile.current_piece = null
	self.queue_free()

@rpc("any_peer","call_local")
func move(gotoTile):
	if not gotoTile is Area2D:
		gotoTile = get_parent().TileArray[gotoTile[0]][gotoTile[1]]
	self.CurrentTile.current_piece = null
	self.CurrentTile = gotoTile
	gotoTile.current_piece = self
	self.position = gotoTile.position
	if checkUpgrade() : ForceUpgrade(UpgradePieceType)
	get_tree().call_group("Piece","MoveEventCallHandler")

@rpc("any_peer","call_local")
func special(gotoTile,current_piece):
	pass

#---------------------------------------------------------------------------------------------------

#TODO place move/capture valid chceck in child
# help -> var IngameColor | var CurrentTile

func moveValid(gotoTile) -> bool:
	return false
func captureValid(gotoTile) -> bool:
	return false
func specialValid(gotoTile) -> bool:
	return false

#---------------------------------------------------------------------------------------------------

var UpgradePieceType = ""
func checkUpgrade() -> bool :
	return false
func ForceUpgrade(PieceType): 
	CurrentTile.current_piece = null
	ChessBoard.addPiece(CurrentTile.tile_index,UpgradePieceType,IngameColor)
	self.queue_free()

#---------------------------------------------------------------------------------------------------

func setTexture(IngameColor : bool) -> void:
	pass

func DOAFLIP(color : bool) :
	$Sprite2D.flip_v = color

@rpc("any_peer","call_local")
func ForceChangeColor() :
	IngameColor = !IngameColor
	setTexture(IngameColor)

func ChangeColor(color : bool):
	IngameColor = color
	setTexture(color)

#---------------------------------------------------------------------------------------------------

func MoveEventCallHandler():
	pass

#---------------------------------------------------------------------------------------------------



#---------------------------------------------------------------------------------------------------
var FloatingTile : ChessTile = null

func Action():
	if FloatingTile == null : return
	var floating_piece = FloatingTile.floating_piece
	var current_piece = FloatingTile.current_piece
	
	# floating piece is self ?
	
	if IngameColor != Global.IngameColorToMoveNext : return
	
	if CurrentTile == FloatingTile : return
	
	for i in 4 : 
		print(i)
		
		if Global.GameMods[i] != null and Global.GameMods[i].Validate(current_piece,floating_piece,FloatingTile):
			break
	
	FloatingTile = null


#---------------------------------------------------------------------------------------------------
# Light up tiles to help with where they can go
@export var HelpTileLightup : bool = true

func ColorTiles():
	for i in 8:
		for x in 8:
			var tile_node = get_parent().TileArray[i][x]
			if tile_node.current_piece != null and tile_node.current_piece.IngameColor == self.IngameColor :
				continue # cant move or capture if your own color piece is there
			if tile_node.current_piece == null and moveValid(tile_node) : tile_node.MoveHelpColoring(true)
			elif tile_node.current_piece != null and captureValid(tile_node) : tile_node.MoveHelpColoring(true)
			elif specialValid(tile_node) : tile_node.MoveHelpColoring(true)

func ColorTilesBack():
	for i in 8:
		for x in 8:
			get_parent().TileArray[i][x].MoveHelpColoring(false)

#---------------------------------------------------------------------------------------------------
# "follow the mouse" part
var dragging
var mouse_in = false

func _input(event):
	if Global.YourColor != IngameColor : return
	if event is InputEventMouseButton:
		if event.is_pressed() && mouse_in:
			print("HERE")
			if HelpTileLightup : ColorTiles()
			dragging = true
		else:
			if HelpTileLightup : ColorTilesBack()
			#restart to the position it was before if it didnt know where to go
			position = CurrentTile.position
			dragging = false
			Action()
			
	elif event is InputEventMouseMotion:
		if dragging:
			if HelpTileLightup : ColorTiles()

func _physics_process(delta):
	#TODO better mouse movements coz they stupid af 
	if dragging:
		velocity = Input.get_last_mouse_velocity()*2
		move_and_slide()

func _on_area_2d_mouse_entered():
	mouse_in = true

func _on_area_2d_mouse_exited():
	mouse_in = false
