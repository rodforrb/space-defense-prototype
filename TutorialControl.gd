extends TileMap

# end tutorial and go to first level
func _on_EndTurn_pressed():
	get_tree().change_scene("res://Level1.tscn")
