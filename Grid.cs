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
	private Ship1 attackNode;
	private Ship1 defendNode;
	private Bullets bulletNode;
	private Vector2[] obstacles;
	
	//https://docs.godotengine.org/en/3.1/getting_started/workflow/best_practices/scenes_versus_scripts.html
	//Based on the information provided in the above link, I will define the animated projectiles as a seperate scene
	//it may also be a good idea to make the ships other scenes as well.
	//public PackedScene _bullet = (PackedScene)ResourceLoader.load("Bullets.tscn", "", false);
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// iterate children of this node (all the Node2D characters)
		var _bullet = ResourceLoader.Load("Bullets.tscn") as PackedScene;
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			// WorldToMap converts pixel coordinates to grid coordinates
			GD.Print("Node loaded: ", child, " at ", WorldToMap(child.GetPosition()));
			//PackedScene _bullet = (PackedScene)ResourceLoader.load("res://Bullets.tscn");
			/*
			var bullet_instance = _bullet.Instance() as Area2D;
			AddChild(bullet_instance);
			bullet_instance.SetPosition(attackNode.Position);*/
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

	
	public void attack( Ship1 attacker, Ship1 defender, shipClass.Projectile proj) //TODO
	{
		if (!Array.Exists(RangeCheck(attacker.getAttackRange(), WorldToMap(attacker.Position)), element => element == WorldToMap(defender.Position))) return;
		
		int f = attacker.firepower * proj.firepower;
		int p = attacker.penetration * proj.penetration;
		int a = attacker.accuracy * proj.accuracy;
		attackNode = attacker;
		defendNode = defender;
		
		var _bullet = ResourceLoader.Load("Bullets.tscn") as PackedScene;
		//_bullet.Bulle(WorldToMap(attacker.Position), WorldToMap(defender.Position), f, p, a, defender.evasion);
		var bullet_instance = _bullet.Instance() as Area2D;
		AddChild(bullet_instance);
		//bullet_instance.SetPosition(attackNode.Position);
		
		
		Bullets bull = new Bullets(WorldToMap(attacker.Position), WorldToMap(defender.Position), f, p, a, defender.evasion);
		
		bullet_instance.Connect("hit_target", this, "attackhits" );
		
		//GetNode("Bullets").Set("Target", this);
		//GetNode("Bullets").Connect("hit_target", bul, "MethodOnTheObject");
		
		
		return;
	}
	
	public void attackhits(Vector2 cell, int fp, int pen)
	{
		/*for (int i = 0; i <GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			if (cell == WorldToMap(child.GetPosition()))
				(Ship1)child.take_damage(fp, pen);
		}*/
		
		defendNode.take_damage(fp, pen);
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
					//if the user clicks on an enemy ship while one of their ships is selected it will try to attack them
					//TODO: Make the player choose a weapon first.
					if (this.selected == child && cell == WorldToMap(enemyShip.Position))
					{
						//Ship1 temps = (Ship1)child;
						//shipClass.Projectile weapon = child.Call("getWeapon1");
						shipClass.Projectile weapon = new shipClass.Projectile("Gun", 1, 2, 2, 8, 1, "normal");
						attack((Ship1)child, (Ship1)enemyShip, weapon );
					
					
					// if the current child is selected this statement is entered.
					// MapToWorld converts grid to pixel coordinates
					}else if (this.selected == child )
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