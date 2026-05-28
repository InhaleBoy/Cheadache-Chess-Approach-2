extends Board

@rpc("any_peer","call_local")
func SetNumber(value,cap):
	$Content/Label.text = str(value) + " / " + str(cap)
