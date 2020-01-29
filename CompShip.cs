using Godot;
using System;

public class CompShip : Ship1
{
	/*
    public const int maxHP = 50;//maximum hp
    public int HP { get; set;} = 50;//current hp
	name or id?
    public int firepower { get; set; } = 5;//the ships firepower multiplier
    public int penetration { get; set; } = 5;//the ships ability to ignore armour
    public int armour { get; set; } = 5;//the ships resistance to damage
    public int accuracy { get; set; } = 5;//odds of hitting an opponent
    public int evasion { get; set; } = 5;//odds of dodging an attack
	*/
    private int range = 1;
	
	public int shipType = 1;
	
	public CompShip(){

	}
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	// requests the ship's actions to the grid
	public void PlayTurn()
	{
		Vector2 location = this.GetPosition();
		Random r = new Random();
		int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west
		//get node "this.name" for dynamic?
		Node2D compShip = (Node2D)GetNode("/root/Game/Grid/CompShip");
		
		Vector2 shipCell = compShip.Position; 
		int gridSize = ((Grid) GetNode("/root/Game/Grid")).gridSize;
		Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - range * gridSize);
		Vector2 moveEast = new Vector2(shipCell.x + range * gridSize, shipCell.y);
		Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + range * gridSize);
		Vector2 moveWest = new Vector2(shipCell.x - range * gridSize, shipCell.y);

		switch (randDirection){
			// todo: is statically referencing the grid the best way ?
			case 0:
				compShip.SetPosition(moveNorth);	
				break;			
			case 1:
				compShip.SetPosition(moveEast);
				break;
			case 2:
				compShip.SetPosition(moveSouth);
				break;			
			case 3:
				compShip.SetPosition(moveWest);
				break;
		}
	}
}
