using Godot;
using System;

/*	This is the main code for controlling objects on the Grid */
public class Grid : TileMap
{
	// currently selected node/sprite
	private Node2D selected = null;

  	private Vector2[] validMoves = null;
	private Vector2[] atkRange = null;
	private bool turnEnd = false;

	private int gridSize = 32;
	private bool playerTurn = true;
	private Ship1[] playerNodes;
	private Ship1[] aiNodes;
	private Vector2[] obstacles;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// iterate children of this node (all the Node2D characters)
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			
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

				}else if ((j != 0) & (i != 0)){
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
	public bool move(Ship1 ship, Vector2 target)
	{
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
	public bool Attack(Ship1 ship, Vector2 target, ProjectileType projType = ProjectileType.Default)
	{
		

		return true;
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
			// check if specifically left mouse press event (BUTTON_LEFT = 1)
			// list of enums here but I can't figure out how to access them: https://docs.godotengine.org/en/3.1/classes/class_@globalscope.html
			if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 1)
			{
				Vector2 cell = WorldToMap(mouseClick.Position);
				GD.Print("Mouse Click at: ", mouseClick.Position, ", Cell: ", cell);
				Node2D enemyShip = (Node2D)GetNode("CompShip");
				
				
				
				// check if user clicked on something
				for (int i = 0; i < GetChildCount(); i++)
				{
					Node2D child = (Node2D)GetChild(i);
          
					// if user clicked on a child
					//currently only works for friendly child nodes
					if (cell == WorldToMap(child.GetPosition()) && child !=  enemyShip)
					{
						if (this.selected != child)
						{
							this.selected = child;
							GD.Print("Selected: ", child);
							Ship1 ship = (Ship1)child;
							
							
							//Populates the valid moves for the selected ship
							this.validMoves = RangeCheck((int)this.selected.Call("GetRange"), WorldToMap(this.selected.GetPosition()));
							
							// draw movement range
							addRange(this.validMoves, "SpriteStar4");
							
							//checks if enemy is in attack range
							this.atkRange = RangeCheck(ship.getAttackRange(), WorldToMap(ship.Position));
							if (Array.Exists((atkRange), element => element == WorldToMap(enemyShip.Position)))
							{
								// draw attack range if an enemy is nearby
								addRange(atkRange, "SpriteStar5"); 
								//if they are, turn is not over
								turnEnd = false;	
							}
							break;
						} 
					}
					// if the current child is selected this statement is entered.
					// MapToWorld converts grid to pixel coordinates
					if (this.selected == child )
					{

						//if a valid position is selected, the child is moved.
						
						// try to move ship, inner loop runs for invalid moves
						if (!move((Ship1)child, cell))
						{
							// we don't otherwise HAVE to do anything if move() fails
				 			GD.Print("Invalid move!");
						}
						removeRange(this.validMoves);
						removeRange(this.atkRange);
					 	this.selected = null;
					}
				}
			} // end of left click
			
			// right click
			if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 2)
			{
				// eg. clear selection
				if (this.selected != null)
				{
					GD.Print("Deselected ", this.selected);
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
		for (int i = 0; i < GetNode("ComputerShips").GetChildCount(); i++)
		{
			CompShip child = (CompShip)GetNode("ComputerShips").GetChild(i);
			child.PlayTurn();
		}
	}
}