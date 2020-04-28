extends TileMap

# projectile scene
var laser

# board state information
var validShips = []	 # array of grid positions of movable ships
var playerShips = [] # array of ships
var enemyShips = []  # array of ships

# obstacle tile indices
var obstacle_tiles = [12, 13]

# active ship selection information
var selectedShip = null
var selectedRange = []
var attackTiles = [] 

# turn information
var playerTurn = true
var mouseTile = null	# tile under mouse

# async signal to resynchronize turns
signal computer_done

#endscreen confirmations
var defeatConfirm = false
var victoryConfirm = false

# runs when node (grid) is loaded
func _ready():
	laser = preload("Laser.tscn")
	var ships = get_children()
	for ship in ships:
		# round pixel positions to grid
		ship.position = map_to_world(world_to_map(ship.position))

		# 0: player
		# 1: computer
		if ship.team == 0:
			playerShips.append(ship)
		else:
			enemyShips.append(ship)
	draw_moves()




# calculates and updates available movement tiles
# should be called whenever ship selection or the grid is updated
func draw_moves():
	del_range(validShips)
	del_range(selectedRange)
	validShips.clear()

	# no movement if not player's turn.
	if !playerTurn: return


	# if ship is selected, draw movement range
	if selectedShip != null:
		#If ship has not had the change to upgrade, show upgrade menu
		if selectedShip.hasDoneUpgrade == false:
			selectedShip.hasDoneUpgrade = true;
			var upMenu = get_node("../UpgradeMenu");
			upMenu.call("showMenu", selectedShip);
		# draw selected ship movement range tiles
		selectedRange = range_check(selectedShip.range, world_to_map(selectedShip.position))
		add_range(selectedRange, "YellowTransparency")

	# draw ships with available moves
	for ship in playerShips:
		# ship can be moved
		if ship.range > 0:
			validShips.append(world_to_map(ship.position))
		# otherwise, check for attacks
		else:
			var attackRange = range_check(ship.maxRange, world_to_map(ship.position))
			for eShip in enemyShips:
				# at least one attackable ship
				if world_to_map(eShip.position) in attackRange:
					validShips.append(world_to_map(ship.position))
					break


	# highlight ships which can move
	if validShips.size() > 0:
		add_range(validShips, "YellowTransparency")
		# regular end turn button
		get_node("../Panel/EndTurn/Sprite").visible = false

	# no ships left; player's turn is over
	else:
		# highlight end turn button
		get_node("../Panel/EndTurn/Sprite").visible = true

	
# redraw red attack tiles on the grid
# called during player turn every time mouse moves over a new tile
# @param tile, tile coordinates under mouse (when a ship is selected)
func draw_attack(tile = null):
	# remove old tiles
	del_range(attackTiles, true)
	attackTiles.clear()

	# if no ship selected, no attacks to draw
	if selectedShip == null: return

	# if no mouse tile is provided, centre on the selected ship
	if tile == null:
		tile = world_to_map(selectedShip.position)
	
	# get array of cells which can be attacked
	var attackRange = range_check(selectedShip.maxRange, tile)
	attackRange.append(tile)

	# iterate enemy ships to find ones in range
	var shipTile
	for ship in enemyShips:
		shipTile = world_to_map(ship.position)
		if world_to_map(ship.position) in attackRange:
			attackTiles.append(shipTile)
	
	# draw attackable tiles in range on the grid
	add_range(attackTiles, "RedTransparency", true)

# draws range tiles on the grid
# @param moves, set of move/tile coordinates
# @param tileString, name of tile to use
# @param top, whether to draw tiles above everything (used for stacking tiles)
func add_range(moves, tileString, top = false):
	var tile = tile_set.find_tile_by_name(tileString)
	if !top:
		for cell in moves:
			set_cellv(cell, tile)
	else:
		var tileMap = get_node("../TileMapTop")
		for cell in moves:
			tileMap.set_cellv(cell, tile)

# removes/undraws range tiles from the grid
# @param moves, set of move/tile coordinates
# @param top, whether the tiles are in the topmost layer
func del_range(moves, top = false):
	if !top:
		for cell in moves:
			set_cellv(cell, -1)
	else:
		var tileMap = get_node("../TileMapTop")
		for cell in moves:
			tileMap.set_cellv(cell, -1)


# Receives a request to move a ship
# Ship1 ship, ship to move
# Vector2 target, target space coordinates
# return true if moved, false if blocked
func move(ship, target):
	# target is not in range
	if !target in range_check(ship.range, world_to_map(ship.position)):
		return false
	
	# calculate movement
	var vector = target - world_to_map(ship.position)
	var steps = abs(vector.x) + abs(vector.y)
	var distance = sqrt(vector.x*vector.x + vector.y*vector.y)

	# sound effect
	get_node("../SoundEffect/grid_interact").play();

	# prep movement
	var sprite = ship.get_node("Sprite")
	ship.range -= steps

	# align sprite
	sprite.look_at(map_to_world(target) + Vector2(16,16))
	sprite.rotation_degrees += 90
	
	# move ship
	ship.position = map_to_world(target)
	sprite.offset = Vector2(0, 32*distance)

	# animate movement
	var tween = ship.get_node("Tween")

	# animate sprite
	tween.interpolate_property(sprite, "offset", sprite.offset, Vector2(0,0), 0.2);
	tween.start();

	# wait for animation to complete
	yield(tween, "tween_completed")

	return true	

# handles request for an attack
# Ship ship1, attacking ship
# Ship ship2, attacked ship
# return true if hit, false if miss
func attack(ship1, ship2):
	
	# target ship is not in range
	if !(world_to_map(ship2.position) in range_check(ship1.maxRange, world_to_map(ship1.position))):
		return false

	# ship must have enough AP
	if ship1.AP < 1: return false

	# calculate distance
	var distance = ship2.position.distance_to(ship1.position)

	# sound effect
	get_node("../SoundEffect/attack_1").play()

	# create the projectile
	var projectile = laser.instance()
	add_child(projectile)
	projectile.position = ship1.position
	
	# align projectile
	var sprite = projectile.get_node("Sprite")
	sprite.look_at(ship2.position + Vector2(16,16))
	sprite.rotation_degrees += 90
	
	# animate projectile
	var tween = projectile.get_node("Tween")
	tween.interpolate_property(projectile, "position", ship1.position, ship2.position, distance/320);
	tween.start();
	
	# wait for animation to complete
	yield(tween, "tween_completed")

	# apply damage
	ship2.call("take_hit", projectile.firepower)
	ship1.AP = max(0, ship1.AP-projectile.cost)

	# handle destroyed ship
	if ship2.HP <= 0:
		# sound effect
		get_node("../SoundEffect/destroy_1").play()

		# remove from team list
		if ship2.team == 0:
			playerShips.remove(playerShips.find(ship2))
		else:
			enemyShips.remove(enemyShips.find(ship2))
			ship2.call("DropLoot")

		# remove from grid
		remove_child(ship2)

		check_victory()

	# cleanup projectile animation and return to player
	remove_child(projectile)
	
	

# handles all user input and consequently the player's turn
func _input(event):
	# block input if not user turn
	if !playerTurn: return

	# mouse movement
	if event is InputEventMouseMotion:

		# get mouse tile
		var tile = world_to_map(event.position)

		# update if mouse moved over a new tile
		if tile != mouseTile:
			mouseTile = tile
			on_mouse_moved(tile)

	# mouse click (up or down)
	elif event is InputEventMouseButton:
		var tile = world_to_map(event.position)

		# left click down
		if event.button_index == BUTTON_LEFT and event.is_pressed():
			# block player input and process (potentially async) click
			playerTurn = false
			on_left_clicked(tile)

			playerTurn = true
			# update movement tiles
			draw_moves()
			# update interface
			get_node("../Panel").update()

	# gridCS._Input(event)

# handles mouse movement
# called when a new tile is hovered over
# @param tile, grid coordinates of mouseover cell
func on_mouse_moved(tile):
	# update attackable cells
	draw_attack(tile)

	# hover over enemy ship to see stats
	var hover = selectedShip
	# find if any ship is hovered
	for ship in enemyShips + playerShips:
		# ship is under mouse
		if tile == world_to_map(ship.position):
			hover = ship
			break
	# update interface
	get_node("../Panel").update(hover)
		

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
		# (allowed and successful move )
		return
	
	return

# check whether a victory condition is met and end the match if necessary
func check_victory():
	# no enemy ships; player wins
	if enemyShips.size() == 0:
		# stop turn
		playerTurn = false

		# update the global state
		State.nextLevel()
		for ship in playerShips:
			ship.call("refundCurr");

		# end and return to level select
		yield(get_tree().create_timer(1), "timeout")
		
		get_node("../VictoryEnd").popup()
		#I'll put a soudn queue here later
		#We also could make the pop up have fancier graphics
		while(!victoryConfirm):
			yield(get_tree().create_timer(0.1), "timeout")
		get_tree().change_scene("res://level_select.tscn")

	# no player ships; player loses
	elif playerShips.size() == 0:
		# stop turn
		playerTurn = false
		# end and return to level select
		yield(get_tree().create_timer(1), "timeout")
		get_node("../DefeatEnd").popup()
		#play sound q
		while(!defeatConfirm):
			yield(get_tree().create_timer(0.1), "timeout")
		
		get_tree().change_scene("res://level_select.tscn")


# calculate which tiles are within range of a given location
# return an array of Vector2 tile locations
# @param rng, range in tiles
# @param tile, grid coordinate of central tile
# @return array of tiles within range
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

# handles "end turn" button press
func _on_end_turn():
	# cannot end turn if it isn't the player's turn
	if !playerTurn: return

	# end the player's turn
	playerTurn = false
	selectedShip = null
	draw_moves()

	# run the computer's turn
	comp_turn()
	check_victory()

	# wait for all ships to finish their turns
	yield(self, "computer_done")

	# restore per-turn ship points
	for ship in playerShips:
		ship.AP = ship.maxAP
		ship.range = ship.maxRange

	for ship in enemyShips:
		ship.AP = ship.maxAP
		ship.range = ship.maxRange

	# enable player's next turn
	playerTurn = true
	draw_moves()

# responsible for playing the computer's turn
func comp_turn():
	for ship in enemyShips:
		while ship.range > 0:
			# run individual ship AI
			ship.call("PlayTurn")

			# wait for movement and pause
			yield(get_tree().create_timer(0.4), "timeout")

	# let parent function know it can continue
	emit_signal("computer_done")





func _on_DefeatConfirm_pressed():
	defeatConfirm = true


func _on_VictoryConfirm_pressed():
	victoryConfirm = true
