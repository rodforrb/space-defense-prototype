extends MarginContainer

var difficulty = State.difficulty
var volume = State.volume

func _ready():
	get_node("VBox except the labels show up/Volume/HSlider").value = volume
	
	if difficulty == 0: _on_diff_0_pressed()
	elif difficulty == 1: _on_diff_1_pressed()
	elif difficulty == 2: _on_diff_2_pressed()
	

func _on_Return_pressed():
	get_tree().change_scene("res://MainMenu.tscn")


func _on_HSlider_value_changed(value):
	volume = get_node("VBox except the labels show up/Volume/HSlider").value
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Master"), linear2db(volume/100.0))

# Easy
func _on_diff_0_pressed():
	State.difficulty = 0
	get_node("VBox except the labels show up/Difficulty/diff_0").modulate = Color(1, 1, 0, 1)
	get_node("VBox except the labels show up/Difficulty/diff_1").modulate = Color(0.7, 0.9, 1, 1)
	get_node("VBox except the labels show up/Difficulty/diff_2").modulate = Color(0.7, 0.9, 1, 1)

# Normal
func _on_diff_1_pressed():
	State.difficulty = 1
	get_node("VBox except the labels show up/Difficulty/diff_0").modulate = Color(0.7, 0.9, 1, 1)
	get_node("VBox except the labels show up/Difficulty/diff_1").modulate = Color(1, 1, 0, 1)
	get_node("VBox except the labels show up/Difficulty/diff_2").modulate = Color(0.7, 0.9, 1, 1)

# Hard
func _on_diff_2_pressed():
	State.difficulty = 2
	get_node("VBox except the labels show up/Difficulty/diff_0").modulate = Color(0.7, 0.9, 1, 1)
	get_node("VBox except the labels show up/Difficulty/diff_1").modulate = Color(0.7, 0.9, 1, 1)
	get_node("VBox except the labels show up/Difficulty/diff_2").modulate = Color(1, 1, 0, 1)
	
