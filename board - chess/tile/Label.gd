extends Label


# Called when the node enters the scene tree for the first time.
func _ready():
	var TileColor = get_parent().TileColor
	self.text = str(get_parent().tile_index)
	if TileColor: self.add_theme_color_override("font_color", Color.BLACK)
	else : self.add_theme_color_override("font_color", Color.WHITE)
	
	
