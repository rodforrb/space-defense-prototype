extends TileMap
var gridCS
var grid
var laser

var validShips = []	 # array of grid positions
var playerShips = [] # array of ships
var enemyShips = []  # array of ships

var obstacle_tiles = [12, 13]

var selectedShip = null
var selectedRange = []

var playerTurn = true
var mouseTile = null


func _ready():
	laser = preload("Laser.tscn")
	var ships = get_children()
	for ship in ships:
		# 0: player
		# 1: computer
		if ship.team == 0:
			playerShips.append(ship)
		else:
			enemyShips.append(ship)
	
	draw_moves()
	
func draw_moves():
	del_range(validShips)
	del_range(selectedRange)
	validShips.clear()

	if selectedShip != null:
		# draw selected ship movement range tiles
		selectedRange = range_check(selectedShip.range, world_to_map(selectedShip.position))
		add_range(selectedRange, "YellowTransparency")

	for ship in playerShips:
		if ship.range > 0:
			validShips.append(world_to_map(ship.position))
		
		# if ship can only attack, check if any enemy ships are within range
		# elif ship.AP > 2:
		# 	for compShip in enemyShips:
		# 		
	
	# highlight ships which can move
	if validShips.size() > 0:
		add_range(validShips, "YellowTransparency")
	

		
func add_range(moves, tileString):
	var tile = tile_set.find_tile_by_name(tileString)
	for cell in moves:
		set_cellv(cell, tile)

func del_range(moves):
	for cell in moves:
		set_cellv(cell, -1)
		
# Receives a request to move a ship
# Ship1 ship, ship to move
# Vector2 target, target space coordinates
# return true if moved, false if blocked
func move(ship, target):
	# target is not in range
	if !target in range_check(ship.range, world_to_map(ship.position)):
		return false
	
	var vector = target - world_to_map(ship.position)
	var distance = int(sqrt(abs(vector.x) + abs(vector.y)))

	# sound effect
	get_node("../SoundEffect/grid_interact").play();

	# prep movement
	var sprite = ship.get_node("Sprite")
	sprite.look_at(map_to_world(target) + Vector2(16,16))
	sprite.rotation_degrees += 90

	# move ship
	ship.position = map_to_world(target)

	# animate movement
	var tween = ship.get_node("Tween")
	tween.interpolate_property(sprite, "offset", Vector2(0, distance*32), Vector2(0,0), 0.2*distance);
	ship.position = map_to_world(target)
	tween.start();

	return true

# handles request for an attack
# Ship ship1, attacking ship
# Ship ship2, attacked ship
# return true if hit, false if miss
func attack(ship1, ship2):
	# target ship is not in range
	if !(world_to_map(ship2.position) in range_check(ship1.range, world_to_map(ship1.position))):
		return false

	if ship1.AP < 1: return false

	# proceed with attack
	playerTurn = false

	var distance = ship2.position.distance_to(ship1.position)

	# sound effect
	get_node("../SoundEffect/attack_1").play()

	# create the object
	var projectile = laser.instance()
	add_child(projectile)
	projectile.position = ship1.position
	
	var sprite = projectile.get_node("Sprite")
	sprite.look_at(ship2.position + Vector2(16,16))
	sprite.rotation_degrees += 90
	
	# animate projectile
	var tween = projectile.get_node("Tween")
	tween.interpolate_property(projectile, "position", ship1.position, ship2.position, distance/320);
	tween.start();
	
	yield(tween, "tween_completed")
	remove_child(projectile)
	playerTurn = true
	
	

# handles all user input and consequently the player's turn
func _input(event):
	# block input if not user turn
	if !playerTurn: return

	# mouse movement
	if event is InputEventMouseMotion:
		# currently nothing happens if no ship selected
		if selectedShip == null: return

		var tile = world_to_map(event.position)

		# update if mouse moved over a new tile
		if tile != mouseTile:
			mouseTile = tile
			on_mouse_moved(event)

	# mouse click (up or down)
	elif event is InputEventMouseButton:
		var tile = world_to_map(event.position)

		# left click down
		if event.button_index == BUTTON_LEFT and event.is_pressed():
			on_left_clicked(tile)

			# update movement tiles
			draw_moves()

	# gridCS._Input(event)

# handles mouse movement
# called when a new tile is hovered over
func on_mouse_moved(tile):
	pass

# handles left clicks
# called when left mouse button is pressed down
func on_left_clicked(tile):

	# if the selected ship is clicked again, deselect it
	if selectedShip != null and world_to_map(selectedShip.position) == tile:
		selectedShip = null
		
		# play sound effect
		get_node("../SoundEffect/grid_interact").play()
		
		return

	# find what was clicked
	# check player ships
	for ship in playerShips:
		# a new ship was clicked
		if tile == world_to_map(ship.position):
			# select ship
			selectedShip = ship

			# play sound effect
			get_node("../SoundEffect/grid_interact").play()

			return
	
	# if no ship is selected, cannot move or attack
	if selectedShip == null: return

	# continue to check enemy ships
	for ship in enemyShips:
		# enemy ship was attacked
		if tile == world_to_map(ship.position):
				# try to attack
				attack(selectedShip, ship)
				return
	
	# lastly, try to move to empty space
	# inner block runs if moved successfully
	if move(selectedShip, tile):
		return
	
	return

# called when end turn button is pressed
func on_end_turn():
	pass

func comp_turn():
	pass

func check_victory():
	pass

# calculate tiles within range of a given location
# return an array of Vector2 tile locations
func range_check(rng, tile, mainX = 0, mainY = 0, aduX = 0, aduY = 0):
	var locations = []
	if mainX == 0 and mainY == 0:
		locations += (range_check(rng-1, Vector2(tile.x+1, tile.y),1,0,0,1));
		locations += (range_check(rng-1, Vector2(tile.x-1, tile.y),-1,0,0,1));
		locations += (range_check(rng-1, Vector2(tile.x, tile.y+1),0,1,1,0));
		locations += (range_check(rng-1, Vector2(tile.x, tile.y-1),0,-1,1,0));
	elif rng >= 0:
		# check if space can be moved to
		if !get_cellv(tile) in obstacle_tiles:
			locations.append(tile);
			locations += (range_check(rng-1, Vector2(tile.x-aduX, tile.y-aduY),mainX,mainY,aduX,aduY));
			locations += (range_check(rng-1, Vector2(tile.x+aduX, tile.y+aduY),mainX,mainY,aduX,aduY));
			locations += (range_check(rng-1, Vector2(tile.x+mainX, tile.y+mainY),mainX,mainY,aduX,aduY));
	
	return locations
