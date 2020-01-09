using Godot;
using System;

/*	This is the main code for controlling objects on the Grid */

public class Grid : TileMap
{
	// currently selected node/sprite
	private Node2D selected = null;
	private Vector2[] validMoves = null;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// iterate children of this node (all the Node2D characters)
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			// WorldToMap converts pixel coordinates to grid coordinates
			GD.Print("Node loaded: ", child, " at ", WorldToMap(child.GetPosition()));
			GD.Print(child.Call("GetRange"));
		}
    }
	private int gridSize = 32;


	

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
	


	private int SumOfPrevious(int startNum)
	{
		int final = 0;
		for (int i = startNum; i > 0; i--)
		{
			final += i;
		}

		return final;
	}
	
	/* returns a vector array of cells in range of a given position
	* int range - radius around central position
	* Vector2 currentPos - central position
	* return Vector2[] of positions within range
	*/
	private Vector2[] RangeCheck(int range, Vector2 currentPos)
	{
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
		int tile = TileSet.FindTileByName("SpriteStar4");
		for(int i = 0; i < possibleLocations.Length; i++){
			SetCellv(possibleLocations[i], tile);
		}
		 return possibleLocations;
	}

	// Removes all blue tiles from the grid 
	private void removeRange(Vector2[] moves)
	{
		int tile = TileSet.FindTileByName("Sprite");
		for(int i = 0; i < moves.Length; i++){
			
			SetCellv(moves[i], tile);
		}
	}

	// Called when there is input
	public override void _Input(InputEvent @event)
	{
		
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
					
					// if user clicked on a child
					//currently only works for friendly child nodes
					if (cell == WorldToMap(child.GetPosition()) && child !=  (Node2D)GetNode("CompShip"))
					{
						if (this.selected != child)
						{
							this.selected = child;
							GD.Print("Selected: ", child);
							//Populates the valid moves for the selected ship
							this.validMoves = RangeCheck((int)this.selected.Call("GetRange"), WorldToMap(this.selected.GetPosition()));
							break;
						} 
					}
					// if the current child is selected this statement is entered.
					// MapToWorld converts grid to pixel coordinates
					if (this.selected == child )
					{

						//if a valid position is seledted, the child is moved.
						if (GetCellv(cell) == TileSet.FindTileByName("SpriteStar4"))
						{
							this.selected.SetPosition(MapToWorld(cell));
							removeRange(this.validMoves);
							GD.Print(this.selected, " moved to ", cell);
							this.selected = null;
						}
					//if another cell is clicked, removes blue tiles and unselects child.
					//*****Change this functionality when battle system is more developed.*****
					}else{
						removeRange(this.validMoves);
						this.selected = null;
					}
					
				}
			}
			
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
	private void _on_CompMove_pressed(){
		Node2D compShip = (Node2D)GetNode("CompShip");
		Vector2 shipCell = compShip.Position; 
		Random r = new Random();
		int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west
		//to do check ray cast for valid move, snapped, add collision with other ship
		
		GD.Print("direction test:", randDirection);
		switch (randDirection){
			case 0:
			    Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - 1 * gridSize);
				compShip.SetPosition(moveNorth);
				break;			
			case 1:
			    Vector2 moveEast = new Vector2(shipCell.x + 1 * gridSize, shipCell.y);
				compShip.SetPosition(moveEast);
				break;
			case 2:
			    Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + 1 * gridSize);
				compShip.SetPosition(moveSouth);
				break;			
			case 3:
			    Vector2 moveWest = new Vector2(shipCell.x - 1 * gridSize, shipCell.y);
				compShip.SetPosition(moveWest);
				break;
		}
	}
}