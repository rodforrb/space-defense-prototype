using Godot;
using System;
using System.Collections.Generic;
	
/*	This is the main code for controlling objects on the Grid */
public class Grid : TileMap
{
	// currently selected node/sprite
	private Ship1 selected = null;

	private Vector2[] validMoves = null;
	private Vector2[] atkRange = null;
	private bool turnEnd = false;

	public int gridSize = 32;
	private bool playerTurn = true;
	public List<Ship1> playerShips {get;} = new List<Ship1>();
	public List<CompShip> computerShips {get;} = new List<CompShip>();
	private Ship1 attackNode;
	private Ship1 defendNode;
	private Bullets gunNode;
	private Vector2[] obstacles;
	
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
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	
	/* returns a vector array of cells in range of a given position
	* int range - radius around central position
	* Vector2 currentPos - central position
	* return Vector2[] of positions within range
	*/
	private Vector2[] RangeCheck(int range, Vector2 currentPos)
	{
		// internal function to calculate sum
		int SumOfPrevious(int startNum)
		{
			int final = 0;
			for (int i = startNum; i > 0; i--)
			{
				final += i;
			}
			return final;
		}
		Vector2[] possibleLocations = new Vector2[((2*range+1) * (2*range+1)) - (SumOfPrevious(range)*4)];
		int iterator = 0;
		for (int i = 0; i <= range; i++)
		{	
			for(int j = range-i; j >= 0;j--)
			{
				if (i == 0 & j != 0)
				{
					possibleLocations[iterator] = new Vector2(currentPos.x + j, currentPos.y);
					possibleLocations[iterator+1] = new Vector2(currentPos.x - j, currentPos.y);
					iterator+=2;
				}
				else if (j==0 & i!=0)
				{
					possibleLocations[iterator] = new Vector2(currentPos.x, currentPos.y + i);
					possibleLocations[iterator+1] = new Vector2(currentPos.x, currentPos.y - i);
					iterator+=2;

				}
				else if ((j != 0) & (i != 0)){
					possibleLocations[iterator] = new Vector2(currentPos.x + j, currentPos.y + i);
					possibleLocations[iterator+1] = new Vector2(currentPos.x + j, currentPos.y - i);
					possibleLocations[iterator+2] = new Vector2(currentPos.x - j, currentPos.y + i);
					possibleLocations[iterator+3] = new Vector2(currentPos.x - j, currentPos.y - i);
					iterator+=4;
				}
			}
		}
		//removes the last elenment from the array, which is always (0,0)
		Array.Resize(ref possibleLocations, possibleLocations.Length - 1);
		//Sets all the possible movement cells to a blue tile.
	
		return possibleLocations;
	}

	private void addRange(Vector2[] moves, String tileString)
	{
		int tile = TileSet.FindTileByName(tileString);
		for(int i = 0; i < moves.Length; i++)
		{
			SetCellv(moves[i], tile);
		}
	}

	// Removes all blue tiles from the grid 
	private void removeRange(Vector2[] moves)
	{
		// int tile = TileSet.FindTileByName("Sprite");
		for(int i = 0; i < moves.Length; i++){
			
			SetCellv(moves[i], -1);
		}
	}

	/* Receives a request to move a ship
	*  Ship1 ship, ship to move
	*  Vector2 target, target space coordinates
	*  return true if moved, false if blocked
	*/
	public bool Move(Node2D ShipNode, Vector2 target)
	{
		GD.Print(ShipNode, target);
		Ship1 ship = (Ship1)ShipNode;
//		float distance = WorldToMap(ship.Position).DistanceTo(target);

		// target out of range
		if (!Array.Exists(RangeCheck(ship.GetRange(), WorldToMap(ship.Position)), element => element == target)) return false;
		

		// move ship to new position
		ship.SetPosition(MapToWorld(target));
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
		
		int f = attacker.firepower * proj.firepower;
		int p = attacker.penetration * proj.penetration;
		int a = attacker.accuracy * proj.accuracy;
		attackNode = attacker;
		defendNode = defender;
		
		defender.take_damage(f, p, a);
		if (defender.HP <= 0)
		{
			RemoveChild(defender);
			// remove from the appropriate list
			try
			{
				computerShips.Remove((CompShip)defender);
				
			} catch (System.InvalidCastException e) {
				playerShips.Remove(defender);
			}
		}
		return;
	}
	
	
	public void attackhits(Vector2 cell, int fp, int pen, int acc)
	{
		/*for (int i = 0; i <GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			if (cell == WorldToMap(child.GetPosition()))
				(Ship1)child.take_damage(fp, pen);
		}*/
		
		
		defendNode.take_damage(fp, pen, acc);
	}

	/* Called whenever there is user input
	*  consequently this manages all actions for the player's turn
	*/
	public override void _Input(InputEvent @event)
	{
		// ignore user input while it is not their turn
		if (!this.playerTurn) return;
		
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
							GD.Print("Selected: ", ship);
							
							// remove old range tiles
							if (this.validMoves != null) removeRange(this.validMoves);
							if (this.atkRange != null) removeRange(this.atkRange);

							//Populates the valid moves for the selected ship
							this.validMoves = RangeCheck(ship.GetRange(), WorldToMap(ship.GetPosition()));
							// draw movement range
							addRange(this.validMoves, "SpriteStar4");
							
							this.atkRange = RangeCheck(ship.getAttackRange(), WorldToMap(ship.Position));
							// draw attack range always
							addRange(atkRange, "SpriteStar5");

							// //checks if enemy is in attack range
							// {
							// 	// draw attack range
							// 	addRange(atkRange, "SpriteStar5"); 
							// 	//if they are, turn is not over
							// 	turnEnd = false;	
							// }
							return; // exit because ship was found
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
						}
						return;
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
						GD.Print("Invalid move!");
					}
					removeRange(this.validMoves);
					removeRange(this.atkRange);
					// deselect
					this.selected = null;
				}
				return; // this 
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
			}
		}
	}
}

// pressed when user ends their turn
	private void _on_CompMove_pressed()
	{
		this.playerTurn = false;
		ComputerTurn();
		// todo: reset values to 'turn start' values
		this.playerTurn = true;	
	}

	// runs the computer player's turn
	private void ComputerTurn()
	{
		foreach (CompShip compShip in computerShips)
		{
			compShip.PlayTurn();
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
	// returns 0 for no victory, 1 for player victory, 2 for computer victory
	private int CheckVictory()
	{
		if (computerShips.Count == 0) return 1; // player wins in a tie

		if (playerShips.Count == 0) return 2;

		return 0;
	}
}