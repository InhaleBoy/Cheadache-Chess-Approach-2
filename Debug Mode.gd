extends Control

func _input(event):
	if Input.is_action_just_pressed("ui_end"):
		pass
	if Input.is_action_just_pressed("ui_home") :
		Global.YourColor = !Global.YourColor
