using Godot;
using System;

/*	This is the main code for controlling objects on the Grid */
public class Grid : TileMap
{
	// currently selected node/sprite
	private Node2D selected = null;
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
		 return possibleLocations;
	}

	/* Receives a request to move a ship
	*  Ship1 ship, ship to move
	*  Vector2 target, target space coordinates
	*  return true if moved, false if blocked
	*/
	public bool move(Ship1 ship, Vector2 target)
	{
		float distance = WorldToMap(ship.Position).DistanceTo(target);

		// target out of range
		if (distance > ship.GetRange()) return false;

		// move ship to new position
		ship.SetPosition(MapToWorld(target));
		return true;
	}

	//public bool attack( -- projectile class -- , Vector2 target) //TODO

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
				
				// check if user clicked on something
				for (int i = 0; i < GetChildCount(); i++)
				{
					Node2D child = (Node2D)GetChild(i);
					// if user clicked on a child (ship)
					if (cell == WorldToMap(child.GetPosition()))
					{
						if (this.selected == null)
						{
							this.selected = child;
							
							GD.Print("Selected: ", child);
							break;
						} else {
							// something was previously selected, decide what to do
							// clicked new child: update current selection
							if (this.selected != child)
							{
								this.selected = child;
								GD.Print("Selected: ", child);
								break;
							}
						}
					}
					// clicked any other cell: move the selected ship
					// MapToWorld converts grid to pixel coordinates
					if (this.selected != null)
					{
						// try to move ship, returns false for invalid moves
						if (!move((Ship1)child, cell))
						{
				 			GD.Print("Invalid move!");
							break;
						}
						
				 		GD.Print(this.selected, " moved to ", cell);
						// deselect the ship after moving it
				 		this.selected = null;
				 		break;
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