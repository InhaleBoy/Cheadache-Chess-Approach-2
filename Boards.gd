extends Control

func addBoard(modifier : Modifier):
	
	modifier.ModifiersBoard = modifier.ModifiersBoard.instantiate()
	
	print(modifier)
	
	$VBoxContainer/MainBoards.add_child(modifier.ModifiersBoard)
