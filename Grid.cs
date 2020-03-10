using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
	
/*	This is the main code for controlling objects on the Grid */
public class Grid : TileMap
{
	// currently selected node/sprite
	private Ship1 selected = null;
	// mouse position tracker, uses grid coordinates
	private Vector2 mouseTile = new Vector2(0,0);

	private Vector2[] validMoves = null;
	private Vector2[] atkRange = null;
	private Vector2[] mouseRange = null;
	private List<Vector2> validShips = new List<Vector2>();

	private bool victory = false;
	private bool defeat = false;

	//array of tilemap indecies for obstacles
	private int[] obst = new int[]{12,13};

	public int gridSize = 32;
	private bool playerTurn = true;
	public List<Ship1> playerShips {get;} = new List<Ship1>();
	public List<CompShip> computerShips {get;} = new List<CompShip>();
	
	public PackedScene _bullet = ResourceLoader.Load("Bullets.tscn") as PackedScene;
	//public Node bullet_conatianer = GetNode("bullet_container");
	
	//https://docs.godotengine.org/en/3.1/getting_started/workflow/best_practices/scenes_versus_scripts.html
	//Based on the information provided in the above link, I will define the animated projectiles as a seperate scene
	//it may also be a good idea to make the ships other scenes as well.
	//public PackedScene _bullet = (PackedScene)ResourceLoader.load("Bullets.tscn", "", false);
	
	/* Used to get instantiated Grid object
	 * Unfortunately GetNode cannot be used by a static class.
	 * @return Grid
	*/
	public Grid GetGrid ()
	{
		return (GetNode<Grid>("/root/Game/Grid"));
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// iterate children of this node (all the Node2D characters)
		//var _bullet = ResourceLoader.Load("Bullets.tscn") as PackedScene;
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);

			// rounding to fix grid alignment issues from editor
			child.SetPosition(MapToWorld(WorldToMap(child.GetPosition())));
			
			/* Sometimes it's better to ask for forgiveness.
			 * See if the child is a ship by trying to convert it to a ship.
			 * If it fails, it wasn't a ship.
			 * CompShip check must be before Ship1 check because of inheritance, and class-specific variables are wrong when casting.
			*/
			try
			{
				CompShip childShip = (CompShip)child;
				computerShips.Add((CompShip)childShip);

			} catch (System.Exception e) {
				// not a compship, maybe a Ship1
				try{
					Ship1 childShip = (Ship1)child;
					playerShips.Add(childShip);

				} catch (System.Exception e2) {
					// it wasn't a ship so don't worry about it
				}
			}

			// WorldToMap converts pixel coordinates to grid coordinates
			GD.Print("Node loaded: ", child, " at ", WorldToMap(child.GetPosition()));
		}
		DrawMoves();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	
	/* returns a vector array of cells in range of a given position
	* int range - radius around central position
	* Vector2 currentPos - central position
	* int mainX - used to recursively check main axis. Should not be included in initial function call
	* int mainY - used to recursively check main axis. Should not be included in initial function call
	* int aduX - used to recursively check secondary axis. Should not be included in initial function call
	* int aduY - used to recursively check secondary axis. Should not be included in initial function call
	* return Vector2[] of positions within range
	*/

	private Vector2[] RangeCheck(int range,Vector2 currentPos, int mainX = 0, int mainY = 0, int aduX = 0, int aduY = 0)
	{
		List<Vector2> allLocations = new List<Vector2>();
		//being checking along the main axis
		if(mainX == 0 && mainY == 0)
		{
			
			allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x+1,currentPos.y),1,0,0,1));
			allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x-1,currentPos.y),-1,0,0,1));
			allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x,currentPos.y+1),0,1,1,0));
			allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x,currentPos.y-1),0,-1,1,0));
		//Checks all spaces around current location. Then "moves" to those spaces and repates the process until the process is complete
		}else if(range>=0){
			bool isMovable = true;
			foreach (int obs in obst){
				if(GetCellv(currentPos) == obs){
						isMovable = false;
				}
			}
			if(isMovable)
				{	
					allLocations.Add(currentPos);
					allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x-aduX,currentPos.y-aduY),mainX,mainY,aduX,aduY));
					allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x+aduX,currentPos.y+aduY),mainX,mainY,aduX,aduY));
					allLocations.AddRange(RangeCheck(range-1,new Vector2(currentPos.x+mainX,currentPos.y+mainY),mainX,mainY,aduX,aduY));
				}

		}	

		return allLocations.ToArray();
	}


	private void addRange(Vector2[] moves, String tileString)
	{
		int tile = TileSet.FindTileByName(tileString);
		for(int i = 0; i < moves.Length; i++)
		{
			SetCellv(moves[i], tile);
		}
	}

	private void addRange(Vector2[] moves, String tileString, TileMap tileMap)
	{
		int tile = TileSet.FindTileByName(tileString);
		for(int i = 0; i < moves.Length; i++)
		{
			tileMap.SetCellv(moves[i], tile);
		}
	}

	// Removes all blue tiles from the grid 
	private void removeRange(Vector2[] moves)
	{
		if (moves == null) return;
		// int tile = TileSet.FindTileByName("Sprite");
		for(int i = 0; i < moves.Length; i++){
			
			SetCellv(moves[i], -1);
		}
	}

	// Removes all blue tiles from the grid 
	private void removeRange(Vector2[] moves, TileMap tileMap)
	{
		if (moves == null) return;
		// int tile = TileSet.FindTileByName("Sprite");
		for(int i = 0; i < moves.Length; i++){
			
			tileMap.SetCellv(moves[i], -1);
		}
	}

	/* Receives a request to move a ship
	*  Ship1 ship, ship to move
	*  Vector2 target, target space coordinates
	*  return true if moved, false if blocked
	*/
	public bool Move(Node2D ShipNode, Vector2 target)
	{
		Ship1 ship = (Ship1)ShipNode;
		Sprite shipSprite = ship.GetNode<Sprite>("Sprite");
		// check if target position is out of range
		if (!Array.Exists(RangeCheck(ship.range, WorldToMap(ship.Position)), element => element == target))
			return false;

    // calculate x+y movement distance
		Vector2 vector =  target - WorldToMap(ship.Position);
		Vector2 intVector = new Vector2((int)vector.x, (int)vector.y);
		int distance = (int) (Math.Abs(intVector.x) + Math.Abs(intVector.y));
		//plays a sound effect for valid move
		//known issue: cuts if destruciton ends game
		//use signal, _on_<sound name>_finished() method
		//This specific issue will likely be handled in end game screens
		AudioStreamPlayer grid_interact = (AudioStreamPlayer) GetNode("/root/Game/SoundEffect/grid_interact");
        grid_interact.Play();		
		// move ship along path to new position
		while (WorldToMap(ship.Position) != target)
		{
			GD.Print(intVector.x, ',', intVector.y);
			// more vertical distance to travel
			if (Math.Abs(intVector.x) < Math.Abs(intVector.y))
			{
				// move 1 unit in y direction
				ship.SetPosition(ship.Position + new Vector2(0,intVector.y*gridSize/Math.Abs(intVector.y)));

				// rotate ship
				if (intVector.y < 0) 
					shipSprite.RotationDegrees = 0;
				else 
					shipSprite.RotationDegrees = 180;
			}
			else // move horizontally
			{
				// move 1 unit in x direction
				ship.SetPosition(ship.Position + new Vector2(intVector.x*gridSize/Math.Abs(intVector.x), 0));

				// rotate ship
				if (intVector.x < 0)
					shipSprite.RotationDegrees = 270;
				else
					shipSprite.RotationDegrees = 90;
			}
			ship.GetNode<AnimationPlayer>("AnimationPlayer").Play("Move");
			

			// recalculate x+y movement distance
			vector = target - WorldToMap(ship.Position);
			intVector = new Vector2((int)vector.x, (int)vector.y);

			// pause on each tile briefly
			// TODO
		}

		ship.range -= distance;

		return true;
	}

	/* Receives a request to attack a target
	* Ship1 ship, attacking ship
	* Vector2 target, grid coordinates to attack
	* ProjectileType projType
	* return true if hit, false if miss
	*/
	public void Attack(Ship1 attacker, Ship1 defender, Projectile proj) //TODO
	{
		// target out of range
		if (!Array.Exists(RangeCheck(attacker.getAttackRange(), WorldToMap(attacker.Position)), element => element == WorldToMap(defender.Position))) return;
	
		// not enough points to attack
		if (attacker.AP < 1) return;

		//plays a sound effect on good attack
		AudioStreamPlayer attack_1 = (AudioStreamPlayer) GetNode("/root/Game/SoundEffect/attack_1");
        attack_1.Play();

		// consume points and proceed with attacking
		attacker.AP = Math.Max(0, attacker.AP-1);

		int f = attacker.firepower * proj.firepower;
		int p = attacker.penetration * proj.penetration;
		int a = attacker.accuracy * proj.accuracy;
		
		defender.take_damage(f, p, a);
		// remove defender if no HP
		if (defender.HP <= 0)
		{
			RemoveChild(defender);
			//boom sound on death
			AudioStreamPlayer destroy_1 = (AudioStreamPlayer) GetNode("/root/Game/SoundEffect/destroy_1");
			destroy_1.Play();	   
			// remove from the appropriate list
			try
			{
				computerShips.Remove((CompShip)defender);
				
			} catch (System.InvalidCastException e) {
				playerShips.Remove(defender);
			}
		}
		CheckVictory();
		return;
	}

	/* Draws or undraws highlighted move indicator tiles
	*/
	public void DrawMoves()
	{
		// remove old range tiles
		removeRange(this.mouseRange, GetNode<TileMap>("/root/Game/TileMapTop"));
		removeRange(this.validMoves);
		removeRange(this.atkRange);
		
		removeRange(validShips.ToArray());
		this.validShips.Clear();

		// check if any ships have moves left
		foreach (Ship1 ship in playerShips)
		{
			// ship can move
			if (ship.range > 0)
			{
				this.validShips.Add(WorldToMap(ship.Position));
			}
			else if (ship.AP > 2) // TODO ensure ship has enough AP for an attack
			{
				// otherwise see if there is an attackable ship
				foreach (CompShip compShip in computerShips)
				{
					if (Array.Exists(atkRange, element => element == WorldToMap(compShip.Position)))
						this.validShips.Add(WorldToMap(ship.Position));
						break;
				}
			}
		}
		// if some ships have moves remaining
		if (validShips.Count > 0)
		{
			// highlight ships which can move or attack
			addRange(this.validShips.ToArray(), "YellowTransparency");

			
			GetNode<Sprite>("/root/Game/Panel/EndTurn/Sprite").Visible = false;
		} else {
			// highlight EndTurn button if no moves
			GetNode<Sprite>("/root/Game/Panel/EndTurn/Sprite").Visible = true;
		}

		// ship is selected, new tiles should be drawn
		if (this.selected != null)
		{
			//Populates the valid moves for the selected ship
			this.validMoves = RangeCheck(selected.GetRange(), WorldToMap(selected.GetPosition()));
			// draw movement range
			addRange(this.validMoves, "BlueTransparency");
			
			// get array of attackable spaces
			this.atkRange = RangeCheck(selected.getAttackRange(), WorldToMap(selected.Position));
			// create list of attackable ships
			List<Vector2> shipsInRange = new List<Vector2>();
			foreach (CompShip compShip in computerShips)
			{
				if (Array.Exists(atkRange, element => element == WorldToMap(compShip.Position)))
					shipsInRange.Add(WorldToMap(compShip.Position));
			}

			// rewrite atkRange tiles to only be on ships
			this.atkRange = shipsInRange.ToArray();
			addRange(atkRange, "RedTransparency");

			// copy selected ship's sprite to UI panel
			GetNode<Sprite>("/root/Game/Panel/PanelContainer/PanelSprite").Texture = this.selected.GetTexture();
			
			Vector2 barTile;
			// draw HP bars on UI panel
			int numBars = (int)Math.Ceiling(6 * (float)selected.HP / (float)selected.maxHP);
			while (numBars > 0)
			{
				barTile = new Vector2(4 + numBars, 16);
				SetCellv(barTile, TileSet.FindTileByName("HPBar"));

				numBars--;
			}				
			
			// remove old ap bars
			for (int x = 5; x <= 10; x++)
			{
				barTile = new Vector2(x+9, 16);
				SetCellv(barTile, -1);			
			}				
			// draw AP bars on UI panel
			numBars = (int)Math.Ceiling(6 * (float)selected.range / (float)selected.maxRange);
			while (numBars > 0)
			{
				barTile = new Vector2(13 + numBars, 16);
				SetCellv(barTile, TileSet.FindTileByName("HPBar"));

				numBars--;
			}				
			// set HP text, insert two spaces for alignment with low numbers because I can't find the align setting
			GetNode<RichTextLabel>("/root/Game/Panel/HPLabel/HP").Text = (selected.HP < 10 ? "  " : "") + selected.HP;
			GetNode<RichTextLabel>("/root/Game/Panel/HPLabel/HPMax").Text = selected.maxHP.ToString();
						
			// set Range text 
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel/Range").Text = (selected.range < 10 ? "  " : "") + selected.range.ToString();
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel/MaxRange").Text = selected.maxRange.ToString();
			
			// set AP text 
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel2/AP").Text = (selected.AP < 10 ? "  " : "") + selected.AP.ToString();
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel2/APMax").Text = selected.maxAP.ToString();
			
			

		}  
		// no-ship-selected cleanup
		else 
		{
			GetNode<Sprite>("/root/Game/Panel/PanelContainer/PanelSprite").Texture = null;
			// remove hp and ap bars
			Vector2 barTile;
			for (int x = 5; x <= 10; x++)
			{
				barTile = new Vector2(x, 16);
				SetCellv(barTile, -1);			
				barTile = new Vector2(x+9, 16);
				SetCellv(barTile, -1);			
			}				
			// unset hp text
			GetNode<RichTextLabel>("/root/Game/Panel/HPLabel/HP").Text = "  0";
			GetNode<RichTextLabel>("/root/Game/Panel/HPLabel/HPMax").Text = "0";

			// uset Range text 
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel/Range").Text = "  0";
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel/MaxRange").Text = "0";
			
			// uset AP text 
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel2/AP").Text = "  0";
			GetNode<RichTextLabel>("/root/Game/Panel/APLabel2/APMax").Text = "0";
		}
	}

	/* Called whenever there is user input
	*  consequently this manages all actions for the player's turn
	*/
	public override void _Input(InputEvent @event)
	{
		// ignore user input while it is not their turn
		if (!this.playerTurn) return;

		// mouseover / mouse moved event
		if (@event is InputEventMouseMotion mouseMoved)
		{
			// currently nothing happens if no ship selected
			if (this.selected == null) return;

			Vector2 newMouseTile = WorldToMap(mouseMoved.Position);
			if (newMouseTile != mouseTile)
			{
				this.mouseTile = newMouseTile;
				// check if mouse position is in ship movement range
				if (Array.Exists(RangeCheck(selected.range, WorldToMap(selected.Position)), element => element == mouseTile))
				{
					// draw attack range of ship at new location
					DrawMoves();
					this.mouseRange = RangeCheck(this.selected.getAttackRange(), mouseTile);
					addRange(mouseRange, "RedTransparency", GetNode<TileMap>("/root/Game/TileMapTop"));
				}
			}


			return;
		}
		
		// mouse press/release event
		if (@event is InputEventMouseButton mouseClick)
		{
			// left click mouse press event (BUTTON_LEFT = 1)
			// list of enums here but I can't figure out how to access them: https://docs.godotengine.org/en/3.1/classes/class_@globalscope.html
			if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 1)
			{
				Vector2 cell = WorldToMap(mouseClick.Position);
				int tileIndex = GetCellv(cell);
				GD.Print("Mouse Click at: ", mouseClick.Position, ", Cell: ", cell, ", Tile: ", tileIndex);
				
				// iterate player ships to find what was clicked
				foreach (Ship1 ship in playerShips)
				{
					// if user clicked on this ship
					if (cell == WorldToMap(ship.GetPosition()))
					{
						// specifically, a new ship
						if (this.selected != ship)
						{
							this.selected = ship;
							//plays a sound effect
							AudioStreamPlayer grid_interact = (AudioStreamPlayer) GetNode("/root/Game/SoundEffect/grid_interact");
          					grid_interact.Play();
							GD.Print("Selected: ", ship);
							
							// redraw movement range
							DrawMoves();

							return; // "select ship" action complete
						}
					}
				}

				// iterate enemy ships to find what was clicked
				foreach (CompShip compShip in computerShips)
				{
					// if user clicked on this ship
					if (cell == WorldToMap(compShip.GetPosition()))
					{
						//if the user clicks on an enemy ship while one of their ships is selected it will try to attack them
						//TODO: Make the player choose a weapon first.
						if (this.selected !=  null)
						{
							//Ship1 temps = (Ship1)child;
							//shipClass.Projectile weapon = child.Call("getWeapon1");
							Projectile weapon = new Projectile(ProjectileType.Gun, 1, 2, 2, 8, 1, "normal");
							Attack(this.selected, compShip, weapon );
							// redraw movement range tiles in case a ship was removed
							DrawMoves();
						}
						return; // "enemy ship clicked" action complete
					}
				}

				// a ship wasn't clicked, if a ship is selected try to move it to the space
				if (this.selected != null)
				{

					//if a valid position is selected, the child is moved.
					
					// try to move ship, inner loop runs for invalid moves
					if (!Move(this.selected, cell))
					{
						// we don't otherwise HAVE to do anything if move() fails
						// TODO - user feedback
					}

					// deselect ship if no moves available
					if (selected.range <= 0)
					{
						// can't attack at all
						if (selected.AP <= 0){}
							// this.selected = null;
						else {
							// redraw and check if anything is attackable
							DrawMoves();
							// if not, deselect
							if (this.atkRange.Length == 0)
							{
								// this.selected = null;
							}
						}
					}
					// redraw movement range tiles
					DrawMoves();
				}
				return; // "move ship" action complete 
			} // end of left click
			
		// right click
		if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 2)
		{
			// eg. clear selection
			if (this.selected != null)
			{
				GD.Print("Deselected ", this.selected);
				// remove old range tiles
				if (this.validMoves != null) removeRange(this.validMoves);
				if (this.atkRange != null) removeRange(this.atkRange);
				this.selected = null;
				// redraw movement range tiles with no ship selected
				DrawMoves();
			}
		}
	}
}

// pressed when user ends their turn
	private void _on_CompMove_pressed()
	{
		// block player actions
		this.playerTurn = false;

		// deselect ship and redraw (remove) movement tiles
		this.selected = null;
		DrawMoves();

		// reset everyone's action points
		foreach (Ship1 ship in playerShips)
			ship.ResetPoints();
		foreach (CompShip compShip in computerShips)
			compShip.ResetPoints();

		// carry out computer's turn	
		ComputerTurn();
		
		// resume player's turn
		DrawMoves();
		this.playerTurn = true;	
	}

	// runs the computer player's turn
	private void ComputerTurn()
	{
		foreach (CompShip compShip in computerShips)
		{
			compShip.PlayTurn();
			CheckVictory();
			System.Threading.Thread.Sleep(200);
		}
	}
	
	//this function returns a node that is found at specific coordinates
	private Node2D get_cell_node(Vector2 cord)
	{
		for (int i = 0; i <GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			if (cord == WorldToMap(child.GetPosition()))
				return child;
		}
		return null;
	}
	
	//return true is space is occupied and not out of bounds, false otherwise
	private bool is_cell_vacant(Vector2 cord)
	{
		if (get_cell_node(cord) != null)
			return false;
		if (cord.x <= 0 || cord.x > gridSize || cord.y <= 0 || cord.y > gridSize/2)
			return false;
		
		return true;
	}

	// check whether a victory condition has been met by either side
	// if so, ends the match appropriately
	//todo, screens, check for audio cutting
	//possibly add some victory/defeat flourish too
	private void CheckVictory()
	{
		// enemy has 0 ships, player wins
		if (computerShips.Count == 0)
		{
			// increment state variables to unlock next level
			State.maxLevel = State.currentLevel + 1;
			State.currentLevel += 1;
			// end match and return to level select
			bool victory = true;
			GetTree().ChangeScene("res://level_select.tscn");
		}
		// player has 0 ships, player loses
		else if (playerShips.Count == 0)
		{
			// just end match and return to level select
			bool defeat = true;
			GetTree().ChangeScene("res://level_select.tscn");
		}
	}
}