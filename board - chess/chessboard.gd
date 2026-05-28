extends Board
class_name ChessBoard

@onready var tile = load("res://board - chess/tile/chess_tile.tscn")

#---------------------------------------------------------------------------------------------------

#-------------------------------------------------------------------------------

func _ready():
	Global.ChessBoardObject = self
	addTiles()
	addPieces()
	flipBoard(Global.YourColor)

func flipBoard(color : bool):
	
	if color :
		$"Tiles&Pieces".rotation = deg_to_rad(0)
		$"Tiles&Pieces".position.x = 50
		$"Tiles&Pieces".position.y = 50
	else :
		$"Tiles&Pieces".rotation = deg_to_rad(180)
		$"Tiles&Pieces".position.x = 1650
		$"Tiles&Pieces".position.y = 1650
		
	get_tree().call_group("Piece","DOAFLIP",!color)
	pass
	
	# Test

func _input(event):
	if Input.is_action_just_pressed("ui_page_down"):
		flipBoard(!BoardColor)
		BoardColor = !BoardColor

#---------------------------------------------------------------------------------------------------


func win(color : bool):
	process_mode = Node.PROCESS_MODE_DISABLED
	$WinNode/WinLabel.text = str(Global.color(color)) + " wins !!!"
	$WinNode.visible = true








func addTiles():
	var x_cond = 0
	var y_cond = 0
	var wORb : bool = true
	
	for i in 64:
		var tile_ins = tile.instantiate()
		
		if(i%8 == 0):
			wORb = !wORb
		wORb = !wORb
		
		if x_cond == 8:
			x_cond = 0
			y_cond += 1
		
		tile_ins.new([y_cond,x_cond],wORb,Vector2(x_cond * 200 + 100,y_cond * 200 + 100),self)
		
		$"Tiles&Pieces".add_child(tile_ins)
		$"Tiles&Pieces".TileArray[y_cond][x_cond] = tile_ins
		
		x_cond+=1

func addPieces():
	
	for i in 8:
		addPiece([1,i],"pawn",false)
		addPiece([6,i],"pawn",true)
	
	addPiece([0,4],"king",false)
	addPiece([7,4],"king",true)
	
	addPiece([0,3],"queen",false)
	addPiece([7,3],"queen",true)
	
	addPiece([0,0],"rook",false)
	addPiece([0,7],"rook",false)
	addPiece([7,0],"rook",true)
	addPiece([7,7],"rook",true)
	
	addPiece([0,1],"horse",false)
	addPiece([0,6],"horse",false)
	addPiece([7,1],"horse",true)
	addPiece([7,6],"horse",true)
	
	addPiece([0,2],"goniec",false)
	addPiece([0,5],"goniec",false)
	addPiece([7,2],"goniec",true)
	addPiece([7,5],"goniec",true)

func addPiece(tile_index:Array,type:String,color:bool):
	var goto_tile = $"Tiles&Pieces".TileArray[tile_index[0]][tile_index[1]]
	var piece_ins = load("res://board - chess/piece/pieces_scene/" + type + ".tscn").instantiate()
	if piece_ins == null:
		print(" addPiece() -> no scene with that path is present")
		return
	if  goto_tile == null:
		print(" addPiece() -> no tile with that coordinates is present")
		return
	if goto_tile.current_piece != null:
		print(" addPiece() -> there is a piece in way")
		return
	piece_ins._new(goto_tile,color,self)
	$"Tiles&Pieces".add_child(piece_ins)
